using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class Articles
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public decimal PrixHT { get; set; }
        public Marque Marque { get; set; }
        public SousFamilles SousFamille { get; set; }
    }
}
