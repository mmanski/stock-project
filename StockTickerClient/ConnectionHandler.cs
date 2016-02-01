using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTickerClient
{
    class ConnectionHandler
    {
        private IHubProxy _myHub;
        public HubConnection connection;
        public FileSaving FileSaver;
        public User _username = new User();

        public ConnectionHandler()
        {
        }
        public IHubProxy getMyHub()
        {
            return _myHub;
        }
        public string Connect(string username)
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
                        return "There was an error opening the connection:{0}"+
                                          task.Exception.GetBaseException();
                    }
                    else
                    {
                        FileSaver.SubscriptionToFile(_username.Username);


                        return "Connected! Press Any Key!";

                    }

                }).Wait();
            }
            catch (Exception e)
            {
                return "Connection problem!" + e.Message;

            }
            return "Connected! Press Any Key";
        }
        public string GetAllStockItems()
        {
            var StockItems = _myHub.Invoke<string>("GetAllStockItems").Result;
            return StockItems;
        }

    }
}
