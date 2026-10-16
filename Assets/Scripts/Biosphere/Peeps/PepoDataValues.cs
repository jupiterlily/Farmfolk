public class Value : NumericalAspect
{
	     public string opposite;
	     //radian for position on the wheel
	     public float location = 360 / 9;

	     public Value() : base("undefined", 100)
	     {

	     }

	     Value(string Name, float Value, float MaxValue, string Opposite, float Location) : base(Name, MaxValue)
	     {
		          name = Name;
		          value = Value;
		          maxValue = MaxValue;
		          opposite = Opposite;
		          location = Location;
	     }
}

