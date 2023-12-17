using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace j_red.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        public static List<Transform> HUDHelmetPositions = new List<Transform>();
        public const int MAX_NUM_PLAYERS = 4;

        [HarmonyPatch("Awake")] // for private methods
        [HarmonyPostfix]
        static void CacheCameraContainer(ref PlayerControllerB __instance)
        {
            // Get the Transform associated with the GameObject the PlayerControllerB script is attached to/a component of.
            Transform _player = __instance.transform;

            // Add the player instance's HUDHelmetPosition element to list of items to update position on.
            Transform _helmetPos = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition");

            if (_helmetPos) HUDHelmetPositions.Add(_helmetPos);
        }
        

        [HarmonyPatch("LateUpdate")] // for private methods
        [HarmonyPostfix]
        static void LateUpdatePatch(ref float ___drunkness, ref PlayerControllerB __instance)
        {
            if (!__instance.inTerminalMenu)
            {
                __instance.cameraContainerTransform.position = new Vector3(
                    __instance.cameraContainerTransform.position.x,
                    __instance.playerModelArmsMetarig.transform.position.y, // copy fixed height from metarig
                    __instance.cameraContainerTransform.position.z
                );

                // For multiple players
                foreach (Transform t in HUDHelmetPositions)
                {
                    t.position = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity); // Banish the game object
                }
            }
        }
    }
}
