using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
	     public TileData tileData;
	     float tileSize;
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
		          tileSize = tileData.tileWidth;
		          //draw  the  tile based on the volume of  material   in  it
		          transform.localScale = new Vector3(tileSize, tileSize, tileSize);
	     }

	     // Update is called once per frame
	     void Update()
	     {
		          if (isHovered)
		          {
				GetComponent<Renderer>().material.color = Color.yellow;
		          }
	     }

	     public void TileVisualUpdate(float latitude)
	     {
		          SoilUtilities.CalculateDerivedProperties(tileData.composition);
		          float height = SoilUtilities.CalculateEffectiveVolume(tileData.composition) + (tileData.surfaceMoisture / 1000);
		          latitude += tileData.tileGridRef.x * 0.1f; // Adjust latitude based on tile position for variety
		          transform.localScale = new Vector3(tileSize, height, tileSize);

		          // Set position so that bottom of tile sits on ground
		          // Since scale.y is height, center is at height / 2
		          transform.position = new Vector3(
		              tileData.tileGridRef.x,
		              height / 2f,
		              tileData.tileGridRef.y
		          );
		          Color tileColor = TileUtilities.GetSoilColor(tileData.composition);
		          //set colour of tile based on composition and moisture
		          if (tileData.surfaceMoisture > 300)
		          {
				tileColor = Color.Lerp(tileColor, Constants.DEEP_WATER_COLOR, tileData.surfaceMoisture / 100f);
		          }
		          if (tileData.surfaceMoisture > 15)
		          {
				tileColor = Color.Lerp(tileColor, Constants.SHALLOW_WATER_COLOR, tileData.surfaceMoisture / 100f);
		          }
		          //set colour of tile based on current sunlight levels
		          float currentEnergy = SolarEnergyCalculator.CalculateCurrentEnergyAtLatitude(latitude, SimulationConstants.DayOfYear, SimulationConstants.TimeOfDay);
		          float lightFactor = Mathf.Clamp01(currentEnergy / SimulationConstants.SolarConstant);
		          tileColor = Color.Lerp(tileColor, Color.white, lightFactor * 0.3f); // Blend with white based on light factor
							   //blend with black based on how much light there isn't
		          tileColor = Color.Lerp(tileColor, Color.black, (1 - lightFactor) * 0.85f); // Blend with black based on light factor
		          GetComponent<Renderer>().material.color = tileColor;
	     }

	     public List<WorldTile> GetNeighbors()
	     {
		          return new List<WorldTile> { NTile, STile, ETile, WTile, NETile, NWTile, SETile, SWTile };
	     }


}
