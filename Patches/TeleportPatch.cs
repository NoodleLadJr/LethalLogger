using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalLogger.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    internal class TeleportPatch
    {
        private static LethalLoggerBase pluginInstance;

        public static void Initialize(LethalLoggerBase plugin) { pluginInstance = plugin; }


        [HarmonyPatch("TeleportPlayerOutWithInverseTeleporter")]
        [HarmonyPrefix]
        static void PrintoutExitPatch(ShipTeleporter __instance, int playerObj, Vector3 teleportPos)
        {
            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerObj];

            pluginInstance.mls.LogInfo(playerControllerB.playerUsername + " teleporting to " + teleportPos.ToString());
            pluginInstance.currentRound.randomTeleports.Add((playerControllerB.playerUsername,teleportPos));
        }

    }
}
