using System;
using System.Collections.Generic;
using UnityEngine;

public static class MatterLibrary
{
	     // ---------- Constant specific heat (J/kg·K) ----------
	     private static readonly Dictionary<string, float> _constantCp = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
    {
        { "sand", 753f },
        { "silt", 745f },
        { "clay", 880f },
        { "organicmaterial", 837f },
        { "oxygen", 918f },
        { "nitrogen", 1040f },
        // add other constant?cp materials here as needed
    };

	     // ---------- Temperature?dependent tables (sorted arrays) ----------
	     private class CpTable
	     {
		          public float[] Temperatures;   // sorted ascending
		          public float[] CpValues;       // same length, J/kg·K

		          public float GetCp(float temp)
		          {
				int index = Array.BinarySearch(Temperatures, temp);
				if (index >= 0)
					     return CpValues[index];

				// Not an exact match – interpolate between neighbours
				int insert = ~index;
				if (insert == 0)
					     return CpValues[0];                    // below first
				if (insert >= Temperatures.Length)
					     return CpValues[^1]; // above last

				float t0 = Temperatures[insert - 1];
				float t1 = Temperatures[insert];
				float v0 = CpValues[insert - 1];
				float v1 = CpValues[insert];
				return v0 + (v1 - v0) * (temp - t0) / (t1 - t0);
		          }
	     }

	     private static readonly Dictionary<string, CpTable> _cpTables = new Dictionary<string, CpTable>(StringComparer.OrdinalIgnoreCase);

	     // Static constructor – builds all tables once
	     static MatterLibrary()
	     {
		          // Water vapour (kJ/kgK -> J/kgK)
		          var wvTemps = new float[] { 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3500, 4000, 4500, 5000, 5500, 6000 };
		          var wvCp = new float[] { 1850, 1851, 1852, 1855, 1859, 1864, 1871, 1880, 1890, 1901, 1926, 1954, 1984, 2015, 2047, 2080, 2113, 2147, 2182, 2217, 2252, 2288, 2323, 2358, 2392, 2425, 2458, 2490, 2521, 2552, 2609, 2662, 2711, 2756, 2798, 2836, 2872, 2904, 2934, 2962, 2987, 3011, 3033, 3053, 3072, 3090, 3163, 3217, 3258, 3292, 3322, 3350 };

		          _cpTables["water vapour"] = new CpTable { Temperatures = wvTemps, CpValues = wvCp };

		          // Liquid water (already J/kgK)
		          var lwTemps = new float[] { 273.15f, 278.15f, 283.15f, 288.15f, 293.15f, 298.15f, 303.15f, 308.15f, 313.15f, 318.15f, 323.15f, 328.15f, 333.15f, 338.15f, 343.15f, 348.15f, 353.15f, 358.15f, 363.15f, 368.15f, 373.15f };
		          var lwCp = new float[] { 4180, 4190, 4200, 4210, 4220, 4230, 4240, 4250, 4260, 4270, 4280, 4290, 4300, 4310, 4320, 4330, 4340, 4350, 4360, 4370, 4380 };
		          _cpTables["liquid water"] = new CpTable { Temperatures = lwTemps, CpValues = lwCp };

		          // Ice (kJ/kgK -> J/kgK)
		          var iceTemps = new float[] { 203.15f, 208.15f, 213.15f, 218.15f, 223.15f, 228.15f, 233.15f, 238.15f, 243.15f, 248.15f, 253.15f, 258.15f, 263.15f, 268.15f, 273.15f };
		          var iceCp = new float[] { 1389, 1463, 1536, 1609, 1681, 1751, 1818, 1851, 1882, 1913, 1943, 1972, 2000, 2027, 2050 };

		          _cpTables["ice"] = new CpTable { Temperatures = iceTemps, CpValues = iceCp };

		          // Carbon dioxide (kJ/kgK -> J/kgK)
		          var co2Temps = new float[] { 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 450, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3500, 4000, 4500, 5000, 5500, 6000 };
		          var co2Cp = new float[] { 709f, 735f, 763f, 791f, 819f, 846f, 871f, 895f, 918f, 939f, 978f, 1014f, 1046f, 1075f, 1102f, 1126f, 1148f, 1168f, 1187f, 1204f, 1220f, 1234f, 1247f, 1259f, 1270f, 1280f, 1290f, 1298f, 1306f, 1313f, 1326f, 1338f, 1348f, 1356f, 1364f, 1371f, 1377f, 1383f, 1388f, 1393f, 1397f, 1401f, 1404f, 1408f, 1411f, 1414f, 1427f, 1437f, 1446f, 1455f, 1465f, 1476f };

		          _cpTables["carbon dioxide"] = new CpTable { Temperatures = co2Temps, CpValues = co2Cp };
	     }

	     // ---------- Public fast lookup ----------
	     public static float GetSpecificHeat(string materialName, float temperatureKelvin)
	     {
		          if (string.IsNullOrEmpty(materialName))
				return 1000f; // fallback

		          // 1. Check constant cp (O(1))
		          if (_constantCp.TryGetValue(materialName, out float cp))
				return cp;

		          // 2. Check temperature?dependent tables (O(log N))
		          if (_cpTables.TryGetValue(materialName, out CpTable table))
				return table.GetCp(temperatureKelvin);

		          // 3. Not found – log warning and return fallback
		          Debug.LogWarning($"Specific heat for '{materialName}' not found. Returning 1000 J/kgK.");
		          return 1000f;
	     }

	     // ---------- Legacy methods (keep for compatibility, but they now forward) ----------
	     public static Dictionary<string, Dictionary<float, float>> SpecificHeatCapacitiespour()
	     {
		          // Returns the old?style dictionaries for water phases (not used by new code)
		          var result = new Dictionary<string, Dictionary<float, float>>();
		          if (_cpTables.TryGetValue("water vapour", out var wv))
				result["water vapour"] = ToDict(wv);
		          if (_cpTables.TryGetValue("liquid water", out var lw))
				result["liquid water"] = ToDict(lw);
		          if (_cpTables.TryGetValue("ice", out var ice))
				result["ice"] = ToDict(ice);
		          return result;
	     }

	     private static Dictionary<float, float> ToDict(CpTable table)
	     {
		          var d = new Dictionary<float, float>();
		          for (int i = 0; i < table.Temperatures.Length; i++)
				d[table.Temperatures[i]] = table.CpValues[i];
		          return d;
	     }
}