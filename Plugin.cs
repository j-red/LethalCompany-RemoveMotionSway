using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using j_red.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace j_red
{
    [BepInPlugin(GUID, ModName, ModVersion)]
    public class ModBase : BaseUnityPlugin
    {
        private const string GUID = "jred.RemoveMotionSway";
        private const string ModName = "Remove Motion Sway";
        private const string ModVersion = "1.1.1";

        private readonly Harmony harmony = new Harmony(GUID);

        private static ModBase Instance;

        internal ManualLogSource logger;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(GUID);
            logger.LogInfo("Remove Motion Sway initialized.");

            // harmony.PatchAll();
            harmony.PatchAll(typeof (ModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}
