using System.Collections.Generic;
using System.Linq;

public static class ColourLibrary
{
	     private static readonly Dictionary<string, WorldColourData> _pigments = new();
	     internal static IReadOnlyDictionary<string, WorldColourData> Raw => _pigments;
	     public static IReadOnlyDictionary<string, WorldColourData> All
	     {
		          get
		          {
				GeneticsLibrary.EnsureBuilt();
				return _pigments;
		          }
	     }
	     public static WorldColourData Get(string name)
	     {
		          GeneticsLibrary.EnsureBuilt();
		          return _pigments[name];
	     }
	     internal static void Rebuild()
	     {
		          _pigments.Clear();

		          RegisterAnimalPigments();
		          RegisterPlantPigments();
		          RegisterMicrobialPigments();
		          RegisterOrganicDyes();
		          RegisterInorganicPigments();
		          RegisterPearlescentPigments();
		          RegisterLuminescentPigments();
		          RegisterStructuralColours();
	     }

	     // ---------------------------------------------------------------------------
	     // Animal pigments
	     // ---------------------------------------------------------------------------
	     private static void RegisterAnimalPigments()
	     {
		          // Melanins: the two classes that between them account for all human
		          // hair, skin and eye colour, plus the cephalopod variant.
		          Register(new WorldColourData("eumelanin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.02f, 0.15f, 0.30f, 0.85f, 0.55f, 0.06f)));

		          Register(new WorldColourData("pheomelanin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.11f, 0.02f, 0.25f, 0.95f, 0.95f, 0.55f)));

		          Register(new WorldColourData("sepialanin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.06f, 0.11f, 0.30f, 0.65f, 0.05f, 0.35f)));

		          // Haemoglobins. Oxy is the bright arterial red that shows through
		          // lips, ears and nail beds; deoxy is the darker venous maroon that
		          // dominates cold or bruised tissue. Between them they drive skin,
		          // lip and mucosal colour.
		          Register(new WorldColourData("oxyhaemoglobin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.97f, 1.02f, 0.65f, 0.90f, 0.85f, 0.98f)));

		          Register(new WorldColourData("deoxyhaemoglobin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.94f, 0.98f, 0.55f, 0.80f, 0.35f, 0.55f)));

		          // Bile pigments: green and yellow-brown, visible in bruises and jaundice.
		          Register(new WorldColourData("biliverdin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.28f, 0.38f, 0.55f, 0.85f, 0.35f, 0.60f)));

		          Register(new WorldColourData("bilirubin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.12f, 0.17f, 0.55f, 0.85f, 0.55f, 0.80f)));

		          // Astaxanthin: the red-orange of salmon flesh, flamingo feathers,
		          // crustacean shells.
		          Register(new WorldColourData("astaxanthin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.01f, 0.05f, 0.70f, 0.95f, 0.65f, 0.90f)));

		          // Pteridines: the four common members, replacing the old catch-all.

		          Register(new WorldColourData("xanthopterin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.13f, 0.16f, 0.55f, 0.85f, 0.85f, 0.98f)));

		          Register(new WorldColourData("sepiapterin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.14f, 0.18f, 0.45f, 0.75f, 0.80f, 0.95f)));

		          // Visual pigments for the eyes of nocturnal or aquatic animals.
		          Register(new WorldColourData("rhodopsin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.92f, 0.97f, 0.65f, 0.90f, 0.30f, 0.55f)));

		          Register(new WorldColourData("porphyropsin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.95f, 1.00f, 0.70f, 0.95f, 0.35f, 0.60f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Plant pigments
	     // ---------------------------------------------------------------------------
	     private static void RegisterPlantPigments()
	     {
		          // Chlorophylls — the two that between them colour essentially all leaves.
		          Register(new WorldColourData("chlorophylla", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.32f, 0.38f, 0.60f, 0.90f, 0.40f, 0.65f)));

		          Register(new WorldColourData("chlorophyllb", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.22f, 0.28f, 0.60f, 0.85f, 0.55f, 0.75f)));

		          // Carotenoid family. Note: hue.y > hue.x in every case (including 1.02
		          // for reds) so Mathf.Lerp never wraps through cyan at mid-density.
		          Register(new WorldColourData("carotene", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.05f, 0.10f, 0.70f, 0.95f, 0.80f, 0.98f)));

		          Register(new WorldColourData("lycopene", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.97f, 1.02f, 0.70f, 0.95f, 0.70f, 0.90f)));

		          Register(new WorldColourData("lutein", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.13f, 0.18f, 0.55f, 0.85f, 0.85f, 0.95f)));

		          Register(new WorldColourData("zeaxanthin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.15f, 0.60f, 0.90f, 0.80f, 0.95f)));

		          Register(new WorldColourData("fucoxanthin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.06f, 0.11f, 0.65f, 0.90f, 0.65f, 0.85f)));

		          // Generic carotenoid kept as a broad orange class for organisms that
		          // want "orange" without committing to a specific molecule.

		          // Anthocyanins: the classic red-purple-blue family of flowers and fruit.
		          Register(new WorldColourData("anthocyanin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.72f, 0.85f, 0.40f, 0.85f, 0.70f, 0.35f)));

		          Register(new WorldColourData("cyanidin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.92f, 0.98f, 0.55f, 0.85f, 0.55f, 0.85f)));

		          Register(new WorldColourData("delphinidin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.70f, 0.78f, 0.50f, 0.80f, 0.50f, 0.80f)));

		          Register(new WorldColourData("pelargonidin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.01f, 0.06f, 0.65f, 0.90f, 0.70f, 0.90f)));

		          Register(new WorldColourData("malvidin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.78f, 0.86f, 0.55f, 0.85f, 0.45f, 0.75f)));

		          // Betalains: the Caryophyllales' alternative to anthocyanins.
		          Register(new WorldColourData("betalain", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.90f, 0.98f, 0.55f, 0.90f, 0.75f, 0.45f)));

		          Register(new WorldColourData("betacyanin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.88f, 0.95f, 0.65f, 0.90f, 0.50f, 0.80f)));

		          Register(new WorldColourData("betaxanthin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.08f, 0.13f, 0.65f, 0.90f, 0.80f, 0.95f)));

		          // Browns and UV shields.
		          Register(new WorldColourData("tannin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.05f, 0.11f, 0.40f, 0.70f, 0.30f, 0.55f)));

		          Register(new WorldColourData("scytonemin", ColourType.BiologicalPigment, ColourBlend.Multiply,
		              new ColourData(0.08f, 0.13f, 0.55f, 0.85f, 0.50f, 0.75f)));

		          // Algal and cyanobacterial pigments.
		          Register(new WorldColourData("phycocyanin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.58f, 0.65f, 0.55f, 0.85f, 0.55f, 0.85f)));

		          Register(new WorldColourData("phycoerythrin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.95f, 1.00f, 0.60f, 0.90f, 0.60f, 0.85f)));

		          Register(new WorldColourData("bacteriochlorophyll", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.75f, 0.85f, 0.50f, 0.85f, 0.35f, 0.65f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Microbial pigments
	     // ---------------------------------------------------------------------------
	     private static void RegisterMicrobialPigments()
	     {
		          Register(new WorldColourData("violacein", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.75f, 0.85f, 0.55f, 0.85f, 0.25f, 0.55f)));

		          Register(new WorldColourData("prodigiosin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.95f, 1.00f, 0.70f, 0.95f, 0.55f, 0.80f)));

		          Register(new WorldColourData("pyocyanin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.50f, 0.58f, 0.55f, 0.85f, 0.55f, 0.85f)));

		          Register(new WorldColourData("flexirubin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.15f, 0.65f, 0.90f, 0.80f, 0.95f)));

		          Register(new WorldColourData("staphyloxanthin", ColourType.BiologicalPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.14f, 0.70f, 0.95f, 0.75f, 0.95f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Organic dyes — plant, insect and mollusc sourced
	     // ---------------------------------------------------------------------------
	     private static void RegisterOrganicDyes()
	     {
		          // Blues
		          Register(new WorldColourData("indigo", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.60f, 0.67f, 0.70f, 0.95f, 0.20f, 0.50f)));

		          Register(new WorldColourData("woad", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.58f, 0.65f, 0.55f, 0.85f, 0.30f, 0.60f)));

		          // Purples
		          Register(new WorldColourData("tyrian purple", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.82f, 0.88f, 0.55f, 0.85f, 0.35f, 0.65f)));

		          Register(new WorldColourData("logwood", ColourType.OrganicPigment, ColourBlend.Multiply,
		              new ColourData(0.75f, 0.85f, 0.45f, 0.75f, 0.10f, 0.35f)));

		          //// Reds
		          //Register(new WorldColourData("carmine", ColourType.OrganicPigment, ColourBlend.Lerp,
		          //    new ColourData(0.95f, 1.00f, 0.70f, 0.95f, 0.55f, 0.80f)));

		          //Register(new WorldColourData("kermesic acid", ColourType.OrganicPigment, ColourBlend.Lerp,
		          //    new ColourData(0.96f, 1.02f, 0.70f, 0.95f, 0.60f, 0.85f)));

		          //Register(new WorldColourData("laccaic acid", ColourType.OrganicPigment, ColourBlend.Lerp,
		          //    new ColourData(0.95f, 1.02f, 0.65f, 0.90f, 0.55f, 0.80f)));

		          //Register(new WorldColourData("alizarin", ColourType.OrganicPigment, ColourBlend.Multiply,
		          //    new ColourData(0.01f, 0.05f, 0.65f, 0.90f, 0.45f, 0.75f)));

		          //Register(new WorldColourData("purpurin", ColourType.OrganicPigment, ColourBlend.Lerp,
		          //    new ColourData(0.99f, 1.04f, 0.70f, 0.95f, 0.55f, 0.80f)));

		          //Register(new WorldColourData("lawsone", ColourType.OrganicPigment, ColourBlend.Lerp,
		          //    new ColourData(0.03f, 0.08f, 0.70f, 0.95f, 0.55f, 0.80f)));

		          //Register(new WorldColourData("dragon's blood", ColourType.OrganicPigment, ColourBlend.Lerp,
		          //    new ColourData(0.97f, 1.02f, 0.75f, 0.95f, 0.40f, 0.65f)));

		          // Yellows and oranges
		          Register(new WorldColourData("curcumin", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.11f, 0.16f, 0.70f, 0.95f, 0.85f, 0.98f)));

		          Register(new WorldColourData("bixin", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.03f, 0.08f, 0.70f, 0.95f, 0.65f, 0.90f)));

		          Register(new WorldColourData("crocetin", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.14f, 0.70f, 0.95f, 0.80f, 0.95f)));

		          Register(new WorldColourData("gamboge", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.12f, 0.17f, 0.65f, 0.90f, 0.80f, 0.95f)));

		          Register(new WorldColourData("safflower", ColourType.OrganicPigment, ColourBlend.Lerp,
		              new ColourData(0.04f, 0.10f, 0.65f, 0.95f, 0.70f, 0.90f)));

		          // Browns and blacks
		          Register(new WorldColourData("juglone", ColourType.OrganicPigment, ColourBlend.Multiply,
		              new ColourData(0.06f, 0.11f, 0.55f, 0.85f, 0.30f, 0.60f)));

		          Register(new WorldColourData("sepia ink", ColourType.OrganicPigment, ColourBlend.Multiply,
		              new ColourData(0.07f, 0.12f, 0.35f, 0.65f, 0.10f, 0.30f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Inorganic pigments — minerals, metal oxides, carbon blacks, whites
	     // ---------------------------------------------------------------------------
	     private static void RegisterInorganicPigments()
	     {
		          // Earth pigments — the ochre family.
		          Register(new WorldColourData("red ochre", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.01f, 0.05f, 0.65f, 0.90f, 0.35f, 0.60f)));

		          Register(new WorldColourData("yellow ochre", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.14f, 0.55f, 0.85f, 0.55f, 0.80f)));

		          Register(new WorldColourData("brown ochre", ColourType.InorganicPigment, ColourBlend.Multiply,
		              new ColourData(0.07f, 0.11f, 0.50f, 0.80f, 0.35f, 0.60f)));

		          // Blacks.
		          Register(new WorldColourData("magnetite", ColourType.InorganicPigment, ColourBlend.Multiply,
		              new ColourData(0.00f, 0.00f, 0.00f, 0.15f, 0.05f, 0.20f)));

		          Register(new WorldColourData("carbon black", ColourType.InorganicPigment, ColourBlend.Multiply,
		              new ColourData(0.00f, 0.00f, 0.00f, 0.10f, 0.02f, 0.15f)));

		          // Whites.
		          Register(new WorldColourData("titanium white", ColourType.InorganicPigment, ColourBlend.Screen,
		              new ColourData(0.00f, 0.00f, 0.00f, 0.00f, 0.95f, 1.00f)));

		          Register(new WorldColourData("calcium carbonate", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.14f, 0.05f, 0.20f, 0.90f, 0.98f)));

		          // Greens and blues (copper minerals).
		          Register(new WorldColourData("malachite", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.42f, 0.48f, 0.55f, 0.85f, 0.40f, 0.65f)));

		          Register(new WorldColourData("azurite", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.58f, 0.65f, 0.65f, 0.90f, 0.35f, 0.60f)));

		          Register(new WorldColourData("turquoise", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.48f, 0.55f, 0.55f, 0.85f, 0.65f, 0.90f)));

		          Register(new WorldColourData("verdigris", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.40f, 0.48f, 0.55f, 0.85f, 0.55f, 0.80f)));

		          Register(new WorldColourData("ultramarine", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.62f, 0.68f, 0.70f, 0.95f, 0.55f, 0.85f)));

		          Register(new WorldColourData("Egyptian blue", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.55f, 0.62f, 0.55f, 0.85f, 0.45f, 0.70f)));

		          Register(new WorldColourData("Prussian blue", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.60f, 0.66f, 0.65f, 0.90f, 0.25f, 0.55f)));

		          // Reds, oranges, yellows (mercury, cadmium, arsenic compounds).
		          Register(new WorldColourData("cinnabar", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.97f, 1.02f, 0.75f, 0.95f, 0.55f, 0.80f)));

		          Register(new WorldColourData("realgar", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.03f, 0.08f, 0.75f, 0.95f, 0.60f, 0.85f)));

		          Register(new WorldColourData("orpiment", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.12f, 0.16f, 0.75f, 0.95f, 0.85f, 0.98f)));

		          Register(new WorldColourData("cadmium yellow", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.10f, 0.15f, 0.70f, 0.95f, 0.85f, 0.98f)));

		          Register(new WorldColourData("cadmium red", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.01f, 0.05f, 0.70f, 0.95f, 0.65f, 0.90f)));

		          // Violets.
		          Register(new WorldColourData("manganese violet", ColourType.InorganicPigment, ColourBlend.Lerp,
		              new ColourData(0.78f, 0.85f, 0.35f, 0.65f, 0.45f, 0.75f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Pearlescent pigments — structural reflectors, mother-of-pearl, guanine
	     // ---------------------------------------------------------------------------
	     private static void RegisterPearlescentPigments()
	     {
		          Register(new WorldColourData("guanine", ColourType.PearlescentPigment, ColourBlend.Screen,
		              new ColourData(0.00f, 1.00f, 0.00f, 0.10f, 0.85f, 1.00f)));

		          Register(new WorldColourData("nacre", ColourType.PearlescentPigment, ColourBlend.Lerp,
		              new ColourData(0.55f, 0.65f, 0.05f, 0.20f, 0.90f, 1.00f)));

		          Register(new WorldColourData("chitin iridescence", ColourType.PearlescentPigment, ColourBlend.Screen,
		              new ColourData(0.00f, 1.00f, 0.00f, 0.05f, 0.80f, 0.95f)));

		          Register(new WorldColourData("collagen blue", ColourType.PearlescentPigment, ColourBlend.Lerp,
		              new ColourData(0.58f, 0.65f, 0.55f, 0.85f, 0.70f, 0.90f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Luminescent pigments — bioluminescence and fluorescence
	     // ---------------------------------------------------------------------------
	     private static void RegisterLuminescentPigments()
	     {
		          Register(new WorldColourData("luciferin", ColourType.LuminescentPigment, ColourBlend.Add,
		              new ColourData(0.20f, 0.25f, 0.65f, 0.90f, 0.90f, 1.00f)));

		          Register(new WorldColourData("coelenterazine", ColourType.LuminescentPigment, ColourBlend.Add,
		              new ColourData(0.55f, 0.62f, 0.60f, 0.90f, 0.85f, 1.00f)));

		          Register(new WorldColourData("aequorin", ColourType.LuminescentPigment, ColourBlend.Add,
		              new ColourData(0.45f, 0.52f, 0.55f, 0.85f, 0.85f, 1.00f)));

		          Register(new WorldColourData("GFP", ColourType.LuminescentPigment, ColourBlend.Add,
		              new ColourData(0.30f, 0.36f, 0.65f, 0.90f, 0.85f, 1.00f)));

		          Register(new WorldColourData("RFP", ColourType.LuminescentPigment, ColourBlend.Add,
		              new ColourData(0.97f, 1.02f, 0.65f, 0.90f, 0.85f, 1.00f)));

		          Register(new WorldColourData("bacteriorhodopsin", ColourType.LuminescentPigment, ColourBlend.Lerp,
		              new ColourData(0.78f, 0.85f, 0.55f, 0.85f, 0.55f, 0.85f)));
	     }

	     // ---------------------------------------------------------------------------
	     // Structural colours — iris tones (unchanged from your original)
	     // ---------------------------------------------------------------------------
	     private static void RegisterStructuralColours()
	     {
		          Register(new WorldColourData("violet iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(275f / 360f, 275f / 360f, 0.25f, 0.60f, 0.40f, 0.55f)));
		          Register(new WorldColourData("blue iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(215f / 360f, 275f / 360f, 0.20f, 0.65f, 0.45f, 0.95f)));
		          Register(new WorldColourData("teal iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(195f / 360f, 195f / 360f, 0.20f, 0.60f, 0.25f, 0.95f)));
		          Register(new WorldColourData("green iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(120f / 360f, 120f / 360f, 0.55f, 0.90f, 0.85f, 0.65f)));
		          Register(new WorldColourData("grey iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(215f / 360f, 215f / 360f, 0.00f, 0.20f, 0.85f, 0.55f)));
		          Register(new WorldColourData("yellow iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(0.11f, 0.15f, 0.55f, 0.90f, 0.85f, 0.55f)));
		          Register(new WorldColourData("red iris", ColourType.StructuralColour, ColourBlend.Lerp,
		              new ColourData(0.00f, 0.01f, 0.55f, 0.90f, 0.85f, 0.55f)));
	     }
	     private static void Register(WorldColourData p) => _pigments[p.Name] = p;
}
// ---------------------------------------------------------------------------
// BodyLibrary — top-level groupings of body parts
// ---------------------------------------------------------------------------

public static class BodyLibrary
{
	     private static readonly Dictionary<string, BodyPlan> _plans = new();

	     public static IReadOnlyDictionary<string, BodyPlan> Plans
	     {
		          get
		          {
				GeneticsLibrary.EnsureBuilt();
				return _plans;
		          }
	     }
	     public static BodyPlan Default => Plans["humanlike"];
	     public static BodyPlan Get(string name) => Plans[name];

	     internal static void Rebuild()
	     {
		          _plans.Clear();

		          // Default humanlike plan.
		          _plans["humanlike"] = BodyPlan.Build("humanlike")
		              .Group("head", "head", "humanhair0", "eye", "brows", "nose", "mouth")
		              .Group("skin", "skin")
		              .Group("body", "height", "mass", "reproductive")
		              .Finish();

		          // Future plans — nothing else changes in the codebase:
		          // _plans["canine"]  = BodyPlan.Build("canine")
		          //     .Group("head", "snout", "ear", "coat", "eye")
		          //     .Group("body", "height", "mass", "reproductive", "tail")
		          //     .Finish();
		          //
		          // _plans["plant"]   = BodyPlan.Build("plant")
		          //     .Group("body", "stalk", "leaf", "chlorophyll", "flower")
		          //     .Finish();
	     }
}
// ---------------------------------------------------------------------------
// BodyPartLibrary — every organ/part and the alleles that compose it
// ---------------------------------------------------------------------------
public static class BodyPartLibrary
{
	     private static readonly Dictionary<string, BodyPartData> _parts = new();

	     public static IReadOnlyDictionary<string, BodyPartData> All
	     {
		          get
		          {
				GeneticsLibrary.EnsureBuilt();
				return _parts;
		          }
	     }
	     internal static BodyPartData Lookup(string name) => _parts[name];

	     internal static void Rebuild()
	     {
		          _parts.Clear();

		          Register("head", "HeadShape", "Ears");
		          Register("humanhair", "HairTexture", "HairPigmentPheomelanin", "HairPigmentEumelanin", "HairCanitiesOnset", "HairCanitiesComplete");
		          //we want to generate one with 2 -4  random pigments		          
		          RegisterHairVariants(1, 2, 4);
		          Register("eye", "EyeDarkness", "EyeHeight", "EyeOpenness",
				     "EyeInnerCorner", "EyeOuterCorner",
				     "EyeIrisSaturation", "EyeIrisStructure",
				     "EyePigmentType", "EyePigmentDensity");
		          Register("brows", "BrowsThickness", "BrowHeight", "BrowInnerCorner", "BrowOuterCorner");
		          Register("nose", "NoseHeight", "NostrilShape", "NasalBridge");
		          Register("mouth", "MouthHeight", "TopLipShape", "TopLipThickness", "TopLipWidth",
				     "BottomLipShape", "BottomLipThickness", "BottomLipWidth", "Teeth");
		          Register("skin", "SkinDepth", "SkinSaturation", "SkinHue", "SkinOlive", "Freckles", "InvertFreckles", "SecondaryFreckles", "InvertSecondaryFreckles");
		          Register("height", "Height");
		          Register("mass", "MassGenetics");
		          Register("reproductive", "SexChromosome", "Lifespan", "PubertyOnset", "PubertyComplete", "MenstrualCycleLength", "GestationalPeriod", "EggsReleasedMin", "EggsReleasedMax", "EggDivision", "Menopause", "Andropause");
	     }
	     // Registers `count` hair parts, each carrying a random number of pigments in [min, max].
	     // Names are "humanhair0".."humanhair{count-1}".
	     internal static void RegisterHairVariants(int count, int min, int max)
	     {
		          var allPigmentGenes = GeneticsLibrary.BiologicalPigments
		              .Select(p => "HairPigment" + GeneticsLibrary.PigmentKey(p.Name))
		              .ToList();

		          int lo = System.Math.Max(1, min);
		          int hi = System.Math.Min(max, allPigmentGenes.Count);
		          if (hi < lo)
				hi = lo;

		          var rng = new System.Random();   // seed if you want reproducibility

		          for (int i = 0; i < count; i++)
		          {
				int n = rng.Next(lo, hi + 1);
				var chosen = allPigmentGenes.OrderBy(_ => rng.Next()).Take(n).ToList();

				var genes = new List<string> { "HairTexture", "HairCanitiesOnset", "HairCanitiesComplete" };
				genes.AddRange(chosen);

				Register("humanhair" + i, genes.ToArray());
		          }
	     }
	     private static BodyPartData Register(string partName, params string[] geneNames)
	     {
		          var alleles = geneNames.Select(GeneLibrary.Lookup).ToList();
		          var part = new BodyPartData(partName, alleles);
		          _parts[partName] = part;
		          return part;
	     }
}
// ---------------------------------------------------------------------------
// AlleleLibrary — human-readable descriptions for each allele value
// Key = AlleleData.range.name, Value = list indexed by the allele's value
// ---------------------------------------------------------------------------
public static class AlleleLibrary
{
	     public static string Describe(string traitName, int value)
	     {
		          if (!GeneLibrary.Descriptions.TryGetValue(traitName, out var list))
				return $"{traitName} {value}";
		          int idx = value - 1;
		          return (idx >= 0 && idx < list.Count) ? list[idx] : $"{traitName} {value}";
	     }
}
public static class GeneticsLibrary
{
	     private enum State
	     {
		          NotBuilt, Building, Built
	     }
	     private static State _state;

	     public static void EnsureBuilt()
	     {
		          if (_state == State.Built)
				return;
		          if (_state == State.Building)
				throw new System.InvalidOperationException("GeneticsLibrary rebuild cycle.");

		          _state = State.Building;
		          try
		          {
				ColourLibrary.Rebuild();          // ← must come first
				GeneLibrary.Rebuild();
				BodyPartLibrary.Rebuild();
				BodyLibrary.Rebuild();
				_state = State.Built;
		          }
		          catch { _state = State.NotBuilt; throw; }
	     }

	     // ---- Pigment helpers ------------------------------------------------

	     public static IEnumerable<WorldColourData> BiologicalPigments =>
	         ColourLibrary.Raw.Values.Where(p => p.type == ColourType.BiologicalPigment);

	     public static IEnumerable<WorldColourData> StructuralColours =>
	         ColourLibrary.Raw.Values.Where(p => p.type == ColourType.StructuralColour);

	     public static string[] BiologicalPigmentNames() =>
	         BiologicalPigments.Select(p => p.Name).ToArray();

	     public static string[] StructuralColourNames() =>
	         StructuralColours
	             .Select(p => p.Name.Replace(" iris", ""))
	             .ToArray();

	     // "eumelanin" -> "Eumelanin";  "red iris structure" -> "RedIrisStructure"
	     public static string PigmentKey(string name) =>
	         string.Concat(name.Split(' ', '-')
	             .Where(w => w.Length > 0)
	             .Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1)));

	     // Six identical density steps reused for every pigment gene.
	     public static string[] PigmentDensityDescriptors(string pigment) => new[]
	     {
        "no " + pigment,
        "trace of " + pigment,
        "light " + pigment,
        "medium " + pigment,
        "heavy " + pigment,
        "saturated " + pigment,
    };
}
// ---------------------------------------------------------------------------
// GeneLibrary — every allele, keyed by range.name for fast lookup. to add another genetic feature, add a new AlleleData here and add it to the appropriate BodyPartData in BodyPartLibrary.
// ---------------------------------------------------------------------------
public static class GeneLibrary
{
	     private static readonly Dictionary<string, AlleleData> _genes = new();
	     private static readonly Dictionary<string, List<string>> _descriptions = new();

	     private static readonly List<string> _visibleGenes = new();

	     // Public surface — boots on first access.
	     public static IReadOnlyDictionary<string, AlleleData> All
	     {
		          get
		          {
				GeneticsLibrary.EnsureBuilt();
				return _genes;
		          }
	     }
	     public static IReadOnlyDictionary<string, List<string>> Descriptions
	     {
		          get
		          {
				GeneticsLibrary.EnsureBuilt();
				return _descriptions;
		          }
	     }
	     public static List<string> VisibleGenes
	     {
		          get
		          {
				GeneticsLibrary.EnsureBuilt();
				return _visibleGenes;
		          }
	     }
	     public static AlleleData Get(string name)
	     {
		          GeneticsLibrary.EnsureBuilt();
		          return _genes[name];
	     }

	     // Internal — used by sibling libraries during their own rebuild.
	     internal static AlleleData Lookup(string name) => _genes[name];

	     internal static void Rebuild()
	     {
		          _genes.Clear();
		          _descriptions.Clear();

		          // ---- Hair ----
		          Register(GeneData.Count("HeadShape", InheritanceRule.EitherOr, () => PepoUIUtilities.heads.Length, "head shape", true));
		          Register(GeneData.Explicit("HairTexture", InheritanceRule.LeanDominant, true,
		              "straight", "wavy", "curly", "coily", "kinky"));
		          //  Register(GeneData.Explicit("HairEumelanin", InheritanceRule.LeanDominant, true,
		          //  "white", "lightest", "very light", "light", "medium light", "dull light",
		          //    "light brown", "medium brown", "dark brown", "very dark brown", "darkest brown", "black"));
		          //  Register(GeneData.Explicit("HairPheomelanin", InheritanceRule.WithinRange, true,
		          //      "deep red", "red", "deep orange", "orange", "no pheomelanin"));
		          Register(GeneData.Generated("HairCanitiesOnset", InheritanceRule.EitherOr, 75, "starts greying at ", 15));
		          Register(GeneData.Generated("HairCanitiesComplete", InheritanceRule.EitherOr, 75, "finishes greying at ", 15));
		          // ---- Hair pigments (auto-generated, replaces HairEumelanin / HairPheomelanin) ----
		          foreach (var pigment in GeneticsLibrary.BiologicalPigments)
				Register(GeneData.Explicit(
				    "HairPigment" + GeneticsLibrary.PigmentKey(pigment.Name),
				    InheritanceRule.WithinRange, true,
				    GeneticsLibrary.PigmentDensityDescriptors(pigment.Name)));
		          // ---- Eyes ----
		          //      Register(GeneData.Explicit("EyeColor", InheritanceRule.ChooseDominant, true,
		          //     "violet", "blue", "teal", "green", "yellow", "grey", "red"));
		          Register(GeneData.Explicit("EyeIrisStructure", InheritanceRule.EitherOr, true,
		          GeneticsLibrary.StructuralColourNames()));          // ← auto from StructuralColour entries
		          Register(GeneData.Explicit("EyeIrisSaturation", InheritanceRule.EitherOr, true,
		              "pale", "light", "medium", "bright", "intense"));
		          Register(GeneData.Explicit("EyePigmentType", InheritanceRule.EitherOr, true,
		              GeneticsLibrary.BiologicalPigmentNames()));
		          //generate an eye pigment gene for each biological pigment in the colour library.
		          Register(GeneData.Explicit("EyePigmentDensity", InheritanceRule.EitherOr, true,
		              "clear", "honey", "amber", "hazel", "brown", "dark brown", "black"));
		          Register(GeneData.Explicit("EyeDarkness", InheritanceRule.WithinRange, true, "light", "medium", "dark"));
		          Register(GeneData.Explicit("EyeHeight", InheritanceRule.EitherOr, true, "low-set", "medium-set", "high-set"));
		          Register(GeneData.Explicit("EyeOpenness", InheritanceRule.EitherOr, false, "narrow", "medium", "wide"));
		          Register(GeneData.Count("EyeInnerCorner", InheritanceRule.EitherOr, () => PepoUIUtilities.eyesMascInner.Length, "inner corner", true));
		          Register(GeneData.Count("EyeOuterCorner", InheritanceRule.EitherOr, () => PepoUIUtilities.eyesMascOuter.Length, "outer corner", true));

		          // ---- Nose ----
		          Register(GeneData.Count("NostrilShape", InheritanceRule.EitherOr, () => PepoUIUtilities.nostrils.Length, "nostril shape", true));
		          Register(GeneData.Explicit("NoseHeight", InheritanceRule.EitherOr, true, "low bridge", "medium bridge", "high bridge"));
		          Register(GeneData.Count("NasalBridge", InheritanceRule.EitherOr, () => PepoUIUtilities.noseBridge.Length, "nasal bridge", true));

		          // ---- Mouth ----
		          Register(GeneData.Explicit("MouthHeight", InheritanceRule.EitherOr, true, "low", "medium", "high"));
		          Register(GeneData.Count("TopLipShape", InheritanceRule.EitherOr, () => PepoUIUtilities.topLips.Length, "top lip shape", true));
		          Register(GeneData.Explicit("TopLipWidth", InheritanceRule.EitherOr, true, "very narrow", "narrow", "wide", "very wide"));
		          Register(GeneData.Explicit("TopLipThickness", InheritanceRule.EitherOr, true, "thin", "medium", "full", "very full"));
		          Register(GeneData.Count("BottomLipShape", InheritanceRule.EitherOr, () => PepoUIUtilities.bottomLips.Length, "bottom lip shape", true));
		          Register(GeneData.Explicit("BottomLipWidth", InheritanceRule.EitherOr, true, "very narrow", "narrow", "wide", "very wide"));
		          Register(GeneData.Explicit("BottomLipThickness", InheritanceRule.EitherOr, true, "thin", "medium", "full", "very full"));

		          // ---- Skin ----
		          Register(GeneData.Explicit("SkinDepth", InheritanceRule.WithinRange, true, "very light", "light", "medium light", "medium", "medium deep", "deep", "very deep"));
		          Register(GeneData.Explicit("SkinSaturation", InheritanceRule.EitherOr, true, "muted", "soft", "moderate", "saturated", "vivid", "rich"));
		          Register(GeneData.Explicit("SkinHue", InheritanceRule.EitherOr, true, "very cool", "cool", "neutral-cool", "neutral", "neutral-warm", "warm", "very warm"));
		          Register(GeneData.Explicit("SkinOlive", InheritanceRule.EitherOr, true, "very olive", "olive", "slightly olive"));
		          // ---- Skin pigments (auto-generated, replaces SkinDepth/SkinSaturation/SkinHue/SkinOlive) ----
		          foreach (var pigment in GeneticsLibrary.BiologicalPigments)
				Register(GeneData.Explicit(
				    "Skin" + GeneticsLibrary.PigmentKey(pigment.Name),
				    InheritanceRule.WithinRange, true,
				    GeneticsLibrary.PigmentDensityDescriptors(pigment.Name)));
		          // ---- Freckles ----
		          Register(GeneData.Count("Freckles", InheritanceRule.EitherOr, () => PepoUIUtilities.freckles.Length, "freckle pattern", true));
		          Register(GeneData.Explicit("InvertFreckles", InheritanceRule.EitherOr, false, "not inverted", "inverted"));
		          Register(GeneData.Count("SecondaryFreckles", InheritanceRule.EitherOr, () => PepoUIUtilities.freckles.Length, "secondary freckle pattern", true));
		          Register(GeneData.Explicit("InvertSecondaryFreckles", InheritanceRule.EitherOr, false, "not inverted", "inverted"));

		          // ---- Brows ----
		          Register(GeneData.Explicit("BrowsThickness", InheritanceRule.LeanDominant, true, "thin", "medium", "thick", "bushy"));
		          Register(GeneData.Explicit("BrowHeight", InheritanceRule.EitherOr, true, "low", "medium", "high"));
		          Register(GeneData.Count("BrowInnerCorner", InheritanceRule.EitherOr, () => PepoUIUtilities.browsInner1.Length, "brow inner corner", true));
		          Register(GeneData.Count("BrowOuterCorner", InheritanceRule.EitherOr, () => PepoUIUtilities.browsOuter1.Length, "brow outer corner", true));

		          // ---- Body ----
		          Register(GeneData.Explicit("Height", InheritanceRule.EitherOr, true, "very short", "short", "below average", "average", "above average", "tall", "very tall"));
		          Register(GeneData.Explicit("MassGenetics", InheritanceRule.EitherOr, true, "very light", "light", "average", "heavy", "very heavy"));
		          Register(GeneData.Explicit("SexChromosome", InheritanceRule.EitherOr, false, "XX", "XY"));

		          // ---- Misc ----
		          Register(GeneData.Count("Teeth", InheritanceRule.EitherOr, () => PepoUIUtilities.teeth.Length, "teeth set", true));
		          Register(GeneData.Count("Ears", InheritanceRule.EitherOr, () => PepoUIUtilities.ears.Length, "ear shape", true));

		          // ---- Metabolic ----
		          Register(GeneData.Generated("Lifespan", InheritanceRule.EitherOr, 75, "reaches end of lifespan at ", 15));
		          Register(GeneData.Generated("PubertyOnset", InheritanceRule.EitherOr, 15, "starts puberty at ", 8));
		          Register(GeneData.Generated("PubertyComplete", InheritanceRule.EitherOr, 45, "finishes puberty at ", 8));
		          Register(GeneData.Generated("MenstrualCycleLength", InheritanceRule.EitherOr, 55, "menstrual cycle days: ", 15));
		          Register(GeneData.Generated("GestationalPeriod", InheritanceRule.EitherOr, 156, "gestation weeks: ", 1));
		          Register(GeneData.Generated("EggsReleasedMin", InheritanceRule.EitherOr, 15, "minimum eggs released: ", 1));
		          Register(GeneData.Generated("EggsReleasedMax", InheritanceRule.EitherOr, 15, "maximum eggs released: ", 1));
		          Register(GeneData.Generated("EggDivision", InheritanceRule.EitherOr, 15, "Egg divisions per 1000: ", 1));
		          Register(GeneData.Generated("Menopause", InheritanceRule.EitherOr, 75, "reaches menopause at ", 15));
		          Register(GeneData.Generated("Andropause", InheritanceRule.EitherOr, 75, "reaches andropause at ", 15));
	     }

	     private static AlleleData Register(GeneData def)
	     {
		          var a = new AlleleData(def.Name, def.Min, def.Max, def.Inheritance, def.Descriptors);
		          _genes[def.Name] = a;
		          _descriptions[def.Name] = new List<string>(def.Descriptors);
		          if (def.visible)
		          {
				_visibleGenes.Add(def.Name);
		          }
		          return a;
	     }
}