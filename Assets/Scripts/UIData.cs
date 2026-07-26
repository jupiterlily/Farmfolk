using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

// ─── Base ──────────────────────────────────────────────────────────
public abstract class UIComponentData
{
	     public string ComponentName
	     {
		          get; set;
	     }
	     public GameObject ComponentObject
	     {
		          get; set;
	     }
	     public Vector2 MinimumSize = new Vector2(20, 20); //
	     public Vector2 PreferredSize = new Vector2(50, 30);
	     public bool VerticalLayout = true;
	     public RectOffset Padding = new RectOffset(5, 5, 5, 5);
	     public int Spacing = 5;
	     public TextAnchor ChildAlignment = TextAnchor.UpperLeft;

	     public bool DisplayLabel = true;

	     public bool ReverseArrangement = false;
	     public bool ControlChildWidth = true;
	     public bool ControlChildHeight = true;
	     public bool UseChildScaleWidth = false;
	     public bool UseChildScaleHeight = false;
	     public bool ChildForceExpandWidth = true;
	     public bool ChildForceExpandHeight = false;
	     public RectTransform rect = new RectTransform();
	     protected UIComponentData(string name = "Component")
	     {
		          ComponentName = name;
	     }
}

// ─── Structural (have a LayoutGroup) ─────────────────────────────
public abstract class UIStructuralComponent : UIComponentData
{
	     protected UIStructuralComponent(string name = "Structural") : base(name) { }
}

// ─── Window ────────────────────────────────────────────────────────
public class UIWindow : UIStructuralComponent
{
	     public List<UITab> Tabs = new List<UITab>();
	     public bool CanResize = true;
	     public bool CanClose = true;
	     public bool CanMinimize = true;
	     //  public Vector2 DefaultSize = new Vector2(400, 600);

	     public UIWindow(string name, List<UITab> tabs = null) : base(name)
	     {
		          if (tabs != null)
				Tabs = tabs;
	     }
}

// ─── Tab ───────────────────────────────────────────────────────────
public class UITab : UIStructuralComponent
{
	     public List<UISection> Sections = new List<UISection>();

	     public UITab(string name, List<UISection> sections = null) : base(name)
	     {
		          if (sections != null)
				Sections = sections;
	     }
}

// ─── Section ──────────────────────────────────────────────────────
public class UISection : UIStructuralComponent
{
	     //a section can contain fields or sections.
	     public List<UIComponentData> Components = new List<UIComponentData>();

	     public UISection(string name, bool Vertical, List<UIComponentData> components = null) : base(name)
	     {

		          if (components != null)
		          {
				MinimumSize.y = 0;  //height
				MinimumSize.x = 0; //length
				if (!Vertical)
				{

				}
				foreach (var comp in components)
				{
					     if (!(comp is UIField) && !(comp is UISection))
						          throw new ArgumentException($"Component '{comp.ComponentName}' is not a UIField or UISection");
					     Components.Add(comp);
					     MinimumSize += comp.MinimumSize;
				}
				// here a section should size itself according to the components within it.
		          }
	     }
}
//sections for pepos, tileData , PopulationData, etc. can be derived from this base class to add specific data binding and functionality.
public class UISection_Pepo : UISection
{
	     public PepoData pepo;
	     public UISection_Pepo(string name, PepoData pepo, bool Vertical, List<UIComponentData> components = null) : base(name, Vertical, components)
	     {
		          this.pepo = pepo;
	     }
}

public class UISection_Tile : UISection
{
	     public TileData tileData;
	     public UISection_Tile(string name, TileData tileData, bool Vertical, List<UIComponentData> components = null) : base(name, Vertical, components)
	     {
		          this.tileData = tileData;
	     }
}

public class UISection_Population : UISection
{
	     public PepoPopulation population;
	     public UISection_Population(string name, PepoPopulation populationData, bool Vertical, List<UIComponentData> components = null) : base(name, Vertical, components)
	     {
		          this.population = populationData;
	     }
}



// ─── Fields ────────────────────────────────────────────────────────
public abstract class UIField : UIComponentData
{
	     public string LabelText
	     {
		          get; set;
	     }
	     public int FontSize = 14;
	     public bool IsReadOnly = false;

	     // Data binding delegates
	     public Func<object> Getter
	     {
		          get; set;
	     }
	     public Action<object> Setter
	     {
		          get; set;
	     }

	     public T GetValue<T>() => (T)Getter?.Invoke();
	     public void SetValue<T>(T value) => Setter?.Invoke(value);



	     protected UIField(string name, string label) : base(name) => LabelText = label;

	     // Virtual refresh – derived fields override if they need to update visuals from data
	     public virtual void Refresh()
	     {
	     }
}

// ─── Concrete Field Types ─────────────────────────────────────────

public class TextInputField : UIField
{
	     public int MaxLength = 50;
	     public string DefaultText = "";
	     public TMP_Text Text;
	     public TextInputField(string label, string defaultText = "", int fontSize = 14)
	         : base("Field_TextInput", label)
	     {
		          DefaultText = defaultText;
		          FontSize = fontSize;
		          IsReadOnly = false;
		          //MinimumSize = new Vector2(FontSize * defaultText.Length * 0.5f, fontSize + 4);
		          VerticalLayout = false;
		          MinimumSize = UIUtilities.GetFieldMinimumSize(this, defaultText.Length);
	     }
}



public class ReadOnlyField : UIField
{
	     public string DefaultText = "";
	     public TMP_Text Text;
	     public ReadOnlyField(string label, string defaultText = "", int fontSize = 14)
	         : base("Field_ReadOnly", label)
	     {
		          DefaultText = defaultText;
		          FontSize = fontSize;
		          IsReadOnly = true;
		          MinimumSize = UIUtilities.GetFieldMinimumSize(this, defaultText.Length);
		          VerticalLayout = false;
	     }
	     public override void Refresh()
	     {
		          if (Text != null)
				Text.text = Getter?.Invoke()?.ToString() ?? DefaultText;
	     }
}

public class RangeField : UIField
{
	     public float MinValue = 0f;
	     public float MaxValue = 100f;
	     public bool UseIntStep = false;
	     public UnityEngine.UI.Slider Slider;
	     public RangeField(string label, float min = 0f, float max = 100f, bool intStep = false, int fontSize = 14)
	         : base("Field_Range", label)
	     {
		          MinValue = min;
		          MaxValue = max;
		          UseIntStep = intStep;
		          FontSize = fontSize;
		          MinimumSize = UIUtilities.GetFieldMinimumSize(this, 50);
		          VerticalLayout = false;
	     }
}

public class ToggleField : UIField
{
	     public bool DefaultValue = false;
	     public RadioButton button;


	     public ToggleField(string label, bool defaultValue = false, int fontSize = 14)
	         : base("Field_Toggle", label)
	     {
		          DefaultValue = defaultValue;
		          FontSize = fontSize;
		          MinimumSize = UIUtilities.GetFieldMinimumSize(this, fontSize);
		          VerticalLayout = false;
	     }
}

public class DropdownField : UIField
{
	     public List<string> Options = new List<string>();          // static fallback
	     public Func<List<string>> OptionsGetter;                   // dynamic provider
	     public int SelectedIndex = 0;
	     public TMP_Dropdown dropdown;
	     public DropdownField(string label, List<string> options = null, int selectedIndex = 0, int fontSize = 14)
	         : base("Field_Dropdown", label)
	     {
		          if (options != null)
				Options = options;
		          SelectedIndex = selectedIndex;
		          FontSize = fontSize;
		          int longestString = 0;
		          foreach (string option in Options)
		          {
				if (option.Length > longestString)
					     longestString = option.Length;
		          }
		          MinimumSize = UIUtilities.GetFieldMinimumSize(this, longestString);
		          VerticalLayout = true;
	     }
	     public override void Refresh()
	     {
		          if (dropdown == null)
				return;
		          List<string> optionList = OptionsGetter?.Invoke() ?? Options;
		          if (optionList != null)
		          {
				dropdown.options.Clear();
				foreach (string opt in optionList)
					     dropdown.options.Add(new TMP_Dropdown.OptionData(opt));
				dropdown.RefreshShownValue();
		          }
		          if (Getter != null)
				dropdown.value = (int)Getter();
	     }
}
public class SpriteField : UIField
{
	     public Sprite Image;

	     public SpriteField(string label, Sprite image = null, int fontSize = 14)
	         : base("Field_Image", label)
	     {
		          Image = image;
		          FontSize = fontSize;
		          MinimumSize = new Vector2(image.rect.width, image.rect.height);
		          VerticalLayout = false;
	     }
}

public class GraphField : UIField
{
	     public GraphType GraphType = GraphType.radar;
	     public float Radius = 1f;
	     public float Size = 80f;

	     public GraphField(string label, GraphType type = GraphType.radar, float radius = 1f, float size = 80f)
	         : base("Field_Graph", label)
	     {
		          GraphType = type;
		          Radius = radius;
		          Size = size;
		          MinimumSize = new Vector2(size, size);
		          VerticalLayout = false;
	     }
}
public class PortraitField : UIField
{
	     //a portrait is afield that displays all of the portrait sprites layered on top of each other to create a composite image of a pepo. it can be used to display a pepo's face, body, or any other composite image.
	     public PepoData subject;
	     public PortraitField(string label, int fontSize = 14)
	         : base("Field_Portrait", label)
	     {

	     }
}

public class PortraitScrollerField : UIField
{
	     //this is essentially a dropdown
	     public List<PepoData> Subjects = new List<PepoData>();

	     public PortraitScrollerField(string label, List<PepoData> subjects = null, float cardWidth = 100f, float cardHeight = 130f)
	         : base("Field_PortraitScroller", label)
	     {
		          if (subjects != null)
				Subjects = subjects;
		          MinimumSize = new Vector2(cardWidth, cardHeight);
		          VerticalLayout = false;
	     }
}

public class ButtonField : UIField
{
	     // public string ButtonText = "Click";
	     public Action OnClick;
	     public UnityEngine.UI.Button UnityButton; // <-- NEW

	     public ButtonField(string label, string buttonText = "Click", Action onClick = null, int fontSize = 14)
	         : base("Field_Button", label)
	     {
		          LabelText = buttonText;
		          OnClick = onClick;
		          FontSize = fontSize;
		          VerticalLayout = false;
		          MinimumSize = UIUtilities.GetFieldMinimumSize(this, buttonText.Length);
		          // Sizing will be set via UIUtilities.GetFieldMinimumSize later
	     }
}

// ─── Tooltip ──────────────────────────────────────────────────────
public class UITooltip : UIComponentData
{
	     public UIField TooltipField;
	     public UISection popUp;
	     public UITooltip(string name, UIField tooltipField) : base(name)
	     {
		          TooltipField = tooltipField;
	     }
}