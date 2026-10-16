using System.Collections.Generic;

public static class BodyLibrary
{

}
public static class BodyPartLibrary
{
	     public static BodyPartData Eyes = new("eye", new List<AlleleData> { AlleleLibrary.EyeColor, AlleleLibrary.EyeDarkness, AlleleLibrary.EyeHeight, AlleleLibrary.EyeInnerCorner, AlleleLibrary.EyeOuterCorner });

	     public static BodyPartData Nose = new("nose", new List<AlleleData> { AlleleLibrary.NoseHeight, AlleleLibrary.NostrilShape, AlleleLibrary.NasalBridge });

	     public static BodyPartData Mouth = new("mouth", new List<AlleleData> { AlleleLibrary.MouthHeight, AlleleLibrary.TopLipShape, AlleleLibrary.TopLipThickness, AlleleLibrary.TopLipWidth, AlleleLibrary.BottomLipShape, AlleleLibrary.BottomLipThickness, AlleleLibrary.BottomLipWidth });
}
public static class AlleleLibrary
{
	     //within here we will have our allele library which is where we constuct the alleles and have dictionaries for their descriptions.

	     public static AlleleData HeadShape = new("HeadShape", 1, PepoUIUtilities.heads.Length, InheritanceRule.EitherOr);

	     public static AlleleData HairPheomelanin = new("HairPheomelanin", 1, 5, InheritanceRule.ChooseDominant);
	     public static AlleleData HairEumelanin = new("HairEumelanin", 1, 8, InheritanceRule.WithinRange);

	     //  public static AlleleData EyeShape = new ("EyeShape", 1, PepoUIUtilities.femmeEyes.Length,              InheritanceRule.EitherOr);
	     public static AlleleData EyeColor = new("EyeColor", 1, 6, InheritanceRule.ChooseDominant);
	     public static AlleleData EyeDarkness = new("EyeDarkness", 1, 3, InheritanceRule.WithinRange);
	     public static AlleleData EyeHeight = new("EyeHeight", 1, 3, InheritanceRule.EitherOr);
	     public static AlleleData EyeOpenness = new("EyeOpenness", 1, 3, InheritanceRule.EitherOr);
	     public static AlleleData EyeInnerCorner = new("EyeInnerCorner", 1, PepoUIUtilities.eyesMascInner.Length, InheritanceRule.EitherOr);
	     public static AlleleData EyeOuterCorner = new("EyeOuterCorner", 1, PepoUIUtilities.eyesMascOuter.Length, InheritanceRule.EitherOr);

	     public static AlleleData NostrilShape = new("NostrilShape", 1, PepoUIUtilities.nostrils.Length, InheritanceRule.EitherOr);
	     public static AlleleData NoseHeight = new("NoseHeight", 1, 3, InheritanceRule.EitherOr);
	     public static AlleleData NasalBridge = new("NasalBridge", 1, PepoUIUtilities.noseBridge.Length, InheritanceRule.EitherOr);

	     public static AlleleData MouthHeight = new("MouthHeight", 1, 3, InheritanceRule.EitherOr);
	     public static AlleleData TopLipShape = new("TopLipShape", 1, 10, InheritanceRule.EitherOr);
	     public static AlleleData TopLipWidth = new("TopLipWidth", 1, 4, InheritanceRule.EitherOr);
	     public static AlleleData TopLipThickness = new("TopLipThickness", 1, 4, InheritanceRule.EitherOr);
	     public static AlleleData BottomLipShape = new("BottomLipShape", 1, 10, InheritanceRule.EitherOr);
	     public static AlleleData BottomLipWidth = new("TopLipWidth", 1, 4, InheritanceRule.EitherOr);
	     public static AlleleData BottomLipThickness = new("TopLipThickness", 1, 4, InheritanceRule.EitherOr);

	     public static AlleleData SkinDepth = new("SkinDepth", 1, 5, InheritanceRule.WithinRange);
	     public static AlleleData SkinSaturation = new("SkinSaturation", 1, 5, InheritanceRule.WithinRange);
	     public static AlleleData SkinHue = new("SkinHue", 1, 5, InheritanceRule.WithinRange);

	     public static AlleleData Freckles = new("Freckles", 1, PepoUIUtilities.freckles.Length, InheritanceRule.EitherOr);
	     public static AlleleData InvertFreckles = new("InvertFreckles", 1, 2, InheritanceRule.EitherOr);
	     public static AlleleData SecondaryFreckles = new("SecondaryFreckles", 1, PepoUIUtilities.freckles.Length, InheritanceRule.EitherOr);
	     public static AlleleData InvertSecondaryFreckles = new("InvertSecondaryFreckles", 1, 2, InheritanceRule.EitherOr);

	     public static AlleleData HairTexture = new("HairTexture", 1, 4, InheritanceRule.LeanDominant);

	     public static AlleleData Height = new("Height", 1, 20, InheritanceRule.EitherOr);
	     public static AlleleData MassGenetics = new("MassGenetics", 1, 50, InheritanceRule.EitherOr);

	     public static AlleleData BrowsThickness = new("BrowsThickness", 1, 4, InheritanceRule.LeanDominant);
	     public static AlleleData BrowHeight = new("BrowHeight", 1, 3, InheritanceRule.EitherOr);
	     public static AlleleData BrowInnerCorner = new("BrowInnerCorner", 1, PepoUIUtilities.browsInner1.Length, InheritanceRule.EitherOr);
	     public static AlleleData BrowOuterCorner = new("BrowOuterCorner", 1, PepoUIUtilities.browsOuter1.Length, InheritanceRule.EitherOr);

	     public static AlleleData Teeth = new("Teeth", 1, 4, InheritanceRule.EitherOr);

	     public static AlleleData Ears = new("Ears", 1, 4, InheritanceRule.EitherOr);

}