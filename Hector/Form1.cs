using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Data.SQLite;

namespace Hector
{
    public partial class FormMain : Form
    {
        private TreeView treeView;
        private List<Familles> ListFamilles;
        private List<Marque> ListeMarques;

        public FormMain()
        {
            InitializeComponent();
            splitContainer1.Panel1MinSize =200;

            treeView.AfterSelect += treeView_AfterSelect;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Création d'un nouvel objet TreeView
            TreeView treeView = new TreeView();

            // Définition de la taille et de la position du TreeView pour remplir la partie gauche du SplitContainer
            treeView.Dock = DockStyle.Fill;



            // Ajout du TreeView à la partie gauche (Panel1) du SplitContainer
            splitContainer1.Panel1.Controls.Add(treeView);

          


            LoadTreeView();

            // Création d'un nouvel objet ListView
            ListView listView = new ListView();

            // Définition de la vue en mode "Détails"
            listView.View = View.Details;

            // Définition du style d'ancrage pour remplir la partie droite du SplitContainer
            listView.Dock = DockStyle.Fill;

            // Ajout du ListView à la partie droite (Panel2) du SplitContainer
            splitContainer1.Panel2.Controls.Add(listView);



            // Obtenir le répertoire de démarrage de l'application (où se trouve l'exécutable)
            string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Construire le chemin absolu vers le fichier de base de données SQLite
            string databasePath = Path.Combine(directory, "Hector.SQLite");

            // Chaîne de connexion à la base de données SQLite
            string connectionString = $"Data Source={databasePath};Version=3;";
         

           
        }




        // Méthode pour charger la structure du TreeView
        private void LoadTreeView()
        {
            // Effacer tous les nœuds existants
            treeView.Nodes.Clear();

            // Ajouter les nœuds racines
            TreeNode allArticlesNode = treeView.Nodes.Add("Tous les articles");
            TreeNode familiesNode = treeView.Nodes.Add("Familles");
            TreeNode brandsNode = treeView.Nodes.Add("Marques");

            // Ajouter les familles comme nœuds enfants sous le nœud "Familles"
            foreach (Familles famille in listeFamilles)
            {
                TreeNode familyNode = familiesNode.Nodes.Add(famille.Nom);

                // Ajouter les sous-familles comme nœuds enfants sous chaque nœud "Famille"
                foreach (SousFamilles sousFamille in famille.SousFamilles)
                {
                    familyNode.Nodes.Add(sousFamille.Nom);
                }
            }

            // Ajouter les marques comme nœuds enfants sous le nœud "Marques"
            foreach (Marque marque in listeMarques)
            {
                brandsNode.Nodes.Add(marque.Nom);
            }
        }

        // Événement AfterSelect du TreeView pour gérer la sélection de nœud
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Gérer la sélection de nœud pour charger les données correspondantes dans le ListView
            TreeNode selectedNode = e.Node;

            if (selectedNode.Text == "Tous les articles")
            {
                // Charger tous les articles dans le ListView
                LoadAllArticles();
            }
            else if (selectedNode.Parent != null && selectedNode.Parent.Text == "Familles")
            {
                // Charger les sous-familles correspondant à la famille sélectionnée dans le ListView
                string selectedFamily = selectedNode.Text;
                LoadSubFamilies(selectedFamily);
            }
            else if (selectedNode.Parent != null && selectedNode.Parent.Text == "Marques")
            {
                // Charger les articles correspondant à la marque sélectionnée dans le ListView
                string selectedBrand = selectedNode.Text;
                LoadArticlesByBrand(selectedBrand);
            }
        }

        // Méthode pour gérer le tri des données dans le ListView
        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Implémentez la logique de tri ici
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fichierToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Créez une instance de la fenêtre d'importation
            ImportForm importForm = new ImportForm();

            // Affichez la fenêtre modale pour permettre à l'utilisateur de choisir le fichier à importer
            if (importForm.ShowDialog() == DialogResult.OK)
            {
                // Récupérez le mode d'importation sélectionné dans la fenêtre modale
                ImportMode importMode = importForm.SelectedImportMode;
            }
        }

        private void exporterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Afficher la fenêtre modale d'exportation
            ExportForm exportForm = new ExportForm();
            exportForm.StartPosition = FormStartPosition.CenterParent;
            exportForm.ShowDialog();
        }
    }
}
