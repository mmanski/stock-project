using Microsoft.AspNet.SignalR.Client;
using StockTickerDTO;
using System;
using System.Collections.Generic;
using System.IO;

namespace StockTickerClient
{
    class Program
    {
        private static IHubProxy myHub;
        private static HubConnection connection;

        private static void Main(string[] args)
        {
            Start();
        }

        public static void StockSubscription()
        {
            if (myHub != null)
            {
                var StockItems = myHub.Invoke<string>("GetAllStockItems").Result;

                Console.WriteLine("Chose one stock from this list:");
                Console.WriteLine("Stock Items: {0}", StockItems);

                string symbol = "";

                while (symbol != "x")
                {
                    Console.WriteLine("Insert stock symbol you want to subscribe on (x - exit):");
                    symbol = Console.ReadLine();
                    if (symbol != "x")
                    {
                        var subscribeSymbol = myHub.Invoke<StockItem>("Subscribe", symbol).Result;

                        if (subscribeSymbol != null)
                        {
                            Console.WriteLine("Stock subscript! Check your StockSubscription file");
                            break;
                        }
                        else Console.WriteLine("Stock no exist");
                    }
                }

                SubscriptionToFile();
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Connection problem!");
                Console.ReadKey();
            }
        }

        public static void StockUnsubscript()
        {
            if (myHub != null)
            {
                string symbol = "";
                while (symbol != "x")
                {
                    Console.WriteLine("Insert stock symbol you want to unsubscribe on (x - exit):");
                    symbol = Console.ReadLine();
                    if (symbol != "x")
                    {
                        myHub.Invoke("Unsubscribe", symbol);
                        Console.WriteLine("Stock unsubscript!");
                        break;
                    }
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Connection problem!");
                Console.ReadKey();
            }
        }

        public static void Login()
        {
            Console.WriteLine("Write your name: ");

            var userName = Console.ReadLine();

            Connect(userName);

            if (connection != null)
                Run();
        }

        private static void Connect(string username)
        {
            var querystringData = new Dictionary<string, string>();
            querystringData.Add("username", username);
            connection = new HubConnection("http://localhost:49954/", querystringData);
            myHub = connection.CreateHubProxy("StockTickerHub");

            try
            {
                connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error opening the connection:{0}",
                                          task.Exception.GetBaseException());
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Connected");
                    }

                }).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection problem! {0}", e.Message);
                Console.ReadKey();
            }
        }

        private static void Start()
        {
            while (connection == null || connection.State.ToString() == "Disconnected")
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
                            Console.ReadKey();
                            break;
                    }
                }
            }
        }

        private static void Run()
        {
            while (connection != null && connection.State.ToString() == "Connected")
            {
                Console.Clear();
                Console.WriteLine("Subscription");
                Console.WriteLine("Choose a option");
                Console.WriteLine("1 - Subscript symbol");
                Console.WriteLine("2 - Unsubscript symbol");
                Console.WriteLine("0 - logout");

                int z;
                bool parse = int.TryParse(Console.ReadLine(), out z);

                if (parse)
                {
                    switch (z)
                    {
                        case 1:
                            Console.Clear();
                            StockSubscription();
                            break;
                        case 2:
                            Console.Clear();
                            StockUnsubscript();
                            break;
                        case 0:
                            if (connection != null) connection.Stop(); Start();
                            break;
                        default:
                            Console.WriteLine("Error, please repeat.");
                            Console.ReadKey();
                            break;
                    }
                }
            }
        }

        private static void SubscriptionToFile()
        {
            if (myHub != null)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + @"StockSubscription.txt";

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();

                    myHub.On<StockItem>("subscriptionUpdate", subscribeSymbol =>
                    {
                        File.AppendAllText(path, String.Format("Product information received:{0} Value:{1}\n ", subscribeSymbol.Symbol, subscribeSymbol.Value));
                        //using (TextWriter tw = new StreamWriter(path))
                        //using (TextWriter tw = new StreamWriter(path,true))
                        //{
                        //    tw.WriteLine("Product information received: " + subscribeSymbol.Symbol + " Value:" + subscribeSymbol.Value);
                        //    tw.Close();
                        //}
                    });
                }
                else if (File.Exists(path))
                {
                    myHub.On<StockItem>("subscriptionUpdate", subscribeSymbol =>
                    {
                        File.AppendAllText(path, String.Format("Product information received:{0} Value:{1}\n ", subscribeSymbol.Symbol, subscribeSymbol.Value));
                        //using (TextWriter tw = new StreamWriter(path))
                        //using (TextWriter tw = new StreamWriter(path,true))
                        //{
                        //    tw.WriteLine("Product information received: " + subscribeSymbol.Symbol + " Value:" + subscribeSymbol.Value);
                        //    tw.Close();
                        //}
                    });
                }
            }
        }
    }
}
