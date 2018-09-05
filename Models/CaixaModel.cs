using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaixaWEB.Models
{
    public class CaixaModel
    {
        public User User { get; set; }
        public string Deposit { get; set; }
        public string Resultado { get; set; }
        public int N100 { get; set; }
        public int N50 { get; set; }
        public int N20 { get; set; }
        public int N10 { get; set; }
        public int N5 { get; set; }
        public int N2 { get; set; }        
    }
}
