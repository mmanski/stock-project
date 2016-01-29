using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using StockTickerDTO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace StockTickerClient
{
    class Program
    {

        private static void Main(string[] args)
        {
            Start();
            Run();
        }


        public static void IndexSubscription()
        {
            Console.WriteLine("Input index: ");

            var index = Console.ReadLine();

            if (String.IsNullOrEmpty(index))
            {
                Console.WriteLine("index is required: ");
            }
            else
            {
                Console.WriteLine("Your index :{0}", index);
            }
        }

        public static void ActionSubscription()
        {
            Console.WriteLine("Action index: ");

            var action = Console.ReadLine();

            if (String.IsNullOrEmpty(action))
            {
                Console.WriteLine("action is required: ");
            }
            else
            {
                Console.WriteLine("Your action :{0}", action);
            }
        }

        public static void Login()
        {
            Console.WriteLine("Write your name: ");

            var userName = Console.ReadLine();
            Connect(userName);
        }

        private static void Connect(string username)
        {
            var querystringData = new Dictionary<string, string>();
            querystringData.Add("username", username);

            var connection = new HubConnection("http://localhost:49954/", querystringData);
            var myHub = connection.CreateHubProxy("StockTickerHub");

            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                }

            }).Wait();

            myHub.On<StockItem>("subscriptionUpdate", param =>
            {
                Console.WriteLine("Product information received: " + param.Symbol + " Value:" + param.Value);
            });


            string symbol = "";
            while (symbol != "x")
            {
                Console.WriteLine("Insert stock symbol you want to subscribe on (x - exit):");
                symbol = Console.ReadLine();
                if (symbol != "x") myHub.Invoke<StockItem>("Subscribe", symbol).Wait();
            }


            Console.Read();
            connection.Stop();
        }

        private static void Start()
        {

            Console.Clear();
            Console.WriteLine("Authentication");
            Console.WriteLine("Choose an option");
            Console.WriteLine("1 - sign in");
            Console.WriteLine("0 - close");
            int z;
            bool parse = int.TryParse(Console.ReadLine(), out z);
            if (parse)
            {
                switch (z)
                {
                    case 1:
                        Console.Clear();
                        Login();
                        break;
                    case 0:
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("Error, please repeat.");
                        Console.WriteLine();
                        break;
                }
            }
            Console.ReadKey(true);
        }

        private static void Run()
        {
            Console.Clear();
            Console.WriteLine("Subscription");
            Console.WriteLine("Choose a option");
            Console.WriteLine("1 - index");
            Console.WriteLine("2 - action");
            Console.WriteLine("0 - logout");
            int z;
            bool parse = int.TryParse(Console.ReadLine(), out z);
            if (parse)
            {
                switch (z)
                {
                    case 1:
                        Console.Clear();
                        IndexSubscription();
                        break;
                    case 2:
                        Console.Clear();
                        ActionSubscription();
                        break;
                    case 0:

                        break;
                    default:
                        Console.WriteLine("Error, please repeat.");
                        Console.WriteLine();
                        break;
                }
            }
            Console.ReadKey(true);

        }
    }
}
