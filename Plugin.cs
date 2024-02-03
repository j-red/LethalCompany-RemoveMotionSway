using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using j_red.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using UnityEngine;

namespace j_red
{
    public class ModConfig
    {
        public ConfigEntry<bool> lockFOV;
        [Range(0f, 132f)]
        public ConfigEntry<float> FOV;
        public ConfigEntry<float> terminalFOV;

        public ConfigEntry<bool> disableMotionSway;
        [Range(0f, 10f)]
        public ConfigEntry<float> motionSwayIntensity;

    }

    [BepInPlugin(GUID, ModName, ModVersion)]
    public class ModBase : BaseUnityPlugin
    {
        private const string GUID = "jred.RemoveMotionSway";
        private const string ModName = "Remove Motion Sway";
        private const string ModVersion = "1.2.3";

        private readonly Harmony harmony = new Harmony(GUID);

        private static ModBase Instance;
        public static ModConfig config;

        internal ManualLogSource logger;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                config = new ModConfig();

                // Motion Sway intensity controls
                config.disableMotionSway = Config.Bind("General", "Disable Motion Sway", true, "If motion sway/head bobbing should be disabled.");
                config.motionSwayIntensity = Config.Bind("General", "Motion Sway Intensity", 1f, "If enabled, how strong the motion sway effect will be. Range [0.0, 10.0]. Recommended max 3.0.");

                // FOV Controls
                config.lockFOV = Config.Bind("General", "Lock FOV", true, "Determines if the player field of view should remain locked. Disable for mod compatibility.");
                config.FOV = Config.Bind("General", "Field of View", 66f, "FOV to use when locked. Has no effect if LockFOV is false.");
                config.terminalFOV = Config.Bind("General", "Terminal Field of View", 66f, "FOV to use in terminal window.");
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(GUID);
            logger.LogInfo("Remove Motion Sway v" + ModVersion + " initialized.");

            // harmony.PatchAll();
            harmony.PatchAll(typeof (ModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}
