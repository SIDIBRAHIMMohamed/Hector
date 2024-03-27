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
        private TreeView TreeView;
        private ListView ListView;
        private List<Familles> ListFamilles;
        private List<Marque> ListeMarques;

        public FormMain()
        {
            InitializeComponent();
            splitContainer1.Panel1MinSize =200;

            // Initialisation des listes
            ListFamilles = new List<Familles>();
            ListeMarques = new List<Marque>();
           



            // Initialisez votre ListView
            InitializeListView();

            // Initialisez votre TreeView
            InitializeTreeView();

            

            // Obtenir le répertoire de démarrage de l'application (où se trouve l'exécutable)
            string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Construire le chemin absolu vers le fichier de base de données SQLite
            string databasePath = Path.Combine(directory, "Hector.SQLite");

            // Chaîne de connexion à la base de données SQLite
            string connectionString = $"Data Source={databasePath};Version=3;";






        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
            ChargerFamilles();
            ChargerMarques();
            LoadAllArticles(); // Appelez LoadAllArticles() après l'initialisation du ListView
            LoadTreeView();



        }
        private void InitializeListView()
        {
            ListView = new ListView();
            ListView.View = View.Details;
            ListView.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(ListView);
        }

        private void InitializeTreeView()
        {
            TreeView = new TreeView();
            TreeView.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(TreeView);
            TreeView.AfterSelect += TreeView_AfterSelect;
        }



        private void ChargerFamilles()
        {
            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT RefFamille, Nom FROM Familles";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int refFamille = Convert.ToInt32(reader["RefFamille"]);
                            string nomFamille = reader["Nom"].ToString();

                            // Créer un objet Famille et l'ajouter à ListFamilles
                            Familles famille = new Familles(refFamille, nomFamille);
                            ListFamilles.Add(famille);
                        }
                    }
                }
            }
        }

        // Méthode pour charger les marques depuis la base de données
        private void ChargerMarques()
        {
            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT RefMarque, Nom FROM Marques";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int refMarque = Convert.ToInt32(reader["RefMarque"]);
                            string nomMarque = reader["Nom"].ToString();

                            // Créer un objet Marque et l'ajouter à ListeMarques
                            Marque marque = new Marque(refMarque, nomMarque);
                            ListeMarques.Add(marque);
                        }
                    }
                }
            }
        }

        private void LoadAllArticles()
        {
            // Effacer le contenu actuel du ListView
            ListView.Items.Clear();

            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT A.Description, A.RefArticle, A.RefMarque, F.Nom AS Famille, SF.Nom AS SousFamille, A.PrixHT
                             FROM Articles A
                             INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                             INNER JOIN Familles F ON SF.RefFamille = F.RefFamille";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string refMarque = reader["RefMarque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamille = reader["SousFamille"].ToString();
                            decimal prixHT = Convert.ToDecimal(reader["PrixHT"]);

                            // Créer un ListViewItem pour représenter l'article
                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, refMarque, famille, sousFamille, prixHT.ToString() });

                            // Ajouter l'item au ListView
                            ListView.Items.Add(item);
                        }
                    }
                }
            }
        }

        // Méthode pour charger les sous-familles
        private void LoadSubFamilies(Familles famille)
        {
            // Effacer le contenu actuel du ListView
            ListView.Items.Clear();

            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Construction de la requête pour sélectionner les sous-familles de la famille spécifiée
                string query = "SELECT RefSousFamille, Nom FROM SousFamilles WHERE RefFamille = @refFamille";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@refFamille", famille.reFamille);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int refSousFamille = Convert.ToInt32(reader["RefSousFamille"]);
                            string nomSousFamille = reader["Nom"].ToString();

                            // Créer un ListViewItem pour représenter la sous-famille
                            ListViewItem item = new ListViewItem(new string[] { nomSousFamille, refSousFamille.ToString() });

                            // Ajouter l'item au ListView
                            ListView.Items.Add(item);
                        }
                    }
                }
            }
        }

        // Méthode pour charger les articles d'une marque spécifique
        private void LoadArticlesBrand(Marque marque)
        {
            // Effacer le contenu actuel du ListView
            ListView.Items.Clear();

            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Construction de la requête pour sélectionner les articles de la marque spécifiée
                string query = @"SELECT A.Description, A.RefArticle, A.RefMarque, F.Nom AS Famille, SF.Nom AS SousFamille, A.PrixHT
                             FROM Articles A
                             INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                             INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                             WHERE A.RefMarque = @refMarque";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@refMarque", marque.RefMarque);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string refMarque = reader["RefMarque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamille = reader["SousFamille"].ToString();
                            decimal prixHT = Convert.ToDecimal(reader["PrixHT"]);

                            // Créer un ListViewItem pour représenter l'article
                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, refMarque, famille, sousFamille, prixHT.ToString() });

                            // Ajouter l'item au ListView
                            ListView.Items.Add(item);
                        }
                    }
                }
            }
        }



        // Méthode pour récupérer la liste des sous-familles pour une famille donnée
        private List<SousFamilles> GetSubFamiliesForFamily(Familles famille)
        {
            List<SousFamilles> subFamilies = new List<SousFamilles>();

            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Construction de la requête pour sélectionner les sous-familles de la famille spécifiée
                string query = "SELECT RefSousFamille, Nom FROM SousFamilles WHERE RefFamille = @refFamille";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@refFamille", famille.reFamille);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int refSousFamille = Convert.ToInt32(reader["RefSousFamille"]);
                            string nomSousFamille = reader["Nom"].ToString();

                            // Créer un objet SousFamilles et l'ajouter à la liste
                            SousFamilles sousFamille = new SousFamilles(refSousFamille, nomSousFamille);
                            subFamilies.Add(sousFamille);
                        }
                    }
                }
            }

            return subFamilies;
        }





        private void LoadTreeView()
        {
            // Effacer tous les nœuds existants
            TreeView.Nodes.Clear();

            // Ajouter les nœuds racines
            TreeNode allArticlesNode = TreeView.Nodes.Add("Tous les articles");
            TreeNode familiesNode = TreeView.Nodes.Add("Familles");
            TreeNode brandsNode = TreeView.Nodes.Add("Marques");

            // Vérifier si la liste des familles est vide
            if (ListFamilles != null && ListFamilles.Count > 0)
            {
                // Ajouter les familles comme nœuds enfants sous le nœud "Familles"
                foreach (Familles famille in ListFamilles)
                {
                    TreeNode familyNode = familiesNode.Nodes.Add(famille.Nom);
                    List<SousFamilles> sousFamilles = GetSubFamiliesForFamily(famille);

                    // Effacer les nœuds enfants existants de ce nœud famille
                    familyNode.Nodes.Clear();

                    // Ajouter les sous-familles comme nœuds enfants sous chaque nœud "Famille"
                    foreach (SousFamilles sousFamille in sousFamilles)
                    {
                        familyNode.Nodes.Add(sousFamille.Nom);
                    }
                }
            }
            else
            {
                // Si la liste des familles est vide, affichez un message d'erreur ou effectuez une autre action appropriée.
                MessageBox.Show("La liste des familles est vide. Assurez-vous que les familles sont correctement chargées.");
            }

            // Ajouter les marques comme nœuds enfants sous le nœud "Marques"
            foreach (Marque marque in ListeMarques)
            {
                brandsNode.Nodes.Add(marque.Nom);
            }
        }







        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Récupérer le nœud sélectionné
            TreeNode selectedNode = e.Node;

            // Vérifier si le nœud sélectionné est une sous-famille (un nœud feuille)
            if (selectedNode != null && selectedNode.Parent != null && selectedNode.Nodes.Count == 0)
            {
                // Récupérer l'identifiant de la sous-famille à partir du texte du nœud
                int RefSousFamille = GetRefSousFamilleByName(selectedNode.Text);

                // Charger les articles de la sous-famille sélectionnée
                LoadArticlesBySubFamily(RefSousFamille);
            }
        }



        private int GetRefSousFamilleByName(string sousFamilleName)
        {
            // Initialiser l'identifiant de la sous-famille à une valeur par défaut
            int refSousFamille = -1;

            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Construction de la requête pour récupérer l'identifiant de la sous-famille par son nom
                string query = "SELECT RefSousFamille FROM SousFamilles WHERE Nom = @sousFamilleName";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sousFamilleName", sousFamilleName);

                    // Exécuter la requête pour récupérer l'identifiant de la sous-famille
                    object result = command.ExecuteScalar();

                    // Vérifier si le résultat est non nul et convertible en entier
                    if (result != null && int.TryParse(result.ToString(), out int parsedRefSousFamille))
                    {
                        // Affecter l'identifiant de la sous-famille récupérée
                        refSousFamille = parsedRefSousFamille;
                    }
                }
            }

            // Retourner l'identifiant de la sous-famille
            return refSousFamille;
        }





        private void LoadArticlesBySubFamily(int refSousFamille)
        {
            // Effacer le contenu actuel du ListView
            ListView.Items.Clear();

            // Chaîne de connexion à la base de données SQLite
            string connectionString = "Data Source=Hector.SQLite;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Construction de la requête pour sélectionner les articles de la sous-famille spécifiée
                string query = @"SELECT A.Description, A.RefArticle, A.RefMarque, F.Nom AS Famille, SF.Nom AS SousFamille, A.PrixHT
                         FROM Articles A
                         INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                         WHERE SF.RefSousFamille = @refSousFamille";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@refSousFamille", refSousFamille);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string refMarque = reader["RefMarque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamille = reader["SousFamille"].ToString();
                            decimal prixHT = Convert.ToDecimal(reader["PrixHT"]);

                            // Créer un ListViewItem pour représenter l'article
                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, refMarque, famille, sousFamille, prixHT.ToString() });

                            // Ajouter l'item au ListView
                            ListView.Items.Add(item);
                        }
                    }
                }
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
