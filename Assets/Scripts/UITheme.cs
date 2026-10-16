using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "UITheme", menuName = "UI/Theme")]
public class UITheme : ScriptableObject
{
	     // ─── BASE UNITS ──────────────────────────────────────────────
	     // These are the fundamental building blocks. Everything else derives from them.

	     [Header("--- Base Sizing ---")]
	     public float BaseUnit = 4;              // The atomic unit (padding, spacing)
	     public float ControlHeight = 6f;        // Height of all interactive controls
	     public float LabelWidth = 30f;           // Default label width
	     public int FontSize = 12;               // Base font size


	     public float DefaultContainerMinWidth => Mathf.Max(ControlHeight * 2f, MinTextFieldWidth);
	     public float DefaultContainerMinHeight => Mathf.Max(ControlHeight * 2f, MinTextFieldWidth);
	     public float DefaultSectionMinWidth => Mathf.Max(ControlHeight * 2f, MinTextFieldWidth);
	     public float DefaultSectionMinHeight => Mathf.Max(ControlHeight * 2f, MinTextFieldWidth);
	     public float DefaultTabMinWidth => Mathf.Max(ControlHeight * 3f, MinTextFieldWidth);
	     public float DefaultTabMinHeight => Mathf.Max(ControlHeight * 3f, MinTextFieldWidth);
	     public float DefaultWindowMinWidth => Mathf.Max(ControlHeight * 4f, MinTextFieldWidth);
	     public float DefaultWindowMinHeight => Mathf.Max(ControlHeight * 4f, MinTextFieldWidth);

	     //sizes for headers, footers and sidebars. I think these should be a percentage of screen width though maybe.
	     public int screenWidth = 1920;
	     public int screenHeight = 1200;
	     public float HeaderPercentage = 1f / 15f;
	     public float FooterPercentage = 1f / 8f;
	     public float LeftSidebarPercentage = 1f / 5f;
	     public float RightSidebarPercentage = 1f / 8f;
	     public float BodyWidth => screenWidth - (screenWidth * LeftSidebarPercentage) - (screenWidth * RightSidebarPercentage);
	     public float BodyHeight => screenHeight - (screenHeight * HeaderPercentage) - (screenHeight * FooterPercentage);

	     // ─── DERIVED SIZING (Auto-calculated) ──────────────────────
	     public float FieldHeight => ControlHeight;
	     public float InputFieldHeight => ControlHeight;
	     public float DropdownHeight => ControlHeight;
	     public float SliderHeight => ControlHeight;
	     public float ButtonHeight => ControlHeight + 4f;
	     public float ButtonMinWidth => LabelWidth;

	     public float MinTextFieldWidth => LabelWidth + SmallPadding; // Minimum width for text fields
	     public float MinDropdownWidth => LabelWidth;
	     public float MinRangeWidth => LabelWidth;
	     public float MinToggleWidth => LabelWidth;
	     public float MinButtonWidth => LabelWidth * 2f;
	     public float MinGraphSize => 80f;
	     public float MinImageSize => 48f;

	     public float SmallPadding => BaseUnit * 0.5f;
	     public float MediumPadding => BaseUnit;
	     public float LargePadding => BaseUnit * 1.5f;

	     public float SmallSpacing => BaseUnit * 0.5f;
	     public float MediumSpacing => BaseUnit;
	     public float LargeSpacing => BaseUnit * 1.5f;

	     public float TabHeight => ControlHeight + 4f;
	     public float SectionPadding => BaseUnit;
	     public float SectionSpacing => BaseUnit * 0.5f;
	     public float WindowPadding => BaseUnit * 1.5f;

	     public float LabelMinWidth => LabelWidth * 0.75f;
	     public float ControlMinWidth => LabelWidth * 1.5f;

	     public float SliderHandleSize => ControlHeight * 0.6f;
	     public float DropdownListMaxHeight => ControlHeight * 6f;

	     public Vector2 IconSize => new Vector2(BodyFontSize + (SmallPadding * 2), FontSize + (SmallPadding * 2));
	     public Vector2 ThumbnailSize => new Vector2(TitleFontSize * 2 + (MediumPadding * 2), (TitleFontSize * 2) + (MediumPadding * 2));

	     public float TitleFontSize => FontSize * 1.6f;
	     public float HeadingFontSize => FontSize * 1.3f;
	     public float SubheadingFontSize => FontSize * 1.1f;
	     public float BodyFontSize => FontSize;
	     public float SmallFontSize => FontSize * 0.8f;

	     // ─── SPRITES ───────────────────────────────────────────────────
	     [Header("--- Sprites ---")]
	     public Sprite WindowBackground;
	     public Sprite SectionBackground;
	     public Sprite FieldBackground;
	     public Sprite ButtonBackground;
	     public Sprite SliderHandle;
	     public Sprite SliderBackground;
	     public Sprite ToggleOn;
	     public Sprite ToggleOff;
	     public Sprite DropdownBackground;

	     [Header("--- Icons ---")]
	     public Sprite IconClose;
	     public Sprite IconMinimise;
	     public Sprite IconMaximise;
	     public Sprite IconPin;
	     public Sprite IconEyedropper;
	     public Sprite IconDropdownArrow;

	     // ─── COLORS ───────────────────────────────────────────────────
	     [Header("--- Colors ---")]
	     public Color WindowBG = new Color(0.15f, 0.15f, 0.2f, 0.95f);
	     public Color SectionBG = new Color(0.2f, 0.2f, 0.3f, 0.5f);
	     public Color TabBG = new Color(0.15f, 0.15f, 0.2f, 0.9f);
	     public Color TabButtonNormal = new Color(0.25f, 0.25f, 0.35f, 1f);
	     public Color TabButtonSelected = new Color(0.3f, 0.5f, 0.7f, 1f);
	     public Color TabButtonHover = new Color(0.3f, 0.4f, 0.5f, 1f);

	     public Color InputBackground = new Color(0.1f, 0.1f, 0.1f, 0.5f);
	     public Color InputTextColor = Color.white;
	     public Color InputPlaceholderColor = Color.gray;
	     public Color InputFocusedColor = new Color(0.2f, 0.4f, 0.6f, 0.3f);

	     public Color SliderTrackColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);
	     public Color SliderFillColor = new Color(0.2f, 0.5f, 0.8f, 1f);
	     public Color SliderHandleColor = Color.white;

	     public Color ButtonNormalColor = new Color(0.2f, 0.4f, 0.6f);
	     public Color ButtonHighlightedColor = new Color(0.3f, 0.5f, 0.7f);
	     public Color ButtonPressedColor = new Color(0.1f, 0.2f, 0.3f);
	     public Color ButtonTextColor = Color.white;

	     public Color DropdownBackgroundColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
	     public Color DropdownItemNormal = new Color(0.2f, 0.2f, 0.2f);
	     public Color DropdownItemHighlighted = new Color(0.3f, 0.3f, 0.4f);
	     public Color DropdownItemSelected = new Color(0.2f, 0.4f, 0.6f);

	     public Color TextColor = Color.white;
	     public Color TextSecondary = new Color(0.7f, 0.7f, 0.7f);
	     public Color PlaceholderColor = Color.gray;
	     public Color SeparatorColor = new Color(0.3f, 0.3f, 0.4f, 0.5f);

	     // ─── FONTS ───────────────────────────────────────────────────
	     [Header("--- Fonts ---")]
	     public TMP_FontAsset TitleFontAsset;
	     public TMP_FontAsset HeadingFontAsset;
	     public TMP_FontAsset SubheadingFontAsset;
	     public TMP_FontAsset BodyFontAsset;
	     public TMP_FontAsset SmallFontAsset;

	     // ─── PREFABS ─────────────────────────────────────────────────
	     [Header("--- Prefabs (Optional) ---")]
	     public GameObject ButtonPrefab;
	     public GameObject InputFieldPrefab;
	     public GameObject DropdownPrefab;
	     public GameObject SliderPrefab;
	     public GameObject TogglePrefab;

	     // ─── ALIGNMENT ──────────────────────────────────────────────
	     // These are used as defaults for layout groups

	     public TextAnchor DefaultAlignment = TextAnchor.MiddleLeft;
	     public TextAnchor CenterAlignment = TextAnchor.MiddleCenter;
	     public TextAnchor LabelAlignment = TextAnchor.MiddleLeft;
	     public TextAnchor ValueAlignment = TextAnchor.MiddleRight;
}