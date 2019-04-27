using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AMQRR.Common.Factories;
using AMQRR.Common.Configuration;

namespace AMQRR.Caller
{
    class Program
    {
        public static string _baseUri = Url.API_URL;

        static void Main(string[] args)
        {
            var orderFactory = new RandomOrderFactory();

            using (var client = new HttpClient() { BaseAddress = new Uri(_baseUri) })
            {
                while (true)
                {
                    var order = orderFactory.Create();



                }

            }

        }
    }
}
