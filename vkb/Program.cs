using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = 7690451,
                Login = File.ReadAllText("_login.txt"),
                Password = File.ReadAllText("_password.txt"),
                Settings = Settings.All
            });

            GetUserInfo(vk);
            //Console.ReadKey();
        }

        static public void GetUserInfo(VkApi vk)
        {
            VkParameters param = new VkParameters();
            param.Add<string>("group_id", "51223244");
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
