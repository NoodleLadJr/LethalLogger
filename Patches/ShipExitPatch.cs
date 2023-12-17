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
        private static LethalLoggerBase pluginInstance;

        public static void Initialize(LethalLoggerBase plugin) { pluginInstance = plugin; }


        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static void PrintoutExitPatch( StartOfRound __instance) {


            foreach (PlayerControllerB playercontrollerb in GameObject.FindObjectsOfType<PlayerControllerB>())
            {
                if (playercontrollerb != null && (playercontrollerb.isPlayerControlled || playercontrollerb.isPlayerDead))
                {
                    if (playercontrollerb.isPlayerDead) {
                        pluginInstance.currentRound.dead++;
                        pluginInstance.currentRound.playerStatus[playercontrollerb.playerUsername] = playercontrollerb.causeOfDeath.ToString();

                    } else { 
                        pluginInstance.currentRound.living++;
                        pluginInstance.currentRound.playerStatus[playercontrollerb.playerUsername] = "Alive";
                    }
                }

            }

            foreach (GrabbableObject grabbable in UnityEngine.Object.FindObjectsOfType<GrabbableObject>())
            {
                if (grabbable != null && grabbable.scrapValue == 0)
                {
                    if (pluginInstance.currentRound.gear.ContainsKey(grabbable.GetType().Name)) { pluginInstance.currentRound.gear[grabbable.GetType().Name] += 1; }
                    else
                    {
                        pluginInstance.currentRound.gear[grabbable.GetType().Name] = 1;
                    }
                }
            }

            foreach (UnlockableItem unlockable in __instance.unlockablesList.unlockables)
            {
                if(unlockable.hasBeenUnlockedByPlayer)
                {
                   pluginInstance.mls.LogInfo(unlockable.unlockableName);
                    pluginInstance.currentRound.unlockables.Add(unlockable.unlockableName);
                }
            }
            pluginInstance.currentRound.scrapMax = __instance.GetValueOfAllScrap(false);
            pluginInstance.currentRound.scrapReal = __instance.GetValueOfAllScrap(true);
            pluginInstance.currentRound.planetName = __instance.currentLevel.PlanetName;
            pluginInstance.currentRound.weather = __instance.currentLevel.currentWeather.ToString();
            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            pluginInstance.currentRound.quota = timeOfDay.profitQuota;
            pluginInstance.currentRound.daysRemaining = timeOfDay.daysUntilDeadline;
            pluginInstance.currentRound.seed = __instance.randomMapSeed.ToString();

            System.IO.Directory.CreateDirectory("LethalLoggerOutput");
            String date = DateTime.Today.ToString("dd-mm-yyyy");
            String FILEOUT = String.Format("LethalLoggerOutput/LethalLoggerOut{0}.json",date);

            try
            {
                if (File.Exists(FILEOUT))
                {
                    string existing = File.ReadAllText(FILEOUT);
                    List<RoundInfo> existingData = JsonConvert.DeserializeObject<List<RoundInfo>>(existing) ?? new List<RoundInfo>();
                    existingData.Add(pluginInstance.currentRound);
                    string jstring = JsonConvert.SerializeObject(existingData,Formatting.Indented);
                    File.WriteAllText(FILEOUT, jstring);
                }
                else 
                {
                    string jstring = JsonConvert.SerializeObject(new List<RoundInfo> { pluginInstance.currentRound});
                    File.WriteAllText(FILEOUT, jstring);
                }
               pluginInstance.mls.LogInfo("logged successfully");
            }
            catch (Exception ex)
            {
                pluginInstance.mls.LogWarning(ex);
            }


            //clear round info
            pluginInstance.currentRound = new RoundInfo();

        }
    }
}
