using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace DxBotFunc
{
    public class SetUpBot
    {
        private readonly TelegramBotClient botClient;

        private const string SetUpFunctionName = "setup";
        private const string UpdateFunctionName = "handleupdate";

        public SetUpBot()
        {
            botClient = new TelegramBotClient("5575860587:AAFpOrlK7jFFegMgT4xz7nOJbb9h54MPGzI");
        }


        [FunctionName(SetUpFunctionName)]
        public async Task Setup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var handleUpdateFunctionUrl = req.GetEncodedUrl().ToString().Replace(SetUpFunctionName, UpdateFunctionName,
                ignoreCase: true, culture: CultureInfo.InvariantCulture);
            await botClient.SetWebhookAsync(handleUpdateFunctionUrl);

        }

        [FunctionName(UpdateFunctionName)]
        public async Task Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {

            var request = await req.ReadAsStringAsync();
            var update = JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(request);

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message?.Text != null)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/help")
                {


                    await botClient.SendTextMessageAsync(message.Chat, "Welcome to the DX spot bot search");
                    await botClient.SendTextMessageAsync(message.Chat, "To search by the call enter the full callsign. E.g.: UR5ECW");
                    await botClient.SendTextMessageAsync(message.Chat, "To search by the the beginning part of the call add * to the end. E.g.: VP8*");
                    await botClient.SendTextMessageAsync(message.Chat, "To search by country enter country name as follows: c=Easter island");
                    await botClient.SendTextMessageAsync(message.Chat, "To get last 10 spots press /last");
                    await botClient.SendTextMessageAsync(message.Chat, "To get X last spots enter the number X");
                    return;
                }
                var client = new ClusterClient();

                List<SpotObject> response;
                if (message.Text.ToLower().StartsWith("c="))
                {
                    response = await client.GetCountrySpots(message.Text.ToLower().Substring(2));

                }
                else if (message.Text.ToLower() == "/last")
                {
                    response = await client.GetLastSpots(10);
                }

                else if (int.TryParse(message.Text, out int top))
                {
                    response = await client.GetLastSpots(top);
                }
                else
                {
                    response = await client.GetSpots(message.Text.ToLower());
                }

                if (response.Count == 0)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Nothing was found");
                    return;
                }
                response.Reverse();
                foreach (var so in response)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "spot: " +
                                                                        so.DxCall + " " + so.Frequency + " " + so.Time.ToString("MM/dd/yyyy H:mm") + " " + so.Call + " " + so.Info);

                }

            }
        }
    }

}

