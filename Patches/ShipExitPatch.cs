using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Xml.Linq;

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
            public String seed;
            public IDictionary<string, int> gear = new Dictionary<string, int>();
            public IDictionary<string, string> playerStatus = new Dictionary<string, string>();

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
            roundInfo.weather = __instance.currentLevel.currentWeather.ToString();
            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            roundInfo.quota = timeOfDay.profitQuota;
            roundInfo.daysRemaining = timeOfDay.daysUntilDeadline;
            roundInfo.seed = __instance.randomMapSeed.ToString();

            const String FILEOUT = "LethalLoggerOut.json";
            try
            {
                if (File.Exists(FILEOUT))
                {
                    string existing = File.ReadAllText(FILEOUT);
                    List<RoundInfo> existingData = JsonConvert.DeserializeObject<List<RoundInfo>>(existing) ?? new List<RoundInfo>();
                    existingData.Add(roundInfo);
                    string jstring = JsonConvert.SerializeObject(existingData);
                    File.WriteAllText(FILEOUT, jstring);
                }
                else 
                {
                    string jstring = JsonConvert.SerializeObject(new List<RoundInfo> { roundInfo});
                    File.WriteAllText(FILEOUT, jstring);
                }
                logger.LogInfo("logged successfully");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                logger.LogWarning(ex);
            }

            //count dead players done
            //get each player controller and get the cause of death done
            //get player names done
            //get planet name done
            //get planet weather done
            //get planet done
            //get gear
            //get current quota
            //TODO: get unlockables
            //get seed
            //write file

        }
    }
}
