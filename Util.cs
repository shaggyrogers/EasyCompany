/*
   Util.cs
   =======

   Description:           Utility functions

*/

using System;
using System.Linq;
using UnityEngine;

namespace EasyCompany
{
    internal static class Util
    {
        // Find the nearest Component to the player of a given type matching a specified function
        // Every game object should be a Component
        public static T FindNearestComponentOfTypeWhere<T>(Func<T, bool> whereFn) where T : Component
        {
            T result = null;
            float minDist = float.MaxValue;
            UnityEngine.Vector3 playerPos = GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position;

            foreach (T obj in UnityEngine.Object.FindObjectsOfType<T>().Where(whereFn))
            {
                float dist = UnityEngine.Vector3.Distance(playerPos, obj.transform.position);

                // If closer, pick this object as new nearest object
                if (dist < minDist)
                {
                    minDist = dist;
                    result = obj;
                }
            }

            return result;
        }

        // Find nearest Component to the player of a given type
        public static T FindNearestComponentOfType<T>() where T : Component
        {
            return FindNearestComponentOfTypeWhere<T>(c => true);
        }

        // Raycast from the player's position/camera rotation, stopping if a collision is found within maxDist units.
        // If a collision was found, returns true and sets hit to the location of the collision.
        // FIXME: Seems slightly off - will always hit the floor when looking below the horizon?
        public static bool RaycastFromPlayer(out UnityEngine.Vector3 hit, float maxDist = Mathf.Infinity)
        {
            // Ensure we can get player camera transform
            if (GameNetworkManager.Instance?.localPlayerController?.gameplayCamera == null)
            {
                Main.log.LogWarning("RaycastFromPlayer unable to get gameplayCamera!");
                hit = new UnityEngine.Vector3();

                return false;
            }

            Transform camera = GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform;

            // Perform raycasting
            // HACK: Shift camera position up when raycasting to give better result
            var ray = new Ray(camera.position + Vector3.up * 0.5f, camera.forward);
            RaycastHit rayHit;

            // NOTE: LayerMask taken from Turret.CheckForPlayersInLineOfSight()
            if (Physics.Raycast(ray, out rayHit, maxDist, 1051400, QueryTriggerInteraction.Ignore))
            {
                // Collision detected
                hit = rayHit.point;

                return true;
            }

            // No collision
            hit = UnityEngine.Vector3.zero;

            return false;
        }
    }
}