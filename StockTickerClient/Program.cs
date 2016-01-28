using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using StockTickerDTO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace StockTickerClient
{
    class Program
    {
        private static XDocument _xmlStorage;
        private static List<User> _users;
        private static List<StockItem> _stockItems;
        private static string _path;
        private static User _currentUser;

        private static void Initialize()
        {
            _path = AppDomain.CurrentDomain.BaseDirectory + @"Users.xml";
            _currentUser = new User();

            if (File.Exists(_path))
            {
                _xmlStorage = XDocument.Load(_path);
            }
            else
            {
                Serialize(_users);
                _xmlStorage = XDocument.Load(_path);
            }
            _users = DeserializeParams(_xmlStorage);
        }


        private static void Main(string[] args)
        {
            ServerConnection();
            Initialize();
            Start();
            Run();
        }


        public static void IndexSubscription()
        {
            Console.WriteLine("Input index: ");

            var index = Console.ReadLine();

            if (String.IsNullOrEmpty(index))
            {
                Console.WriteLine("index is required: ");
            }
            else
            {
                Console.WriteLine("Your index :{0}", index);
            }
        }

        public static void ActionSubscription()
        {
            Console.WriteLine("Action index: ");

            var action = Console.ReadLine();

            if (String.IsNullOrEmpty(action))
            {
                Console.WriteLine("action is required: ");
            }
            else
            {
                Console.WriteLine("Your action :{0}", action);
            }
        }

        public static void Register()
        {
            Console.WriteLine("Choose a name: ");

            var userName = Console.ReadLine();

            if (String.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Name is required: ");
            }
            else
            {
                var userExist = UserExist(userName);

                while(userExist)
                {
                    Console.WriteLine("user exist: ");
                    userName = Console.ReadLine();
                    userExist = UserExist(userName);
                }
                Console.WriteLine("Choose a password: ");

                var password = Console.ReadLine();

                while(String.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Password is required: ");
                    password = Console.ReadLine();
                }
                var user = CreateUser(userName, password);
            }
        }

        public static void Login()
        {
            Console.WriteLine("Write your name: ");

            var userName = Console.ReadLine();
            var user = GetUser(userName);

            if (user == null)
            {
                Console.WriteLine("Invalid username");
            }
            else
            {
                Console.WriteLine("Write your password: ");

                var password = Console.ReadLine();
                var correctPassword = CorrectPassword(user, password);

                if (correctPassword)
                {
                    user.Logged = true;

                    Serialize(_users);

                    _currentUser = user;
                }
                else
                {
                    Console.WriteLine("Invalid password");
                }
            }
        }

        private static bool CorrectPassword(User user, string password)
        {
            //var user = GetUser(userName);
            var response = false;
            if (user != null)
            {
                response = user.Password == password ? true : false;
            }
            return response;
        }

        private static bool UserExist(string userName)
        {
            var user = GetUser(userName);

            var response = user == null ? false : true;

            return response;
        }

        private static User GetUser(string userName)
        {
            var user = new User();

            if (_users != null)
            {
                user = !String.IsNullOrEmpty(userName) ? _users.FirstOrDefault(x => x.Username == userName) : null;
            }

            return user;
        }

        private static int RandomID()
        {
            int id = new Random().Next();

            var milisecond = DateTime.Now.Millisecond;
            var second = DateTime.Now.Second;

            id = id * milisecond * second;
            id = id < 0 ? id * -1 : id;

            return id;
        }

        private static User CreateUser(string userName, string password)
        {
            var user = new User();
            var id = RandomID();

            if (!String.IsNullOrEmpty(userName) || !String.IsNullOrEmpty(password))
            {
                user = new User
                {
                    Id = id,
                    Logged = false,
                    Password = password,
                    Username = userName,
                };

                _users.Add(user);
                Serialize(_users);
            }

            return user;
        }

        private static void Logout()
        {
            _currentUser.Logged = false;
            var user = GetUser(_currentUser.Username);
            user.Logged = false;
            Serialize(_users);
            Start();
        }

        private static void Serialize(IEnumerable<User> userEntityList)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (TextWriter xmlWriter = new StreamWriter(_path))
            {
                serializer.Serialize(xmlWriter, userEntityList);
            }
        }

        private static List<User> DeserializeParams(XDocument doc)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            XmlReader reader = doc.CreateReader();

            using (reader)
            {
                List<User> result = (List<User>)serializer.Deserialize(reader);
                reader.Close();

                return result;
            }
        }

        private static void ServerConnection()
        {
            //Set connection
            var connection = new HubConnection("http://localhost:49954/");
            //Make proxy to hub based on hub name on server
            var myHub = connection.CreateHubProxy("StockTickerHub");
            //Start connection

            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                }

            }).Wait();

            /*myHub.Invoke<string>("Hello").ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(task.Result);
                }
            });*/

            //myHub.On<string>("addMessage", param =>
            //{
                //Console.WriteLine("Message reveived from server: " + param);
            //});

            myHub.On<int>("addProduct", info =>
            {
                Console.WriteLine("Product information received: " + info);
            });

            //myHub.Invoke<string>("SendMessage", "I'm doing something!!!").Wait();

            Console.WriteLine("Insert product ID you are interested in:");
            int id = Int32.Parse(Console.ReadLine());
            myHub.Invoke<int>("SendProductById", id).Wait();


            Console.ReadLine();
            Console.Read();
            connection.Stop();
        }

        private static void Start()
        {
            while (!_currentUser.Logged)
            {
                Console.Clear();
                Console.WriteLine("Authentication");
                Console.WriteLine("Choose a option");
                Console.WriteLine("1 - sign in");
                Console.WriteLine("2 - sign up ");
                Console.WriteLine("0 - close");
                int z;
                bool parse = int.TryParse(Console.ReadLine(), out z);
                if (parse)
                {
                    switch (z)
                    {
                        case 1:
                            Console.Clear();
                            Login();
                            break;
                        case 2:
                            Console.Clear();
                            Register();
                            break;
                        case 0:
                            Environment.Exit(1);
                            break;
                        default:
                            Console.WriteLine("Error, please repeat.");
                            Console.WriteLine();
                            break;
                    }
                }
                Console.ReadKey(true);
            }
        }

        private static void Run()
        {
            while (_currentUser.Logged)
            {
                Console.Clear();
                Console.WriteLine("Subscription");
                Console.WriteLine("Choose a option");
                Console.WriteLine("1 - index");
                Console.WriteLine("2 - action");
                Console.WriteLine("0 - logout");
                int z;
                bool parse = int.TryParse(Console.ReadLine(), out z);
                if (parse)
                {
                    switch (z)
                    {
                        case 1:
                            Console.Clear();
                            IndexSubscription();
                            break;
                        case 2:
                            Console.Clear();
                            ActionSubscription();
                            break;
                        case 0:
                            Logout();
                            break;
                        default:
                            Console.WriteLine("Error, please repeat.");
                            Console.WriteLine();
                            break;
                    }
                }
                Console.ReadKey(true);
            }
        }
    }
}
