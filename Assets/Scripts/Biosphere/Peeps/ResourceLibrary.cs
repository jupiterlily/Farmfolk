using System.Collections.Generic;
using UnityEngine;

public static class ResourceLibrary
{
	     public static List<NaturalResource> NaturalResources()
	     {
		          //water, tree, fruit, log, bush, berry, nut, mushroom, insects, rabbit, fish, bird, deer
		          NaturalResource Water = new NaturalResource("water", 0, 0.001f, 0.001f, "one millilitre of water", true, 0, 0, Vector3.zero);
		          Water.category.Add("thirst");
		          Water.category.Add("hygine");
		          NaturalResource Tree = new NaturalResource("tree", 0, 50f, 20f, "a large tree", true, 0, 0, Vector3.zero);
		          NaturalResource Fruit = new NaturalResource("fruit", 0, 0.2f, 0.3f, "a piece of fruit", true, 5, 1, Vector3.zero);
		          NaturalResource Log = new NaturalResource("log", 0, 10f, 5f, "a log", false, 0, 0, Vector3.zero);
		          NaturalResource Bush = new NaturalResource("bush", 0, 5f, 2f, "a bush", true, 0, 0, Vector3.zero);
		          NaturalResource Berry = new NaturalResource("berry", 0, 0.05f, 0.1f, "a berry", true, 1, 0, Vector3.zero);
		          NaturalResource Nut = new NaturalResource("nut", 0, 0.1f, 0.15f, "a nut", true, 2, 0, Vector3.zero);
		          NaturalResource Mushroom = new NaturalResource("mushroom", 0, 0.1f, 0.2f, "a mushroom", true, 3, 0, Vector3.zero);
		          NaturalResource Insects = new NaturalResource("insects", 0, 0.05f, 0.1f, "a handful of insects", true, 2, 0, Vector3.zero);
		          NaturalResource Rabbit = new NaturalResource("rabbit", 0, 3f, 4f, "a rabbit", true, 20, 5, Vector3.zero);
		          NaturalResource Fish = new NaturalResource("fish", 0, 2f, 3f, "a fish", true, 15, 3, Vector3.zero);
		          NaturalResource Bird = new NaturalResource("bird", 0, 1f, 2f, "a bird", true, 10, 2, Vector3.zero);
		          NaturalResource Deer = new NaturalResource("deer", 0, 7f, 8f, "a deer", true, 30, 8, Vector3.zero);
		          //mouse, stick, feather, hide, meat, fur, claw, tooth, bone, shell, honey, beehive, wolf, clay, squirrel, crab


		          return new List<NaturalResource>() { Water, Tree, Fruit, Log, Bush, Berry, Nut, Mushroom, Insects, Rabbit, Fish, Bird, Deer };
	     }

	     public static List<PhysicalResource> PhysicalResources()
	     {
		          PhysicalResource Water = new PhysicalResource("water", 0, 0.001f, 0.001f, "one millilitre of water", true, 0, 0, Vector3.zero);

		          return new List<PhysicalResource> { Water };
	     }
}