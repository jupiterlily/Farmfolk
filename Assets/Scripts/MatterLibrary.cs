using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ====================================================================
//  MATTER LIBRARY – central repository for elements, compounds,
//  and thermophysical properties.
// ====================================================================

public static class MatterLibrary
{
	     // ================================================================
	     // 1.  ELEMENTS
	     // ================================================================

	     public static class Elements
	     {
		          // Non‑metals
		          public static readonly Element Hydrogen = new Element("Hydrogen", "H", 1, 1.008f);
		          public static readonly Element Carbon = new Element("Carbon", "C", 6, 12.011f);
		          public static readonly Element Nitrogen = new Element("Nitrogen", "N", 7, 14.007f);
		          public static readonly Element Oxygen = new Element("Oxygen", "O", 8, 15.999f);
		          public static readonly Element Fluorine = new Element("Fluorine", "F", 9, 18.998f);
		          public static readonly Element Phosphorus = new Element("Phosphorus", "P", 15, 30.974f);
		          public static readonly Element Sulfur = new Element("Sulfur", "S", 16, 32.065f);
		          public static readonly Element Chlorine = new Element("Chlorine", "Cl", 17, 35.45f);

		          // Alkali & alkaline earth
		          public static readonly Element Sodium = new Element("Sodium", "Na", 11, 22.990f);
		          public static readonly Element Magnesium = new Element("Magnesium", "Mg", 12, 24.305f);
		          public static readonly Element Potassium = new Element("Potassium", "K", 19, 39.098f);
		          public static readonly Element Calcium = new Element("Calcium", "Ca", 20, 40.078f);
		          public static readonly Element Strontium = new Element("Strontium", "Sr", 38, 87.62f);

		          // Transition metals
		          public static readonly Element Titanium = new Element("Titanium", "Ti", 22, 47.867f);
		          public static readonly Element Chromium = new Element("Chromium", "Cr", 24, 51.996f);
		          public static readonly Element Manganese = new Element("Manganese", "Mn", 25, 54.938f);
		          public static readonly Element Iron = new Element("Iron", "Fe", 26, 55.845f);
		          public static readonly Element Cobalt = new Element("Cobalt", "Co", 27, 58.933f);
		          public static readonly Element Nickel = new Element("Nickel", "Ni", 28, 58.693f);
		          public static readonly Element Copper = new Element("Copper", "Cu", 29, 63.546f);
		          public static readonly Element Zinc = new Element("Zinc", "Zn", 30, 65.38f);
		          public static readonly Element Silver = new Element("Silver", "Ag", 47, 107.868f);
		          public static readonly Element Tin = new Element("Tin", "Sn", 50, 118.710f);
		          public static readonly Element Gold = new Element("Gold", "Au", 79, 196.967f);
		          public static readonly Element Lead = new Element("Lead", "Pb", 82, 207.2f);

		          /// <summary>Lookup table symbol → Element for formula parsing.</summary>
		          internal static readonly Dictionary<string, Element> SymbolMap =
		              typeof(Elements)
			   .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
			   .Where(f => f.FieldType == typeof(Element))
			   .Select(f => (Element)f.GetValue(null))
			   .ToDictionary(e => e.Symbol);
	     }

	     // ================================================================
	     // 2.  MOLECULE PARSER
	     // ================================================================

	     /// <summary>
	     /// Parses a chemical formula (e.g., "H2O", "Fe2O3", "CaMg(CO3)2")
	     /// and returns a dictionary of element → count.
	     /// </summary>
	     public static Dictionary<Element, int> ParseMolecule(string formula)
	     {
		          var dict = new Dictionary<Element, int>();
		          int i = 0;
		          while (i < formula.Length)
		          {
				// Read element symbol (1 or 2 letters)
				string symbol = formula[i].ToString();
				i++;
				if (i < formula.Length && char.IsLower(formula[i]))
				{
					     symbol += formula[i];
					     i++;
				}

				// Read count (optional digits)
				string countStr = "";
				while (i < formula.Length && char.IsDigit(formula[i]))
				{
					     countStr += formula[i];
					     i++;
				}
				int count = string.IsNullOrEmpty(countStr) ? 1 : int.Parse(countStr);

				if (Elements.SymbolMap.TryGetValue(symbol, out Element element))
				{
					     if (dict.ContainsKey(element))
						          dict[element] += count;
					     else
						          dict.Add(element, count);
				}
				else
				{
					     Debug.LogWarning($"Unknown element symbol '{symbol}' in formula '{formula}'");
				}
		          }
		          return dict;
	     }

	     // ================================================================
	     // 3.  COMPOUND DEFINITIONS (using factory to reduce duplication)
	     // ================================================================

	     /// <summary>
	     /// Helper to create a Compound instance with automatic composition and
	     /// molecular weight calculation.  No tables are stored – the Compound
	     /// queries the library for specific heat via GetSpecificHeat.
	     /// </summary>
	     private static Compound CreateCompound(
	         string name,
	         string formula,
	         float meltingPoint,
	         float boilingPoint,
	         float latentHeatFusion,
	         float latentHeatVaporization,
	         float thermalConductivity,
	         float electricalConductivity,
	         float defaultDensity = 1000f)
	     {
		          var composition = ParseMolecule(formula);
		          float molecularWeight = composition.Sum(kvp => kvp.Key.AtomicMass * kvp.Value);

		          return new Compound(
		              name,
		              formula,
		              composition,
		              molecularWeight,
		              meltingPoint,
		              boilingPoint,
		              latentHeatFusion,
		              latentHeatVaporization,
		              thermalConductivity,
		              electricalConductivity,
		              defaultDensity);
	     }

	     public static class Compounds
	     {
		          // ---- Elements as compounds ----
		          public static readonly Compound ElementIron = CreateCompound("Iron", "Fe", 1811f, 3134f, 247000f, 6090000f, 80.4f, 1e7f, 7874f);
		          public static readonly Compound ElementSilicon = CreateCompound("Silicon", "Si", 1687f, 3173f, 1790000f, 12800000f, 148f, 1e-4f, 2330f);
		          public static readonly Compound ElementAluminum = CreateCompound("Aluminum", "Al", 933f, 2792f, 397000f, 10900000f, 237f, 1e7f, 2700f);
		          public static readonly Compound ElementMagnesium = CreateCompound("Magnesium", "Mg", 923f, 1363f, 368000f, 5270000f, 156f, 1e7f, 1738f);
		          public static readonly Compound ElementCalcium = CreateCompound("Calcium", "Ca", 1115f, 1757f, 207000f, 3870000f, 201f, 1e7f, 1550f);
		          public static readonly Compound ElementSodium = CreateCompound("Sodium", "Na", 371f, 1156f, 113000f, 4090000f, 141f, 1e7f, 968f);
		          public static readonly Compound ElementPotassium = CreateCompound("Potassium", "K", 336f, 1032f, 102000f, 2090000f, 102f, 1e7f, 862f);
		          public static readonly Compound ElementGold = CreateCompound("Gold", "Au", 1337f, 3133f, 126000f, 3340000f, 317f, 1e7f, 19300f);

		          // ---- Oxides ----
		          public static readonly Compound Silica = CreateCompound("Silica", "SiO2", 2000f, 3220f, 1790000f, 128000000f, 1.48f, 1e-12f, 2650f);
		          public static readonly Compound Hematite = CreateCompound("Hematite", "Fe2O3", 1838f, 3200f, 400000f, 6000000f, 3.5f, 1e-12f, 5240f);
		          public static readonly Compound Magnetite = CreateCompound("Magnetite", "Fe3O4", 1870f, 3000f, 400000f, 6000000f, 5.0f, 1e-12f, 5180f);
		          public static readonly Compound Goethite = CreateCompound("Goethite", "FeOOH", 400f, 1000f, 400000f, 6000000f, 2.0f, 1e-12f, 4300f);
		          public static readonly Compound Corundum = CreateCompound("Corundum", "Al2O3", 2345f, 3250f, 400000f, 6000000f, 35f, 1e-12f, 3980f);
		          public static readonly Compound Gibbsite = CreateCompound("Gibbsite", "Al(OH)3", 300f, 500f, 400000f, 6000000f, 1.0f, 1e-12f, 2400f);
		          public static readonly Compound Magnesia = CreateCompound("Magnesia", "MgO", 3125f, 3600f, 400000f, 6000000f, 60f, 1e-12f, 3580f);
		          public static readonly Compound Calcite = CreateCompound("Calcite", "CaCO3", 1600f, 2800f, 500000f, 1300000f, 3.5f, 1e-12f, 2710f);
		          public static readonly Compound Dolomite = CreateCompound("Dolomite", "CaMg(CO3)2", 1600f, 2800f, 500000f, 1300000f, 3.5f, 1e-12f, 2850f);
		          public static readonly Compound Lime = CreateCompound("Lime", "CaO", 2890f, 3600f, 400000f, 6000000f, 20f, 1e-12f, 3340f);
		          public static readonly Compound Rutile = CreateCompound("Rutile", "TiO2", 2100f, 3200f, 400000f, 6000000f, 8.9f, 1e-12f, 4230f);
		          public static readonly Compound Chromite = CreateCompound("Chromite", "FeCr2O4", 2200f, 3000f, 400000f, 6000000f, 5.0f, 1e-12f, 5100f);
		          public static readonly Compound Pyrolusite = CreateCompound("Pyrolusite", "MnO2", 1800f, 2800f, 400000f, 6000000f, 2.0f, 1e-12f, 5020f);

		          // ---- Silicates ----
		          public static readonly Compound Forsterite = CreateCompound("Forsterite", "Mg2SiO4", 1890f, 3200f, 400000f, 6000000f, 4.0f, 1e-12f, 3200f);
		          public static readonly Compound Fayalite = CreateCompound("Fayalite", "Fe2SiO4", 1200f, 2800f, 400000f, 6000000f, 4.0f, 1e-12f, 4300f);
		          public static readonly Compound Enstatite = CreateCompound("Enstatite", "MgSiO3", 1570f, 2800f, 400000f, 6000000f, 3.5f, 1e-12f, 3200f);
		          public static readonly Compound Ferrosilite = CreateCompound("Ferrosilite", "FeSiO3", 1200f, 2800f, 400000f, 6000000f, 3.5f, 1e-12f, 4000f);
		          public static readonly Compound Hornblende = CreateCompound("Hornblende", "Ca2(Mg,Fe)5Si8O22(OH)2", 1400f, 2800f, 400000f, 6000000f, 3.0f, 1e-12f, 3200f);
		          public static readonly Compound Biotite = CreateCompound("Biotite", "K(Mg,Fe)3AlSi3O10(OH)2", 1200f, 2500f, 400000f, 6000000f, 2.0f, 1e-12f, 3000f);
		          public static readonly Compound Muscovite = CreateCompound("Muscovite", "KAl2(AlSi3O10)(OH)2", 1300f, 2500f, 400000f, 6000000f, 2.0f, 1e-12f, 2800f);
		          public static readonly Compound Orthoclase = CreateCompound("Orthoclase", "KAlSi3O8", 1450f, 2800f, 400000f, 6000000f, 2.1f, 1e-12f, 2550f);
		          public static readonly Compound Plagioclase = CreateCompound("Plagioclase", "NaAlSi3O8", 1500f, 2800f, 400000f, 6000000f, 2.1f, 1e-12f, 2650f);
		          public static readonly Compound Kaolinite = CreateCompound("Kaolinite", "Al2Si2O5(OH)4", 1000f, 2000f, 500000f, 1300000f, 2.5f, 1e-12f, 2600f);
		          public static readonly Compound Montmorillonite = CreateCompound("Montmorillonite", "(Na,Ca)0.33(Al,Mg)2(Si4O10)(OH)2·nH2O", 800f, 1500f, 500000f, 1300000f, 2.5f, 1e-12f, 2600f);
		          public static readonly Compound Illite = CreateCompound("Illite", "K0.65Al2.0(Al0.65Si3.35O10)(OH)2", 800f, 1500f, 500000f, 1300000f, 2.5f, 1e-12f, 2600f);

		          // ---- Dissolved ions ----
		          public static readonly Compound AmmoniumIon = CreateCompound("Ammonium", "NH4", 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		          public static readonly Compound NitrateIon = CreateCompound("Nitrate", "NO3", 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		          public static readonly Compound PhosphateIon = CreateCompound("Phosphate", "PO4", 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		          public static readonly Compound SulfateIon = CreateCompound("Sulfate", "SO4", 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		          public static readonly Compound CarbonateIon = CreateCompound("Carbonate", "CO3", 0f, 0f, 0f, 0f, 0f, 0f, 0f);

		          // ---- Atmospheric gases ----
		          public static readonly Compound OxygenGas = CreateCompound("Oxygen", "O2", 54.36f, 90.20f, 13900f, 213000f, 0.026f, 0f, 1.429f);
		          public static readonly Compound NitrogenGas = CreateCompound("Nitrogen", "N2", 63.15f, 77.36f, 25700f, 199000f, 0.026f, 0f, 1.251f);
		          public static readonly Compound CarbonDioxideGas = CreateCompound("CarbonDioxide", "CO2", 194.65f, 216.55f, 184000f, 574000f, 0.016f, 0f, 1.977f);
		          public static readonly Compound WaterVapour = CreateCompound("WaterVapour", "H2O", 273.15f, 373.15f, 334000f, 2260000f, 0.025f, 0f, 0.6f);

		          // ---- Organic compounds ----
		          public static readonly Compound Cellulose = CreateCompound("Cellulose", "C6H10O5", 500f, 600f, 237500f, 2250000f, 0.4f, 0f, 1500f);
		          public static readonly Compound Lignin = CreateCompound("Lignin", "C31H34O11", 500f, 600f, 237500f, 2250000f, 0.4f, 0f, 1400f);
		          public static readonly Compound Glucose = CreateCompound("Glucose", "C6H12O6", 419f, 500f, 200000f, 2000000f, 0.5f, 0f, 1540f);
	     }

	     // ================================================================
	     // 4.  SPECIFIC HEAT TABLES (constant and temperature‑dependent)
	     // ================================================================

	     /// <summary>Constant specific heats (J/kg·K) for materials without tables.</summary>
	     private static readonly Dictionary<string, float> _constantCp = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
    {
        { "sand", 753f },
        { "silt", 745f },
        { "clay", 880f },
        { "organic matter", 837f },
        // add more constant‑cp materials here as needed
    };

	     /// <summary>Internal representation of a temperature‑dependent Cp table.</summary>
	     private class CpTable
	     {
		          public float[] Temperatures;   // sorted ascending
		          public float[] CpValues;       // same length, J/kg·K

		          public float GetCp(float temp)
		          {
				int index = Array.BinarySearch(Temperatures, temp);
				if (index >= 0)
					     return CpValues[index];

				int insert = ~index;
				if (insert == 0)
					     return CpValues[0];
				if (insert >= Temperatures.Length)
					     return CpValues[^1];

				float t0 = Temperatures[insert - 1];
				float t1 = Temperatures[insert];
				float v0 = CpValues[insert - 1];
				float v1 = CpValues[insert];
				return v0 + (v1 - v0) * (temp - t0) / (t1 - t0);
		          }
	     }

	     private static readonly Dictionary<string, CpTable> _cpTables = new Dictionary<string, CpTable>(StringComparer.OrdinalIgnoreCase);

	     /// <summary>Static constructor – builds all temperature‑dependent tables once.</summary>
	     static MatterLibrary()
	     {
		          // ---- Water vapour ----
		          AddCpTable("water vapour",
		              new float[] { 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3500, 4000, 4500, 5000, 5500, 6000 },
		              new float[] { 1850, 1851, 1852, 1855, 1859, 1864, 1871, 1880, 1890, 1901, 1926, 1954, 1984, 2015, 2047, 2080, 2113, 2147, 2182, 2217, 2252, 2288, 2323, 2358, 2392, 2425, 2458, 2490, 2521, 2552, 2609, 2662, 2711, 2756, 2798, 2836, 2872, 2904, 2934, 2962, 2987, 3011, 3033, 3053, 3072, 3090, 3163, 3217, 3258, 3292, 3322, 3350 });

		          // ---- Liquid water ----
		          AddCpTable("liquid water",
		              new float[] { 273.15f, 278.15f, 283.15f, 288.15f, 293.15f, 298.15f, 303.15f, 308.15f, 313.15f, 318.15f, 323.15f, 328.15f, 333.15f, 338.15f, 343.15f, 348.15f, 353.15f, 358.15f, 363.15f, 368.15f, 373.15f },
		              new float[] { 4180, 4190, 4200, 4210, 4220, 4230, 4240, 4250, 4260, 4270, 4280, 4290, 4300, 4310, 4320, 4330, 4340, 4350, 4360, 4370, 4380 });

		          // ---- Ice ----
		          AddCpTable("ice",
		              new float[] { 203.15f, 208.15f, 213.15f, 218.15f, 223.15f, 228.15f, 233.15f, 238.15f, 243.15f, 248.15f, 253.15f, 258.15f, 263.15f, 268.15f, 273.15f },
		              new float[] { 1389, 1463, 1536, 1609, 1681, 1751, 1818, 1851, 1882, 1913, 1943, 1972, 2000, 2027, 2050 });

		          // ---- Iron ----
		          AddCpTable("iron",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800 },
		              new float[] { 340, 450, 500, 560, 620, 700, 800, 950, 1050, 1150, 1250, 1350, 1450, 1550, 1650, 1750, 1850 });

		          // ---- Copper ----
		          AddCpTable("copper",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300 },
		              new float[] { 340, 385, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490 });

		          // ---- Aluminium ----
		          AddCpTable("aluminium",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900 },
		              new float[] { 700, 900, 940, 980, 1020, 1060, 1100, 1140 });
		          _cpTables["aluminum"] = _cpTables["aluminium"]; // alias

		          // ---- Silicon ----
		          AddCpTable("silicon",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600 },
		              new float[] { 500, 710, 750, 780, 810, 840, 870, 900, 930, 970, 1010, 1050 });

		          // ---- Titanium ----
		          AddCpTable("titanium",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800 },
		              new float[] { 380, 520, 560, 590, 620, 650, 680, 710, 740, 800, 850, 900, 950 });

		          // ---- Magnesium ----
		          AddCpTable("magnesium",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900 },
		              new float[] { 800, 1020, 1050, 1080, 1110, 1140, 1170, 1200 });

		          // ---- Sodium ----
		          AddCpTable("sodium",
		              new float[] { 200, 300, 370 },
		              new float[] { 900, 1230, 1300 });

		          // ---- Potassium ----
		          AddCpTable("potassium",
		              new float[] { 200, 300, 336 },
		              new float[] { 600, 757, 800 });

		          // ---- Calcium ----
		          AddCpTable("calcium",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100 },
		              new float[] { 500, 647, 670, 690, 710, 730, 750, 770, 790, 810 });

		          // ---- Zinc ----
		          AddCpTable("zinc",
		              new float[] { 200, 300, 400, 500, 600 },
		              new float[] { 330, 388, 400, 420, 440 });

		          // ---- Tin ----
		          AddCpTable("tin",
		              new float[] { 200, 300, 400, 500 },
		              new float[] { 200, 227, 240, 255 });

		          // ---- Lead ----
		          AddCpTable("lead",
		              new float[] { 200, 300, 400, 500, 600 },
		              new float[] { 110, 128, 135, 142, 150 });

		          // ---- Silver ----
		          AddCpTable("silver",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200 },
		              new float[] { 210, 235, 245, 255, 265, 275, 285, 295, 305, 315, 325 });

		          // ---- Gold ----
		          AddCpTable("gold",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300 },
		              new float[] { 115, 129, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180 });

		          // ---- Silicon Dioxide (silica, quartz) ----
		          AddCpTable("silicon dioxide",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000 },
		              new float[] { 400, 730, 780, 820, 860, 890, 920, 950, 980, 1020, 1060, 1100, 1140, 1180 });
		          _cpTables["silica"] = _cpTables["silicon dioxide"];

		          // ---- Aluminium Oxide (corundum) ----
		          AddCpTable("aluminium oxide",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000, 2200 },
		              new float[] { 400, 770, 840, 900, 950, 990, 1020, 1050, 1080, 1130, 1180, 1220, 1250, 1280, 1310 });
		          _cpTables["aluminum oxide"] = _cpTables["aluminium oxide"];

		          // ---- Calcium Carbonate (limestone) ----
		          AddCpTable("calcium carbonate",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1100 },
		              new float[] { 400, 820, 860, 900, 940, 980, 1020, 1060, 1100, 1140 });
		          _cpTables["limestone"] = _cpTables["calcium carbonate"];

		          // ---- Oxygen gas ----
		          AddCpTable("oxygen",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000, 2500, 3000 },
		              new float[] { 880, 918, 930, 940, 950, 960, 970, 980, 990, 1010, 1030, 1050, 1070, 1090, 1130, 1170 });

		          // ---- Nitrogen gas ----
		          AddCpTable("nitrogen",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000, 2500, 3000 },
		              new float[] { 1000, 1040, 1050, 1060, 1070, 1080, 1090, 1100, 1110, 1130, 1150, 1170, 1190, 1210, 1250, 1290 });

		          // ---- Hydrogen gas ----
		          AddCpTable("hydrogen",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000, 2500, 3000 },
		              new float[] { 13000, 14300, 14400, 14500, 14600, 14700, 14800, 14900, 15000, 15200, 15400, 15600, 15800, 16000, 16500, 17000 });

		          // ---- Dry air ----
		          AddCpTable("air",
		              new float[] { 200, 250, 300, 350, 400, 450, 500, 550, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000, 2500, 3000 },
		              new float[] { 1003, 1003, 1005, 1008, 1013, 1020, 1029, 1040, 1052, 1078, 1106, 1134, 1162, 1214, 1261, 1302, 1337, 1367, 1439, 1490 });

		          // ---- Carbon (graphite) ----
		          AddCpTable("carbon",
		              new float[] { 200, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000, 2500, 3000 },
		              new float[] { 200, 709, 800, 850, 900, 950, 1000, 1050, 1100, 1180, 1250, 1320, 1380, 1440, 1550, 1650 });
		          _cpTables["graphite"] = _cpTables["carbon"];

		          // ---- Carbon dioxide ----
		          AddCpTable("carbon dioxide",
		              new float[] { 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3500, 4000, 4500, 5000, 5500, 6000 },
		              new float[] { 709f, 735f, 763f, 791f, 819f, 846f, 871f, 895f, 918f, 939f, 978f, 1014f, 1046f, 1075f, 1102f, 1126f, 1148f, 1168f, 1187f, 1204f, 1220f, 1234f, 1247f, 1259f, 1270f, 1280f, 1290f, 1298f, 1306f, 1313f, 1326f, 1338f, 1348f, 1356f, 1364f, 1371f, 1377f, 1383f, 1388f, 1393f, 1397f, 1401f, 1404f, 1408f, 1411f, 1414f, 1427f, 1437f, 1446f, 1455f, 1465f, 1476f });
	     }

	     private static void AddCpTable(string key, float[] temps, float[] values)
	     {
		          _cpTables[key] = new CpTable { Temperatures = temps, CpValues = values };
	     }

	     // ================================================================
	     // 5.  PUBLIC API
	     // ================================================================

	     /// <summary>
	     /// Returns the specific heat (J/kg·K) for a given material at a given temperature.
	     /// Looks up in constant Cp dictionary or interpolates from tables.
	     /// </summary>
	     public static float GetSpecificHeat(string materialName, float temperatureKelvin)
	     {
		          if (string.IsNullOrEmpty(materialName))
				return 1000f;

		          // 1. Check constant cp
		          if (_constantCp.TryGetValue(materialName, out float cp))
				return cp;

		          // 2. Check temperature‑dependent tables
		          if (_cpTables.TryGetValue(materialName, out CpTable table))
				return table.GetCp(temperatureKelvin);

		          // 3. Not found – warn and return fallback
		          Debug.LogWarning($"Specific heat for '{materialName}' not found. Returning 1000 J/kgK.");
		          return 1000f;
	     }

	     // (Optional) Legacy method to convert a table to Dictionary if needed.
	     // private static Dictionary<float, float> ToDict(CpTable table) { ... }
}