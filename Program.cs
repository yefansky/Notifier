// Demo practice by https://www.youtube.com/watch?v=NWt7pGXiY20&list=RDCMUCe1nKo3WGGzyTgDqmTdZzlA&start_radio=1&t=517s&ab_channel=AnduinXue
// 如何使用30行代码快速爬下来一个网站的数据？Anduin演示真实的黑客行为
// 修改为 爬b站 "剑网3缘起" 直播间变动
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.HSharp.Methods;

namespace Notifier
{
    class Program
    {
        static string sURL = "https://search.bilibili.com/live?keyword=%E5%89%91%E7%BD%913%E7%BC%98%E8%B5%B7";
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        public static async Task MainAsync()
        {
            var dataSource = await GetDataSource();

            while (true)
            {
                var newData = await GetDataSource();
                var newItems = newData.Except(dataSource);

                if (newItems.Any())
                {
                    // Push notification
                    dataSource = newData;
                }
                else
                {
                    Console.WriteLine("no changed!");
                }
                await Task.Delay(3000);
            }
        }

        public async static Task<List<string>> GetDataSource()
        {
            var stringCollection = new List<string>();
            try {
                var client = new HttpClient();
                var response = await client.GetAsync(sURL);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var doc = HtmlConvert.DeserializeHtml(body);

                var items = doc.AllUnder
                .Where(t => t.Properties.ContainsKey("class"))
                .Where(t => t.Properties["class"] == "item-title");

                foreach (var item in items)
                {
                    var title = item.Properties["title"];
                    //Console.WriteLine(title);
                    stringCollection.Add(title);
                }
            }   
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }

            return stringCollection;
        }
    }
}

