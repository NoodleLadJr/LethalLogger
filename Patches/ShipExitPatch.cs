using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalLogger.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class ShipExitPatch
    {
        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static void PrintoutExitPatch( StartOfRound __instance) {
            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("LethalLogger");
            logger.LogInfo("Adniel Hamed");
            //set to true to get collected, missed will be the diff
            logger.LogInfo("Current Scrap Value is" + __instance.GetValueOfAllScrap(true) + " out of a max " + __instance.GetValueOfAllScrap());
            
        }
    }
}
