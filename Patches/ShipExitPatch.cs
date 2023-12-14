using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalLogger.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class ShipExitPatch
    {

        public class RoundInfo
        {
            public int living;
            public int dead;
            public int quota;
            public int scrapMax;
            public int scrapReal;
            public int daysRemaining;
            public String planetName;
            public String weather;
            public IDictionary<String, int> gear;
            public IDictionary<String, String> playerStatus;   

        }

        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static void PrintoutExitPatch( StartOfRound __instance) {

            RoundInfo roundInfo = new RoundInfo();

            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("LethalLogger");
            logger.LogInfo("Adniel Hamed");
            //set to true to get collected, missed will be the diff


            foreach (PlayerControllerB playercontrollerb in GameObject.FindObjectsOfType<PlayerControllerB>())
            {
                if (playercontrollerb != null && (playercontrollerb.isPlayerControlled || playercontrollerb.isPlayerDead))
                {
                    if (playercontrollerb.isPlayerDead) {
                        roundInfo.dead++;
                        roundInfo.playerStatus[playercontrollerb.playerUsername] = playercontrollerb.causeOfDeath.ToString();
                        logger.LogInfo(playercontrollerb.causeOfDeath); 
                    } else { 
                        roundInfo.living++;
                        roundInfo.playerStatus[playercontrollerb.playerUsername] = "Alive";
                    }
                }

            }

            foreach (GrabbableObject grabbable in UnityEngine.Object.FindObjectsOfType<GrabbableObject>())
            {
                if (grabbable != null && grabbable.scrapValue == 0)
                {
                    logger.LogInfo(grabbable.GetType().Name);
                    if (roundInfo.gear.ContainsKey(grabbable.GetType().Name)) { roundInfo.gear[grabbable.GetType().Name] += 1; }
                    else
                    {
                        roundInfo.gear[grabbable.GetType().Name] = 1;
                    }
                }
            }
            roundInfo.scrapMax = __instance.GetValueOfAllScrap(false);
            roundInfo.scrapReal = __instance.GetValueOfAllScrap(true);
            roundInfo.planetName = __instance.currentLevel.PlanetName;
            roundInfo.weather = __instance.currentLevel.PlanetName;
            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            roundInfo.quota = timeOfDay.profitQuota;
            roundInfo.daysRemaining = timeOfDay.daysUntilDeadline;
            //count dead players done
            //get each player controller and get the cause of death done
            //get player names done
            //get planet name done
            //get planet weather done
            //get planet done
            //get gear
            //get current quota
            //get unlockables
            //write file

        }
    }
}
