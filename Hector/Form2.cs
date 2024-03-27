using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hector
{
    public enum ImportMode { 
    
           Overwrite,
           Add
    }
    public partial class ImportForm : Form
    {
        private BackgroundWorker integrationWorker;
        public ImportMode SelectedImportMode { get; private set; }
        private String CSVFileName;




        public ImportForm()
        {
            InitializeComponent();
            integrationWorker = new BackgroundWorker();
            integrationWorker.DoWork += IntegrationWorker_DoWork;
            integrationWorker.RunWorkerCompleted += IntegrationWorker_RunWorkerCompleted;
        }





        private int GetFamilyIdOrCreateNew(SQLiteConnection connection, SQLiteTransaction transaction, string familyName)
        {
            int refFamille = 0;
            string query = "SELECT RefFamille FROM Familles WHERE Nom = @Nom";

            using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Nom", familyName);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    refFamille = Convert.ToInt32(result);
                }
                else
                {
                    // La famille n'existe pas, insérez-la
                    string insertQuery = "INSERT INTO Familles (Nom) VALUES (@Nom); SELECT last_insert_rowid();";

                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@Nom", familyName);
                        object insertResult = insertCommand.ExecuteScalar();
                        refFamille = Convert.ToInt32(insertResult);
                    }
                }
            }

            return refFamille;
        }

        private int GetSubFamilyIdOrCreateNew(SQLiteConnection connection, SQLiteTransaction transaction, int refFamille, string subFamilyName)
        {
            int refSousFamille = 0;
            string query = "SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Nom AND RefFamille = @RefFamille";

            using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Nom", subFamilyName);
                command.Parameters.AddWithValue("@RefFamille", refFamille);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    refSousFamille = Convert.ToInt32(result);
                }
                else
                {
                    // La sous-famille n'existe pas, insérez-la
                    string insertQuery = "INSERT INTO SousFamilles (Nom, RefFamille) VALUES (@Nom, @RefFamille); SELECT last_insert_rowid();";

                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@Nom", subFamilyName);
                        insertCommand.Parameters.AddWithValue("@RefFamille", refFamille);
                        object insertResult = insertCommand.ExecuteScalar();
                        refSousFamille = Convert.ToInt32(insertResult);
                    }
                }
            }

            return refSousFamille;
        }

        private int GetBrandIdOrCreateNew(SQLiteConnection connection, SQLiteTransaction transaction, string brandName)
        {
            int refMarque = 0;
            string query = "SELECT RefMarque FROM Marques WHERE Nom = @Nom";

            using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Nom", brandName);
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    refMarque = Convert.ToInt32(result);
                }
                else
                {
                    // La marque n'existe pas, insérez-la
                    string insertQuery = "INSERT INTO Marques (Nom) VALUES (@Nom); SELECT last_insert_rowid();";

                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@Nom", brandName);
                        object insertResult = insertCommand.ExecuteScalar();
                        refMarque = Convert.ToInt32(insertResult);
                    }
                }
            }

            return refMarque;
        }

        private void IntegrationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Récupérez le mode d'importation à partir des arguments
            ImportMode mode = (ImportMode)e.Argument;

            // Récupérez le chemin du fichier CSV
            string filePath = BtnSelectFile.Text;

            int I = 0;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Le fichier CSV spécifié est introuvable.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Définir le maximum de la barre de progression en fonction du nombre de lignes dans le fichier CSV
             int totalLines = File.ReadLines(filePath).Count();
             progressBar1.Invoke((MethodInvoker)(() => progressBar1.Maximum = totalLines));

            // Effectuez le traitement d'intégration ici
            try
            {
                // Ouvrez une connexion à la base de données SQLite
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=Hector.SQLite;Version=3;"))
                {
                    connection.Open();

                    if (mode == ImportMode.Overwrite)
                    {
                        // Supprimer toutes les données existantes dans les tables
                        ClearDatabase(connection);
                    }
                    
                    int linesProcessed = 0;

                    // Démarrez une transaction pour garantir l'intégrité des données
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        // Lisez le fichier CSV et insérez les données dans la base de données
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            // Ignorez la première ligne (entête)
                            reader.ReadLine();
                          
                            while (!reader.EndOfStream)
                            {
                                I++;
                                string line = reader.ReadLine();
                                string[] values = line.Split(';');

                                // Vérifiez et obtenez l'ID de la famille ou créez une nouvelle famille si elle n'existe pas
                                int refFamille = GetFamilyIdOrCreateNew(connection, transaction, values[3]);

                                // Vérifiez et obtenez l'ID de la sous-famille ou créez une nouvelle sous-famille si elle n'existe pas
                                int refSousFamille = GetSubFamilyIdOrCreateNew(connection, transaction, refFamille, values[4]);

                                // Vérifiez et obtenez l'ID de la marque ou créez une nouvelle marque si elle n'existe pas
                                int refMarque = GetBrandIdOrCreateNew(connection, transaction, values[2]);

                                decimal prixHT = decimal.Parse(values[5], CultureInfo.InvariantCulture);


                                // Insérez les données dans la table Article
                                InsertArticle(connection, transaction, values[0], values[1], refMarque, refSousFamille, prixHT);

                               // Mettre à jour la barre de progression
                                linesProcessed++;
                                progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = linesProcessed));
                            }
                        }

                        // Validez la transaction
                        transaction.Commit();
                    }
                }


                // Transmettez le résultat du traitement via la propriété Result
                e.Result = "Intégration terminée !, "+I+" element ajoutees";
            }
            catch (Exception ex)
            {
                // En cas d'erreur, transmettez le message d'erreur comme résultat
                e.Result = "Erreur lors de l'intégration : " + ex.Message;
            }
        }

        private void ClearDatabase(SQLiteConnection connection)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {


                // Supprimer toutes les données de la table Article
                string deleteArticlesQuery = "DELETE FROM Articles";
                using (SQLiteCommand command = new SQLiteCommand(deleteArticlesQuery, connection, transaction))
                {
                    command.ExecuteNonQuery();
                }


                // Validez la transaction
                transaction.Commit();
            }
        }

        private void InsertArticle(SQLiteConnection connection, SQLiteTransaction transaction, string description, string refArticle, int refMarque, int refSousFamille, decimal prixHT)
        {
            string query = "INSERT INTO Articles (Description, RefArticle, RefSousFamille, RefMarque, PrixHT,Quantite) VALUES (@Description, @RefArticle, @RefSousFamille, @RefMarque, @PrixHT, @Quantite)";

            using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@RefArticle", refArticle);
                command.Parameters.AddWithValue("@RefSousFamille", refSousFamille);
                command.Parameters.AddWithValue("@RefMarque", refMarque);
                command.Parameters.AddWithValue("@PrixHT", prixHT);
                command.Parameters.AddWithValue("@Quantite", 0);
                command.ExecuteNonQuery();
            }
        }

        private void IntegrationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Afficher le résultat de l'intégration dans une MessageBox
            MessageBox.Show(e.Result.ToString(), "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnImportOverwrite_Click(object sender, EventArgs e)
        {
            if (!integrationWorker.IsBusy)
            {
                SelectedImportMode = ImportMode.Overwrite;
                integrationWorker.RunWorkerAsync(SelectedImportMode);
            }
        }

        private void BtnImportAdd_Click(object sender, EventArgs e)
        {
            if (!integrationWorker.IsBusy)
            {
                SelectedImportMode = ImportMode.Add;
                integrationWorker.RunWorkerAsync(SelectedImportMode);
            }
        }

        private void BtnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers CSV (*.csv)|*.csv";
            openFileDialog.Title = "Sélectionnez un fichier CSV";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BtnSelectFile.Text = openFileDialog.FileName;
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}