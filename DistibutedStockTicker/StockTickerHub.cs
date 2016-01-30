﻿using System;
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

        public StockItem Subscribe(string symbol)
        {
            string username = Context.QueryString["username"];
            var stockItem = stockItems.Find(x => x.Symbol == symbol);
            var user = _userManager.FindUserByName(username);

            _subscriptionHandler.Subscribe(stockItem, user);

            return stockItem;
        }

        public void Unsubscribe(string symbol)
        {
            string username = Context.QueryString["username"];
            var stockItem = StockDataProvider.Container.StockProducts.Find(x => x.Symbol == symbol);
            var user = _userManager.FindUserByName(username);

            _subscriptionHandler.Unscubscribe(stockItem, user);
        }

        public void notifySubscribers()
        {

        }

        public void GetProductBySymbol(string symbol)
        {
            StockItem result = StockDataProvider.Container.StockProducts.Find(x => x.Symbol == symbol);
            string info = result != null
                ? result.Id.ToString() + " " + result.Name + " " + result.Symbol + " " + result.Value
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