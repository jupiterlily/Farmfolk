using UnityEngine;

// Class that defines various metabolic pathways in organisms
public class MetabolicPathways
{
	     // Function for light reactions in photosynthesis
	     public MetabolicCompounds LightReactions(int incomingWatts, MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Consume water and produce oxygen based on incoming light energy
		          pendingCompounds.H2O -= incomingWatts * 6;
		          pendingCompounds.O2 += incomingWatts * 6;

		          // Convert ADP to ATP using the light energy
		          pendingCompounds = ADPtoATP(incomingWatts * 12, pendingCompounds);

		          // Convert NADP to NADPH using the light energy
		          pendingCompounds = NADPtoNADPH(incomingWatts * 12, pendingCompounds);

		          // Check if the updates are valid
		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{incomingWatts} KiloJoules processed");
				return pendingCompounds;
		          }

		          // Log an error if the updates are not valid
		          Debug.Log("Not enough compounds for metabolism");
		          return currentCompounds;
	     }
	     // **NEW: PHOTORESPIRATION (important for plants)**
	     public MetabolicCompounds Photorespiration(MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Simplified photorespiration: consumes O2, produces CO2 and NH3
		          int maxReaction = (int)Mathf.Min(
		              currentCompounds.O2 / 2,
		              currentCompounds.ATP / 3
		          );

		          pendingCompounds.O2 -= maxReaction * 2;
		          pendingCompounds.CO2 += maxReaction;
		          pendingCompounds.NH3 += maxReaction;
		          pendingCompounds.ATP -= maxReaction * 3;

		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"Photorespiration: consumed {maxReaction * 2} O2, produced {maxReaction} CO2");
				return pendingCompounds;
		          }

		          return currentCompounds;
	     }
	     // Function for carbon fixation in photosynthesis
	     public MetabolicCompounds CarbonFixation(MetabolicCompounds currentCompounds, int carbonToFix)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Consume CO2 and produce oxygen during carbon fixation
		          pendingCompounds.O2 += carbonToFix * 6;
		          pendingCompounds.CO2 -= carbonToFix * 6;

		          // Check if the updates are valid
		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{carbonToFix} carbon fixed");
				return pendingCompounds;
		          }

		          // Log an error if the updates are not valid
		          Debug.Log("Not enough compounds for metabolism");
		          return currentCompounds;
	     }
	     // Function for the entire photosynthesis process
	     public MetabolicCompounds Photosynthesis(MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Calculate the maximum amount of glucose to produce based on available compounds
		          int maxGlucoseToProduce = (int)Mathf.Min(currentCompounds.H2O / 12, currentCompounds.ATP / 18, currentCompounds.CO2 / 6, currentCompounds.NADPH / 12);

		          // Consume compounds to produce glucose
		          pendingCompounds.H2O -= maxGlucoseToProduce * 12;
		          pendingCompounds = ATPtoADP(maxGlucoseToProduce * 18, pendingCompounds);
		          pendingCompounds.CO2 -= maxGlucoseToProduce * 6;
		          pendingCompounds = NADPHtoNADP(maxGlucoseToProduce * 12, pendingCompounds);
		          pendingCompounds.Glucose += maxGlucoseToProduce;

		          // Check if the updates are valid
		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{maxGlucoseToProduce} glucose produced");
				return pendingCompounds;
		          }

		          // Log an error if the updates are not valid
		          Debug.Log("Not enough compounds for metabolism");
		          return currentCompounds;
	     }

	     // Function for glycolysis
	     public MetabolicCompounds Glycolysis(MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          int maxGlucoseToUse = (int)Mathf.Min(
		              currentCompounds.Glucose,
		              currentCompounds.ADP / 2,
		              currentCompounds.NAD / 2  // Changed from NADP to NAD
		          );

		          pendingCompounds.Glucose -= maxGlucoseToUse;
		          pendingCompounds.Pi -= maxGlucoseToUse * 2;
		          pendingCompounds.Pyruvate += maxGlucoseToUse * 2;
		          pendingCompounds = ADPtoATP(maxGlucoseToUse * 2, pendingCompounds);

		          // Convert NAD to NADH instead of NADP to NADPH
		          pendingCompounds.NADH += maxGlucoseToUse * 2;
		          pendingCompounds.NAD -= maxGlucoseToUse * 2;

		          pendingCompounds.H2O += maxGlucoseToUse * 2;

		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{maxGlucoseToUse} glucose processed in glycolysis");
				return pendingCompounds;
		          }

		          return currentCompounds;
	     }

	     // 1. LACTATE FERMENTATION (Anaerobic Respiration)
	     public MetabolicCompounds LactateFermentation(MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds.Clone();

		          // Pyruvate + NADH → Lactate + NAD⁺
		          int maxPyruvate = (int)Mathf.Min(
		              pendingCompounds.Pyruvate,
		              pendingCompounds.NADH
		          );

		          pendingCompounds.Pyruvate -= maxPyruvate;
		          pendingCompounds.Lactate += maxPyruvate;
		          pendingCompounds.NADH -= maxPyruvate;
		          pendingCompounds.NAD += maxPyruvate;

		          Debug.Log($"Lactate fermentation: {maxPyruvate} lactate produced");
		          return pendingCompounds;
	     }

	     // 2. STARCH SYNTHESIS
	     public MetabolicCompounds StarchSynthesis(MetabolicCompounds currentCompounds, int glucoseUnits)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds.Clone();

		          // n Glucose + ATP → Starch + ADP + Pi
		          // Requires 1 ATP per glucose for activation
		          int maxUnits = (int)Mathf.Min(
		              pendingCompounds.Glucose,
		              pendingCompounds.ATP
		          );

		          int actualUnits = Mathf.Min(glucoseUnits, maxUnits);

		          pendingCompounds.Glucose -= actualUnits;
		          pendingCompounds.ATP -= actualUnits;
		          pendingCompounds.ADP += actualUnits;
		          pendingCompounds.Pi += actualUnits;
		          pendingCompounds.Starch += actualUnits;

		          Debug.Log($"Starch synthesis: {actualUnits} glucose → starch");
		          return pendingCompounds;
	     }

	     // 3. CELLULOSE SYNTHESIS
	     public MetabolicCompounds CelluloseSynthesis(MetabolicCompounds currentCompounds, int glucoseUnits)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds.Clone();

		          // n Glucose + ATP + UTP → Cellulose + ADP + UDP
		          // Simplified: 2 ATP per glucose
		          int maxUnits = (int)Mathf.Min(
		              pendingCompounds.Glucose,
		              pendingCompounds.ATP / 2
		          );

		          int actualUnits = Mathf.Min(glucoseUnits, maxUnits);

		          pendingCompounds.Glucose -= actualUnits;
		          pendingCompounds.ATP -= actualUnits * 2;
		          pendingCompounds.ADP += actualUnits * 2;
		          pendingCompounds.Pi += actualUnits * 2;
		          pendingCompounds.Cellulose += actualUnits;

		          Debug.Log($"Cellulose synthesis: {actualUnits} glucose → cellulose");
		          return pendingCompounds;
	     }

	     // 4. STARCH/Cellulose BREAKDOWN
	     public MetabolicCompounds StarchBreakdown(MetabolicCompounds currentCompounds, int starchUnits)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds.Clone();

		          // Starch + H₂O → n Glucose (via amylase)
		          int actualUnits = (int)Mathf.Min(pendingCompounds.Starch, starchUnits);

		          pendingCompounds.Starch -= actualUnits;
		          pendingCompounds.Glucose += actualUnits;
		          pendingCompounds.H2O -= actualUnits; // Water used in hydrolysis

		          Debug.Log($"Starch breakdown: {actualUnits} starch → glucose");
		          return pendingCompounds;
	     }

	     // 5. ADD FAD/FADH2 to TCA CYCLE
	     public MetabolicCompounds CitricAcidCycle(MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds.Clone();

		          // For each Acetyl-CoA:
		          // Produces: 3 NADH, 1 FADH2, 1 ATP, 2 CO2
		          int maxAcetylCoA = (int)Mathf.Min(
		              pendingCompounds.AcetylCoA,
		              pendingCompounds.NAD / 3,
		              pendingCompounds.FAD,
		              pendingCompounds.ADP
		          );

		          pendingCompounds.AcetylCoA -= maxAcetylCoA;
		          pendingCompounds.NAD -= maxAcetylCoA * 3;
		          pendingCompounds.NADH += maxAcetylCoA * 3;
		          pendingCompounds.FAD -= maxAcetylCoA;
		          pendingCompounds.FADH2 += maxAcetylCoA;
		          pendingCompounds.ADP -= maxAcetylCoA;
		          pendingCompounds.ATP += maxAcetylCoA;
		          pendingCompounds.CO2 += maxAcetylCoA * 2;

		          Debug.Log($"TCA Cycle: {maxAcetylCoA} Acetyl-CoA processed");
		          return pendingCompounds;
	     }

	     // 6. UREA EXCRETION
	     public MetabolicCompounds ExcreteUrea(MetabolicCompounds currentCompounds, int ureaAmount)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds.Clone();

		          int actualAmount = (int)Mathf.Min(pendingCompounds.Urea, ureaAmount);
		          pendingCompounds.Urea -= actualAmount;

		          Debug.Log($"Excreted {actualAmount} urea");
		          return pendingCompounds;
	     }
	     // **NEW: OXIDATIVE PHOSPHORYLATION (Electron Transport Chain)**
	     public MetabolicCompounds OxidativePhosphorylation(MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Calculate based on available NADH and O2
		          // Each NADH produces ~2.5 ATP, each FADH2 produces ~1.5 ATP
		          // Simplified: NADH → 3 ATP, needs 0.5 O2
		          int maxNADHtoUse = (int)Mathf.Min(
		              currentCompounds.NADH,
		              currentCompounds.ADP / 3,
		              currentCompounds.O2 * 2  // 0.5 O2 per NADH
		          );

		          // Consume NADH and O2 to produce ATP
		          pendingCompounds.NADH -= maxNADHtoUse;
		          pendingCompounds.NAD += maxNADHtoUse;
		          pendingCompounds.O2 -= maxNADHtoUse / 2;
		          pendingCompounds.H2O += maxNADHtoUse;
		          pendingCompounds = ADPtoATP(maxNADHtoUse * 3, pendingCompounds);

		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{maxNADHtoUse} NADH oxidized, producing {maxNADHtoUse * 3} ATP");
				return pendingCompounds;
		          }

		          return currentCompounds;
	     }
	     // **NEW: UREA CYCLE (Nitrogen Metabolism)**
	     public MetabolicCompounds UreaCycle(MetabolicCompounds currentCompounds, int nh3ToConvert)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Simplified urea cycle: 2NH3 + CO2 + 3ATP → Urea + 2ADP + AMP + 2Pi
		          int maxNH3 = (int)Mathf.Min(
		              currentCompounds.NH3 / 2,
		              currentCompounds.CO2,
		              currentCompounds.ATP / 3
		          );

		          int actualNH3 = Mathf.Min(nh3ToConvert, maxNH3);

		          pendingCompounds.NH3 -= actualNH3 * 2;
		          pendingCompounds.CO2 -= actualNH3;
		          pendingCompounds.ATP -= actualNH3 * 3;
		          pendingCompounds.Urea += actualNH3;

		          // ATP breakdown products
		          pendingCompounds.ADP += actualNH3 * 2;
		          pendingCompounds.AMP += actualNH3;
		          pendingCompounds.Pi += actualNH3 * 2;

		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{actualNH3} urea molecules produced from {actualNH3 * 2} NH3");
				return pendingCompounds;
		          }

		          return currentCompounds;
	     }

	     // **NEW: PYRUVATE DECARBOXYLATION (link between glycolysis and TCA)**
	     public MetabolicCompounds PyruvateDecarboxylation(MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          int maxPyruvate = (int)Mathf.Min(
		              currentCompounds.Pyruvate,
		              currentCompounds.NAD
		          );

		          pendingCompounds.Pyruvate -= maxPyruvate;
		          pendingCompounds.CO2 += maxPyruvate;
		          pendingCompounds.AcetylCoA += maxPyruvate;
		          pendingCompounds.NADH += maxPyruvate;
		          pendingCompounds.NAD -= maxPyruvate;

		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{maxPyruvate} pyruvate converted to Acetyl-CoA");
				return pendingCompounds;
		          }

		          return currentCompounds;
	     }
	     // Function for respiration
	     public MetabolicCompounds Respiration(int glucoseToConvert, MetabolicCompounds currentCompounds)
	     {
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // First, run glycolysis
		          pendingCompounds.Glucose -= glucoseToConvert;
		          MetabolicCompounds afterGlycolysis = Glycolysis(new MetabolicCompounds(
		              pendingCompounds.H2O, pendingCompounds.O2, pendingCompounds.CO2,
		              pendingCompounds.Glucose, pendingCompounds.ADP, pendingCompounds.ATP,
		              pendingCompounds.AMP, pendingCompounds.NAD, pendingCompounds.NADH,
		              pendingCompounds.NADP, pendingCompounds.NADPH, pendingCompounds.Pyruvate,
		              pendingCompounds.Pi, pendingCompounds.Cellulose, pendingCompounds.Starch,
		              pendingCompounds.NH3, pendingCompounds.Urea, pendingCompounds.AcetylCoA
		          ));

		          // Then oxidative phosphorylation on the NADH produced
		          pendingCompounds = OxidativePhosphorylation(afterGlycolysis);

		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{glucoseToConvert} glucose fully respired");
				return pendingCompounds;
		          }

		          return currentCompounds;
	     }

	     // Function to grow an organ
	     public MetabolicCompounds GrowOrgan(Organs organ, float growthAmount, MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Calculate the required compounds for growth
		          float h2oRequired = growthAmount * organ.WaterRatio;
		          float glucoseRequired = growthAmount * organ.GlucoseRatio;
		          float starchRequired = growthAmount * 2500 * organ.StarchRatio;
		          float celluloseRatio = growthAmount * 5000 * organ.CelluloseRatio;
		          h2oRequired += (starchRequired + celluloseRatio) * 0.166f;

		          // Consume the required compounds for growth
		          pendingCompounds.Glucose -= (int)glucoseRequired;
		          pendingCompounds.H2O -= (int)h2oRequired;

		          // Check if the updates are valid
		          if (IsValidUpdate(pendingCompounds))
		          {
				Debug.Log($"{glucoseRequired} glucose used to grow {growthAmount} {organ.OrganType}");
				return pendingCompounds;
		          }

		          // Log an error if the updates are not valid
		          Debug.Log("Not enough compounds for metabolism to initiate growth");
		          return currentCompounds;
	     }

	     // Function to convert ATP to ADP
	     public MetabolicCompounds ATPtoADP(int atpToConvert, MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Convert ATP to ADP and release inorganic phosphate
		          pendingCompounds.ATP -= atpToConvert;
		          pendingCompounds.ADP += atpToConvert;
		          pendingCompounds.Pi += atpToConvert;

		          Debug.Log($"{atpToConvert} ATP processed into ADP");
		          return pendingCompounds;
	     }

	     // Function to convert ADP to ATP
	     public MetabolicCompounds ADPtoATP(int adpToConvert, MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Convert ADP to ATP and consume inorganic phosphate
		          pendingCompounds.ATP += adpToConvert;
		          pendingCompounds.ADP -= adpToConvert;
		          pendingCompounds.Pi -= adpToConvert;

		          Debug.Log($"{adpToConvert} ADP processed into ATP");
		          return pendingCompounds;
	     }

	     // Function to convert NADPH to NADP
	     public MetabolicCompounds NADPHtoNADP(int nadphToConvert, MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Convert NADPH to NADP
		          pendingCompounds.NADPH -= nadphToConvert;
		          pendingCompounds.NADP += nadphToConvert;

		          Debug.Log($"{nadphToConvert} NADPH processed into NADP");
		          return pendingCompounds;
	     }

	     // Function to convert NADP to NADPH
	     public MetabolicCompounds NADPtoNADPH(int nadpToConvert, MetabolicCompounds currentCompounds)
	     {
		          // Initialize a copy of the current metabolic compounds
		          MetabolicCompounds pendingCompounds = currentCompounds;

		          // Convert NADP to NADPH
		          pendingCompounds.NADPH += nadpToConvert;
		          pendingCompounds.NADP -= nadpToConvert;

		          Debug.Log($"{nadpToConvert} NADP processed into NADPH");
		          return pendingCompounds;
	     }

	     // Updated IsValidUpdate to include new compounds
	     public bool IsValidUpdate(MetabolicCompounds pendingCompounds)
	     {
		          // Check all compounds are non-negative
		          if (pendingCompounds.ADP < 0)
				return false;
		          if (pendingCompounds.ATP < 0)
				return false;
		          if (pendingCompounds.AMP < 0)
				return false;
		          if (pendingCompounds.CO2 < 0)
				return false;
		          if (pendingCompounds.Glucose < 0)
				return false;
		          if (pendingCompounds.H2O < 0)
				return false;
		          if (pendingCompounds.NAD < 0)
				return false;
		          if (pendingCompounds.NADH < 0)
				return false;
		          if (pendingCompounds.NADP < 0)
				return false;
		          if (pendingCompounds.NADPH < 0)
				return false;
		          if (pendingCompounds.O2 < 0)
				return false;
		          if (pendingCompounds.Pi < 0)
				return false;
		          if (pendingCompounds.Pyruvate < 0)
				return false;
		          if (pendingCompounds.Cellulose < 0)
				return false;
		          if (pendingCompounds.Starch < 0)
				return false;
		          if (pendingCompounds.NH3 < 0)
				return false;
		          if (pendingCompounds.Urea < 0)
				return false;
		          if (pendingCompounds.AcetylCoA < 0)
				return false;

		          return true;
	     }
}
