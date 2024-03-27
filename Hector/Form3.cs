using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hector
{
    public partial class ExportForm : Form
    {
        private DataTable ExportDataTable;
        public ExportForm()
        {
            InitializeComponent();

            //ExportDataTable = Data;
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {

        }
        private void ExportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = (string)e.Argument; // Chemin du fichier spécifié dans la boîte de dialogue
            

            try
            {
                // Ouvrez une connexion à la base de données SQLite
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=Hector.SQLite;Version=3;"))
                {
                    connection.Open();

                    // Sélectionnez les données nécessaires en joignant les tables
                    string query = @"SELECT A.Description, A.RefArticle, A.RefMarque, F.Nom AS Famille, SF.Nom AS SousFamille, A.PrixHT 
                             FROM Articles A 
                             INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille 
                             INNER JOIN Familles F ON SF.RefFamille = F.RefFamille";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Créez un StreamWriter pour écrire dans le fichier CSV
                            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                            {
                                // Écrivez l'en-tête du fichier CSV
                                writer.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.");

                                // Parcourez chaque ligne de résultat et écrivez-la dans le fichier CSV
                                while (reader.Read())
                                {
                                    string line = $"{reader["Description"]};{reader["RefArticle"]};{reader["RefMarque"]};{reader["Famille"]};{reader["SousFamille"]};{reader["PrixHT"]}";
                                    writer.WriteLine(line);
                                }
                            }
                        }
                    }
                }

                e.Result = "Exportation terminée !";
            }
            catch (Exception ex)
            {
                e.Result = "Erreur lors de l'exportation : " + ex.Message;
            }
        }
        

        private void Exporter_Click(object sender, EventArgs e)
        {
            // Créez une boîte de dialogue pour spécifier l'emplacement du fichier exporté
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fichiers CSV (*.csv)|*.csv";
            saveFileDialog.FileName = "export.csv"; // Nom par défaut du fichier exporté

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Démarrez le travail d'exportation dans le thread en arrière-plan
                BackgroundWorker exportWorker = new BackgroundWorker();
                exportWorker.DoWork += ExportWorker_DoWork;
                exportWorker.RunWorkerAsync(saveFileDialog.FileName);
            }
        }
    }
}
