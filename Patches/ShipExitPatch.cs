﻿using BepInEx.Logging;
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
    [HarmonyPatch(typeof(StartOfRound))]
    internal class ShipExitPatch
    {
        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static void PrintoutExitPatch( StartOfRound __instance) {
            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("LethalLogger");
            logger.LogInfo("Adniel Hamed");
            //set to true to get collected, missed will be the diff
            int dead_players = 0;
            int living_players = 0;
            IDictionary<String,int> gear = new Dictionary<String,int>(); 

            foreach (PlayerControllerB playercontrollerb in GameObject.FindObjectsOfType<PlayerControllerB>())
            {
                if (playercontrollerb != null && (playercontrollerb.isPlayerControlled || playercontrollerb.isPlayerDead))
                {
                    if (playercontrollerb.isPlayerDead) { dead_players++; logger.LogInfo(playercontrollerb.causeOfDeath); } else { living_players++; }
                    logger.LogInfo("Studying: " + playercontrollerb.playerUsername);
                }

            }

            foreach (GrabbableObject grabbable in UnityEngine.Object.FindObjectsOfType<GrabbableObject>())
            {
                if (grabbable != null && grabbable.scrapValue == 0)
                {
                    logger.LogInfo(grabbable.GetType().Name);
                    if (gear.ContainsKey(grabbable.GetType().Name)) { gear[grabbable.GetType().Name] += 1; }
                    else
                    {
                        gear[grabbable.GetType().Name] = 1;
                    }
                }
            }

            logger.LogInfo("Living: " + living_players + " dead: " + dead_players);
            logger.LogInfo("Current Scrap Value is " + __instance.GetValueOfAllScrap(true) + " out of a max " + __instance.GetValueOfAllScrap(false));
            logger.LogInfo("Planet is: " + __instance.currentLevel.PlanetName);
            logger.LogInfo("Planet Weather was: " + __instance.currentLevel.currentWeather);
            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            logger.LogInfo("Quota is " +  timeOfDay.profitQuota);
            //count dead players done
            //get each player controller and get the cause of death done
            //get player names done
            //get planet name done
            //get planet weather done
            //get planet done
            //get gear
            //get current quota
            //write file

        }
    }
}
