using System.Collections.Generic;

public class PlaceData
{
	     public string name;
	     public string description;
	     public List<PlaceData> nearbyPlaces;
	     public List<PepoState> accessConditions;
	     public Dictionary<PhysicalResource, int> resourceTable; //resources and their renewal rate in days
	     public List<PhysicalResource> physicalResources;
}
public enum PlaceType
{

}
