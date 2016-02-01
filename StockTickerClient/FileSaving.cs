using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockTickerDTO;
using System.IO;

namespace StockTickerClient
{
    class FileSaving
    {
        private IHubProxy _myHub;

        public FileSaving(IHubProxy myHub)
        {
            _myHub = myHub;
        }
        public void SubscriptionToFile(string name)
        {
            if (_myHub != null)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + @"StockSubscription.txt";

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();

                    _myHub.On<StockItem>("subscriptionUpdate", subscribeSymbol =>
                    {
                        File.AppendAllText(path, String.Format("{0}: Product information received:{1} Value:{2}\n ", name, subscribeSymbol.Symbol, subscribeSymbol.Value));
                    });
                    Console.ReadKey();
                }
                else if (File.Exists(path))
                {
                    _myHub.On<StockItem>("subscriptionUpdate", subscribeSymbol =>
                    {
                        File.AppendAllText(path, String.Format("{0}: Product information received:{1} Value:{2}\n ", name, subscribeSymbol.Symbol, subscribeSymbol.Value));
                    });
                    Console.ReadKey();
                }
            }
        }
    }
}
