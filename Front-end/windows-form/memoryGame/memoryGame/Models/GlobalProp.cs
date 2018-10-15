using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace memoryGame.Models
{
    class GlobalProp
    {
       public static User CurrentUser { get; set; }
        public static string Host { get; set; } = "http://localhost:50910/api";
    }
}
