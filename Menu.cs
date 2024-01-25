/*
   Menu.cs
   =======

   Description:           Implements UI logic. Wrapper for Unity IMGUI.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EasyCompany
{
    // Represents a menu tab and all elements it contains.
    internal class MenuTab
    {
        public string name;
        public UnityEngine.Vector2 scrollPos = new UnityEngine.Vector2(0, 0);
        public List<BaseMenuTabItem> items;

        public MenuTab(string name, List<BaseMenuTabItem> items)
        {
            this.name = name;
            this.items = items;
        }
    }

    // Parent virtual class for elements inside a tab
    internal class BaseMenuTabItem
    {
        public virtual void Draw()
        {
            throw new NotImplementedException("Must use child class");
        }
    }

    // Buttons
    internal class ButtonMenuTabItem : BaseMenuTabItem
    {
        private string label;
        private Action onClick;

        public ButtonMenuTabItem(String label, Action onClick)
        {
            this.label = label;
            this.onClick = onClick;
        }

        public override void Draw()
        {
            if (GUILayout.Button(this.label))
            {
                onClick();
            }
        }
    }

    // Interface for the UI
    internal class Menu
    {
        // Size/position
        private Rect rect = new Rect(10, 10, 128, 256);

        // Tabs
        private int selectedTabIdx = 0;
        private List<MenuTab> tabs = new List<MenuTab>();
        
        public Menu() { }

        public void AddTab(MenuTab tab)
        {
            tabs.Add(tab);
        }

        // Draws the UI.
        public void Draw()
        {
            // Background
            rect = GUILayout.Window(1024, rect, DrawWindow, "Menu");
        }

        private void DrawWindow(int windowID)
        {

            GUILayout.BeginVertical();

            // Draw tabs
            selectedTabIdx = GUILayout.Toolbar(selectedTabIdx, tabs.Select(t => t.name).ToArray<String>());

            // Draw current tab
            MenuTab curTab = tabs[selectedTabIdx];
            curTab.scrollPos = GUILayout.BeginScrollView(curTab.scrollPos);
            curTab.items.ForEach(item => item.Draw());

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            // Drag window (call here so this only happens if no control is clicked)
            // Alternatively could limit drag area to title bar
            // see https://docs.unity3d.com/ScriptReference/GUI.DragWindow.html
            GUI.DragWindow();
        }
    }
}
