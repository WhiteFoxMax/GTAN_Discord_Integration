using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GTANetworkServer;
using GTANetworkShared;
using System.IO;
using Discord;

namespace discord_integration
{
    public class Discord_Int : Script
    {
        private DiscordClient disClient;
        private bool isDiscordRdy = false;
        private bool isDiscordToStart = false;

        public Discord_Int()
        {
            API.onResourceStart += startUpMsg;
            API.onResourceStop += discoenctDiscord;
            API.onResourceStart += connectDisco;

            disClient = new DiscordClient(x =>
            {
                x.AppName = "MaxTest"; //If name dosen't fit bot name it may throw warning and error infomratio messages but will still work.
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });
            disClient.ServerAvailable += setDiscordReady;
            disClient.ServerUnavailable += setDiscordOff;
        }

        #region Discord
        private void connectDisco()
        {
            isDiscordToStart = true;
            var token = "Mjc2ODc5NDQ5NzIxNDA1NDQx.C3aHWQ.AsGYuyfz5kRlu49kNvKmGxjubsU"; //Get your token from https://discordapp.com/developers/applications/me
            disClient.ExecuteAndWait(async () =>
            {
                await disClient.Connect(token, TokenType.Bot); 
            });
        }

        private void setDiscordOff(object sender, ServerEventArgs e)
        {
            isDiscordRdy = false;
        }

        private void setDiscordReady(object sender, ServerEventArgs e)
        {
            isDiscordRdy = true;
        }

        private void discoenctDiscord()
        {
            API.consoleOutput("Dissconeting from Discord...");
            isDiscordToStart = false;
            disClient.Disconnect();
            disClient.Dispose();
        }

        private void sendDiscordMessage(string Text)
        {
            disClient.ExecuteAndWait(async () =>
            {
                var _channel = disClient.GetChannel((ulong)276840705526202369); //If you are the Discord server admin, just right click on the desired channel and select "copy id"
                await _channel.SendMessage(Text);
            });
        }
        #endregion

        private void Log(object sender, LogMessageEventArgs e)
        {
            API.consoleOutput("[" + e.Source + "]" + " [" + e.Severity + "]" + " [" + e.Message + "]");
        }

        private void startUpMsg()
        {
            API.consoleOutput("Starting discord integration.");
        }

        [Command("discordexample")]
        public void DeleteCurrentCar(Client player)
        {
            var carToDel = API.getPlayerVehicle(player);
            API.deleteEntity(carToDel);
            if (isDiscordRdy)
                sendDiscordMessage("Hello, im sent from GTA:N server!");
        }

        [Command("discord", GreedyArg = true)]
        public void showoff(Client player, string Text)
        {
            if (isDiscordRdy)
                sendDiscordMessage(Text);
        }
    }
}