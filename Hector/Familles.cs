using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class Familles
    {
        private int refFamille;
        private string nomFamille;

        public Familles(int refFamille, string nomFamille)
        {
            this.refFamille = refFamille;
            this.nomFamille = nomFamille;
        }

        public int Id { get; set; }
        public string Nom { get { return nomFamille; } }
       
        public int reFamille { get { return refFamille; } }
        public List<SousFamilles> SousFamilles { get; set; }
    }
}
