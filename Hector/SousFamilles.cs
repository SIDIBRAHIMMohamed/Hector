using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class SousFamilles
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public Familles FamilleParent { get; set; }
        public List<Articles> Articles { get; set; }
    }
}
