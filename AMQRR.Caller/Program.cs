using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using AMQRR.Common.Models;

namespace AMQRR.Caller
{
    class Program
    {
        public static string _baseUri = Url.API_URL;

        public static int _customerId;

        static void Main(string[] args)
        {
            // generate a random customerId
            var r = new Random();
            _customerId = r.Next(10000, 100000);
            Console.Title = $"Caller: (CustomerId={_customerId}";
            Console.WindowHeight = 6;
            Console.WindowWidth = 100;

            var stopwatch = new Stopwatch();
            var orderFactory = new RandomOrderFactory();
            var httpClient = new HttpClient() {BaseAddress = new Uri(_baseUri)};

            while (true)
            {
                var order = orderFactory.Create();
                order.Customer = _customerId;

                var serialised = JsonConvert.SerializeObject(order,Formatting.Indented);
                var stringContent = new StringContent(serialised, Encoding.UTF8,"application/json");

                stopwatch.Restart();

                var post = httpClient.PostAsync("/api/orders", stringContent);
                post.Wait();

                stopwatch.Stop();
                

                if (!post.Result.IsSuccessStatusCode)
                {
                    throw new HttpException($"{post.Result.StatusCode} - {post.Result.ReasonPhrase}");
                }

                var responseBody = post.Result.Content.ReadAsStringAsync();
                responseBody.Wait();
                var responseOrder = JsonConvert.DeserializeObject<Order>(responseBody.Result);
                
                Console.WriteLine(
                    $"Sent OrderId {order.OrderId} for Customer {order.Customer}, " + 
                         $"Received OrderId {responseOrder.OrderId} for Customer {responseOrder.Customer} " + 
                         $"(roundtrip: {stopwatch.ElapsedMilliseconds} ms)");
                
                Thread.Sleep(1000);
            }

        }
    }
}
