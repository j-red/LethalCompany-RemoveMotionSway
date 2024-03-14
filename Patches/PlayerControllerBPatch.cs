using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace j_red.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        // public static float fov = 66f; // default is 66f, stored in configuration manager instead
        public static Camera playerCam = null;

        [HarmonyPatch("Awake")] // Patch method by name
        [HarmonyPostfix]
        static void CacheCameraContainer(ref PlayerControllerB __instance)
        {
            // Get the Transform associated with the GameObject the PlayerControllerB script is attached to/a component of.
            Transform _player = __instance.transform;

            // Cache reference to player camera and initial FOV
            playerCam = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera").GetComponent<Camera>();
            // fov = playerCam.fieldOfView;

            if (ModBase.config.disableHUDHelmetVisor.Value)
            {
                GameObject helmet = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/ScavengerHelmet");
                MeshRenderer helmetRenderer = helmet.GetComponent<MeshRenderer>();
                helmetRenderer.enabled = false;
            }

            // Debug.Log("Called AWAKE and updated cache");
        }


        private static Vector3 cameraRotation = new Vector3(90f, 0f, 0f);
        [HarmonyPatch("LateUpdate")]
        [HarmonyPrefix]
        static void LateUpdatePatch(ref PlayerControllerB __instance)
        {
            // Camera position must be updated in prefix patch instead of postfix in order for HUD helmet
            // visor position to be in sync, this is especially useful if helmet visor is re-enabled.

            if (!__instance.inTerminalMenu)
            {
                // Don't touch camera when in special animation, such as when being spawned/revived, using terminal,
                // using ship lever, charging item, climbing ladder, fall damage, being attacked, etc...
                if (!__instance.inSpecialInteractAnimation && !__instance.playingQuickSpecialAnimation)
                {
                    if (ModBase.config.disableMotionSway.Value)
                    {
                        // Motion sway is disabled
                        __instance.cameraContainerTransform.position = new Vector3(
                            __instance.cameraContainerTransform.position.x,
                            __instance.playerModelArmsMetarig.transform.position.y, // copy height from metarig
                            __instance.cameraContainerTransform.position.z
                        );

                        // Apply fixed rotation offset to camera container relative to parent metarig rotation.
                        __instance.cameraContainerTransform.localRotation = Quaternion.Euler(cameraRotation);
                    }
                    else
                    {
                        // Motion sway is still enabled -- scale it by the config multiplier
                        float deltaHeight = __instance.cameraContainerTransform.position.y - __instance.playerModelArmsMetarig.transform.position.y;

                        __instance.cameraContainerTransform.position = new Vector3(
                            __instance.cameraContainerTransform.position.x,
                            __instance.playerModelArmsMetarig.transform.position.y + (deltaHeight * ModBase.config.motionSwayIntensity.Value),
                            __instance.cameraContainerTransform.position.z
                        );

                        // Debug.Log("Intensity: " + ModBase.config.motionSwayIntensity.Value.ToString() + "\tOffset: " + (deltaHeight * ModBase.config.motionSwayIntensity.Value).ToString());
                    }
                }

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
            } else
            {
                // Debug.Log("Instance is in terminal menu");
                // If player is in terminal window, sync terminal FOV
                try
                {
                    if (ModBase.config.lockFOV.Value && playerCam)
                    {
                        playerCam.fieldOfView = ModBase.config.terminalFOV.Value;
                        // Debug.Log("Set Terminal FOV to " + playerCam.fieldOfView.ToString());
                    }
                } catch { }
            }

            // Run following code at the end of every frame

        } // end LateUpdate loop
    }
}
