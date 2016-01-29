using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using StockTickerDTO;

namespace DistibutedStockTicker
{
     [HubName("StockTickerHub")]
    public class StockTickerHub : Hub
    {
        public static int stockIndex = 0;

        public string Hello()
        {
            return "hello!";
        }

        public void SendMessage(string message)
        {
            Clients.All.addMessage(message);
        }

        public void AddTicker(string symbol)
        {
            Clients.All.receiveTicker(symbol);
        }

        public void AddStockProduct(string fullName, string symbol, string index)
        {
            //StockDataSender.Container.StockProducts.Add(new StockItem() { Id = stockIndex, Name = fullName, Symbol = symbol });
            //stockIndex++;
        }

        public void SendProductById(string index)
        {
            StockItem result = StockDataProvider.Container.StockProducts.Find(x => x.Id == Int32.Parse(index));
            string info = result.Id.ToString() + " " + result.Name + " " + result.Symbol;

            Clients.All.addProduct(info);
        }
    }
}