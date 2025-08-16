using System.Collections.Generic;
using UnityEngine;

public static class SoilUtilities
{

	     public static SoilComposition CalculateDerivedProperties(SoilComposition sc)
	     {
		          // 1. Solid volume fractions (m3/m3)
		          float sandVol = sc.sand / Constants.SAND_DENSITY;
		          float siltVol = sc.silt / Constants.SILT_DENSITY;
		          float clayVol = sc.clay / Constants.CLAY_DENSITY;
		          float organicVol = sc.organicMatter / Constants.ORGANIC_DENSITY;
		          float totalSolidVol = sandVol + siltVol + clayVol + organicVol;

		          if (totalSolidVol <= 0f)
				totalSolidVol = Mathf.Epsilon; // Avoid division by zero

		          // 2. Texture percentages
		          float sandPct = sandVol / totalSolidVol * 100f;
		          float siltPct = siltVol / totalSolidVol * 100f;
		          float clayPct = clayVol / totalSolidVol * 100f;

		          // 3. Porosity & bulk density
		          float rawPorosity = CalculatePorosity(sandPct, clayPct, sc.organicMatter);
		          sc.porosity = Mathf.Clamp01(rawPorosity * (1f - sc.compaction * 0.85f));

		          sc.bulkDensity = (sc.sand + sc.silt + sc.clay + sc.organicMatter) / totalSolidVol;
		          sc.bulkDensity *= 1f + (sc.compaction * 0.2f);

		          // 4. Water properties
		          sc.waterCapacity = CalculateWaterCapacity(sc.porosity, clayPct);
		          float maxWater = sc.waterCapacity * Constants.WATER_DENSITY;
		          sc.saturation = maxWater > 0f ? Mathf.Clamp01(sc.water / maxWater) : 0f;

		          // 5. Flow properties
		          sc.infiltrationRate = CalculateInfiltrationRate(sandPct, sc.saturation) * (1f - sc.compaction * 0.9f);

		          float safePorosity = Mathf.Clamp(sc.porosity, 0.01f, 0.95f);
		          sc.percolationRate = CalculatePercolationRate(safePorosity, sandPct, clayPct) * (1f - sc.compaction * 0.9f);
		          sc.hydraulicConductivity = sc.percolationRate * 24f;

		          sc.horizontalPermeability = CalculateHorizontalPermeability(sandPct, clayPct, sc.porosity) * (1f - sc.compaction);
		          sc.verticalPermeability = CalculateVerticalPermeability(sandPct, clayPct, sc.porosity) * (1f - sc.compaction);

		          // 6. Structural properties
		          sc.aeration = sc.porosity * (1f - sc.saturation);
		          sc.erodibility = CalculateErodibility(sandPct, clayPct, sc.organicMatter);

		          return sc;
	     }

	     static float CalculatePorosity(float sandPct, float clayPct, float organicMatter)
	     {
		          // Saxton & Rawls (2006) model with organic matter adjustment
		          return 0.45f - 0.0012f * sandPct + 0.0021f * clayPct +
			  0.025f * (organicMatter / Constants.ORGANIC_DENSITY);
	     }

	     static float CalculateWaterCapacity(float porosity, float clayPct)
	     {
		          // Field capacity approximation (m3 water/m3 soil)
		          return porosity * (0.2f + 0.5f * (clayPct / 100f));
	     }

	     static float CalculateInfiltrationRate(float sandPct, float saturation)
	     {
		          // USDA NRCS model with saturation adjustment
		          float baseRate = 25f * Mathf.Pow(sandPct / 100f, 1.5f);
		          return baseRate * (1f - saturation * 0.8f); // Reduces when wet
	     }

	     static float CalculatePercolationRate(float porosity, float sandPct, float clayPct)
	     {
		          // Kozeny-Carman derived percolation (cm/hour)
		          float k = Mathf.Pow(porosity, 3f) / (1f - porosity);
		          return 10f * k * (sandPct / 100f) * (1f - clayPct / 200f);
	     }

	     static float CalculateHydraulicConductivity(float percolationRate)
	     {
		          // Convert percolation to conductivity (cm/day)
		          return percolationRate * 24f;
	     }

	     static float CalculateErodibility(float sandPct, float clayPct, float organicMatter)
	     {
		          // Revised USLE K-factor approximation
		          float sandFactor = 0.2f + 0.3f * (sandPct / 100f);
		          float clayFactor = 0.3f * (clayPct / 100f);
		          float omFactor = 0.2f * (1f - organicMatter / Constants.ORGANIC_DENSITY);
		          return Mathf.Clamp01(sandFactor + clayFactor + omFactor);
	     }


	     static float CalculateHorizontalPermeability(float sandPct, float clayPct, float porosity)
	     {
		          // Lateral water movement (cm/day)
		          // More influenced by sand and less hindered by clay
		          return 150f * Mathf.Pow(porosity, 2) * (sandPct / 100f) * (1f - clayPct / 150f);
	     }

	     static float CalculateVerticalPermeability(float sandPct, float clayPct, float porosity)
	     {
		          // Vertical drainage (cm/day)
		          // More affected by compaction and clay content
		          return 80f * Mathf.Pow(porosity, 2) * (sandPct / 100f) * (1f - clayPct / 120f);
	     }
	     static void UpdateOrganicDecay(SoilComposition sc, float hours)
	     {
		          // Organic matter decays over time
		          float decayRate = 0.01f * hours; // 1% per hour
		          sc.organicMatter *= (1f - decayRate);
	     }

	     public static float CalculateEffectiveVolume(SoilComposition sc)
	     {
		          // Volume of each solid component (m3/m3)
		          float sandVol = sc.sand / Constants.SAND_DENSITY;
		          float siltVol = sc.silt / Constants.SILT_DENSITY;
		          float clayVol = sc.clay / Constants.CLAY_DENSITY;
		          float organicVol = sc.organicMatter / Constants.ORGANIC_DENSITY;

		          float solidVolume = sandVol + siltVol + clayVol + organicVol;
		          float waterVolume = sc.water / Constants.WATER_DENSITY;
		          float airVolume = sc.aeration; // already calculated from porosity and saturation

		          float total = solidVolume + waterVolume + airVolume;

		          return Mathf.Clamp01(total); // tiles can’t be taller than 1m
	     }

	     public static void UpdateCompaction(SoilComposition sc, float hours, float appliedPressure = 1f)
	     {
		          // Saturated soils compact more under pressure
		          float compactionChange = sc.saturation * appliedPressure * 0.001f * hours;

		          sc.compaction = Mathf.Clamp01(sc.compaction + compactionChange);
	     }

	     public static void ApplySelfCompaction(TileData tileData, float hours)
	     {
		          SoilComposition sc = tileData.composition;

		          // Weight of 1m3 of this soil (kg) × gravity = pressure in Pascals
		          float weight = sc.bulkDensity * Constants.GRAVITY_ACCELERATION; // N/m2

		          // Convert to kPa (1 kPa = 1000 Pa)
		          float selfPressure_kPa = weight / 1000f;

		          // Apply compaction scaled by saturation (wet soil compacts more)
		          UpdateCompaction(sc, hours, appliedPressure: selfPressure_kPa);

		          RecalculateTilePhysics(tileData);
	     }

	     public static void ApplyRainfall(TileData tileData, float mmRain, float hours)
	     {
		          if (mmRain <= 0f)
				return;

		          // Convert mm -> m3/m2 -> kg/m2
		          float addedWater = mmRain / 1000f * Constants.WATER_DENSITY;

		          // Add water to surface moisture
		          tileData.surfaceMoisture += addedWater;

		          // Apply compaction from weight of water
		          // Assume full water column contributes pressure
		          float pressure = addedWater * Constants.GRAVITY_ACCELERATION; // N/m2 = Pascal
		          float pressure_kPa = pressure / 1000f; // convert to kPa for simplicity

		          // Normalize pressure impact (tunable: 1 kPa rain = 1 unit pressure)
		          float effectivePressure = pressure_kPa;

		          // Apply compaction using existing method
		          UpdateCompaction(tileData.composition, hours, appliedPressure: effectivePressure);

		          // Recalculate properties after compaction
		          RecalculateTilePhysics(tileData);
	     }

	     public static void InfiltrateSurfaceMoisture(TileData tileData, float hours)
	     {
		          var sc = tileData.composition;
		          float maxWater = sc.waterCapacity * Constants.WATER_DENSITY;
		          if (sc.water > maxWater)
		          {
				float overflow = sc.water - maxWater;
				sc.water = maxWater;

				tileData.surfaceMoisture += overflow;
		          }
		          if (tileData.surfaceMoisture <= 0f)
				return;

		          // Adjust infiltration rate based on compaction and saturation
		          float compactionModifier = Mathf.Lerp(1f, 0.2f, sc.compaction); // high compaction -> low infiltration
		          float saturationModifier = Mathf.Clamp01(1f - sc.saturation);   // full saturation = no infiltration

		          // infiltrationRate is mm/hour -> convert to kg/m2
		          float maxInfiltration = sc.infiltrationRate * hours / 1000f * Constants.WATER_DENSITY;

		          float effectiveInfiltration = maxInfiltration * compactionModifier * saturationModifier;

		          // Only infiltrate what’s available
		          float infiltrated = Mathf.Min(tileData.surfaceMoisture, effectiveInfiltration);



		          tileData.surfaceMoisture -= infiltrated;
		          sc.water += infiltrated;

		          RecalculateTilePhysics(tileData);
	     }
	     public static float ApplyEvaporation(TileData td, float solarRadiationWatts, float relativeHumidity, float airTempC, float windSpeed, float hours)
	     {
		          SoilComposition sc = td.composition;

		          float evapCoeff = 0.001f; // calibrate this per biome/season

		          // 1. Convert solar to MJ/m2/hour
		          float solarMJ = solarRadiationWatts * 3600f * hours / 1_000_000f;

		          // 2. Estimate potential evap (simplified FAO-56)
		          float tempFactor = Mathf.Max(0.1f, 0.0023f * (airTempC + 17f));
		          float vaporPressureDeficit = Mathf.Clamp01(1f - relativeHumidity); // 0–1
		          float windEffect = 1f + (windSpeed / 2f); // Wind enhances ET

		          float potentialEvap = solarMJ * tempFactor * vaporPressureDeficit * windEffect; // MJ/m2 -> mm
		          float potentialKg = potentialEvap; // 1 mm = 1 kg/m²

		          // 3. Evaporate from surface moisture first
		          float surfaceEvap = Mathf.Min(td.surfaceMoisture, potentialKg);
		          td.surfaceMoisture -= surfaceEvap;
		          float remainingEvap = potentialKg - surfaceEvap;

		          // 4. Then evaporate from subsurface if needed
		          float availableWater = Mathf.Min(sc.water, sc.saturation * Constants.WATER_DENSITY);
		          float waterStressFactor = Mathf.Clamp01(availableWater / (0.1f * Constants.WATER_DENSITY)); // 10 cm root zone

		          float subsurfaceEvap = remainingEvap * waterStressFactor * evapCoeff;
		          subsurfaceEvap = Mathf.Min(subsurfaceEvap, sc.water);

		          sc.water -= subsurfaceEvap;
		          sc.water = Mathf.Max(0f, sc.water);

		          RecalculateTilePhysics(td);

		          return surfaceEvap + subsurfaceEvap;
	     }

	     public static void RedistributeWaterLateral(WorldTile tile, float hours)
	     {
		          SoilComposition sc = tile.tileData.composition;
		          float rate = sc.horizontalPermeability / 100f / 24f;

		          float waterToMove = sc.water;
		          List<WorldTile> neighbors = tile.GetNeighbors();

		          foreach (WorldTile neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;

				SoilComposition nc = neighbor.tileData.composition;
				float deltaSat = sc.saturation - nc.saturation;
				if (deltaSat <= 0f)
					     continue;

				float transfer = deltaSat * rate * hours * Constants.WATER_DENSITY;
				transfer = Mathf.Clamp(transfer, 0f, waterToMove);

				sc.water -= transfer;
				nc.water += transfer;

				waterToMove -= transfer;

				// Erosion only if fine material exists
				float fineTotal = sc.silt + sc.clay;
				if (fineTotal > 0f)
				{
					     float sedimentFraction = sc.erodibility * Mathf.Clamp01(deltaSat);
					     float sedimentTransfer = sedimentFraction * transfer;

					     float siltRatio = sc.silt / fineTotal;
					     float clayRatio = sc.clay / fineTotal;

					     float siltMoved = sedimentTransfer * siltRatio;
					     float clayMoved = sedimentTransfer * clayRatio;

					     sc.silt = Mathf.Max(0f, sc.silt - siltMoved);
					     nc.silt += siltMoved;
					     sc.clay = Mathf.Max(0f, sc.clay - clayMoved);
					     nc.clay += clayMoved;

					     float organicMoved = sedimentTransfer * 0.1f;
					     sc.organicMatter = Mathf.Max(0f, sc.organicMatter - organicMoved);
					     nc.organicMatter += organicMoved;
				}

				RecalculateTilePhysics(neighbor.tileData);
		          }

		          RecalculateTilePhysics(tile.tileData);
	     }

	     public static void UpdateDecompaction(SoilComposition sc, float hours)
	     {
		          // Dry soils and roots loosen soil
		          float dryFactor = 1f - sc.saturation;
		          float decompactionRate = dryFactor * 0.001f * hours; // tweakable
		          sc.compaction = Mathf.Clamp01(sc.compaction - decompactionRate);
	     }
	     public static void RedistributeSurfaceWater(WorldTile tile, float hours)
	     {
		          float flowRate = 0.1f; // m3/hour
		          var neighbors = tile.GetNeighbors();

		          foreach (var neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;

				float heightA = tile.tileData.volume;
				float heightB = neighbor.tileData.volume;

				float delta = heightA - heightB;

				if (delta > 0f)
				{
					     float gradient = Mathf.Clamp01(delta); // simple gradient
					     float flow = flowRate * gradient * hours;

					     float availableWater = tile.tileData.surfaceMoisture;
					     float maxTransfer = Mathf.Min(flow, availableWater);

					     if (maxTransfer <= 0f)
						          continue;

					     tile.tileData.surfaceMoisture -= maxTransfer;
					     neighbor.tileData.surfaceMoisture += maxTransfer;

					     // Apply erosion due to this flow
					     ApplySurfaceErosion(tile.tileData, neighbor.tileData, gradient, maxTransfer);
				}
		          }
	     }


	     public static void ApplySurfaceErosion(TileData source, TileData target, float gradient, float waterMoved)
	     {
		          var sc = source.composition;
		          var tc = target.composition;

		          // erosionFactor: how aggressive erosion is (tweak this)
		          float erosionFactor = gradient * sc.erodibility * 0.05f; // Tunable constant
		          float sedimentMass = erosionFactor * waterMoved; // kg

		          float fineTotal = sc.silt + sc.clay;
		          if (fineTotal > 0f && sedimentMass > 0f)
		          {
				float siltRatio = sc.silt / fineTotal;
				float clayRatio = sc.clay / fineTotal;

				float siltMoved = sedimentMass * siltRatio;
				float clayMoved = sedimentMass * clayRatio;

				// Clamp so we don’t go negative
				siltMoved = Mathf.Min(siltMoved, sc.silt);
				clayMoved = Mathf.Min(clayMoved, sc.clay);

				sc.silt -= siltMoved;
				tc.silt += siltMoved;

				sc.clay -= clayMoved;
				tc.clay += clayMoved;

				float organicMoved = sedimentMass * 0.1f;
				organicMoved = Mathf.Min(organicMoved, sc.organicMatter);

				sc.organicMatter -= organicMoved;
				tc.organicMatter += organicMoved;

				// Optional: reduce source tile height (simulate erosion depth)
				float erosionVolume = sedimentMass / sc.bulkDensity;
				source.volume -= erosionVolume;
				target.volume += erosionVolume;

				RecalculateTilePhysics(source);
				RecalculateTilePhysics(target);
		          }
	     }

	     public static void RecalculateTilePhysics(TileData data)
	     {
		          CalculateDerivedProperties(data.composition);
		          data.volume = CalculateEffectiveVolume(data.composition);
	     }


}
