using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hector
{
    class SousFamilles
    {
        private int refSousFamille;
        private string nomSousFamille;

        public SousFamilles(int refSousFamille, string nomSousFamille)
        {
            this.refSousFamille = refSousFamille;
            this.nomSousFamille = nomSousFamille;
        }

        public int Id { get; set; }
        public string Nom { get{ return nomSousFamille; } }
        public Familles FamilleParent { get; set; }
        public List<Articles> Articles { get; set; }
    }
}
