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
        public static float fov = -1f;
        public static Camera playerCam = null;

        [HarmonyPatch("Awake")] // Patch method by name
        [HarmonyPostfix]
        static void CacheCameraContainer(ref PlayerControllerB __instance)
        {
            // Get the Transform associated with the GameObject the PlayerControllerB script is attached to/a component of.
            Transform _player = __instance.transform;

            // Add the player instance's HUDHelmetPosition element to list of items to update position on. 
            Transform _helmetPos = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition");

            if (_helmetPos) HUDHelmetPositions.Add(_helmetPos);

            // Cache reference to player camera and initial FOV
            playerCam = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera").GetComponent<Camera>();
            fov = playerCam.fieldOfView;
        }


        private static Vector3 cameraRotation = new Vector3(90f, 0f, 0f);
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        static void LateUpdatePatch(ref PlayerControllerB __instance)
        {
            if (!__instance.inTerminalMenu)
            {
                __instance.cameraContainerTransform.position = new Vector3(
                    __instance.cameraContainerTransform.position.x,
                    __instance.playerModelArmsMetarig.transform.position.y, // copy height from metarig
                    __instance.cameraContainerTransform.position.z
                );

                // Apply fixed rotation offset to camera container relative to parent metarig rotation.
                __instance.cameraContainerTransform.localRotation = Quaternion.Euler(cameraRotation);

                // Remove helmet HUD effect
                foreach (Transform t in HUDHelmetPositions)
                {
                    t.position = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity); // Banish the game object
                }

                // Lock camera FOV to initial value
                if (playerCam) playerCam.fieldOfView = fov;
            }
        }
    }
}
