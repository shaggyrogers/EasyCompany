/*
   ESP.cs
   ======

   Description:           Provides the ESP features

*/

using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace EasyCompany
{
    // Represents an ESP target (players, enemies etc)
    public class ESPTarget
    {
        // Whether to draw target
        public bool enabled;

        // Target class name to show in menu
        public String name { get; private set; }

        // Label colour
        public GUIStyle style { get; private set; }

        // Set of target instances after last update
        protected UnityEngine.Object[] cache;

        public ESPTarget(String name, Color colour)
        {
            this.enabled = false;
            this.name = name;
            this.style = new GUIStyle();
            this.style.normal.textColor = colour;
            this.cache = Array.Empty<UnityEngine.Object>();
        }

        // Reload and store the set of target instances
        public virtual void UpdateCache() { throw new NotImplementedException("Implement me pls"); }

        // Iterate over the set of target instances, yielding position and text to draw.
        // Contains any target-specific logic
        public virtual IEnumerable<Tuple<String, Vector3>> IterCache()
        {
            throw new NotImplementedException("Implement me pls");
        }
    }

    // Players
    public class PlayerESPTarget : ESPTarget
    {
        public PlayerESPTarget() : base("Players", Color.green) { }
        public override void UpdateCache() => this.cache = UnityEngine.Object.FindObjectsOfType<PlayerControllerB>();

        public override IEnumerable<Tuple<String, Vector3>> IterCache()
        {
            foreach (PlayerControllerB player in this.cache)
            {
                // Don't draw our position
                if (player == GameNetworkManager.Instance.localPlayerController)
                {
                    continue;
                }

                yield return new Tuple<String, Vector3>(player.playerUsername, player.playerGlobalHead.transform.position);
            }
        }
    }

    // Enemies
    public class EnemyESPTarget : ESPTarget
    {
        public EnemyESPTarget() : base("Enemies", Color.red) { }
        public override void UpdateCache() => this.cache = UnityEngine.Object.FindObjectsOfType<EnemyAI>();

        public override IEnumerable<Tuple<String, Vector3>> IterCache()
        {
            foreach (EnemyAI enemy in this.cache)
            {
                yield return new Tuple<String, Vector3>(enemy.enemyType?.enemyName ?? "Unknown", enemy.transform.position);
            }
        }
    }

    // Objects
    public class ObjectESPTarget : ESPTarget
    {
        public ObjectESPTarget() : base("Loot", new Color(0.9f, 0.9f, 0.9f, 0.85f)) { }
        public override void UpdateCache() => this.cache = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

        public override IEnumerable<Tuple<String, Vector3>> IterCache()
        {
            foreach (GrabbableObject obj in this.cache)
            {
                yield return new Tuple<String, Vector3>(obj.itemProperties?.itemName ?? "Unknown", obj.transform.position);
            }
        }
    }

    // Entrances/exits
    public class ExitESPTarget : ESPTarget
    {
        public ExitESPTarget() : base("Exits", Color.yellow) { }
        public override void UpdateCache() => this.cache = UnityEngine.Object.FindObjectsOfType<EntranceTeleport>();

        public override IEnumerable<Tuple<String, Vector3>> IterCache()
        {
            foreach (EntranceTeleport tp in this.cache)
            {
                // Need to add offset here as position is the bottom
                yield return new Tuple<String, Vector3>(
                    tp.isEntranceToBuilding ? "Entry" : "Exit",
                    tp.entrancePoint.position + Vector3.up * 1.5f
                );
            }
        }
    }

    // Mines
    public class MineESPTarget : ESPTarget
    {
        public MineESPTarget() : base("Mines", new Color(1f, 0.5f, 0f)) { }
        public override void UpdateCache() => this.cache = UnityEngine.Object.FindObjectsOfType<Landmine>();

        public override IEnumerable<Tuple<string, Vector3>> IterCache()
        {
            foreach (var mine in this.cache.OfType<Landmine>().Where(m => !m.hasExploded))
            {
                yield return new Tuple<string, Vector3>("Mine", mine.transform.position);
            }
        }
    }

    // Turrets
    public class TurretESPTarget : ESPTarget
    {
        public TurretESPTarget() : base("Turrets", new Color(1f, 0.5f, 0f)) { }
        public override void UpdateCache() => this.cache = UnityEngine.Object.FindObjectsOfType<Turret>();

        public override IEnumerable<Tuple<string, Vector3>> IterCache()
        {
            foreach (Turret turret in this.cache)
            {
                yield return new Tuple<string, Vector3>("Turret", turret.transform.position);
            }
        }
    }

    // Aiming position
    // Useful to judge where you will land when using the "TP where aiming" feature
    public class AimPosESPTarget : ESPTarget
    {
        public AimPosESPTarget() : base("Aim Position", Color.magenta) { }
        public override void UpdateCache() { }

        public override IEnumerable<Tuple<string, Vector3>> IterCache()
        {
            Vector3 hit;

            if (Util.RaycastFromPlayer(out hit))
            {
                yield return new Tuple<string, Vector3>("x", hit);
            }
        }
    }

    internal class ESP
    {
        // TODO: Probably better to render through walls rather than draw labels

        // Target classes
        // This defines draw order and order these appear in menu.
        private readonly ESPTarget[] targets = new ESPTarget[] {
            new PlayerESPTarget(),
            new EnemyESPTarget(),
            new ObjectESPTarget(),
            new ExitESPTarget(),
            new MineESPTarget(),
            new TurretESPTarget(),
            new AimPosESPTarget(),
        };

        // Cache
        private float lastUpdateTime = 0f;
        private const float updateRate = 1f; // time between updates

        public ESP() { }

        // Update and draw everything
        public void Draw()
        {
            // Reload cache if needed
            Update();

            // Draw all enabled targets
            foreach (var targetCls in GetTargets().Where(t => t.enabled))
            {
                foreach (var info in targetCls.IterCache())
                {
                    DrawTextWorld(info.Item1, info.Item2, targetCls.style);
                }
            }
        }

        // Returns all target classes.
        public ESPTarget[] GetTargets() => targets;

        // Perform periodic cache update if necessary
        // This is intended to minimise expensive FindObjectsOfType() calls
        private void Update()
        {
            if (Time.time - lastUpdateTime < updateRate)
            {
                // Too soon
                return;
            }

            // Update cache for all enabled targets
            foreach (var targetCls in GetTargets().Where(t => t.enabled))
            {
                targetCls.UpdateCache();
            }

            lastUpdateTime = Time.time;
        }

        // Draw text on the screen given a world position and style.
        private void DrawTextWorld(string text, Vector3 pos, GUIStyle style)
        {
            // Add distance to text
            float dist = Vector3.Distance(pos, GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position);
            text = text + " [" + (int)(dist) + "]";

            // Transform to screen position
            Vector3 screenPos;

            if (WorldToScreen(GameNetworkManager.Instance.localPlayerController.gameplayCamera, pos, out screenPos))
            {
                GUI.Label(new Rect(screenPos.x, screenPos.y, 100f, 100f), text, style);
            }
        }

        // Transform a world position to a screen position.
        // Returns true if successful, false otherwise.
        // FIXME: This breaks when spectating.. could remove camera argument and have this
        // get the current Camera?
        private static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screenPos)
        {
            // NOTE: Using Camera.WorldToScreenPoint doesn't give the correct result? Maybe need to
            // scale the result by Camera.pixelWidth / Camera.pixelHeight.
            // Easier to use WorldToViewportPoint.
            screenPos = camera.WorldToViewportPoint(world);

            // Need to scale by screen dimensions
            screenPos.x *= Screen.width;
            screenPos.y *= Screen.height;

            // Next need to mirror horizontally since the origin for the returned coordinates are the
            // bottom-left of the screen, while the UI functions use top-left
            screenPos.y = Screen.height - screenPos.y;

            return screenPos.z > 0f;
        }
    }
}
