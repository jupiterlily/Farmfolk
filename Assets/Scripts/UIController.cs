using System.Collections.Generic;
using UnityEngine;

public class UIWindowController : MonoBehaviour
{
	     private UIWindow windowDef;
	     private List<GameObject> tabs;
	     private List<GameObject> buttons;
	     private int activeTab = 0;

	     public void Register(UIWindow def, List<GameObject> tabGOs, List<GameObject> buttonGOs)
	     {
		          windowDef = def;
		          tabs = tabGOs;
		          buttons = buttonGOs;
		          SwitchTab(0);
	     }

	     public void SwitchTab(int index)
	     {
		          if (tabs == null)
				return;
		          for (int i = 0; i < tabs.Count; i++)
				tabs[i].SetActive(i == index);
		          activeTab = index;
	     }

	     // Future: OnClose(), OnMinimise(), OnResize(), OnDrag() etc.
}
