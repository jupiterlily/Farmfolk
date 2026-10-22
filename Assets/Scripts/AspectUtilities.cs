using System.Collections.Generic;

public static class AspectUtilities
{
	     //here we will have our methods that generate lists of aspects from things like pepo data.
	     public static void PopulateAspectDictionary(PepoData pepo)
	     {
		          pepo.FilterableAspects = new Dictionary<string, NumericalAspect>();
		          NumericalAspect Age = new NumericalAspect("age", 100f, 0f, false, 0f, "years");
		          Age.value = pepo.age;
		          NumericalAspect Mass = new NumericalAspect("mass", 200f, 0f, false, 0f, "kg");
		          NumericalAspect Height = new NumericalAspect("height", 2.5f, 0f, false, 0f, "m");
		          Mass.value = pepo.mass;
		          Height.value = pepo.height;
		          pepo.FilterableAspects.Add(Age.name, Age);
		          pepo.FilterableAspects.Add(Mass.name, Mass);
		          pepo.FilterableAspects.Add(Height.name, Height);
		          foreach (var aspect in pepo.body.physicalAttributes.Values)
		          {
				pepo.FilterableAspects.Add(aspect.name, aspect);
		          }
		          foreach (PersonalityAspect aspect in pepo.personality.FullHexRange())
		          {
				pepo.FilterableAspects.Add(aspect.name, (NumericalAspect)aspect);
		          }
		          foreach (PersonalityAspect aspect in pepo.personality.FullFacetRange())
		          {
				pepo.FilterableAspects.Add(aspect.name, (NumericalAspect)aspect);
		          }
		          foreach (var aspect in pepo.values)
		          {
				pepo.FilterableAspects.Add(aspect.name, (NumericalAspect)aspect);
		          }

	     }
}
