using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockTickerDTO;

namespace DistibutedStockTicker
{
    public class Subscription
    {
        public StockItem StockItem { get; set; }
        public IList<User> Users { get; set; }
    }
}