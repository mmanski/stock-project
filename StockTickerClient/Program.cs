using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTickerClient
{
    class Program
    {
        private static void Main(string[] args) {
            //Set connection
            var connection = new HubConnection("http://localhost:49954/");
            //Make proxy to hub based on hub name on server
            var myHub = connection.CreateHubProxy("StockTickerHub");
            //Start connection

            connection.Start().ContinueWith(task => {
                if (task.IsFaulted) {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                } else {
                    Console.WriteLine("Connected");
                }

            }).Wait();

            myHub.Invoke<string>("Hello").ContinueWith(task => {
                if (task.IsFaulted) {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                } else {
                    Console.WriteLine(task.Result);
                }
            });

            myHub.On<string>("addMessage", param =>
            {
                Console.WriteLine("Message reveived from server: " + param);
            });

            myHub.Invoke<string>("SendMessage", "I'm doing something!!!").Wait();



            Console.Read();
            connection.Stop();
        }
    }
}
