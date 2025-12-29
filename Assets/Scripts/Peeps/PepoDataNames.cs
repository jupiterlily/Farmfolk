using System;
using System.Linq;
[Serializable]
public class PepoName
{
	     // a PepoName can contain multiple NameData entries (forename, middlename, surname, etc.)
	     public NameData[] Name;

	     // Fullname composed from available NameData entries (joins non-empty name parts with a space).
	     public string FullName => Name == null
	         ? string.Empty
	         : string.Join(" ", Name.Where(n => n != null && !string.IsNullOrEmpty(n.name.value)).Select(n => n.name));

	     // Returns the first forename if present, otherwise first non-empty name part
	     public string GetPrimaryName()
	     {
		          if (Name == null || Name.Length == 0)
				return string.Empty;
		          var forename = Name.FirstOrDefault(n => n != null && n.forename && !string.IsNullOrEmpty(n.name.value));
		          if (forename != null)
				return forename.name.value;
		          var first = Name.FirstOrDefault(n => n != null && !string.IsNullOrEmpty(n.name.value));
		          return first?.name.value ?? string.Empty;
	     }
	     //add a name and give it a position in the order to be inserted at.
	     public void AddName(NameData name, int position)
	     {

	     }
}

[Serializable]
public class NameData : Noun
{
	     // Usage / origin flags
	     public bool forename;
	     public bool middlename;
	     public bool surname;
	     public bool nickname;
	     public bool honourific;
	     public bool derogatory; // fixed typo
	     public bool toponym;
	     public bool occupational;
	     public bool necronym;
	     public bool birthname;
	     public bool tributary;
	     public bool virtue;
	     public bool trick;
	     public bool birthParentChoice;
	     public bool seedParentChoice;

	     // gender markers
	     public bool masculine;
	     public bool feminine;

	     // the visible text of the name
	     public QualitativeAspect name;

	     // optional stem / root word data
	     public WordData stem;
	     public Suffix suffix;

	     public NameData(string name)
	     {
		          type = NounType.Proper;
		          this.name.value = name ?? string.Empty;
		          this.word = this.name.value; // keep base.word in sync
	     }

	     public override string ToString()
	     {
		          return name.value;
	     }
}
public class Suffix : WordData
{

}
public class Prefix : WordData
{

}
public class WordData
{
	     public string word;
}
public class Noun : WordData
{
	     public NounType type;
}

public enum NounType
{
	     Proper, Common, Concrete, Abstract, Gerund,
	     Countable, Uncountable, Compound
}

public class Verb : WordData
{
	     public string simple; //i study
	     public string perfect; //i studied
	     public string continuous;//i'm studying
	     public bool regular;
	     public bool transitive;
	     public bool finite;
}
public enum VerbType
{
	     Action, Stative, Linking, Auxillary, Modal, Compound, Dynamic
}