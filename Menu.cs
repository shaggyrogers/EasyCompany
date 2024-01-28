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
        // Label associated with the tab item
        protected string label;

        public BaseMenuTabItem(string label)
        {
            this.label = label;
        }

        // Draw the tab item. Implement in child classes
        public virtual void Draw()
        {
            throw new NotImplementedException("Must use child class");
        }
    }

    // Horizontal container
    internal class HContainerMenuTabItem : BaseMenuTabItem
    {
        private List<BaseMenuTabItem> items;

        // Only need this for player menu, which will always have player name as label.
        public HContainerMenuTabItem(string label, List<BaseMenuTabItem> items) : base(label) 
        {
            this.items = items;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();

            // Draw label
            GUILayout.Label(label);

            // Draw items
            foreach (var item in items) { item.Draw(); }

            GUILayout.EndHorizontal();
        }
    }

    // Buttons
    internal class ButtonMenuTabItem : BaseMenuTabItem
    {
        // Called once when clicking the button
        private Action onClick;

        public ButtonMenuTabItem(String label, Action onClick) : base(label)
        {
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

    // Toggle radio button
    internal class ToggleMenuTabItem : BaseMenuTabItem
    {
        // Called to get the value of the toggle button
        private Func<bool> getValueFn;

        // Called (once) when the value has changed. Must set the new value.
        private Action<bool> setValueFn;

        public ToggleMenuTabItem(String label, Func<bool> getValueFn, Action<bool> setValueFn) : base(label)
        {
            this.getValueFn = getValueFn;
            this.setValueFn = setValueFn;
        }

        public override void Draw()
        {
            bool curVal = getValueFn();
            bool newVal = GUILayout.Toggle(curVal, label);

            //  Call setValueFn if toggle was clicked.
            if (curVal != newVal)
            {
                setValueFn(newVal);
            }
        }
    }

    // Interface class for the UI
    internal class Menu
    {
        // Title label
        private String title;

        // Size/position
        private Rect rect;

        // Tabs
        private int selectedTabIdx = 0;
        private List<MenuTab> tabs = new List<MenuTab>();
        
        public Menu(string title, Rect rect) 
        {
            this.title = title;
            this.rect = rect;
        }

        // Inserts a new tab
        public void AddTab(MenuTab tab)
        {
            // Tab names must be unique
            Debug.Assert(tabs.All(t => t.name != tab.name));

            tabs.Add(tab);
        }

        // Draws the UI.
        public void Draw()
        {
            // Background
            rect = GUILayout.Window(1024, rect, DrawWindow, title);
        }

        // Replace the contents of a tab by name
        public void UpdateTab(String name, List<BaseMenuTabItem> newItems)
        {
            var tab = tabs.FirstOrDefault(t => t.name == name);

            // Tab should exist
            Debug.Assert(tab != null);

            tab.items = newItems;
        }

        // UI window draw function, called by IMGUI
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
            // FIXME: Alternatively could limit drag area to title bar since it's a little annoying to misclick and
            // accidentally drag the window.
            // see https://docs.unity3d.com/ScriptReference/GUI.DragWindow.html
            // FIXME: If loading into a game while dragging, window will disappear!
            GUI.DragWindow();
        }
    }
}
