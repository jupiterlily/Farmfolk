using System.Collections.Generic;
using UnityEngine;

public static class SoilUtilities
{
	     public static SoilComposition CalculateDerivedProperties(SoilComposition sc)
	     {
		          // All calculations now use kg/m2 for water (equivalent to liters for 1m2 area)

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

		          // 4. Water properties - now using kg/m2 (equivalent to liters for 1m2 area)
		          sc.waterCapacity = CalculateWaterCapacity(sc.porosity, clayPct) * 1000f; // Convert m3/m3 to kg/m2
		          sc.saturation = sc.waterCapacity > 0f ? Mathf.Clamp01(sc.water / sc.waterCapacity) : 0f;

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
		          // USDA NRCS model with saturation adjustment (mm/hour)
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
		          return 150f * Mathf.Pow(porosity, 2) * (sandPct / 100f) * (1f - clayPct / 150f);
	     }

	     static float CalculateVerticalPermeability(float sandPct, float clayPct, float porosity)
	     {
		          // Vertical drainage (cm/day)
		          return 80f * Mathf.Pow(porosity, 2) * (sandPct / 100f) * (1f - clayPct / 120f);
	     }

	     public static float CalculateEffectiveVolume(SoilComposition sc)
	     {
		          // Volume of each solid component (m3/m3)
		          float sandVol = sc.sand / Constants.SAND_DENSITY;
		          float siltVol = sc.silt / Constants.SILT_DENSITY;
		          float clayVol = sc.clay / Constants.CLAY_DENSITY;
		          float organicVol = sc.organicMatter / Constants.ORGANIC_DENSITY;

		          float solidVolume = sandVol + siltVol + clayVol + organicVol;
		          float waterVolume = sc.water / 1000f; // Convert kg/m2 back to m3/m3 for volume calculation
		          float airVolume = sc.aeration; // already calculated from porosity and saturation

		          float total = solidVolume + waterVolume + airVolume;

		          return Mathf.Clamp01(total); // tiles can't be taller than 1m
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
		          float weight = sc.bulkDensity * Constants.GRAVITY_ACCELERATION; // N/m2
		          float selfPressure_kPa = weight / 1000f; // Convert to kPa
		          UpdateCompaction(sc, hours, appliedPressure: selfPressure_kPa);
		          RecalculateTilePhysics(tileData);
	     }

	     public static void ApplyRainfall(TileData tileData, float kgRain, float hours)
	     {
		          if (kgRain <= 0f)
				return;

		          // Add water to surface moisture directly (kg/m2)
		          tileData.surfaceMoisture += kgRain;

		          // Apply compaction from weight of water
		          float pressure = kgRain * Constants.GRAVITY_ACCELERATION; // N/m2
		          float pressure_kPa = pressure / 1000f;
		          UpdateCompaction(tileData.composition, hours, appliedPressure: pressure_kPa);
		          RecalculateTilePhysics(tileData);
	     }
	     public static void InfiltrateSurfaceMoisture(TileData tileData, float hours)
	     {
		          var sc = tileData.composition;

		          // Step 1: Handle overflow first
		          if (sc.water > sc.waterCapacity)
		          {
				float overflow = sc.water - sc.waterCapacity;
				sc.water = sc.waterCapacity;
				tileData.surfaceMoisture += overflow;
		          }

		          if (tileData.surfaceMoisture <= 0f)
				return;

		          // Step 2: Compute modifiers based on compaction and current saturation
		          float compactionModifier = Mathf.Lerp(1f, 0.2f, sc.compaction);
		          float saturationModifier = Mathf.Clamp01(1f - sc.saturation); // Reduces infiltration as soil saturates

		          // Step 3: Determine maximum possible infiltration this timestep
		          float maxInfiltration = sc.infiltrationRate * hours;

		          // Step 4: Apply modifiers to compute actual infiltration
		          float effectiveInfiltration = maxInfiltration * compactionModifier * saturationModifier;

		          // Step 5: Clamp infiltration to available surface moisture
		          float infiltrated = Mathf.Min(tileData.surfaceMoisture, effectiveInfiltration);

		          // Step 6: Add infiltrated water to soil, but do not exceed water capacity
		          float availableCapacity = sc.waterCapacity - sc.water;
		          float absorbed = Mathf.Min(infiltrated, availableCapacity);

		          sc.water += absorbed;
		          tileData.surfaceMoisture -= absorbed;

		          // Step 7: If some water could not infiltrate, it remains on the surface
		          // (no "bouncing" back effect)
		          RecalculateTilePhysics(tileData);
	     }

	     public static float ApplyEvaporation(
	         TileData td,
	         float solarRadiationWatts,
	         float relativeHumidity,
	         float airTempC,
	         float windSpeed,
	         float hours)
	     {
		          SoilComposition sc = td.composition;
		          float evapCoeff = 0.01f; // tuning parameter

		          // Convert incoming solar (W/m2) to MJ/m2 over the timestep
		          float solarMJ = solarRadiationWatts * 3600f * hours / 1_000_000f;

		          // Estimate potential evapotranspiration (simplified FAO-56 style)
		          float tempFactor = Mathf.Max(0.1f, 0.0023f * (airTempC + 17f));
		          float vaporPressureDeficit = Mathf.Clamp01(1f - relativeHumidity);
		          float windEffect = 1f + (windSpeed / 2f);

		          // solarMJ -> "potential ET" in mm water depth -> kg/m2 (1 mm = 1 kg/m2)
		          float potentialEvapKg = solarMJ * tempFactor * vaporPressureDeficit * windEffect;

		          // Evaporate from surface layer first
		          float surfaceEvap = Mathf.Min(td.surfaceMoisture, potentialEvapKg);
		          td.surfaceMoisture -= surfaceEvap;
		          float remainingEvap = potentialEvapKg - surfaceEvap;

		          // Then from subsurface soil water
		          float availableWater = Mathf.Min(sc.water, sc.waterCapacity);
		          float waterStressFactor = Mathf.Clamp01(availableWater / (0.1f * 1000f)); // ~10 cm root zone capacity

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
		          float rate = sc.horizontalPermeability / 100f / 24f; // Convert cm/day to m/hour

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

				float transfer = deltaSat * rate * hours * 1000f; // Convert m3 to kg (1000 kg/m3)
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
		          float dryFactor = 1f - sc.saturation;
		          float decompactionRate = dryFactor * 0.001f * hours;
		          sc.compaction = Mathf.Clamp01(sc.compaction - decompactionRate);
	     }

	     public static void RedistributeSurfaceWater(WorldTile tile, float hours)
	     {
		          float flowRate = 0.1f * 1000f; // m3/hour -> kg/hour (1000 kg/m3)
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
					     float gradient = Mathf.Clamp01(delta);
					     float flow = flowRate * gradient * hours;

					     float availableWater = tile.tileData.surfaceMoisture;
					     float maxTransfer = Mathf.Min(flow, availableWater);

					     if (maxTransfer <= 0f)
						          continue;

					     tile.tileData.surfaceMoisture -= maxTransfer;
					     neighbor.tileData.surfaceMoisture += maxTransfer;

					     ApplySurfaceErosion(tile.tileData, neighbor.tileData, gradient, maxTransfer);
				}
		          }
	     }

	     public static void ApplySurfaceErosion(TileData source, TileData target, float gradient, float waterMoved)
	     {
		          var sc = source.composition;
		          var tc = target.composition;

		          float erosionFactor = gradient * sc.erodibility * 0.05f;
		          float sedimentMass = erosionFactor * waterMoved;

		          float fineTotal = sc.silt + sc.clay;
		          if (fineTotal > 0f && sedimentMass > 0f)
		          {
				float siltRatio = sc.silt / fineTotal;
				float clayRatio = sc.clay / fineTotal;

				float siltMoved = sedimentMass * siltRatio;
				float clayMoved = sedimentMass * clayRatio;

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

	     public static void UpdateSoilTemperature(TileData data, float solarWatts, float airTemp, float hours)
	     {
		          float energyAbsorbed = solarWatts * hours * 3600f * 0.7f;
		          float soilMass = 1500f;
		          float heatCapacity = GetSoilHeatCapacity(data.composition);

		          float deltaT_solar = energyAbsorbed / (soilMass * heatCapacity);
		          float thermalExchangeCoeff = 10f;
		          float deltaT_air = (airTemp - data.soilTemperature) * thermalExchangeCoeff * hours / (soilMass * heatCapacity);

		          data.soilTemperature += deltaT_solar + deltaT_air;
	     }

	     public static float PassHeatFromSoil(TileData data, float hours)
	     {
		          SoilComposition sc = data.composition;
		          float latentHeat = Constants.LATENT_HEAT_VAPORIZATION;
		          float soilMass = 1500f;
		          float heatCapacity = GetSoilHeatCapacity(sc);

		          float availableEnergy = soilMass * heatCapacity * data.soilTemperature;
		          float maxEvaporableKg = availableEnergy / latentHeat;
		          float surfaceWater = data.surfaceMoisture;
		          float evaporatedKg = Mathf.Min(surfaceWater, maxEvaporableKg);

		          data.surfaceMoisture -= evaporatedKg;
		          float usedEnergy = evaporatedKg * latentHeat;
		          float deltaT = usedEnergy / (soilMass * heatCapacity);
		          data.soilTemperature = Mathf.Max(0f, data.soilTemperature - deltaT);

		          float unusedEnergy = Mathf.Max(0f, availableEnergy - usedEnergy);
		          return unusedEnergy;
	     }

	     public static float GetSoilHeatCapacity(SoilComposition comp)
	     {
		          float waterCp = 4186f;
		          float organicCp = 2500f;
		          float sandCp = 800f;
		          float clayCp = 900f;
		          float siltCp = 900f;

		          float total = comp.sand + comp.clay + comp.silt + comp.organicMatter + comp.water;
		          if (total == 0)
				return 1000f;

		          float cp = (comp.water * waterCp +
			      comp.organicMatter * organicCp +
			      comp.sand * sandCp +
			      comp.clay * clayCp +
			      comp.silt * siltCp) / total;

		          return cp;
	     }
}