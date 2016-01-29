using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTickerDTO
{
    public class UserDTO
    {
      
        public string Username { get; private set; }
        public string Id { get; private set; }

        public UserDTO(string username, string connectionId)
        {
            this.Username = username;
            this.Id = connectionId;
        }


    }
}
