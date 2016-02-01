using Microsoft.AspNet.SignalR.Client;
using StockTickerDTO;
using System;
using System.Collections.Generic;
using System.IO;

namespace StockTickerClient
{
    class Program
    {
        private static ClientHub _myhub;
        private static IHubProxy myHub;
        private static HubConnection connection;
        public static User _username = new User();
        private static string Message;
        
        private static void Main(string[] args)
        {
            _myhub = new ClientHub();

            Start();
        }

        public static void StockSubscription()
        {
            if (myHub != null)
            {


                Console.WriteLine("Chose one stock from this list:");
                Console.WriteLine("Stock Items: {0}", _myhub.GetAllStockItems());

                Console.WriteLine("Choose an option");
                Console.WriteLine("1 - subscribe to stock symbol");
                Console.WriteLine("0 - subscribe to stock index");

                string input = "";

                int z;
                bool parse = int.TryParse(Console.ReadLine(), out z);

                if (parse)
                {
                    switch (z)
                    {
                        case 1:
                            while (input != "x")
                            {
                                Console.WriteLine("Insert stock symbol you want to subscribe on (x - exit):");
                                input = Console.ReadLine();
                                if (input != "x")
                                {
                                    var subscribeSymbol = myHub.Invoke<StockItem>("SubscribeUserToSymbol", input).Result;

                                    if (subscribeSymbol != null)
                                    {
                                        Console.WriteLine("Stock {0} subscript! Check your StockSubscription file", input);
                                        break;
                                    }
                                    else Console.WriteLine("Stock {0} no exist", input);
                                }
                            }
                            break;
                        case 0:
                            while (input != "x")
                            {
                                Console.WriteLine("Insert stock index you want to subscribe on (x - exit):");
                                input = Console.ReadLine();
                                if (input != "x")
                                {
                                    var subscribeIndex = myHub.Invoke<List<StockItem>>("SubscribeUserToIndex", input).Result;

                                    if (subscribeIndex != null)
                                    {
                                        Console.WriteLine("Stock {0} subscript! Check your StockSubscription file", input);
                                        break;
                                    }
                                    else Console.WriteLine("Stock {0} no exist", input);
                                }
                            }
                            break;
                        default:
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

        public static void StockUnsubscript()
        {
            if (myHub != null)
            {
                Console.WriteLine("Choose an option");
                Console.WriteLine("1 - unsubscribe to stock symbol");
                Console.WriteLine("0 - unsubscribe to stock index");

                string symbol = "";

                int z;
                bool parse = int.TryParse(Console.ReadLine(), out z);

                if (parse)
                {
                    switch (z)
                    {
                        case 1:

                            while (symbol != "x")
                            {
                                Console.WriteLine("Insert stock symbol you want to unsubscribe on (x - exit):");
                                symbol = Console.ReadLine();
                                if (symbol != "x")
                                {
                                    myHub.Invoke("UnsubscribeBySymbol", symbol);
                                    Console.WriteLine("Stock {0} unsubscript!", symbol);
                                    break;
                                }
                            }
                            break;
                        case 0:
                            while (symbol != "x")
                            {
                                Console.WriteLine("Insert stock index you want to unsubscribe on (x - exit):");
                                symbol = Console.ReadLine();
                                if (symbol != "x")
                                {
                                    myHub.Invoke("UnsubscribeByIndex", symbol);
                                    Console.WriteLine("Stock {0} unsubscript!", symbol);
                                    break;
                                }
                            }
                            break;
                        default:
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

            _myhub.Connect(userName);

            myHub = _myhub.getMyHub();
            connection = _myhub.connection;

            if (connection != null)
                SubscriptionView();
            else
                Console.WriteLine("Blad z polaczeniem!");
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

        private static void SubscriptionView()
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
    }
}