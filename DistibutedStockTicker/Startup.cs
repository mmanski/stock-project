using Microsoft.Owin;
using Owin;
using Stock.Ticker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockTickerDTO;

[assembly: OwinStartup(typeof(DistibutedStockTicker.Startup))]
namespace DistibutedStockTicker
{
    public class Startup
    {
        private static readonly StockItem dto = new StockItem();

        public static StockItem StockItemInstance
        {
            get { return dto; }
        }

        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}