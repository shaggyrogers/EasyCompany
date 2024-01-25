/*
   Main.cs
   =======

   Description:           Plugin entry point.

*/

using BepInEx;
using System.Collections.Generic;
using UnityEngine;

namespace EasyCompany
{
    [BepInEx.BepInPlugin(PluginInfo.modGUID, PluginInfo.modName, PluginInfo.modVersion)]
    public class Main : BaseUnityPlugin
    {
        private Menu menu;

        // Called on plugin load, initialises plugin.
        private void Awake()
        {
            Logger.LogInfo($"Initialising {PluginInfo.modName}...");

            // UI set-up
            menu = new Menu();

            // Placeholder items for testing
            menu.AddTab(
                new MenuTab(
                    "Test1",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem("Button1", () => {}),
                        new ButtonMenuTabItem("Button2", () => {}),
                        new ButtonMenuTabItem("Button3", () => {}),
                        new ButtonMenuTabItem("Button4", () => {}),
                        new ButtonMenuTabItem("Button5", () => {}),
                        new ButtonMenuTabItem("Button6", () => {}),
                        new ButtonMenuTabItem("Button7", () => {}),
                        new ButtonMenuTabItem("Button8", () => {}),
                        new ButtonMenuTabItem("Button9", () => {}),
                        new ButtonMenuTabItem("Button10", () => {}),
                        new ButtonMenuTabItem("Button11", () => {}),
                        new ButtonMenuTabItem("Button12", () => {}),
                        new ButtonMenuTabItem("Button13", () => {}),
                    }
                )
            );
            menu.AddTab(
                new MenuTab(
                    "Test2",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem("Button1", () => {}),
                        new ButtonMenuTabItem("Button2", () => {}),
                        new ButtonMenuTabItem("Button3", () => {}),
                        new ButtonMenuTabItem("Button4", () => {}),
                        new ButtonMenuTabItem("Button5", () => {}),
                        new ButtonMenuTabItem("Button6", () => {}),
                        new ButtonMenuTabItem("Button7", () => {}),
                        new ButtonMenuTabItem("Button8", () => {}),
                        new ButtonMenuTabItem("Button9", () => {}),
                        new ButtonMenuTabItem("Button10", () => {}),
                        new ButtonMenuTabItem("Button11", () => {}),
                        new ButtonMenuTabItem("Button12", () => {}),
                        new ButtonMenuTabItem("Button13", () => {}),
                    }
                )
            );
            menu.AddTab(
                new MenuTab(
                    "Test3",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem("Button1", () => {}),
                        new ButtonMenuTabItem("Button2", () => {}),
                        new ButtonMenuTabItem("Button3", () => {}),
                        new ButtonMenuTabItem("Button4", () => {}),
                        new ButtonMenuTabItem("Button5", () => {}),
                        new ButtonMenuTabItem("Button6", () => {}),
                        new ButtonMenuTabItem("Button7", () => {}),
                        new ButtonMenuTabItem("Button8", () => {}),
                        new ButtonMenuTabItem("Button9", () => {}),
                        new ButtonMenuTabItem("Button10", () => {}),
                        new ButtonMenuTabItem("Button11", () => {}),
                        new ButtonMenuTabItem("Button12", () => {}),
                        new ButtonMenuTabItem("Button13", () => {}),
                    }
                )
            );

            Logger.LogInfo($"Initialised {PluginInfo.modName}");
        }

        // Called one or more times per frame.
        public void OnGUI()
        {
            menu.Draw();
        }
    }
}
