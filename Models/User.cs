using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaixaWEB.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public double AccountBalance { get; set; }        
    }
}
