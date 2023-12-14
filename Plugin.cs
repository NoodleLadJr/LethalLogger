using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLogger.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalLogger
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalLoggerBase : BaseUnityPlugin
    {
        private const string modGUID = "Garageman.LethalLogger";
        private const string modName = "LC Tutorial";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static LethalLoggerBase Instance;
        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null) { Instance = this; }
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("LethalLogger is awake");
            harmony.PatchAll(typeof(LethalLoggerBase));
            harmony.PatchAll(typeof(ShipExitPatch));
        }
        

    }
}
