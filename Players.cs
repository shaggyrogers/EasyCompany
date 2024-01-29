/*
   Players.cs
   ==========

   Description:           Provides the player menu features.

*/

using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EasyCompany
{
    internal class Players
    {
        private PlayerControllerB[] players;

        private float lastUpdateTime = 0f;
        private const float updateRate = 1f; // time between updates

        private const String playerMenuTitle = "Players";

        public Players(Menu menu) 
        { 
            players = new PlayerControllerB[0];

            // Create player menu tab
            menu.AddTab(
                new MenuTab(
                    playerMenuTitle,
                    new List<BaseMenuTabItem>()
                )
            );
        }

        // Update player list and menu if necessary
        public void Update(Menu menu)
        {
            if (Time.time - lastUpdateTime < updateRate)
            {
                // Too soon
                return;
            }

            // Update player list
            players = UnityEngine.Object.FindObjectsOfType<PlayerControllerB>().Where(
                p => p.isPlayerControlled
            ).ToArray();

            // Update player menu
            UpdateMenu(menu);

            lastUpdateTime = Time.time;
        }

        // Update the player menu
        private void UpdateMenu(Menu menu) 
        {
            menu.UpdateTab(
                playerMenuTitle,
                players.Select(
                    p => new HContainerMenuTabItem(
                        $"{p.playerUsername} [{p.health}%]",
                        new List<BaseMenuTabItem>()
                        {
                            new ButtonMenuTabItem("Kill", () => KillPlayer(p)),
                            new ButtonMenuTabItem(
                                "Drop Items",
                                // This doesn't require ownership, for some reason
                                () => p.DropAllHeldItemsServerRpc()
                            ),
                        }
                    )
                ).ToList<BaseMenuTabItem>()
            );
        }

        // Kill a player
        private void KillPlayer(PlayerControllerB target)
        {
            // Tell server player was hit by no one for 101 damage from below
            target.DamagePlayerFromOtherClientServerRpc(101, Vector3.up, -1);
        }
    }
}
