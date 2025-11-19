public class PepoName
{
	     //a name has masculine versions, feminine forms and bool that says if it can be aprefix, a siffix or a whole name
	     public NameData forename;
	     public NameData surname;
	     public string fullname => string.Concat(forename, " ", surname);
}
public class NameData : Noun
{
	     public bool forename;
	     public bool middlename;
	     public bool surname;
	     public bool nickname;
	     public WordData rootWord;
	     public NameData(string name)
	     {
		          type = NounType.Proper;
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