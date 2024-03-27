using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class Marque
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public List<Articles> Articles { get; set; }
    }
}
