using System.Collections.Generic;

public static class ResourceLibrary
{
	     public static List<NaturalResource> NaturalResources()
	     {
		          //water, tree, fruit, log, bush, berry, nut, mushroom, insects, rabbit, fish, bird, deer
		          NaturalResource Water = new NaturalResource("water", 0, 0.001f, 0.001f, "one millilitre of water", true, 0, 0);

		          return new List<NaturalResource>();
	     }
}