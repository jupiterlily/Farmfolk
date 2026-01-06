public class MetabolicCompounds
{
	     public float H2O;      // Water
	     public float O2;       // Oxygen
	     public float CO2;      // Carbon Dioxide
	     public float Glucose;  // Glucose
	     public float ADP;      // Adenosine Diphosphate
	     public float ATP;      // Adenosine Triphosphate
	     public float AMP;      // Adenosine Monophosphate
	     public float NAD;      // Nicotinamide Adenine Dinucleotide (oxidized)
	     public float NADH;     // Nicotinamide Adenine Dinucleotide (reduced)
	     public float NADP;     // Nicotinamide Adenine Dinucleotide Phosphate (oxidized)
	     public float NADPH;    // Nicotinamide Adenine Dinucleotide Phosphate (reduced)
	     public float FAD;      // Flavin Adenine Dinucleotide (oxidized)
	     public float FADH2;    // Flavin Adenine Dinucleotide (reduced)
	     public float Pyruvate; // Pyruvic Acid
	     public float Lactate;  // Lactic Acid
	     public float Pi;       // Inorganic Phosphate
	     public float Cellulose;
	     public float Starch;
	     public float NH3;       // Added for urea cycle
	     public float Urea;      // Added for urea cycle
	     public float AcetylCoA; // Added for better metabolism tracking

	     public MetabolicCompounds(float h2O = 0, float o2 = 0, float cO2 = 0, float glucose = 0,
			 float aDP = 0, float aTP = 0, float aMP = 0,
			 float nAD = 0, float nADH = 0, float nADP = 0, float nADPH = 0,
			 float fAD = 0, float fADH2 = 0,
			 float pyruvate = 0, float lactate = 0, float pi = 0,
			 float cellulose = 0, float starch = 0, float nh3 = 0, float urea = 0, float acetylcoa = 0)
	     {
		          H2O = h2O;
		          O2 = o2;
		          CO2 = cO2;
		          Glucose = glucose;
		          ADP = aDP;
		          ATP = aTP;
		          AMP = aMP;
		          NAD = nAD;
		          NADH = nADH;
		          NADP = nADP;
		          NADPH = nADPH;
		          FAD = fAD;
		          FADH2 = fADH2;
		          Pyruvate = pyruvate;
		          Lactate = lactate;
		          Pi = pi;
		          Cellulose = cellulose;
		          Starch = starch;
		          NH3 = nh3;
		          Urea = urea;
		          AcetylCoA = acetylcoa;
	     }

	     // Clone method to create a copy
	     public MetabolicCompounds Clone()
	     {
		          return new MetabolicCompounds(H2O, O2, CO2, Glucose, ADP, ATP, AMP,
				         NAD, NADH, NADP, NADPH, FAD, FADH2,
				         Pyruvate, Lactate, Pi, Cellulose, Starch, NH3, Urea, AcetylCoA);
	     }
}

