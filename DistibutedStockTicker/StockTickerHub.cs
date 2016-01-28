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
        List<StockItem> StockProducts = new List<StockItem>();

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
            StockProducts.Add(new StockItem() { Id = stockIndex, Name = fullName, Symbol = symbol });
            stockIndex++;
        }

        public void SendProductById(int index)
        {
            StockItem result = StockProducts.Find(x => x.Id == index);
            string info = result.Id.ToString() + " " + result.Name + " " + result.Symbol;

            Clients.All.addProduct(info);
        }
    }
}