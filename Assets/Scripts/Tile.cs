using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
	     public TileData tileData;
	     float tileSise;
	     public bool isHovered = false;
	     public bool visible = true;
	     public WorldTile NWTile;
	     public WorldTile NTile;
	     public WorldTile NETile;
	     public WorldTile WTile;
	     public WorldTile ETile;
	     public WorldTile SWTile;
	     public WorldTile STile;
	     public WorldTile SETile;

	     public void Setup(TileData td)
	     {
		          tileData = td;
		          tileSise = tileData.tileWidth;
		          //draw  the  tile based on the volume of  material   in  it
		          transform.localScale = new Vector3(tileSise, tileSise, tileSise);
	     }

	     // Update is called once per frame
	     void Update()
	     {
		          if (isHovered)
		          {
				GetComponent<Renderer>().material.color = Color.yellow;
		          }
	     }

	     public void TileVisualUpdate()
	     {
		          SoilUtilities.CalculateDerivedProperties(tileData.composition);
		          float height = SoilUtilities.CalculateEffectiveVolume(tileData.composition) + (tileData.surfaceWater.Mass / 1000);
		          //latitude += tileData.tileGridRef.y / 3; // Adjust latitude based on tile position for variety
		          transform.localScale = new Vector3(tileSise, height, tileSise);

		          // Set position so that bottom of tile sits on ground
		          // Since scale.y is height, center is at height / 2
		          transform.position = new Vector3(
		              tileData.tileGridRef.x,
		              height / 2f,
		              tileData.tileGridRef.y
		          );
		          Color tileColor = TileUtilities.GetSoilColor(tileData.composition);
		          //set colour of tile based on composition and moisture
		          if (tileData.surfaceWater.Mass > 300)
		          {
				tileColor = Color.Lerp(tileColor, Constants.DEEP_WATER_COLOR, tileData.surfaceWater.Mass / 100f);
		          }
		          if (tileData.surfaceWater.Mass > 15)
		          {
				if (tileData.surfaceWater.CurrentPhase == Phase.Liquid)
				{
					     tileColor = Color.Lerp(tileColor, Constants.SHALLOW_WATER_COLOR, tileData.surfaceWater.Mass / 100f);
				}
				else if (tileData.surfaceWater.CurrentPhase == Phase.Solid)
				{
					     tileColor = Color.Lerp(tileColor, Constants.ICE_COLOR, tileData.surfaceWater.Mass / 100f);
				}

		          }
		          //convert longitude to a time of day (0-1) from -180 to 180 degrees
		          float longitude = tileData.longitude; // -180 to 180
		          float localSolarTime = SimulationConstants.TimeOfDay + (longitude / 360f);
		          localSolarTime = localSolarTime % 1f; // Ensure stays in [0,1]
		          float currentEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(
		              tileData.latitude, SimulationConstants.DayOfYear, localSolarTime);
		          // Blend tile color based on solar energy
		          float lightFactor = Mathf.Clamp01(currentEnergy / SimulationConstants.SolarConstant);
		          tileColor = Color.Lerp(tileColor, Color.white, lightFactor * 0.3f); // Blend with white based on light factor
							   //blend with black based on how much light there isn't
		          tileColor = Color.Lerp(tileColor, Color.black, (1 - lightFactor) * 0.85f); // Blend with black based on light factor
		          GetComponent<Renderer>().material.color = tileColor;

		          //get the NW corner of the mesh and set its height to the average of the North, West and Northwest tiles.

	     }

	     public List<WorldTile> GetNeighbors()
	     {
		          return new List<WorldTile> { NTile, STile, ETile, WTile, NETile, NWTile, SETile, SWTile };
	     }


}
