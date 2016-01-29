﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockTickerDTO;
using System.Threading;

namespace DistibutedStockTicker
{
    public class StockDataProvider
    {
        private static volatile StockDataProvider container;
        private static object syncRoot = new Object();

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
                StockDataProvider.Container.StockProducts.Find(x => x.Symbol == args.Product.Symbol).Value = args.CurrentValue;
                //Console.WriteLine("{0}: New price received for {1} -> {2} [Thread: {3}]", args.Time, args.Product.Symbol, args.CurrentValue, Thread.CurrentThread.ManagedThreadId);
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