using System.Collections.Generic;
using UnityEngine;

public static class PepoLibraryValues
{
	     public static List<Value> basicHumanValues()
	     {
		          var list = new List<Value>();

		          // Openness to change - Agency splits
		          //self direction
		          Value Curiosity = new Value(); //0
		          Curiosity.name = "curiosity";
		          Curiosity.opposite = "dogma";
		          Curiosity.descriptor = "exploring ideas and questioning the world";
		          Curiosity.location = (30f * Mathf.PI / 180f);
		          list.Add(Curiosity);

		          //self direction
		          Value Autonomy = new Value(); //1
		          Autonomy.name = "autonomy";
		          Autonomy.opposite = "servility";
		          Autonomy.descriptor = "acting independently and making own choices";
		          Autonomy.location = (50f * Mathf.PI / 180f);
		          list.Add(Autonomy);

		          //stimulation
		          Value Excitement = new Value(); //2
		          Excitement.name = "excitement";
		          Excitement.opposite = "routine";
		          Excitement.descriptor = "seeking novelty, thrills, and stimulation";
		          Excitement.location = (80f * Mathf.PI / 180f);
		          list.Add(Excitement);

		          //health
		          Value Vitality = new Value(); //3
		          Vitality.name = "vitality";
		          Vitality.opposite = "deprivation";
		          Vitality.descriptor = "pursuing health, energy, and well-being";
		          Vitality.location = (100f * Mathf.PI / 180f);
		          list.Add(Vitality);

		          // Self-enhancement
		          //hedonism
		          Value Indulgence = new Value(); //4
		          Indulgence.name = "indulgence";
		          Indulgence.opposite = "austerity";
		          Indulgence.descriptor = "seeking pleasure and sensuous gratification";
		          Indulgence.location = (120f * Mathf.PI / 180f);
		          list.Add(Indulgence);

		          //achievement
		          Value Accomplishment = new Value(); //5
		          Accomplishment.name = "accomplishment";
		          Accomplishment.opposite = "complacency";
		          Accomplishment.descriptor = "achieving success and demonstrating competence";
		          Accomplishment.location = (140f * Mathf.PI / 180f);
		          list.Add(Accomplishment);

		          // Power splits
		          Value Dominance = new Value(); //6
		          Dominance.name = "dominance";
		          Dominance.opposite = "submission";
		          Dominance.descriptor = "controlling and dominating others";
		          Dominance.location = (170f * Mathf.PI / 180f);
		          list.Add(Dominance);

		          Value Accumulation = new Value(); //7
		          Accumulation.name = "resources";
		          Accumulation.opposite = "generosity";
		          Accumulation.descriptor = "gathering resources, wealth, and possessions";
		          Accumulation.location = (190f * Mathf.PI / 180f);
		          list.Add(Accumulation);
		          //face
		          Value Status = new Value(); //8
		          Status.name = "status";
		          Status.opposite = "shame";
		          Status.descriptor = "maintaining social standing and reputation";
		          Status.location = (210f * Mathf.PI / 180f);
		          list.Add(Status);

		          // Conservation - Security splits
		          //security
		          Value Security = new Value(); //9
		          Security.name = "safety";
		          Security.opposite = "risk";
		          Security.descriptor = "ensuring personal safety and stability";
		          Security.location = (230f * Mathf.PI / 180f);
		          list.Add(Security);
		          //security
		          Value Order = new Value(); //10
		          Order.name = "security";
		          Order.opposite = "chaos";
		          Order.descriptor = "maintaining societal structure and predictability";
		          Order.location = (250f * Mathf.PI / 180f);
		          list.Add(Order);
		          //tradition
		          Value Tradition = new Value(); //11
		          Tradition.name = "tradition";
		          Tradition.opposite = "iconoclasm";
		          Tradition.descriptor = "upholding established customs and heritage";
		          Tradition.location = (270f * Mathf.PI / 180f);
		          list.Add(Tradition);

		          // Conformity splits
		          //conformity
		          Value Obedience = new Value(); //12
		          Obedience.name = "obedience";
		          Obedience.opposite = "rebellion";
		          Obedience.descriptor = "following rules and authority structures";
		          Obedience.location = (290f * Mathf.PI / 180f);
		          list.Add(Obedience);
		          //conformity
		          Value Conformity = new Value(); //13
		          Conformity.name = "cohesion";
		          Conformity.opposite = "authenticity";
		          Conformity.descriptor = "fitting in and meeting social expectations";
		          Conformity.location = (310f * Mathf.PI / 180f);
		          list.Add(Conformity);
		          //humility
		          Value Humility = new Value(); //14
		          Humility.name = "humility";
		          Humility.opposite = "arrogance";
		          Humility.descriptor = "remaining modest and knowing one's place";
		          Humility.location = (330f * Mathf.PI / 180f);
		          list.Add(Humility);

		          // Self-transcendence - Universalism splits
		          Value Preservation = new Value(); //15
		          Preservation.name = "preservation";
		          Preservation.opposite = "extraction";
		          Preservation.descriptor = "protecting nature and the environment";
		          Preservation.location = (350f * Mathf.PI / 180f);
		          list.Add(Preservation);

		          Value Compassion = new Value(); //16
		          Compassion.name = "compassion";
		          Compassion.opposite = "exploitation";
		          Compassion.descriptor = "caring for the welfare of all people";
		          Compassion.location = (10f * Mathf.PI / 180f);
		          list.Add(Compassion);

		          Value Acceptance = new Value(); //17
		          Acceptance.name = "tolerance";
		          Acceptance.opposite = "supremacy";
		          Acceptance.descriptor = "embracing diversity and different perspectives";
		          Acceptance.location = (20f * Mathf.PI / 180f);
		          list.Add(Acceptance);

		          // Benevolence splits
		          Value Nurturing = new Value(); //18
		          Nurturing.name = "caring";
		          Nurturing.opposite = "cruelty";
		          Nurturing.descriptor = "caring for and supporting close others";
		          Nurturing.location = (340f * Mathf.PI / 180f);
		          list.Add(Nurturing);

		          Value Loyalty = new Value(); //19
		          Loyalty.name = "dependability";
		          Loyalty.opposite = "unpredictability";
		          Loyalty.descriptor = "remaining dependable and faithful to one's group";
		          Loyalty.location = (0f * Mathf.PI / 180f);
		          list.Add(Loyalty);

		          foreach (var v in list)
		          {
				v.maxValue = 7;
				v.value = 0;
				v.minValue = -1;
		          }

		          return list;
	     }

	     // Static properties for each value
	     public static Value AgencyThought => basicHumanValues()[0];
	     public static Value AgencyAction => basicHumanValues()[1];
	     public static Value Stimulation => basicHumanValues()[2];
	     public static Value Health => basicHumanValues()[3];
	     public static Value Hedonism => basicHumanValues()[4];
	     public static Value Achievement => basicHumanValues()[5];
	     public static Value PowerDominance => basicHumanValues()[6];
	     public static Value PowerResources => basicHumanValues()[7];
	     public static Value Face => basicHumanValues()[8];
	     public static Value SecurityPersonal => basicHumanValues()[9];
	     public static Value SecuritySociety => basicHumanValues()[10];
	     public static Value Tradition => basicHumanValues()[11];
	     public static Value ConformityRules => basicHumanValues()[12];
	     public static Value ConformityInterpersonal => basicHumanValues()[13];
	     public static Value Humility => basicHumanValues()[14];
	     public static Value UniversalismNature => basicHumanValues()[15];
	     public static Value UniversalismConcern => basicHumanValues()[16];
	     public static Value UniversalismTolerance => basicHumanValues()[17];
	     public static Value BenevolenceCaring => basicHumanValues()[18];
	     public static Value BenevolenceDependability => basicHumanValues()[19];
}