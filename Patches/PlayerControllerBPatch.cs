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


        // public static GameObject cameraContainer;
        // public static Transform HUDHelmetPosition;
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

            for (int i = 0; i < MAX_NUM_PLAYERS; i ++)
            {
                /*
                GameObject _player;
                if (i == 0)
                {
                    _player = GameObject.Find("Player");
                } else
                {
                    _player = GameObject.Find("Player (" + i.ToString() + ")");
                }
                */

                Transform _player = __instance.transform;
                Debug.Log("GAME OBJECT OF INSTANCE TRANSFORM: " + __instance.transform);
                // Debug.Log("GAME OBJECT PARENT OF INSTANCE TRANSFORM: " + __instance.transform.parent);

                // If beyond index of number of players, break.
                Debug.Log("Player " + i.ToString() + ": " + _player.ToString());
                // if (!_player) break;

                // Debug.Log("Scavenger: " + _player.Find("ScavengerModel"));
                Debug.Log("ScavengerModel/metarig/CameraContainer/MainCamera: " + _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera"));
                Debug.Log("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition: " + _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition"));

                // Otherwise, add HUDHelmetPosition element to list of items to update position on.
                Transform _helmetPos = _player.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HUDHelmetPosition");
                
                if (_helmetPos)
                    HUDHelmetPositions.Add(_helmetPos); 
            }

            Debug.Log("Player HUDs: " + HUDHelmetPositions.ToString());
        }
        

        [HarmonyPatch("LateUpdate")] // for private methods
        [HarmonyPostfix]
        static void LateUpdatePatch(ref float ___drunkness, ref PlayerControllerB __instance)
        {
            // ___drunkness = 0f;

            // __instance.playerEye.position = __instance.playerEye.position + new Vector3(1f, -1f, 1f);
            // __instance.gameplayCamera.transform.position = __instance.gameplayCamera.transform.position + new Vector3(1f, 0, 0);

            // /HangarShip/Player/ScavengerModel/metarig/CameraContainer/MainCamera/

            // Environment/HangarShip/Player/ScavengerModel/metarig/CameraContainer object (last in list of MainCamera objects) controls actual player visor movement
            // Environment/HangarShip/Player/ScavengerModel/metarig/ does NOT move when walking

            // Idea: Set CameraContainer position equal to player position plus offset?
            // CameraContainer Y Standing in ship: 2.6372
            // CameraContainer Y Crouched in ship: 1.4179
            // Player Parent Y Standing in Ship: 0.2862
            // Player Parent Y crouched in ship: 0.368
            // Update cached offset whenever crouch state changes?

            // __instance.cameraContainerTransform.position = __instance.playerModelArmsMetarig.transform.position;// + new Vector3(0, 0f, 0);

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
