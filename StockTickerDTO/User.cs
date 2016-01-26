using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTickerDTO
{
    public class User
    {
        public bool Logged { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int Id { get;  set; }
        //public int Id { get; private set; }
    }
}
