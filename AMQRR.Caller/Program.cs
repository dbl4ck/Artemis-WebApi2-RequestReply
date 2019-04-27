using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AMQRR.Common.Factories;
using AMQRR.Common.Configuration;
using Newtonsoft.Json;
using System.Threading;
using System.Web;

namespace AMQRR.Caller
{
    class Program
    {
        public static string _baseUri = Url.API_URL;

        static void Main(string[] args)
        {
            var orderFactory = new RandomOrderFactory();
            var httpClient = new HttpClient() {BaseAddress = new Uri(_baseUri)};

            while (true)
            {
                var order = orderFactory.Create();
                var serialised = JsonConvert.SerializeObject(order,Formatting.Indented);
                var stringContent = new StringContent(serialised, Encoding.UTF8,"application/json");

                Console.WriteLine(serialised);

                var post = httpClient.PostAsync("/api/orders", stringContent);
                post.Wait();

                if (!post.Result.IsSuccessStatusCode)
                {
                    throw new HttpException($"{post.Result.StatusCode} - {post.Result.ReasonPhrase}");
                }

                Thread.Sleep(1000);
            }

        }
    }
}
