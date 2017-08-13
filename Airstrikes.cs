using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Teyhota.Airstrikes
{
    public class Airstrikes : RocketPlugin<Config>
    {
        public static string PluginName = "Airstrikes";
        public static string PluginVersion = "1.0.0";
        public static string RocketVersion = "4.9.3.0";
        public static string UnturnedVersion = "3.20.3.0 +";

        public static Airstrikes Instance;

        public static void Write(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        public static void WriteDebug(string msg)
        {
            if (Instance.Configuration.Instance.Mode == "Debug")
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[DEBUG] " + msg);
                Console.ResetColor();
            }
        }

        public List<Vector3> Vectors;
        public StringBuilder ThisDirectory = 
            new StringBuilder().Append(System.IO.Directory.GetCurrentDirectory()).Append(Path.DirectorySeparatorChar).Append("Plugins").Append(Path.DirectorySeparatorChar).Append("Airstrike").Append(Path.DirectorySeparatorChar);
        
        protected override void Load()
        {
            Instance = this;
            Vectors = new List<Vector3>();

            Write("\n" + PluginName + " " + PluginVersion + " Beta", ConsoleColor.White);
            Write("Made by Teyhota", ConsoleColor.White);
            Write("for Rocket " + RocketVersion + "\n", ConsoleColor.White);

            new API().CheckForUpdates();
            
            if (Configuration.Instance.Mode == "Debug")
            {
                Write("> Debug Mode Enabled\n", ConsoleColor.DarkCyan);
            }
            
            if (Configuration.Instance.AutoAirstrike == true)
            {
                Instance.StartCoroutine(API.AutoStrike());

                WriteDebug("AutoAirstrike enabled");
            }
            else
            {
                WriteDebug("AutoAirstrike disabled");
            }
        }
        
        public void Update()
        {
            foreach (Vector3 vec in Vectors)
            {
                EffectManager.sendEffect(Instance.Configuration.Instance.GroundEffectID, EffectManager.INSANE, vec);
            }
        }
        
        protected override void Unload()
        {
            Write("Visit Plugins.4Unturned.tk for more!", ConsoleColor.Green);
        }
        
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"player_airstrike", "Airstrike set to position: {0}"},
                    {"global_airstrike",  "Airstrike is coming to {0} in {1} minutes!"},
                    {"global_airstrike_sec",  "Airstrike is coming to {0} in {1} seconds!"},
                    {"global_airstrike_now",  "Airstrike is now at {0}!"}
                };
            }
        }
    }

    public class API
    {
        #region CheckForUpdates
        public void CheckForUpdates()
        {
            string updateDir = Airstrikes.Instance.ThisDirectory + "Updates" + Path.DirectorySeparatorChar;

            try
            {
                string updateSite = new WebClient().DownloadString("http://plugins.4unturned.tk/plugins/Airstrikes/update");

                if (updateSite.Length > 7) return;

                if (updateSite == Airstrikes.PluginVersion) return;

                if (!Directory.Exists(updateDir))
                {
                    Directory.CreateDirectory(updateDir);
                }

                if (Airstrikes.Instance.Configuration.Instance.DisableAutoUpdates == "true")
                {
                    Airstrikes.Write("Version " + updateSite + " is now available on Rocket!\n", ConsoleColor.Green);
                }
                else
                {
                    if (File.Exists(updateDir + "Update-" + updateSite + ".zip")) return;

                    try
                    {
                        new WebClient().DownloadFileAsync(new Uri("http://plugins.4unturned.tk/releases/Airstrikes/" + updateSite + ".zip"), updateDir + "Update-" + updateSite + ".zip");

                        Airstrikes.Write("Version " + updateSite + " is now available in the \"Updates\" folder\n", ConsoleColor.Green);
                    }
                    catch
                    {
                        Logger.LogError("An error occured when trying to download updates\nMore info: goo.gl/DckR7x\n");
                    }
                }
            }
            catch
            {
                Logger.LogError("An error occured when trying to search for updates\nMore info: goo.gl/DckR7x\n");
            }
        }
        #endregion

        #region StringToVector3
        public Vector3 StringToVector3(string sVector)
        {
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            string[] sArray = sVector.Split(',');

            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }
        #endregion

        #region AutoStrike
        public static IEnumerator AutoStrike()
        {
            const float DAMAGE = 200;

            while (Airstrikes.Instance.Configuration.Instance.AutoAirstrike == true)
            {
                foreach (Config.Location Preset in Airstrikes.Instance.Configuration.Instance.Locations)
                {
                    Vector3 centerPoint = new API().StringToVector3(Preset.Coords);
                    string centerPointName = Preset.Name;
                    int minutes = Airstrikes.Instance.Configuration.Instance.MinutesBetweenAirstrikes - 1;
                    int amt = Airstrikes.Instance.Configuration.Instance.MinutesBetweenAirstrikes + 1;
                    
                    // x minutes left...
                    for (int i = 0; i < minutes; i++)
                    {
                        amt--;

                        if (Airstrikes.Instance.Configuration.Instance.GlobalMessageColor.StartsWith("#"))
                        {
                            Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor);

                            UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike", centerPointName, amt), hexColor ?? default(Color));
                        }
                        else
                        {
                            UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike", centerPointName, amt), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor, Color.green));
                        }

                        yield return new WaitForSeconds(60f);
                    }

                    // 1 minute left...
                    Airstrikes.Instance.Vectors.Add(centerPoint);
                    
                    if (Airstrikes.Instance.Configuration.Instance.GlobalMessageColor.StartsWith("#"))
                    {
                        Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor);

                        UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike", centerPointName, 1), hexColor ?? default(Color));
                    }
                    else
                    {
                        UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike", centerPointName, 1), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor, Color.green));
                    }

                    yield return new WaitForSeconds(60f);
                    
                    // times up...
                    if (Airstrikes.Instance.Configuration.Instance.GlobalMessageColor.StartsWith("#"))
                    {
                        Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor);

                        UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike_now", centerPointName), hexColor ?? default(Color));
                    }
                    else
                    {
                        UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike_now", centerPointName), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor, Color.green));
                    }

                    yield return new WaitForSeconds(3f);

                    Airstrikes.Instance.Vectors.Remove(centerPoint);

                    // strike...
                    for (int i = 0; i < (Preset.StrikeCount + 1); i++)
                    {
                        yield return new WaitForSeconds(Preset.StrikeSpeed);

                        Ray impactRay = new Ray(new Vector3(UnityEngine.Random.Range(centerPoint.x - Preset.Range, centerPoint.x + Preset.Range), centerPoint.y + 50, UnityEngine.Random.Range(centerPoint.z - Preset.Range, centerPoint.z + Preset.Range)), Vector3.down);

                        if (Physics.Raycast(impactRay, out RaycastHit hit))
                        {
                            EffectManager.sendEffect(20, EffectManager.INSANE, hit.point);
                            DamageTool.explode(hit.point, Preset.DamageIntensity, EDeathCause.MISSILE, CSteamID.Nil, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE);
                            Airstrikes.WriteDebug(hit.point.ToString());
                        }
                    }
                }
            }
        }
        #endregion

        #region Strike
        public static IEnumerator Strike(float initialDelay, float delayBetweenStrikes, int strikeCount, float damageIntensity, int range, Vector3 startPoint)
        {
            const float DAMAGE = 200;

            // timer
            Airstrikes.Instance.Vectors.Add(startPoint);

            if (Airstrikes.Instance.Configuration.Instance.GlobalMessageColor.StartsWith("#"))
            {
                Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor);

                UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike_sec", startPoint.ToString(), initialDelay), hexColor ?? default(Color));
            }
            else
            {
                UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike_sec", startPoint.ToString(), initialDelay), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor, Color.green));
            }

            yield return new WaitForSeconds(initialDelay);

            if (Airstrikes.Instance.Configuration.Instance.GlobalMessageColor.StartsWith("#"))
            {
                Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor);

                UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike_now", startPoint.ToString()), hexColor ?? default(Color));
            }
            else
            {
                UnturnedChat.Say(Airstrikes.Instance.Translate("global_airstrike_now", startPoint.ToString()), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.GlobalMessageColor, Color.green));
            }

            yield return new WaitForSeconds(3f);

            Airstrikes.Instance.Vectors.Remove(startPoint);
            
            // strike...
            for (int i = 0; i < (strikeCount + 1); i++)
            {
                yield return new WaitForSeconds(delayBetweenStrikes);

                Ray impactRay = new Ray(new Vector3(UnityEngine.Random.Range(startPoint.x - range, startPoint.x + range), startPoint.y + 50, UnityEngine.Random.Range(startPoint.z - range, startPoint.z + range)), Vector3.down);

                if (Physics.Raycast(impactRay, out RaycastHit hit))
                {
                    EffectManager.sendEffect(20, EffectManager.INSANE, hit.point);
                    DamageTool.explode(hit.point, damageIntensity, EDeathCause.MISSILE, CSteamID.Nil, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE, DAMAGE);
                    Airstrikes.WriteDebug(hit.point.ToString());
                }
            }
        }
        #endregion
    }
}