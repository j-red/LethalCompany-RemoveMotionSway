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
    }

    [BepInPlugin(GUID, ModName, ModVersion)]
    public class ModBase : BaseUnityPlugin
    {
        private const string GUID = "jred.RemoveMotionSway";
        private const string ModName = "Remove Motion Sway";
        private const string ModVersion = "1.1.1";

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

                config.lockFOV = Config.Bind("General", "Lock FOV", true, "Determines if the player field of view should remain locked. Disable for mod compatibility.");
                // config.SomeValue = Config.Bind("General", "SomeValue", 42, "An example integer value");
                config.FOV = Config.Bind("General", "Field of View", 66f, "FOV to use when locked. Has no effect if LockFOV is false. Lethal Company default is 66 degrees.");
                config.FOV = Config.Bind("General", "Terminal Field of View", 66f, "FOV to use in terminal window. Default is 66 degrees.");
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(GUID);
            logger.LogInfo("Remove Motion Sway initialized.");

            // harmony.PatchAll();
            harmony.PatchAll(typeof (ModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}
