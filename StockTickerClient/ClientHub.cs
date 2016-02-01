using Microsoft.AspNet.SignalR.Client;
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
                        return "Connected. Press Enter.";
                        FileSaver.SubscriptionToFile(_username.Username);
                    }

                }).Wait();
            }
            catch (Exception e)
            {
                return "Connection problem! {0}"+ e.Message;

            }
            return "";
        }
    }
}
