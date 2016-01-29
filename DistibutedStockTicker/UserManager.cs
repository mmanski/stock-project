using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Web;
using StockTickerDTO;

namespace DistibutedStockTicker
{
    public class UserManager
    {
        private readonly ConnectionMapping<UserDTO> _connections = new ConnectionMapping<UserDTO>();

        private static volatile UserManager instance;

        private static object syncRoot = new object();

        private UserManager() { }

        public static UserManager Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null) instance = new UserManager();
                    return instance;
                }
            }
        }

        public UserDTO FindUserByName(string username)
        {
            return _connections.GetUsers().Find(element => element.Username == username);
        }

        public void Remove(UserDTO user)
        {
            _connections.RemoveUser(user);
        }

        public void AddUser(string username, string connectionId)
        {
            _connections.AddUser(new UserDTO(username, connectionId));
        }

        private class ConnectionMapping<T>
        {
            private readonly List<T> _connections = new List<T>();

            public int Count
            {
                get
                {
                    return _connections.Count;
                }
            }

            public List<T> GetUsers()
            {
                return _connections;
            }

            public void AddUser(T user)
            {
                lock (_connections)
                {
                    if (!_connections.Contains(user))
                    {
                        _connections.Add(user);
                    }
                    else
                    {
                        throw new LoginException();
                    }
                }
            }

            public void RemoveUser(T user)
            {
                lock (_connections)
                {
                    if (!_connections.Contains(user))
                    {
                        return;
                    }

                    _connections.Remove(user);

                }
            }
        }
    }

}