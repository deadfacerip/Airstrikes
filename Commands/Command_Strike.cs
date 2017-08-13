using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.API;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Teyhota.Airstrikes.Commands
{
    public class Command_Strike : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "strike";

        public string Help => "Call in an airstrike from your crosshair.";

        public string Syntax => "<range> [delay]";

        public List<string> Aliases => new List<string>() { "airstrike" };

        public List<string> Permissions => new List<string>() { "airstrike.strike" };

        public const string PERMISSION = "airstrike.strike.delay";



        public void Execute(IRocketPlayer caller, string[] command)
        {
            Airstrikes.Write("This is not yet implemented", ConsoleColor.Red);
            return;

            if (caller is ConsolePlayer)
            {
                if (command.Length < 4)
                {
                    Airstrikes.Write("<x> <y> <z> <range> [delay]", ConsoleColor.Red);
                    return;
                }

                if (command.Length == 4)
                {
                    int x = Convert.ToInt32(command[0]);
                    int y = Convert.ToInt32(command[1]);
                    int z = Convert.ToInt32(command[2]);
                    int range = Convert.ToInt32(command[3]);

                    Vector3 startPoint = new Vector3(x, y, z);

                    // nest
                    Logger.Log(Airstrikes.Instance.Translate("airstrike", startPoint));

                    //Airstrikes.Instance.StartCoroutine(API.Strike(Airstrikes.Instance.Configuration.Instance.Delay, Airstrikes.Instance.Configuration.Instance.StrikeSpeed, Airstrikes.Instance.Configuration.Instance.StrikeCount, Airstrikes.Instance.Configuration.Instance.DamageIntensity, range, startPoint));
                }

                if (command.Length == 5)
                {
                    int x = Convert.ToInt32(command[0]);
                    int y = Convert.ToInt32(command[1]);
                    int z = Convert.ToInt32(command[2]);
                    int range = Convert.ToInt32(command[3]);
                    int delay = Convert.ToInt32(command[4]);

                    Vector3 startPoint = new Vector3(x, y, z);

                    // nest
                    Logger.Log(Airstrikes.Instance.Translate("player_airstrike", startPoint));

                    //Airstrikes.Instance.StartCoroutine(API.Strike(delay, Airstrikes.Instance.Configuration.Instance.StrikeSpeed, Airstrikes.Instance.Configuration.Instance.StrikeCount, Airstrikes.Instance.Configuration.Instance.DamageIntensity, range, startPoint));
                }
            }
            else
            {
                UnturnedPlayer callr = (UnturnedPlayer)caller;
                Ray startRay = new Ray(callr.Player.look.aim.position, callr.Player.look.aim.forward);

                if (command.Length == 0)
                {
                    UnturnedChat.Say(caller, Syntax, Color.red);
                    return;
                }

                if (command.Length == 1)
                {
                    int range = Convert.ToInt32(command[0]);

                    // nest
                    if (Physics.Raycast(startRay, out RaycastHit hit))
                    {
                        //if (Airstrikes.Instance.Configuration.Instance.PlayerMessageColor.StartsWith("#"))
                        //{
                        //    Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.PlayerMessageColor);

                        //    UnturnedChat.Say(caller, Airstrikes.Instance.Translate("player_airstrike", hit.point), hexColor ?? default(Color));
                        //}
                        //else
                        //{
                        //    UnturnedChat.Say(caller, Airstrikes.Instance.Translate("player_airstrike", hit.point), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.PlayerMessageColor, Color.green));
                        //}

                        Logger.Log(Airstrikes.Instance.Translate("player_airstrike", hit.point));

                        //Airstrikes.Instance.StartCoroutine(API.Strike(Airstrikes.Instance.Configuration.Instance.Delay, Airstrikes.Instance.Configuration.Instance.StrikeSpeed, Airstrikes.Instance.Configuration.Instance.StrikeCount, Airstrikes.Instance.Configuration.Instance.DamageIntensity, range, hit.point));
                    }
                }
                else if (command.Length == 2)
                {
                    if (!caller.HasPermission(PERMISSION))
                    {
                        UnturnedChat.Say(caller, "You don't have the permission to execute this command", Color.red);
                        return;
                    }

                    int range = Convert.ToInt32(command[0]);
                    float delay = Convert.ToSingle(command[1]);

                    // nest
                    if (Physics.Raycast(startRay, out RaycastHit hit))
                    {
                        //if (Airstrikes.Instance.Configuration.Instance.PlayerMessageColor.StartsWith("#"))
                        //{
                        //    Color? hexColor = UnturnedChat.GetColorFromHex(Airstrikes.Instance.Configuration.Instance.PlayerMessageColor);

                        //    UnturnedChat.Say(caller, Airstrikes.Instance.Translate("player_airstrike", hit.point), hexColor ?? default(Color));
                        //}
                        //else
                        //{
                        //    UnturnedChat.Say(caller, Airstrikes.Instance.Translate("player_airstrike", hit.point), UnturnedChat.GetColorFromName(Airstrikes.Instance.Configuration.Instance.PlayerMessageColor, Color.green));
                        //}

                        Logger.Log(Airstrikes.Instance.Translate("player_airstrike", hit.point));

                        //if (delay < Airstrikes.Instance.Configuration.Instance.Delay)
                        //{
                        //    delay = Airstrikes.Instance.Configuration.Instance.Delay;
                        //}

                        if (delay > 300f)
                        {
                            delay = 300f;
                        }

                        //Airstrikes.Instance.StartCoroutine(API.Strike(delay, Airstrikes.Instance.Configuration.Instance.StrikeSpeed, Airstrikes.Instance.Configuration.Instance.StrikeCount, Airstrikes.Instance.Configuration.Instance.DamageIntensity, range, hit.point));
                    }
                }
            }
        }
    }
}