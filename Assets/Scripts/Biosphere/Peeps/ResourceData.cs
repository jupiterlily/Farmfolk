using System.Collections.Generic;
using UnityEngine;
public abstract class ResourceData
{
	     public string name;
	     public string description;
	     public List<string> category;
	     public int count = 1; //on initialisation is 1
	     public bool productive;
	     public int amountProduced; //we get this much
	     public string resourceProduced; //of this (allows us to have a tree produce apples)
	     public float daysToProduce; //every this many days


}
public class PepoResources
{
	     public List<ResourceData> inventory;
	     public Dictionary<PlaceData, List<ResourceData>> storage; //this is a pepos perception of what is stored there. maybe they get there and it's gone.
	     public List<ResourceData> wishlist; //this is stuff a pepo is looking to acquire

	     //add resource to inventory, remove corresponding amount from wishlist
	     public void AddorRemoveResource(ResourceData resource)
	     {
		          if (inventory == null)
				inventory = new List<ResourceData>();
		          //if the inventory has aresource with the same name as the incoming resource, add the count to the count. 
		          foreach (ResourceData item in inventory)
		          {
				if (item.name == resource.name)
				{
					     item.count += resource.count;
				}
				else
				{
					     inventory.Add(resource);
				}
				foreach (ResourceData wish in wishlist)
				{
					     if (wish.name == resource.name)
					     {
						          wish.count -= resource.count;
					     }
				}
		          }
	     }
}
public class PepoResource : ResourceData
{                   //can be anything, social, emotional but this is aresource as it exists on the pepo.

	     public void IncreaseResource(int amount)
	     {
		          count += amount;
	     }
}
public class PhysicalResource : ResourceData
{
	     //a physical object like food, crafts etc.
	     public float volume; //for initialisation in cubic centimeters
	     public float mass; //for initialisation we do mass of 1 count in kg
	     public Vector3 location;
	     public PhysicalResource(string name, int amount, float mass, float volume, string description, bool renewable, int renewalAmount, float frequency, Vector3 location)
	     {
		          this.name = name;
		          this.description = description;

		          this.volume = volume;
		          this.mass = mass;

		          this.productive = renewable;
		          this.amountProduced = renewalAmount;
		          this.resourceProduced = name;
		          this.daysToProduce = frequency;

		          this.IncreaseResource(amount);
	     }
	     public void IncreaseResource(int amount)
	     {
		          float tempVol = volume / count;
		          float tempMass = mass / count;
		          count += amount;
		          volume += (tempVol * amount);
		          mass += (tempMass * amount);
	     }
}
public class NaturalResource : PhysicalResource
{
	     // Additional NaturalResource-specific fields
	     public bool isHarvestable;

	     public NaturalResource(string name, int amount, float mass, float volume, string description, bool renewable, int renewalAmount, float frequency, Vector3 location, bool isHarvestable = true)
	         : base(name, amount, mass, volume, description, renewable, renewalAmount, frequency, location)
	     {
		          this.isHarvestable = isHarvestable;
	     }
}