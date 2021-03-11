using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;

namespace vkb
{
    class Program
    {
        static void Main(string[] args)
        {
            VkApi vk = new VkApi();
            WebClient webclient = new WebClient() {Encoding = Encoding.UTF8};
            VkParameters param = new VkParameters();
            param.Add<string>("group_id", "201002643");
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = 7690451,
                Login = File.ReadAllText("_login.txt"),
                Password = File.ReadAllText("_password.txt"),
                Settings = Settings.All
            });

            //GetUserInfo(vk, param);
            MainBotModule(vk, webclient, param);
            //Console.ReadKey();
        }

        public static void MainBotModule(VkApi vk, WebClient webclient, VkParameters param)
        {
            dynamic responseLongApi = JObject.Parse(vk.Call("groups.getLongPollServer", param).RawJson);
            var json = string.Empty;
            var url = string.Empty;
            while (true)
            {
                url = string.Format("{0}?act=a_check&key={1}&ts={2}&wait=5", responseLongApi.response.server.ToString(),
                    responseLongApi.response.key.ToString(),
                    json != String.Empty?JObject.Parse(json)["ts"].ToString():responseLongApi.response.ts.ToString());
                json = webclient.DownloadString(url);
                var jsonMsg = json.IndexOf(":[]}", StringComparison.Ordinal) > -1 ? "" : $"{json} \n";
                var col = JObject.Parse(json)["updates"].ToList();
                foreach (var item in col)
                {
                    if (item["type"].ToString() == "message_new")
                    {
                        string token =
                            "cdedaa6a97eca371a457a0e2107ff52d5191179575174c0e42ebc3b176938e95496fbadcd7de9d9191535";
                        string urlBotMsg =
                            $"https://api.vk.com/method/messages.send?v=5.50&access_token={token}&user_id=";
                        Console.WriteLine(item.ToString());
                        string answer = string.Empty;
                        JToken name = JObject.Parse(vk.Call("users.get", param).RawJson)["response"];
                        string c = "";
                        foreach (dynamic t in name)
                        {
                            c = t.first_name;
                        }

                        answer = "Здарова, " + c;
                        webclient.DownloadString(String.Format(urlBotMsg + "{0}&message={1}", item["object"]["user_id"],
                            answer));
                        Thread.Sleep(200);
                    }
                }
            }
        }
        public static void GetUserInfo(VkApi vk, VkParameters param)
        {
            
           
            param.Add<string>("offset", "0");
            param.Add<string>("count", "100");

            var rawJson = JObject.Parse(vk.Call("groups.getMembers", param).RawJson);
            string ids = string.Join(", ", rawJson["response"]["items"].ToArray().Select(x => x.ToString()));

            param.Add<string>("user_ids", ids);
            param.Add<string>("fields", "photo_100");

            JToken dbUsers = JObject.Parse(vk.Call("users.get", param).RawJson)["response"];

            foreach (dynamic item in dbUsers)
            {
                Console.WriteLine($"ID: {item.id}\nИмя: {item.first_name}\nФамилия: {item.last_name}\nСсылка на фото: {item.photo_100}");
            }
        }

    }
}
