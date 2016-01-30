using StockTickerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistibutedStockTicker
{
    public class SubscriptionHandler
    {
        private static volatile SubscriptionHandler instance;
        private static object syncRoot = new object();

        private SubscriptionHandler() { }

        public static SubscriptionHandler Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null) instance = new SubscriptionHandler();
                    return instance;
                }
            }
        }

        public List<SubscriptionModel> Subscriptions
        {
            get
            {
                return _subscriptions;
            }
        }

        private List<SubscriptionModel> _subscriptions = new List<SubscriptionModel>();

        public void Subscribe(StockItem stockItem, UserDTO user)
        {
            try
            {
                var subscription = _subscriptions.Find(x => x.StockItem.Symbol == stockItem.Symbol);
                if (subscription != null)
                {
                    subscription.Users.Add(user);
                }
                else
                {
                    subscription = new SubscriptionModel(stockItem);
                    subscription.Users.Add(user);

                    _subscriptions.Add(subscription);
                }
            }
            catch (System.NullReferenceException) { }
            
            
        }

        public void Unscubscribe(StockItem stockItem, UserDTO user)
        {
            var subscription = _subscriptions.Find(x => x.StockItem.Symbol == stockItem.Symbol);
            if (subscription != null)
            {
                if (subscription.Users.Contains(user))
                {
                    subscription.Users.Remove(user);
                }
            }
        }

        public List<UserDTO> getSubscribedUsers(StockItem stockItem)
        {
            try
            {
                var subscription = _subscriptions.Find(x => x.StockItem.Symbol == stockItem.Symbol);
                if (subscription != null)
                {
                    return subscription.Users;
                }
            }
            catch (NullReferenceException) { }
            return Enumerable.Empty<UserDTO>().ToList<UserDTO>();
        }
    }
}