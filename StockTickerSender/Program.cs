using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace StockTicker
{
    class Program
    {
        static void Main()
        {
            ServerConnection();
        }

        private static void ServerConnection()
        {
            //Set connection
            var connection = new HubConnection("http://localhost:49954/");
            //Make proxy to hub based on hub name on server
            var myHub = connection.CreateHubProxy("StockTickerHub");
            //Start connection

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

            /*myHub.Invoke<string>("Hello").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(task.Result);
                }
            });*/

            myHub.On<string>("addMessage", param =>
            {
                Console.WriteLine("Message reveived from server: " + param);
            });
            
            var stockTicker = Stock.Ticker.StockTicker.CreateInstance("StockData.txt");
            foreach (var stock in stockTicker.AllStocks)
            {
                myHub.Invoke<string>("AddStockProduct", stock.FullName, stock.Symbol, stock.Index.IndexName).Wait();
                Console.WriteLine("Stock: {0} <{1}>", stock.Symbol, stock.Index.IndexName);
            }

            stockTicker.StockChanged += (sender, args) =>
            {
                string symbol = args.Product.Symbol;
                myHub.Invoke<string>("AddTicker", symbol).Wait();
                Console.WriteLine("{0}: New price received for {1} -> {2} [Thread: {3}]", args.Time, args.Product.Symbol, args.CurrentValue, Thread.CurrentThread.ManagedThreadId);
            };

            using (stockTicker.Start())
            {
                Console.ReadKey();
            }

            Console.ReadKey();


            Console.Read();
            connection.Stop();
        }
    }
}
