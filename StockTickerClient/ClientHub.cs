﻿using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTickerClient
{
    class ClientHub
    {
        private IHubProxy _myHub;
        public HubConnection connection;
        public FileSaving FileSaver;
        public User _username = new User();

        public ClientHub()
        {
        }
        public IHubProxy getMyHub()
        {
            return _myHub;
        }
        public void Connect(string username)
        {
            var querystringData = new Dictionary<string, string>();
            querystringData.Add("username", username);
            connection = new HubConnection("http://localhost:49954/", querystringData);
            _myHub = connection.CreateHubProxy("StockTickerHub");

            FileSaver = new FileSaving(_myHub);

            _username.Username = username;

            try
            {
                connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error opening the connection:{0}",
                                          task.Exception.GetBaseException());
                    }
                    else
                    {
                        FileSaver.SubscriptionToFile(_username.Username);
                        Console.WriteLine("Connected! Press Any Key!");
                    }

                }).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection problem! {0}" + e.Message);

            }
        }
        public string GetAllStockItems()
        {
            var StockItems = _myHub.Invoke<string>("GetAllStockItems").Result;
            return StockItems;
        }

    }
}
