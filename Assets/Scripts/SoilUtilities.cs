using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SoilUtilities
{
	     //soil
	     public static SoilComposition CalculateDerivedProperties(SoilComposition sc)
	     {
		          // All calculations now use kg/m2 for water (equivalent to liters for 1m2 area)

		          // 1. Solid volume fractions (m3/m3)
		          float sandVol = sc.sand.Mass / Constants.SAND_DENSITY;
		          float siltVol = sc.silt.Mass / Constants.SILT_DENSITY;
		          float clayVol = sc.clay.Mass / Constants.CLAY_DENSITY;
		          float organicVol = sc.organicMaterial.Mass / Constants.ORGANIC_DENSITY;
		          float totalSolidVol = sandVol + siltVol + clayVol + organicVol;

		          if (totalSolidVol <= 0f)
				totalSolidVol = Mathf.Epsilon; // Avoid division by zero

		          // 2. Texture percentages
		          float sandPct = sandVol / totalSolidVol * 100f;
		          float siltPct = siltVol / totalSolidVol * 100f;
		          float clayPct = clayVol / totalSolidVol * 100f;

		          // 3. Porosity & bulk density
		          float rawPorosity = CalculatePorosity(sandPct, clayPct, sc.organicMaterial.Mass);
		          sc.porosity = Mathf.Clamp01(rawPorosity * (1f - sc.compaction * 0.85f));

		          sc.bulkDensity = (sc.sand.Mass + sc.silt.Mass + sc.clay.Mass + sc.organicMaterial.Mass) / totalSolidVol;
		          sc.bulkDensity *= 1f + (sc.compaction * 0.2f);

		          // 4. Water properties - now using kg/m2 (equivalent to liters for 1m2 area)
		          sc.waterCapacity = CalculateWaterCapacity(sc.porosity, clayPct) * 1000f; // Convert m3/m3 to kg/m2
		          sc.saturation = sc.waterCapacity > 0f ? Mathf.Clamp01(sc.water.Mass / sc.waterCapacity) : 0f;

		          // 5. Flow properties
		          sc.infiltrationRate = CalculateInfiltrationRate(sandPct, sc.saturation) * (1f - sc.compaction * 0.9f);

		          float safePorosity = Mathf.Clamp(sc.porosity, 0.01f, 0.95f);
		          sc.percolationRate = CalculatePercolationRate(safePorosity, sandPct, clayPct) * (1f - sc.compaction * 0.9f);
		          sc.hydraulicConductivity = sc.percolationRate * 24f;

		          sc.horizontalPermeability = CalculateHorizontalPermeability(sandPct, clayPct, sc.porosity) * (1f - sc.compaction);
		          sc.verticalPermeability = CalculateVerticalPermeability(sandPct, clayPct, sc.porosity) * (1f - sc.compaction);

		          // 6. Structural properties
		          sc.aeration = sc.porosity * (1f - sc.saturation);
		          sc.erodibility = CalculateErodibility(sandPct, clayPct, sc.organicMaterial.Mass);

		          return sc;
	     }

	     public static float GetSoilMass(SoilComposition sc)
	     {
		          return sc.sand.Mass + sc.silt.Mass + sc.clay.Mass + sc.organicMaterial.Mass + sc.water.Mass;
	     }
	     static float CalculatePorosity(float sandPct, float clayPct, float organicMaterial)
	     {
		          // Saxton & Rawls (2006) model with organic matter adjustment
		          return 0.45f - 0.0012f * sandPct + 0.0021f * clayPct +
		              0.025f * (organicMaterial / Constants.ORGANIC_DENSITY);
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

		          //hydraulic conductivity is used to determine how quickly water can move through soil when fully saturated
	     }

	     static float CalculateErodibility(float sandPct, float clayPct, float organicMaterial)
	     {
		          // Revised USLE K-factor approximation
		          float sandFactor = 0.2f + 0.3f * (sandPct / 100f);
		          float clayFactor = 0.3f * (clayPct / 100f);
		          float omFactor = 0.2f * (1f - organicMaterial / Constants.ORGANIC_DENSITY);
		          return Mathf.Clamp01(sandFactor + clayFactor + omFactor);
	     }
	     //  public static void ApplySurfaceErosion(TileData source, TileData target, float gradient, float waterMoved)
	     //  {
	     //        // Apply erosion and sediment transfer between tiles based on surface water movement
	     //        var sc = source.composition;
	     //        var tc = target.composition;

	     //        sc.erodibility = CalculateErodibility(sc.sand.Mass, sc.clay.Mass, sc.organicMaterial.Mass);

	     //        float erosionFactor = gradient * sc.erodibility * 0.05f;
	     //        float sedimentMass = erosionFactor * waterMoved;

	     //        float fineTotal = sc.silt.Mass + sc.clay.Mass;
	     //        if (fineTotal > 0f && sedimentMass > 0f)
	     //        {
	     //float siltRatio = sc.silt.Mass / fineTotal;
	     //float clayRatio = sc.clay.Mass / fineTotal;

	     //float siltMoved = sedimentMass * siltRatio;
	     //float clayMoved = sedimentMass * clayRatio;

	     //siltMoved = Mathf.Min(siltMoved, sc.silt.Mass);
	     //clayMoved = Mathf.Min(clayMoved, sc.clay.Mass);

	     //float organicMoved = sedimentMass * 0.1f;
	     //organicMoved = Mathf.Min(organicMoved, sc.organicMaterial.Mass);

	     //ThermalEnergyUtilities.TransferMassAndEnergy(sc.silt, tc.silt, siltMoved);
	     //ThermalEnergyUtilities.TransferMassAndEnergy(sc.clay, tc.clay, clayMoved);
	     //ThermalEnergyUtilities.TransferMassAndEnergy(sc.organicMaterial, tc.organicMaterial, organicMoved);

	     //float erosionVolume = sedimentMass / sc.bulkDensity;
	     //source.volume -= erosionVolume;
	     //target.volume += erosionVolume;

	     //RecalculateTilePhysics(source);
	     //RecalculateTilePhysics(target);
	     //        }
	     //  }
	     public static float CalculateFlowVelocityDarcyWeisbach(float waterDepth, float gradient, float frictionFactor = 0.05f)
	     {
		          // Darcy-Weisbach equation for open channel flow
		          // v = √(8g * R * S / f)
		          // where:
		          // g = gravitational acceleration (9.81 m/s²)
		          // R = hydraulic radius ≈ water depth for overland flow
		          // S = slope gradient
		          // f = Darcy-Weisbach friction factor

		          if (waterDepth <= 0.001f || gradient <= 0f)
				return 0f;

		          float g = 9.81f;
		          float velocity = Mathf.Sqrt((8f * g * waterDepth * gradient) / frictionFactor);

		          return Mathf.Min(velocity, 10f);
	     }
	     public static void ApplySurfaceErosion(TileData source, TileData target, float gradient, float waterMoved)
	     {
		          float flowVelocity = CalculateFlowVelocityDarcyWeisbach(source.surfaceLiquid.Mass / 100, gradient);
		          // Apply erosion and sediment transfer between tiles based on surface water movement
		          var sc = source.composition;
		          var tc = target.composition;

		          // Calculate flow competence (ability to transport different particle sizes)
		          // Flow velocity determines what size particles can be transported
		          float criticalVelocitySand = 0.3f; // m/s - velocity needed to move sand
		          float criticalVelocitySilt = 0.1f;  // m/s - velocity needed to move silt
		          float criticalVelocityClay = 0.05f; // m/s - velocity needed to move clay

		          sc.erodibility = CalculateErodibility(sc.sand.Mass, sc.clay.Mass, sc.organicMaterial.Mass);

		          float erosionFactor = gradient * sc.erodibility * 0.05f;
		          float sedimentMass = erosionFactor * waterMoved;

		          // Determine which particle sizes can be transported based on flow velocity
		          float sandTransportFactor = Mathf.Clamp01((flowVelocity - criticalVelocitySand) / criticalVelocitySand);
		          float siltTransportFactor = Mathf.Clamp01((flowVelocity - criticalVelocitySilt) / criticalVelocitySilt);
		          float clayTransportFactor = Mathf.Clamp01((flowVelocity - criticalVelocityClay) / criticalVelocityClay);

		          // Organic matter is typically easier to transport
		          float organicTransportFactor = Mathf.Clamp01(flowVelocity / criticalVelocityClay);

		          // Calculate how much of each particle size can be moved
		          float totalAvailableMass = sc.sand.Mass + sc.silt.Mass + sc.clay.Mass + sc.organicMaterial.Mass;

		          if (totalAvailableMass > 0f && sedimentMass > 0f)
		          {
				// Distribute sediment transport across all particle sizes based on availability and transportability
				float sandFraction = sc.sand.Mass / totalAvailableMass * sandTransportFactor;
				float siltFraction = sc.silt.Mass / totalAvailableMass * siltTransportFactor;
				float clayFraction = sc.clay.Mass / totalAvailableMass * clayTransportFactor;
				float organicFraction = sc.organicMaterial.Mass / totalAvailableMass * organicTransportFactor;

				float totalTransportFactor = sandFraction + siltFraction + clayFraction + organicFraction;

				if (totalTransportFactor > 0f)
				{
					     // Normalize fractions
					     sandFraction /= totalTransportFactor;
					     siltFraction /= totalTransportFactor;
					     clayFraction /= totalTransportFactor;
					     organicFraction /= totalTransportFactor;

					     float sandMoved = sedimentMass * sandFraction;
					     float siltMoved = sedimentMass * siltFraction;
					     float clayMoved = sedimentMass * clayFraction;
					     float organicMoved = sedimentMass * organicFraction;

					     // Ensure we don't move more than what's available
					     sandMoved = Mathf.Min(sandMoved, sc.sand.Mass);
					     siltMoved = Mathf.Min(siltMoved, sc.silt.Mass);
					     clayMoved = Mathf.Min(clayMoved, sc.clay.Mass);
					     organicMoved = Mathf.Min(organicMoved, sc.organicMaterial.Mass);

					     // Transfer all particle sizes that can be moved
					     if (sandMoved > 0f)
						          ThermalEnergyUtilities.TransferMassAndEnergy(sc.sand, tc.sand, sandMoved);

					     if (siltMoved > 0f)
						          ThermalEnergyUtilities.TransferMassAndEnergy(sc.silt, tc.silt, siltMoved);

					     if (clayMoved > 0f)
						          ThermalEnergyUtilities.TransferMassAndEnergy(sc.clay, tc.clay, clayMoved);

					     if (organicMoved > 0f)
						          ThermalEnergyUtilities.TransferMassAndEnergy(sc.organicMaterial, tc.organicMaterial, organicMoved);

					     float totalMoved = sandMoved + siltMoved + clayMoved + organicMoved;
					     float erosionVolume = totalMoved / sc.bulkDensity;

					     source.volume -= erosionVolume;
					     target.volume += erosionVolume;

					     RecalculateTilePhysics(source);
					     RecalculateTilePhysics(target);
				}
		          }
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



	     public static void ApplySelfCompaction(TileData tileData, float hours)
	     {
		          SoilComposition sc = tileData.composition;
		          float weight = sc.bulkDensity * SimulationConstants.Gravity; // N/m2
		          float selfPressure_kPa = weight / 1000f; // Convert to kPa
		          UpdateCompaction(sc, hours, appliedPressure: selfPressure_kPa);
		          CalculateDerivedProperties(sc);
		          //    RecalculateTilePhysics(tileData);
	     }

	     public static void UpdateDecompaction(SoilComposition sc, float hours)
	     {
		          float dryFactor = 1f - sc.saturation;
		          float decompactionRate = dryFactor * 0.001f * hours;
		          sc.compaction = Mathf.Clamp01(sc.compaction - decompactionRate);
	     }

	     public static void RecalculateTilePhysics(TileData data)
	     {
		          float tileArea = data.tileWidth * data.tileWidth;

		          // ---- 1. All solid materials (bedrock + soil minerals) ----
		          float solidVolume =
		              (data.bedrock.Mass / Constants.GRANITE_DENSITY) +
		              (data.composition.sand.Mass / Constants.SAND_DENSITY) +
		              (data.composition.silt.Mass / Constants.SILT_DENSITY) +
		              (data.composition.clay.Mass / Constants.CLAY_DENSITY) +
		              (data.composition.organicMaterial.Mass / Constants.ORGANIC_DENSITY);

		          // ---- 2. Soil pore water & ice (inside the soil column) ----
		          float soilWaterVol = data.composition.water.Mass / Constants.WATER_DENSITY;
		          float soilIceVol = data.composition.ice.Mass / Constants.ICE_DENSITY;

		          // ---- 3. Aeration (air in pores) ----
		          // IMPORTANT: In CalculateDerivedProperties you set:
		          //   sc.aeration = sc.porosity * (1f - sc.saturation);
		          // This is a fraction (0–1). Since you treat it as an absolute volume in CalculateSoilVolume,
		          // we do the same here to keep everything consistent.
		          float aerationVol = data.composition.aeration;

		          // ---- 4. Total soil column volume (everything that makes up the "ground") ----
		          float soilColumnVolume = solidVolume + soilWaterVol + soilIceVol + aerationVol;

		          // ---- 5. groundElevation = the physical ground surface (includes air & soil water!) ----
		          data.groundElevation = soilColumnVolume / tileArea;

		          // ---- 6. Surface water & ice (ON TOP of the ground) ----
		          float surfaceWaterVol = data.surfaceLiquid.Mass / Constants.WATER_DENSITY;
		          float surfaceIceVol = data.surfaceIce.Mass / Constants.ICE_DENSITY;

		          // ---- 7. Total volume (this is the unified height source for visuals) ----
		          data.volume = soilColumnVolume + surfaceWaterVol + surfaceIceVol;
	     }

	     public static void UpdateSoilTemperature(TileData data, float hours)
	     {
		          ThermalEnergy[] energyData = data.composition.energy;
		          var sorted = energyData.OrderByDescending(v => v.Mass).ToArray();
		          ThermalEnergy largestComponent = sorted[0];
		          float area = data.tileWidth * data.tileWidth;
		          float distance = data.tileWidth;
		          //surface ice and water need to conduct heat to/from each other. 
		          if (data.surfaceIce.Mass > 0f)
		          {
				ThermalEnergyUtilities.ConductHeat(data.surfaceIce, data.surfaceLiquid, area, distance, hours * 3600f);
		          }
		          if (data.surfaceLiquid.Mass > 0f)
		          {
				//transfer the heat between the soil and the surface

				ThermalEnergyUtilities.ConductHeat(data.surfaceLiquid, data.atmosphereTile.nitrogen, area, distance, hours * 3600);
				ThermalEnergyUtilities.ConductHeat(data.surfaceLiquid, largestComponent, area, distance, hours * 3600);
		          }
		          else
		          {
				//radiate heat to the atmosphere from the largest thermal mass in the soil.
				ThermalEnergyUtilities.ConductHeat(data.atmosphereTile.nitrogen, largestComponent, area, distance, hours * 3600);
		          }
		          //distribute heat from the largest component to the smaller components
		          for (int i = 1; i < sorted.Length; i++)
		          {
				ThermalEnergyUtilities.ConductHeat(sorted[0], sorted[i], area, distance, hours / sorted.Length * 3600f);
		          }
	     }
	     //  public static float PassHeatFromSoil(TileData data, float hours, TileManager tileManager)
	     //  {
	     //        SoilComposition sc = data.composition;
	     //        float latentHeat = Constants.LATENT_HEAT_VAPORIZATION;

	     //        // Soil thermal properties
	     //        float soilMass = data.composition.sand.Mass + data.composition.silt.Mass + data.composition.clay.Mass + data.composition.organicMaterial.Mass + data.composition.water.Mass; // kg/m2 (for top 1m of soil)
	     //        float heatCapacity = GetSoilHeatCapacity(sc);

	     //        // Air properties (for the entire atmospheric column)
	     //        float airMassPerTile = 3000f * Constants.AIR_DENSITY; // 3km column of air per tile
	     //        float totalAirMass = airMassPerTile * tileManager.worldsise.x * tileManager.worldsise.y;

	     //        // 1. Calculate heat transfer between soil and air
	     //        float heatTransferCoefficient = 10f; // W/m2·K (typical for soil-air interface)
	     //        float temperatureDifference = data.soilTemperature - tileManager.airTemp;
	     //        float heatFlowRate = heatTransferCoefficient * temperatureDifference; // Watts per m2

	     //        // Convert to energy over time period (Joules)
	     //        float energyTransferred = heatFlowRate * hours * 3600f;

	     //        // 2. Calculate maximum possible evaporation
	     //        float maxEvaporableKg = energyTransferred / latentHeat;
	     //        float surfaceLiquid = data.surfaceLiquid.Mass;
	     //        float evaporatedKg = Mathf.Min(surfaceLiquid, maxEvaporableKg);

	     //        //// 3. Apply evaporation effects
	     //        //data.surfaceMoisture -= evaporatedKg;
	     //        //data.atmosphereTile.waterVapor += evaporatedKg;
	     //        //this is just to keep it running but we now haven't factored in energy for phase changes.
	     //        ThermalEnergyUtilities.TransferMassAndEnergy(data.surfaceLiquid, data.atmosphereTile.waterVapor, evaporatedKg);

	     //        // 4. Calculate energy used for evaporation
	     //        float energyUsed = evaporatedKg * latentHeat;

	     //        // 5. Calculate remaining energy for sensible heat transfer
	     //        float sensibleHeatEnergy = energyTransferred - energyUsed;

	     //        // 6. Update temperatures
	     //        if (sensibleHeatEnergy > 0)
	     //        {
	     //// Soil cools and air warms
	     //float soilDeltaT = sensibleHeatEnergy / (soilMass * heatCapacity);
	     ////data.soilTemperature = Mathf.Max(tileManager.airTemp - 5f, data.soilTemperature - soilDeltaT);

	     //// Only return the portion that should affect the global air temperature
	     //// (since this is called per tile, we return the energy and let TileManager handle the aggregation)
	     //return sensibleHeatEnergy;
	     //        }
	     //        else
	     //        {
	     //// All energy went to evaporation
	     //return 0f;
	     //        }
	     //  }

	     public static float GetSoilHeatCapacity(SoilComposition comp)
	     {
		          float waterCp = 4186f;
		          float organicCp = 2500f;
		          float sandCp = 800f;
		          float clayCp = 900f;
		          float siltCp = 900f;

		          float total = comp.sand.Mass + comp.clay.Mass + comp.silt.Mass + comp.organicMaterial.Mass + comp.water.Mass;
		          if (total == 0)
				return 1000f;

		          float cp = (comp.water.Mass * waterCp +
			      comp.organicMaterial.Mass * organicCp +
			      comp.sand.Mass * sandCp +
			      comp.clay.Mass * clayCp +
			      comp.silt.Mass * siltCp) / total;

		          return cp;
	     }


	     //  public static void ApplyRainfall(TileData tileData, float kgRain, float hours)
	     //  {
	     //        if (kgRain <= 0f)
	     //return;

	     //        //Debug.Log($"[Rainfall] Adding {kgRain} kg/m2 to tile at {Time.time} s");

	     //        // Add water to surface moisture directly
	     //        tileData.surfaceMoisture += kgRain;
	     //        //Debug.Log($"[Rainfall] Surface moisture now: {tileData.surfaceMoisture} kg/m2");

	     //        // Apply compaction from weight of water
	     //        float pressure = kgRain * Constants.GRAVITY_ACCELERATION; // N/m2
	     //        float pressure_kPa = pressure / 1000f;
	     //        UpdateCompaction(tileData.composition, hours, appliedPressure: pressure_kPa);
	     //        RecalculateTilePhysics(tileData);
	     //  }


	     public static void ConductHeatLateral(WorldTile tile, float hours)
	     {//this is conducting heat to our neighbors

		          SoilComposition sc = tile.tileData.composition;
		          List<WorldTile> neighbors = tile.GetNeighbors();

		          ThermalEnergy[] scEnergyData = sc.energy;
		          var scSorted = scEnergyData.OrderByDescending(v => v.Mass).ToArray();
		          // ThermalEnergy largestComponent = scSorted[0];

		          float area = tile.tileData.tileWidth * tile.tileData.tileWidth;
		          float distance = tile.tileData.tileWidth;

		          hours = hours / scSorted.Length;

		          foreach (WorldTile neighbor in neighbors)
		          {
				if (neighbor == null)
					     continue;


				SoilComposition nc = neighbor.tileData.composition;

				ThermalEnergy[] ncEnergyData = nc.energy;
				var ncSorted = ncEnergyData.OrderByDescending(v => v.Mass).ToArray();
				//ThermalEnergy ncLargestComponent = ncSorted[0];

				for (int i = 0; i < ncSorted.Length; i++)
				{
					     ThermalEnergyUtilities.ConductHeat(scEnergyData[i], ncSorted[i], area, distance, hours * 3600f);
				}
				//ThermalEnergyUtilities.ConductHeat(largestComponent, ncLargestComponent, area, distance, hours * 3600f);

		          }

		          //    Debug.Log($"[LateralWater] After redistribution: Total water = {GetTotalWater(tile)} kg");
	     }

	     public static void InfiltrateSurfaceMoisture(TileData tileData, float hours)
	     {
		          var sc = tileData.composition;

		          // Handle overflow first
		          if (sc.water.Mass > sc.waterCapacity)
		          {
				float overflow = sc.water.Mass - sc.waterCapacity;
				//sc.water = sc.waterCapacity;
				//tileData.surfaceMoisture += overflow;
				ThermalEnergyUtilities.TransferMassAndEnergy(sc.water, tileData.surfaceLiquid, overflow);
				//Debug.Log($"[Infiltration] Overflow detected: {overflow}, moved back to surface. Soil water: {sc.water}, Surface moisture: {tileData.surfaceMoisture}");
		          }

		          if (tileData.surfaceLiquid.Mass <= 0f)
				return;

		          float compactionModifier = Mathf.Lerp(1f, 0.2f, sc.compaction);
		          float saturationModifier = Mathf.Clamp01(1f - sc.saturation);

		          float area = tileData.tileWidth * tileData.tileWidth;
		          float maxInfiltration = sc.infiltrationRate * area * hours;   // kg
		          float effectiveInfiltration = maxInfiltration * compactionModifier * saturationModifier;
		          float infiltrated = Mathf.Min(tileData.surfaceLiquid.Mass, effectiveInfiltration);
		          float availableCapacity = sc.waterCapacity - sc.water.Mass;
		          float absorbed = Mathf.Min(infiltrated, availableCapacity);

		          //sc.water += absorbed;
		          //tileData.surfaceMoisture -= absorbed;
		          ThermalEnergyUtilities.TransferMassAndEnergy(tileData.surfaceLiquid, sc.water, absorbed);

		          //Debug.Log($"[Infiltration] Attempted: {infiltrated}, Absorbed: {absorbed}, Soil water: {sc.water}, Surface moisture: {tileData.surfaceMoisture}");
		          CalculateDerivedProperties(sc);
		          //RecalculateTilePhysics(tileData);
	     }

	     public static void UpdateCompaction(SoilComposition sc, float hours, float appliedPressure = 1f)
	     {
		          float compactionChange = sc.saturation * appliedPressure * 0.001f * hours;
		          sc.compaction = Mathf.Clamp01(sc.compaction + compactionChange);
		          // Debug.Log($"[Compaction] Applied pressure: {appliedPressure} kPa, Saturation: {sc.saturation}, New compaction: {sc.compaction}");
	     }

}