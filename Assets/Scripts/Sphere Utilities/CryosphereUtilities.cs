using System.Collections.Generic;
using UnityEngine;

public static class CryosphereUtilities
{
	     // Static reusable collections to reduce allocations
	     private static List<(WorldTile tile, float slope)> _downhillWater = new List<(WorldTile, float)>(8);
	     private static List<(WorldTile tile, float slope)> _downhillIce = new List<(WorldTile, float)>(8);
	     private static Dictionary<WorldTile, float> _waterToMove = new Dictionary<WorldTile, float>(8);
	     private static Dictionary<WorldTile, float> _iceToMove = new Dictionary<WorldTile, float>(8);
	     private static Dictionary<WorldTile, float> _sedimentToMove = new Dictionary<WorldTile, float>(8);

	     public static void RedistributeSurfaceWaterAndIce(WorldTile tile, float hours)
	     {
		          TileData data = tile.tileData;
		          SoilUtilities.RecalculateTilePhysics(data);

		          if (data.surfaceLiquid.Mass <= 0f && data.surfaceIce.Mass <= 0f)
				return;

		          // Clear static collections
		          _downhillWater.Clear();
		          _downhillIce.Clear();
		          _waterToMove.Clear();
		          _iceToMove.Clear();
		          _sedimentToMove.Clear();

		          // Get wind
		          float windSpeed = data.atmosphereTile.windSpeed;
		          Vector3 windDir = data.atmosphereTile.windVelocity.normalized;

		          // ---- 1. Compute downhill neighbours for water and ice ----
		          TileUtilities.GetDownhillWaterNeighbors(tile, _downhillWater);
		          TileUtilities.GetDownhillIceNeighbors(tile, _downhillIce);

		          // ---- 2. Determine if ice is floating ----	          

		          //     float delta = hours * 3600f;
		          float cellSize = data.tileWidth;

		          // ---- 3. Water movement (liquid) ----
		          // Inside RedistributeSurfaceWaterAndIce, after getting _downhillWater

		          // Sort by slope descending (steepest first)
		          _downhillWater.Sort((a, b) => a.slope.CompareTo(b.slope));

		          foreach (var (neighbor, slope) in _downhillWater)
		          {
				if (data.surfaceLiquid.Mass <= 0f || data.volume < neighbor.tileData.volume)
					     break;

				// Compute current water surfaces (could be cached)
				float area = data.tileWidth * data.tileWidth;
				float myDepth = data.surfaceLiquid.Mass / (Constants.WATER_DENSITY * area);
				float mySurface = data.groundElevation + myDepth;

				TileData nd = neighbor.tileData;
				float nDepth = nd.surfaceLiquid.Mass / (Constants.WATER_DENSITY * area);
				float nSurface = nd.groundElevation + nDepth;

				// The amount of water needed to raise neighbor's surface to match mine
				float heightDiff = mySurface - nSurface; // positive
				float neededVolume = heightDiff * area;
				float neededMass = neededVolume * Constants.WATER_DENSITY;

				// Cap by available water
				float massToMove = Mathf.Lerp(0, data.surfaceIce.Mass, slope);

				if (massToMove > 0f)
				{
					     float flowStrength = massToMove * slope; // or use a more physical formula
								 // Erode from source and deposit to neighbor (similar to your existing MoveSediment)
					     float entrainment = Constants.SEDIMENT_ENTRAINMENT_COEFFICIENT * flowStrength / data.surfaceLiquid.Mass;
					     // Move water (and energy)
					     ThermalEnergyUtilities.TransferMassAndEnergy(
					         data.surfaceLiquid,
					         nd.surfaceLiquid,
					         massToMove
					     );

					     // Now move sediment based on flow strength (use the slope and mass moved)

					     MoveSediment(tile, neighbor, entrainment, ref _sedimentToMove);

					     // Recalculate physics on both tiles (or just surfaces)
					     SoilUtilities.RecalculateTilePhysics(data);
					     SoilUtilities.RecalculateTilePhysics(nd);
				}
		          }

		          // ---- 5. Ice movement (solid) ----
		          bool isFloating = IsIceFloating(data);
		          // float maxMoveFraction = isFloating ? 0.2f : 0.01f; // 20% per hour if floating, 1% if grounded
		          _downhillIce.Sort((a, b) => b.slope.CompareTo(a.slope));

		          foreach (var (neighbor, slope) in _downhillIce)
		          {
				if (data.surfaceIce.Mass <= 0f)
					     break;

				// Compute speed: gravity + wind (if floating)
				float gravitySpeed = Constants.ICE_FLOW_RESISTANCE * slope;
				float windComponent = 0f;
				if (isFloating)
				{
					     Vector3 dirToNeighbor = (neighbor.transform.position - tile.transform.position).normalized;
					     windComponent = Vector3.Dot(windDir, dirToNeighbor) * Constants.WIND_DRAG_COEFFICIENT * windSpeed;
				}
				float totalSpeed = Mathf.Max(0f, gravitySpeed + windComponent);
				if (totalSpeed <= 0f)
					     continue;

				float massToMove = Mathf.Lerp(0, data.surfaceIce.Mass, totalSpeed);

				if (massToMove > 0f)
				{
					     // Move ice
					     ThermalEnergyUtilities.TransferMassAndEnergy(data.surfaceIce, neighbor.tileData.surfaceIce, massToMove);

					     // Glacial erosion (only if grounded)
					     if (!isFloating)
					     {
						          float erosionRate = Constants.GLACIAL_EROSION_COEFFICIENT * totalSpeed * slope;
						          float sedimentMass = erosionRate * massToMove;
						          MoveSediment(tile, neighbor, sedimentMass, ref _sedimentToMove);
					     }

					     // Recalc physics
					     SoilUtilities.RecalculateTilePhysics(data);
					     SoilUtilities.RecalculateTilePhysics(neighbor.tileData);
				}
		          }

		          // ---- 7. Clamp total fractions per tile to avoid CFL violation ----
		          ClampTotalFraction(data.surfaceLiquid.Mass, ref _waterToMove);
		          ClampTotalFraction(data.surfaceIce.Mass, ref _iceToMove);
		          // Sediment is moved as a fraction of water/ice mass, so we don't need separate clamp

		          // ---- 8. Apply transfers ----
		          ApplySedimentTransfers(data, _sedimentToMove);
	     }

	     public static bool IsIceFloating(TileData data)
	     {
		          if (data.surfaceIce.Mass <= 0f || data.surfaceLiquid.Mass <= 0f)
				return false;
		          float iceThickness = data.surfaceIce.Mass / (data.tileWidth * data.tileWidth * Constants.ICE_DENSITY);
		          float waterDepth = data.surfaceLiquid.Mass / (data.tileWidth * data.tileWidth * Constants.WATER_DENSITY);
		          // Ice floats if water depth > ice thickness * (ice_density / water_density) * threshold
		          float requiredWaterDepth = iceThickness * (Constants.ICE_DENSITY / Constants.WATER_DENSITY) * Constants.ICE_FLOATING_THRESHOLD;
		          return waterDepth > requiredWaterDepth;
	     }

	     private static void MoveSediment(WorldTile sourceTile, WorldTile destTile, float massFraction, ref Dictionary<WorldTile, float> sedimentDict)
	     {
		          TileData source = sourceTile.tileData;
		          TileData dest = destTile.tileData;
		          float totalSediment = source.composition.sand.Mass + source.composition.silt.Mass + source.composition.clay.Mass + source.composition.organicMaterial.Mass;
		          if (totalSediment <= 0f)
				return;
		          float moveMass = totalSediment * massFraction;
		          if (moveMass > 0f)
				sedimentDict[destTile] = sedimentDict.GetValueOrDefault(destTile, 0f) + moveMass;
	     }

	     private static void ClampTotalFraction(float sourceMass, ref Dictionary<WorldTile, float> dict)
	     {
		          if (sourceMass <= 0f || dict.Count == 0)
				return;
		          float totalFraction = 0f;
		          foreach (var kv in dict)
				totalFraction += kv.Value / sourceMass;
		          if (totalFraction > 1f)
		          {
				float scale = 1f / totalFraction;
				var keys = new List<WorldTile>(dict.Keys);
				foreach (var k in keys)
					     dict[k] *= scale;
		          }
	     }

	     //  private static void ApplyTransfers(ThermalEnergy source, Dictionary<WorldTile, float> dict)
	     //  {
	     //        foreach (var kv in dict)
	     //        {
	     //if (kv.Value <= 0f)
	     //	     continue;
	     //ThermalEnergy dest = kv.Key.tileData.surfaceLiquid; // but for ice it's surfaceIce
	     //			       // We need to pass the correct destination component – we'll handle separately.
	     //			       // We'll split into two methods.
	     //        }
	     //  }

	     // We'll need separate ApplyTransfers for liquid and ice because destination differs.
	     private static void ApplyLiquidTransfers(TileData sourceData, Dictionary<WorldTile, float> dict)
	     {
		          foreach (var kv in dict)
		          {
				if (kv.Value <= 0f)
					     continue;
				ThermalEnergyUtilities.TransferMassAndEnergy(sourceData.surfaceLiquid, kv.Key.tileData.surfaceLiquid, kv.Value);
		          }
	     }

	     private static void ApplyIceTransfers(TileData sourceData, Dictionary<WorldTile, float> dict)
	     {
		          foreach (var kv in dict)
		          {
				if (kv.Value <= 0f)
					     continue;
				ThermalEnergyUtilities.TransferMassAndEnergy(sourceData.surfaceIce, kv.Key.tileData.surfaceIce, kv.Value);
		          }
	     }

	     private static void ApplySedimentTransfers(TileData source, Dictionary<WorldTile, float> dict)
	     {
		          if (dict.Count == 0)
				return;
		          // Compute total sediment mass in source
		          float totalSediment = source.composition.sand.Mass + source.composition.silt.Mass + source.composition.clay.Mass + source.composition.organicMaterial.Mass;
		          if (totalSediment <= 0f)
				return;

		          foreach (var kv in dict)
		          {
				if (kv.Value <= 0f)
					     continue;
				// fraction of total sediment to move
				float fraction = kv.Value / totalSediment;
				// Move each component proportionally
				TileData dest = kv.Key.tileData;
				// Sand
				float sandMass = source.composition.sand.Mass * fraction;
				if (sandMass > 0f)
					     ThermalEnergyUtilities.TransferMassAndEnergy(source.composition.sand, dest.composition.sand, sandMass);
				// Silt
				float siltMass = source.composition.silt.Mass * fraction;
				if (siltMass > 0f)
					     ThermalEnergyUtilities.TransferMassAndEnergy(source.composition.silt, dest.composition.silt, siltMass);
				// Clay
				float clayMass = source.composition.clay.Mass * fraction;
				if (clayMass > 0f)
					     ThermalEnergyUtilities.TransferMassAndEnergy(source.composition.clay, dest.composition.clay, clayMass);
				// Organic
				float organicMass = source.composition.organicMaterial.Mass * fraction;
				if (organicMass > 0f)
					     ThermalEnergyUtilities.TransferMassAndEnergy(source.composition.organicMaterial, dest.composition.organicMaterial, organicMass);
				SoilUtilities.RecalculateTilePhysics(dest);
		          }
		          SoilUtilities.RecalculateTilePhysics(source);
	     }
}