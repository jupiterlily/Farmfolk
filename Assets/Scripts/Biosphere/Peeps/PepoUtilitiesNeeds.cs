public static class PepoUtilitiesNeeds
{
	     public static void ProgressNeedHours(float hours, MaslowHeirachy needs)
	     {
		          foreach (var need in needs.Physiological)
		          {
				need.Deplete(need.decayRate * hours);
				need.DescribeNeed();
		          }
	     }
}
