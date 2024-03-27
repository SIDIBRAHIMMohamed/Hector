
namespace Hector
{
    partial class ImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSelectFile = new System.Windows.Forms.Button();
            this.BtnImportOverwrite = new System.Windows.Forms.Button();
            this.BtnImportAdd = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // BtnSelectFile
            // 
            this.BtnSelectFile.Location = new System.Drawing.Point(78, 68);
            this.BtnSelectFile.Name = "BtnSelectFile";
            this.BtnSelectFile.Size = new System.Drawing.Size(86, 35);
            this.BtnSelectFile.TabIndex = 0;
            this.BtnSelectFile.Text = "Importer";
            this.BtnSelectFile.UseVisualStyleBackColor = true;
            this.BtnSelectFile.Click += new System.EventHandler(this.BtnSelectFile_Click);
            // 
            // BtnImportOverwrite
            // 
            this.BtnImportOverwrite.Location = new System.Drawing.Point(148, 137);
            this.BtnImportOverwrite.Name = "BtnImportOverwrite";
            this.BtnImportOverwrite.Size = new System.Drawing.Size(82, 34);
            this.BtnImportOverwrite.TabIndex = 1;
            this.BtnImportOverwrite.Text = "Remplacer";
            this.BtnImportOverwrite.UseVisualStyleBackColor = true;
            this.BtnImportOverwrite.Click += new System.EventHandler(this.BtnImportOverwrite_Click);
            // 
            // BtnImportAdd
            // 
            this.BtnImportAdd.Location = new System.Drawing.Point(32, 137);
            this.BtnImportAdd.Name = "BtnImportAdd";
            this.BtnImportAdd.Size = new System.Drawing.Size(83, 34);
            this.BtnImportAdd.TabIndex = 2;
            this.BtnImportAdd.Text = "Ajouter";
            this.BtnImportAdd.UseVisualStyleBackColor = true;
            this.BtnImportAdd.Click += new System.EventHandler(this.BtnImportAdd_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(78, 227);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // ImportForm
            // 
            this.ClientSize = new System.Drawing.Size(291, 271);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.BtnImportAdd);
            this.Controls.Add(this.BtnImportOverwrite);
            this.Controls.Add(this.BtnSelectFile);
            this.Name = "ImportForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnSelectFile;
        private System.Windows.Forms.Button BtnImportOverwrite;
        private System.Windows.Forms.Button BtnImportAdd;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}