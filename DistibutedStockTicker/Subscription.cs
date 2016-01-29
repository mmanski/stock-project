using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockTickerDTO;

namespace DistibutedStockTicker
{
    public class SubscriptionModel
    {
        public StockItem StockItem { get; set; }
        public List<UserDTO> Users { get; set; }

        public SubscriptionModel(StockItem stockItem)
        {
            this.StockItem = stockItem;
            this.Users = new List<UserDTO>();
        }
    }
}