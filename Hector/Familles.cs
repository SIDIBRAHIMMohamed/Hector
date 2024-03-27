using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class Familles
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public List<SousFamilles> SousFamilles { get; set; }
    }
}
