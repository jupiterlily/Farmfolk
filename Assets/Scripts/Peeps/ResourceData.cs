
using System.Collections.Generic;

public class ResourceData
{
	     public string name;
	     public string description;
	     public List<string> category;
	     public float volume; //for initialisation in cubic centimeters
	     public float mass; //for initialisation we do mass of 1 count in kg
	     public int count = 1; //on initialisation is 1
	     public bool productive;
	     public int amountProduced; //we get this much
	     public string resourceProduced; //of this (allows us to have a tree produce apples)
	     public float daysToProduce; //every this many days

	     public void IncreaseResource(int amount)
	     {
		          float tempVol = volume / count;
		          float tempMass = mass / count;
		          count += amount;
		          volume += (tempVol * amount);
		          mass += (tempMass * amount);
	     }
}
public class PepoResources
{
	     public List<ResourceData> inventory;
	     public Dictionary<ResourceData, PlaceData> storage; //this is a pepos perception of what is stored there. maybe they get there and it's gone.
	     public List<ResourceData> wishlist; //this is stuff a pepo is looking to acquire
}
public class PepoResource : ResourceData
{                   //can be anything, physical, social, emotional but this is aresource as it exists on the pepo.
	     //food	     

}
public class PhysicalResource : ResourceData
{

}
public class NaturalResource : PhysicalResource
{

	     public NaturalResource(string name, int amount, float mass, float volume, string description, bool renewable, int renewalAmount, float frequency)
	     {
		          this.name = name;
		          this.description = description;

		          this.volume = volume;
		          this.mass = mass;

		          this.productive = renewable;
		          this.amountProduced = amount;
		          this.resourceProduced = name;
		          this.daysToProduce = amount;

		          this.IncreaseResource(amount);
	     }
}