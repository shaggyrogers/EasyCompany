/*
   Main.cs
   =======

   Description:           Plugin entry point.

*/

using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace EasyCompany
{
    [BepInEx.BepInPlugin(PluginInfo.modGUID, PluginInfo.modName, PluginInfo.modVersion)]
    public class Main : BaseUnityPlugin
    {
        private static Menu menu;
        internal static ManualLogSource log;

        // Called on plugin load, initialises plugin.
        private void Awake()
        {
            Logger.LogInfo($"Initialising {PluginInfo.modName}...");

            log = base.Logger;

            // UI set-up
            menu = new Menu();

            // Cheats
            menu.AddTab(
                new MenuTab(
                    "Cheats",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem("Add $50", () => { Exploits.AddMoney(50); }),
                        new ButtonMenuTabItem("Add $250", () => { Exploits.AddMoney(250); }),
                        new ButtonMenuTabItem("TP Where Aiming", Exploits.TeleportWhereLooking),
                        new ButtonMenuTabItem("Fix Nearest Steam Valve", Exploits.FixNearestValve),
                        new ButtonMenuTabItem("Unlock Nearest Locked Door", Exploits.UnlockNearestLockedDoor),
                        new ButtonMenuTabItem("Set Level To Intern", Exploits.SetPlayerLevelIntern),
                        new ButtonMenuTabItem("Set Level To Boss", Exploits.SetPlayerLevelBoss),
                    }
                )
            );

            // Not much point to these other than messing with other players
            menu.AddTab(
                new MenuTab(
                    "Griefing",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem("Explode Nearest Mine", Exploits.ExplodeNearestMine),
                        new ButtonMenuTabItem("Beserk Nearest Turret", Exploits.BeserkNearestTurret),
                        new ButtonMenuTabItem("Toggle Ship Lights", Exploits.ToggleShipLights),
                        new ButtonMenuTabItem("Trigger Player-Dropped Noisemakers", Exploits.TriggerDroppedNoisemakers),
                        new ButtonMenuTabItem("Force Company Monster Attack", Exploits.TriggerCompanyDeskAttack),
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
