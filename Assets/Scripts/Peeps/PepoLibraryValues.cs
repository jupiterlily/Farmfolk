using System.Collections.Generic;
using UnityEngine;

public static class PepoLibraryValues
{
	     public static List<Value> basicHumanValues()
	     {
		          var list = new List<Value>();

		          // Openness to change
		          Value Agency = new Value(); //0
		          Agency.name = "agency";
		          Agency.opposite = "obedience";
		          Agency.descriptor = "independent thought and action—choosing, creating, and exploring";
		          Agency.location = (40f * Mathf.PI / 180f);
		          list.Add(Agency);

		          Value Stimulation = new Value(); //1
		          Stimulation.name = "stimulation";
		          Stimulation.opposite = "stability"; // More than routine - active resistance to growth
		          Stimulation.descriptor = "excitement, novelty and challenge in life";
		          Stimulation.location = (80f * Mathf.PI / 180f);
		          list.Add(Stimulation);

		          // Self-enhancement
		          Value Hedonism = new Value();//2
		          Hedonism.name = "hedonism";
		          Hedonism.opposite = "humility"; // Sacrificing personal pleasure for group cohesion
		          Hedonism.descriptor = "pleasure or sensuous gratification for oneself";
		          Hedonism.location = (120f * Mathf.PI / 180f);
		          list.Add(Hedonism);

		          Value Achievement = new Value();//3
		          Achievement.name = "achievement";
		          Achievement.opposite = "concern"; // Shared success vs individual accomplishment
		          Achievement.descriptor = "personal success through demonstrating competence according to social standards";
		          Achievement.location = (160f * Mathf.PI / 180f);
		          list.Add(Achievement);

		          Value Power = new Value();//4
		          Power.name = "power";
		          Power.opposite = "solidarity"; // Exactly right - collective power vs individual dominance
		          Power.descriptor = "social status and prestige, control or dominance over people and resources";
		          Power.location = (200f * Mathf.PI / 180f);
		          list.Add(Power);

		          // Conservation
		          Value Security = new Value();//5
		          Security.name = "security";
		          Security.opposite = "discovery"; // The courage to be exposed and uncertain
		          Security.descriptor = "safety, harmony, and stability of society, of relationships, and of self";
		          Security.location = (240f * Mathf.PI / 180f);
		          list.Add(Security);

		          Value Conformity = new Value();//6
		          Conformity.name = "conformity";
		          Conformity.opposite = "authenticity"; // Active non-conformity, not just individualism
		          Conformity.descriptor = "restraint of actions, inclinations, and impulses likely to upset or harm others and violate social expectations or norms";
		          Conformity.location = (280f * Mathf.PI / 180f);
		          list.Add(Conformity);

		          Value Tradition = new Value();//7
		          Tradition.name = "tradition";
		          Tradition.opposite = "innovation"; // Not just progress - active dismantling of established systems
		          Tradition.descriptor = "respect, commitment, and acceptance of the customs and ideas that one's culture or religion provides";
		          Tradition.location = (320f * Mathf.PI / 180f);
		          list.Add(Tradition);

		          // Self-transcendence
		          Value Benevolence = new Value();//8
		          Benevolence.name = "benevolence";
		          Benevolence.opposite = "exploitation"; // Active harm vs passive self-interest
		          Benevolence.descriptor = "preserving and enhancing the welfare of those with whom one is in frequent personal contact (the 'in-group')";
		          Benevolence.location = (0f * Mathf.PI / 180f);
		          list.Add(Benevolence);

		          Value Universalism = new Value();//9
		          Universalism.name = "universalism";
		          Universalism.opposite = "supremacy"; // Fear of the other vs embrace of all
		          Universalism.descriptor = "understanding, appreciation, tolerance, and protection for the welfare of all people and for nature";
		          Universalism.location = (360f * Mathf.PI / 180f);
		          list.Add(Universalism);

		          foreach (var v in list)
		          {
				v.maxValue = 7;//7 = acted on in the last 12 hours, 6 = acted on in the last day, 5 = acted on in the last week = 4 acted on in the last month, 3 =  acted on in the last quarter, 2 = acted on in the last 6 months, 1 = acted on in the last year
				v.value = 0;
		          }

		          return list;
	     }

	     // Openness to change
	     public static Value Agency => basicHumanValues()[0];
	     public static Value Stimulation => basicHumanValues()[1];
	     public static Value Hedonism => basicHumanValues()[2];
	     public static Value Achievement => basicHumanValues()[3];
	     public static Value Power => basicHumanValues()[4];
	     public static Value Security => basicHumanValues()[5];
	     public static Value Conformity => basicHumanValues()[6];
	     public static Value Tradition => basicHumanValues()[7];
	     public static Value Benevolence => basicHumanValues()[8];
	     public static Value Universalism => basicHumanValues()[9];

	     //Openness to change

	     //    Self-direction – independent thought and action—choosing, creating, and exploring
	     //    Stimulation – excitement, novelty and challenge in life

	     //Self-enhancement

	     //    Hedonism – pleasure or sensuous gratification for oneself
	     //    Achievement – personal success through demonstrating competence according to social standards
	     //    Power – social status and prestige, control or dominance over people and resources

	     //Conservation

	     //    Security – safety, harmony, and stability of society, of relationships, and of self
	     //    Conformity – restraint of actions, inclinations, and impulses likely to upset or harm others and violate social expectations or norms
	     //    Tradition – respect, commitment, and acceptance of the customs and ideas that one's culture or religion provides

	     //Self-transcendence

	     //    Benevolence – preserving and enhancing the welfare of those with whom one is in frequent personal contact (the 'in-group')
	     //    Universalism – understanding, appreciation, tolerance, and protection for the welfare of all people and for nature
}
