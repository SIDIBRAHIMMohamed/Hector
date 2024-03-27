using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class Marque
    {
        private int refMarque;
        private string nomMarque;

        public Marque(int refMarque, string nomMarque)
        {
            this.refMarque = refMarque;
            this.nomMarque = nomMarque;
        }

        public int Id { get; set; }
        public string Nom { get { return nomMarque; } }

        public int RefMarque { get; set; }

        public List<Articles> Articles { get; set; }
    }
}
