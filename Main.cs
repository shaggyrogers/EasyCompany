﻿/*
   Main.cs
   =======

   Description:           Plugin entry point.

*/

using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyCompany
{
    [BepInEx.BepInPlugin(PluginInfo.modGUID, PluginInfo.modName, PluginInfo.modVersion)]
    public class Main : BaseUnityPlugin
    {
        private static Menu menu;
        internal static ManualLogSource log;
        
        private ESP esp;
        private Players players;


        // Called on plugin load, initialises plugin.
        private void Awake()
        {
            Logger.LogInfo($"Initialising {PluginInfo.modName}...");

            log = base.Logger;

            // UI setup
            menu = new Menu($"{PluginInfo.modName} Menu", new Rect(10, 10, 340, 240));

            // Cheat setup
            esp = new ESP();
            players = new Players(menu);

            // Menu setup
            menu.AddTab(
                new MenuTab(
                    "Cheats",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem("Add $50", () => { Exploits.AddMoney(50); }),
                        new ButtonMenuTabItem("Add $250", () => { Exploits.AddMoney(250); }),
                        new ButtonMenuTabItem("TP Where Aiming", Exploits.TeleportWhereLooking),
                        new ButtonMenuTabItem("TP To Ship", Exploits.TeleportToShip),
                        new ButtonMenuTabItem("Fix Nearest Steam Valve", Exploits.FixNearestValve),
                        new ButtonMenuTabItem("Unlock Nearest Locked Door", Exploits.UnlockNearestLockedDoor),
                        new ButtonMenuTabItem("Spawn 4xItem From Held Gift", () => Exploits.MultiGiftBoxSpawn(4)),
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
                        new ButtonMenuTabItem("Spawn Mimic from Held Mask (Aim Pos)", Exploits.SpawnMimic),
                        new ButtonMenuTabItem("Force Game Start", Exploits.ForceGameStart),
                        new ButtonMenuTabItem("Force Game End", Exploits.ForceGameEnd),
                    }
                )
            );

            // ESP menu
            menu.AddTab(
                new MenuTab(
                    "ESP",
                    esp.GetTargets().Select(
                        target => new ToggleMenuTabItem(
                            target.name,
                            () => target.enabled,
                            (bool enabled) => target.enabled = enabled
                        )
                    ).ToList<BaseMenuTabItem>()
                )
            );

            // Testing menu
            menu.AddTab(
                new MenuTab(
                    "Test",
                    new List<BaseMenuTabItem>()
                    {
                        new ButtonMenuTabItem(
                            "Submit Max Challenge Moon Score",
                            async () => Exploits.ReallyGoodAtChallengeMoon()
                        )
                    }
                )    
            );

            Logger.LogInfo($"Initialised {PluginInfo.modName}");
        }

        // Called one or more times per frame.
        public void OnGUI()
        {
            players.Update(menu);
            esp.Draw();
            menu.Draw();
        }
    }
}
