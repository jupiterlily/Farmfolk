using System.Collections.Generic;

public class PlaceData
{
	     public string name;
	     public string description;
	     public List<PlaceData> nearbyPlaces;
	     //public List<PepoState> accessConditions;
	     public List<PhysicalResource> physicalResources;
	     public PepoPopulation pepos = new PepoPopulation(0);

	     public PlaceData(string name, string description)
	     {
		          this.name = name;
		          this.description = description;
	     }
}
public enum PlaceType
{

}
