using System.Collections.Generic;
using UnityEngine;

public static class SoilUtilities
{


	     public static SoilComposition CalculateDerivedProperties(SoilComposition sc)
	     {
		          // 1. Calculate volume fractions (m³/m³)
		          float sandVol = sc.sand / Constants.SAND_DENSITY;
		          float siltVol = sc.silt / Constants.SILT_DENSITY;
		          float clayVol = sc.clay / Constants.CLAY_DENSITY;
		          float organicVol = sc.organicMatter / Constants.ORGANIC_DENSITY;
		          float totalSolidVol = sandVol + siltVol + clayVol + organicVol;

		          // 2. Texture percentages (0-100)
		          float sandPct = (sandVol / totalSolidVol) * 100f;
		          float siltPct = (siltVol / totalSolidVol) * 100f;
		          float clayPct = (clayVol / totalSolidVol) * 100f;
		          float rawPorosity = CalculatePorosity(sandPct, clayPct, sc.organicMatter);
		          // 3. Core properties
		          sc.porosity = rawPorosity * (1f - sc.compaction * 0.85f); // reduce up to 85% at max compaction
		          sc.bulkDensity = (sc.sand + sc.silt + sc.clay + sc.organicMatter) / totalSolidVol;
		          sc.bulkDensity *= 1f + (sc.compaction * 0.2f); // +20% bulk density at full compaction


		          // 4. Water properties
		          sc.waterCapacity = CalculateWaterCapacity(sc.porosity, clayPct);
		          sc.saturation = Mathf.Clamp01(sc.water / (sc.waterCapacity * Constants.WATER_DENSITY));

		          // 5. Flow properties
		          sc.infiltrationRate = CalculateInfiltrationRate(sandPct, sc.saturation) * (1f - sc.compaction * 0.9f);

		          sc.percolationRate = CalculatePercolationRate(sc.porosity, sandPct, clayPct) * (1f - sc.compaction * 0.9f);

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
		          // Field capacity approximation (m³ water/m³ soil)
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
		          // Volume of each solid component (m³/m³)
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

	     public static void UpdateCompaction(SoilComposition sc, float appliedPressure = 1f)
	     {
		          // Saturated soils compact more under pressure
		          float compactionChange = sc.saturation * appliedPressure * 0.005f;
		          sc.compaction = Mathf.Clamp01(sc.compaction + compactionChange);
	     }

	     public static float ApplyEvaporation(SoilComposition sc, float energyWattsPerM2, float hours)
	     {
		          float evapCoeff = 0.0001f; // Tuned empirically
		          float evapRate = energyWattsPerM2 * evapCoeff * hours; // kg/m²
		          float maxEvaporableWater = Mathf.Min(sc.water, sc.saturation * Constants.WATER_DENSITY); // avoid overdraw

		          float evaporated = Mathf.Min(evapRate, maxEvaporableWater);
		          sc.water -= evaporated;

		          CalculateDerivedProperties(sc); // Update saturation, aeration, etc.

		          return evaporated;
	     }


	     public static void RedistributeWaterLateral(WorldTile tile, float hours)
	     {
		          SoilComposition sc = tile.tileData.composition;

		          // Convert cm/day to m³ water per hour per m²
		          float rate = sc.horizontalPermeability / 100f / 24f; // m/hour

		          float totalTransfer = 0f;
		          float waterToMove = sc.water;

		          List<WorldTile> neighbors = tile.GetNeighbors();

		          foreach (WorldTile neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;

				SoilComposition neighborSC = neighbor.tileData.composition;

				float deltaSaturation = sc.saturation - neighborSC.saturation;

				if (deltaSaturation > 0)
				{
					     float transfer = deltaSaturation * rate * hours * Constants.WATER_DENSITY; // kg/m²

					     transfer = Mathf.Min(transfer, waterToMove); // Don't overdraw
					     sc.water -= transfer;
					     neighborSC.water += transfer;

					     waterToMove -= transfer;
					     totalTransfer += transfer;

					     float sedimentFraction = sc.erodibility * Mathf.Clamp01(deltaSaturation); // Adjust based on how erodible the soil is
					     float sedimentTransfer = sedimentFraction * transfer; // Proportional to water moved

					     // Move fine materials (silt and clay are more mobile)
					     float siltRatio = sc.silt / (sc.silt + sc.clay + 0.001f);
					     float clayRatio = sc.clay / (sc.silt + sc.clay + 0.001f);

					     float siltMoved = sedimentTransfer * siltRatio;
					     float clayMoved = sedimentTransfer * clayRatio;

					     // Update soil compositions
					     sc.silt -= siltMoved;
					     neighborSC.silt += siltMoved;

					     sc.clay -= clayMoved;
					     neighborSC.clay += clayMoved;

					     float organicMoved = sedimentTransfer * 0.1f; // or scale by organic concentration

					     sc.organicMatter -= organicMoved;
					     neighborSC.organicMatter += organicMoved;


					     CalculateDerivedProperties(neighborSC); // Update neighbor after change
				}



		          }

		          CalculateDerivedProperties(sc); // Recalculate current tile after redistribution
	     }


}
