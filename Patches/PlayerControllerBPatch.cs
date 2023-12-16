using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;

namespace j_red.Patches
{

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        /*
        // [HarmonyPatch(nameof(PlayerControllerB.Update))] // only works for Public methods
        [HarmonyPatch("Update")] // for private methods
        [HarmonyPostfix]
        static void UpdatePatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1f;
        }
        */

        public static List<Transform> HUDHelmetPositions = new List<Transform>();
        public const int MAX_NUM_PLAYERS = 4;

        [HarmonyPatch("Awake")] // for private methods
        [HarmonyPostfix]
        static void CacheCameraContainer(ref PlayerControllerB __instance)
        {

            // Debug.Log("I am player: " + __instance.ToString());

            // cameraContainer = GameObject.Find("/Environment/HangarShip/Player/ScavengerModel/metarig/CameraContainer");

            /*
            cameraContainer = GameObject.Find("CameraContainer");
            
            if (!cameraContainer)
            {
                cameraContainer = GameObject.Find("CameraContainer");
                
                Debug.LogWarning("Couldn't find camera container from XPath, replaced with:" + cameraContainer.ToString());
            }
            */

            // HUDHelmetPosition = cameraContainer.transform.Find("MainCamera/HUDHelmetPosition");

            // Get the Transform associated with the GameObject the PlayerControllerB script is attached to/a component of.
            Transform _player = __instance.transform;

            // Add the player instance's HUDHelmetPosition element to list of items to update position on.
            Transform _helmetPos = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition");

            // Debug.Log("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition: " + _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition"));

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

                
                // Debug.Log(cameraContainer.transform.Find("HUDHelmetPosition"));
                // cameraContainer.transform.Find("HUDHelmetPosition").transform.position = __instance.cameraContainerTransform.position;

                // if (Input.GetKey(KeyCode.LeftShift)) {...}

                /* 
                HUDHelmetPosition.position = new Vector3(
                    __instance.cameraContainerTransform.position.x, 
                    HUDHelmetPosition.position.y, 
                    __instance.cameraContainerTransform.position.z
                );
                */

                // For single-player
                // HUDHelmetPosition.position = Vector3.zero; // take it awayyyy

                // For multiple players
                foreach (Transform t in HUDHelmetPositions)
                {
                    t.position = Vector3.zero; // take it awayyyy
                    // Debug.Log(t.position);
                }

                // Debug.Log("HUD Pos:" + HUDHelmetPosition.position.ToString() + ", Camera Container pos: " + cameraContainer.transform.position.ToString() + "\n\tMeta Rig Position: " + __instance.playerModelArmsMetarig.transform.position.ToString());
            }

            // Need to remove camera overlay or fix the height of the bobbing element;
        }
    }

    /*
    [HarmonyPatch(typeof(PlayerAnimationEvents))]
    internal class PlayerAnimationEventsPatch
    {
        // Prevent lock arms to camera (theoretically -- actually does nothing?)
        // [HarmonyPatch(nameof(PlayerAnimationEvents.LockArmsToCamera))]

        // This patch prevents local footstep noise; does NOT prevent serverside footstep sounds.
        [HarmonyPatch(nameof(PlayerAnimationEvents.PlayFootstepLocal))]
        [HarmonyPrefix]
        static void PlayerAnimationEventPatch(ref bool __runOriginal)
        {
            __runOriginal = false;
        }
    }
    */

    /*
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        // Prevent lock arms to camera (theoretically -- actually does nothing?)
        // [HarmonyPatch(nameof(PlayerAnimationEvents.LockArmsToCamera))]

        // This patch prevents local footstep noise; does NOT prevent serverside footstep sounds.
        
        [HarmonyPatch(nameof(HUDManager.ShakeCamera))]
        [HarmonyPrefix]
        static void RemoveScreenShake(ref bool __runOriginal)
        {
            __runOriginal = false;
        }
    }
    */
}
