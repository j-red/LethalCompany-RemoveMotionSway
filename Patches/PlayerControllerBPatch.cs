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
        // public static float fov = 66f; // default is 66f, stored in configuration manager instead
        public static Camera playerCam = null;

        [HarmonyPatch("Awake")] // Patch method by name
        [HarmonyPostfix]
        static void CacheCameraContainer(ref PlayerControllerB __instance)
        {
            // Remove previously cached entries
            // HUDHelmetPositions.Clear(); // this fixes the multiple login issue

            // Get the Transform associated with the GameObject the PlayerControllerB script is attached to/a component of.
            Transform _player = __instance.transform;

            // Add the player instance's HUDHelmetPosition element to list of items to update position on. 
            Transform _helmetPos = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition");

            if (_helmetPos) HUDHelmetPositions.Add(_helmetPos);

            // Cache reference to player camera and initial FOV
            playerCam = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera").GetComponent<Camera>();
            // fov = playerCam.fieldOfView;

            // Debug.Log("Called AWAKE and updated cache");
        }


        private static Vector3 cameraRotation = new Vector3(90f, 0f, 0f);
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        static void LateUpdatePatch(ref PlayerControllerB __instance)
        {
            HUDHelmetPositions.Clear(); // this fixes the multiple login issue
            CacheCameraContainer(ref __instance);

            if (!__instance.inTerminalMenu)
            {

                if (ModBase.config.disableMotionSway.Value)
                {
                    // Motion sway is disabled
                    __instance.cameraContainerTransform.position = new Vector3(
                        __instance.cameraContainerTransform.position.x,
                        __instance.playerModelArmsMetarig.transform.position.y, // copy height from metarig
                        __instance.cameraContainerTransform.position.z
                    );
                } else
                {
                    // Motion sway is still enabled -- scale it by the config multiplier
                    float deltaHeight = __instance.cameraContainerTransform.position.y - __instance.playerModelArmsMetarig.transform.position.y;

                    // Original: (A)  __instance.cameraContainerTransform.position.y
                    // New: (B)       __instance.playerModelArmsMetarig.transform.position.y
                    // Offset: (B - A)
                    // Influence: I in range [0, 10.0]
                    // Target: A = B + (I * (B - A)) = BI * BI - AI, or A = B + (I * (A - B)) = B + AI - BI => if I = 0, then pos = B

                    // If Intensity = 0, Output = B
                    // Intensity = 1, output = A
                    // Intensity > 1, output = (A - B) * Intensity + B

                    __instance.cameraContainerTransform.position = new Vector3(
                        __instance.cameraContainerTransform.position.x,
                        __instance.playerModelArmsMetarig.transform.position.y + (deltaHeight * ModBase.config.motionSwayIntensity.Value),
                        __instance.cameraContainerTransform.position.z
                    );

                    // Debug.Log("Intensity: " + ModBase.config.motionSwayIntensity.Value.ToString() + "\tOffset: " + (deltaHeight * ModBase.config.motionSwayIntensity.Value).ToString());
                }

                /* __instance.cameraContainerTransform.position = new Vector3(
                    __instance.cameraContainerTransform.position.x,
                    __instance.playerModelArmsMetarig.transform.position.y, // copy height from metarig
                    __instance.cameraContainerTransform.position.z
                ); */

                // Apply fixed rotation offset to camera container relative to parent metarig rotation.
                __instance.cameraContainerTransform.localRotation = Quaternion.Euler(cameraRotation);

                // Lock camera FOV to initial value
                if (ModBase.config.lockFOV.Value)
                {
                    // Debug.Log("FOV is locked");
                    if (playerCam)
                    {
                        // playerCam.fieldOfView = fov;
                        playerCam.fieldOfView = ModBase.config.FOV.Value;
                        // Debug.Log("Set Camera FOV to " + playerCam.fieldOfView.ToString());
                    }

                } else {
                    // ...
                }

                // Remove helmet HUD effect
                foreach (Transform t in HUDHelmetPositions)
                {
                    // t.position = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity); // Banish the game object. Maybe place it behind the player instead?
                    try
                    {
                        t.position = __instance.transform.position - (__instance.transform.forward * 10f); // place behind player?
                        // Debug.Log(t.position);
                    }
                    catch
                    {
                        Debug.LogWarning("Failed to update position for helmet");
                    }
                }

            } else
            {
                // If player is in terminal window, sync terminal FOV
                if (ModBase.config.lockFOV.Value && playerCam)
                    playerCam.fieldOfView = ModBase.config.terminalFOV.Value;
            }

            // Run following code at the end of every frame

        } // end LateUpdate loop
    }
}
