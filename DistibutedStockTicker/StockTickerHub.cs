using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using StockTickerDTO;
using System.Threading.Tasks;

namespace DistibutedStockTicker
{
    [HubName("StockTickerHub")]
    public class StockTickerHub : Hub
    {
        private readonly UserManager _userManager = UserManager.Instance;

        private readonly SubscriptionHandler _subscriptionHandler = SubscriptionHandler.Instance;

        private List<StockItem> stockItems = StockDataProvider.Container.StockProducts;

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

        public StockItem SubscribeUserToSymbol(string symbol) //*
        {
            string username = Context.QueryString["username"];
            var stockItem = stockItems.Find(x => x.Symbol == symbol);
            var user = _userManager.FindUserByName(username);

            _subscriptionHandler.Subscribe(stockItem, user);

            return stockItem;
        }

        public List<StockItem> SubscribeUserToIndex(string index)
        {
            string username = Context.QueryString["username"];
            var indexItems = stockItems.FindAll(x => x.Id == index);
            var user = _userManager.FindUserByName(username);

            foreach (var stockItem in indexItems)
            {
                _subscriptionHandler.Subscribe(stockItem, user);
            }

            return stockItems;
        }

        public void UnsubscribeBySymbol(string symbol)
        {
            string username = Context.QueryString["username"];
            var stockItem = StockDataProvider.Container.StockProducts.Find(x => x.Symbol == symbol);
            var user = _userManager.FindUserByName(username);

            _subscriptionHandler.Unscubscribe(stockItem, user);
        }

        public void UnsubscribeByIndex(string index)
        {
            string username = Context.QueryString["username"];
            var indexItems = StockDataProvider.Container.StockProducts.FindAll(x => x.Id == index);
            var user = _userManager.FindUserByName(username);
            foreach (var stockItem in indexItems)
            {
                _subscriptionHandler.Unscubscribe(stockItem, user);
            }
        }



    public void notifySubscribers()
        {

        }

        public void GetProductBySymbol(string symbol)
        {
            StockItem result = StockDataProvider.Container.StockProducts.Find(x => x.Symbol == symbol);
            string info = result != null
                ? result.Id + " " + result.Name + " " + result.Symbol + " " + result.Value
                : "Product not found";
            string name = Context.QueryString["username"];

            Clients.Client(_userManager.FindUserByName(name).Id).addProduct(info);
        }

        public string GetAllStockItems()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            foreach (StockItem stockItem in StockDataProvider.Container.StockProducts)
            {
                string result = stockItem.Id.ToString() + " " + stockItem.Name + " " + stockItem.Symbol + " " + stockItem.Value;
                stringBuilder.AppendLine(result);
            }

            return stringBuilder.ToString();
        }

        public override Task OnConnected()
        {
            string name = Context.QueryString["username"];

            _userManager.AddUser(name, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.QueryString["username"];
            _userManager.Remove(_userManager.FindUserByName(name));

            return base.OnDisconnected(stopCalled);
        }
    }

}