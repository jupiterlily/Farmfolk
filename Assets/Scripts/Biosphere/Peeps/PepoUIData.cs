using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// PortraitLayer.cs - Represents a single layer in the portrait
[System.Serializable]
public class PortraitLayer
{
	     public string layerName;
	     public Sprite sprite;
	     public int sortingOrder;
	     public Color color = Color.white;
	     public Vector2 positionOffset = Vector2.zero;

	     public PortraitLayer(Sprite image, string name, Color colouring, Vector2 offset)
	     {
		          layerName = name;
		          sprite = image;
		          color = colouring;
		          positionOffset = offset;
		          if (name == "HairUp")
		          {
				sortingOrder = 0;
		          }
		          if (name == "HairBack")
		          {
				sortingOrder = 1;
		          }
		          if (name == "Ears")
		          {
				sortingOrder = 2;
		          }
		          if (name == "Head")
		          {
				sortingOrder = 3;
		          }
		          if (name == "Nasal Bridge")
		          {
				sortingOrder = 4;
		          }
		          if (name == "Freckles")
		          {
				sortingOrder = 5;
		          }
		          if (name == "SecondaryFreckles")
		          {
				sortingOrder = 5;
		          }
		          if (name == "Stubble")
		          {
				sortingOrder = 6;
		          }
		          if (name == "Eyebrows" || name == "BrowInnerCorner" || name == "BrowOuterCorner")
		          {
				sortingOrder = 7;
		          }
		          if (name == "EyeColour")
		          {
				sortingOrder = 8;
		          }
		          if (name == "EyeShape" || name == "EyeInnerCorner" || name == "EyeOuterCorner")
		          {
				sortingOrder = 9;
		          }
		          if (name == "Beard")
		          {
				sortingOrder = 10;
		          }
		          if (name == "BottomLip")
		          {
				sortingOrder = 11;
		          }
		          if (name == "TopLip")
		          {
				sortingOrder = 12;
		          }
		          if (name == "Teeth")
		          {
				sortingOrder = 13;
		          }
		          if (name == "Moustache")
		          {
				sortingOrder = 14;
		          }
		          if (name == "Nostrils")
		          {
				sortingOrder = 15;
		          }
		          if (name == "HairTendrils")
		          {
				sortingOrder = 16;
		          }
		          if (name == "HairFront")
		          {
				sortingOrder = 17;
		          }
	     }
}

// PortraitFeature.cs - Base class for all portrait features
public class PepoPortrait
{
	     public List<PortraitLayer> layers;

	     public PepoPortrait(List<PortraitLayer> portraitLayers)
	     {
		          layers = portraitLayers.OrderBy(l => l.sortingOrder).ToList();
	     }
}