using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DistibutedStockTicker
{
     [HubName("StockTickerHub")]
    public class StockTickerHub : Hub
    {
        public string Hello()
        {
            
            return "hello!";
        }

        public void SendMessage(string message)
        {
            Clients.All.addMessage(message);
        }
    }
}