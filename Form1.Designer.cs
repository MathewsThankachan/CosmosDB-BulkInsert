namespace BulkInsert
{
    partial class Form1
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
            this.btnBulkInsert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBulkInsert
            // 
            this.btnBulkInsert.Location = new System.Drawing.Point(371, 158);
            this.btnBulkInsert.Name = "btnBulkInsert";
            this.btnBulkInsert.Size = new System.Drawing.Size(410, 127);
            this.btnBulkInsert.TabIndex = 0;
            this.btnBulkInsert.Text = "BulkInsert";
            this.btnBulkInsert.UseVisualStyleBackColor = true;
            this.btnBulkInsert.Click += new System.EventHandler(this.btnBulkInsert_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1209, 437);
            this.Controls.Add(this.btnBulkInsert);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.Button btnBulkInsert;
    }
}

