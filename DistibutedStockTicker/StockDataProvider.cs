using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockTickerDTO;
using System.Threading;
using Microsoft.AspNet.SignalR;

namespace DistibutedStockTicker
{
    public class StockDataProvider
    {
        private static volatile StockDataProvider container;
        private static object syncRoot = new Object();

        private readonly SubscriptionHandler _subscriptionHandler = SubscriptionHandler.Instance;

        private StockDataProvider()
        {
            var stockTicker = Stock.Ticker.StockTicker.CreateInstance(AppDomain.CurrentDomain.BaseDirectory + "StockData.txt");
            foreach (var stock in stockTicker.AllStocks)
            {
                StockProducts.Add(new StockItem() { Id = stock.Index.IndexName, Name = stock.FullName, Symbol = stock.Symbol, Value = 0.00 });
                Console.WriteLine("Stock: {0} <{1}>", stock.Symbol, stock.Index.IndexName);
            }

            stockTicker.StockChanged += (sender, args) =>
            {
                string symbol = args.Product.Symbol;
                var stockItem = StockDataProvider.Container.StockProducts.Find(x => x.Symbol == symbol);
                stockItem.Value = args.CurrentValue;
                // TODO: log info here
                var hub = GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>();
                List<string> connectionIds =_subscriptionHandler.getSubscribedUsers(stockItem).Select(x => x.Id).ToList();

                hub.Clients.Clients(connectionIds).subscriptionUpdate(stockItem);
            };

            stockTicker.Start();
        }

        public List<StockItem> StockProducts = new List<StockItem>();

        public static StockDataProvider Container
        {
            get
            {
                lock (syncRoot)
                {
                    if (container == null) container = new StockDataProvider();
                }
                return container;
            }
        }
    }
}