namespace Vaja1_ArtimeticniKodirnik {
    partial class AritmeticnoKodiranje {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.columnHeaderP7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderP5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderP4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderP3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderP2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderP1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblRezultat = new System.Windows.Forms.Label();
            this.tbRezultat = new System.Windows.Forms.TextBox();
            this.gbPostopek = new System.Windows.Forms.GroupBox();
            this.lvPostopek = new System.Windows.Forms.ListView();
            this.columnHeaderP6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderF4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderF3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderF1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvTabela = new System.Windows.Forms.ListView();
            this.columnHeaderF2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbFrekvence = new System.Windows.Forms.GroupBox();
            this.btnDekodirajText = new System.Windows.Forms.Button();
            this.btnDekodirajDatoteko = new System.Windows.Forms.Button();
            this.btnKodirajDatoteko = new System.Windows.Forms.Button();
            this.btnKodirajText = new System.Windows.Forms.Button();
            this.gbVhod = new System.Windows.Forms.GroupBox();
            this.cbBiti = new System.Windows.Forms.ComboBox();
            this.tbText = new System.Windows.Forms.TextBox();
            this.gbPostopek.SuspendLayout();
            this.gbFrekvence.SuspendLayout();
            this.gbVhod.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeaderP7
            // 
            this.columnHeaderP7.Text = " ";
            // 
            // columnHeaderP5
            // 
            this.columnHeaderP5.Text = "Operations + Output";
            this.columnHeaderP5.Width = 117;
            // 
            // columnHeaderP4
            // 
            this.columnHeaderP4.Text = "(nL, nH)";
            // 
            // columnHeaderP3
            // 
            this.columnHeaderP3.Text = "Step";
            // 
            // columnHeaderP2
            // 
            this.columnHeaderP2.Text = "(L, H)";
            // 
            // columnHeaderP1
            // 
            this.columnHeaderP1.Text = "Symbol";
            // 
            // lblRezultat
            // 
            this.lblRezultat.AutoSize = true;
            this.lblRezultat.Location = new System.Drawing.Point(6, 303);
            this.lblRezultat.Name = "lblRezultat";
            this.lblRezultat.Size = new System.Drawing.Size(40, 13);
            this.lblRezultat.TabIndex = 3;
            this.lblRezultat.Text = "Result:";
            // 
            // tbRezultat
            // 
            this.tbRezultat.Location = new System.Drawing.Point(52, 300);
            this.tbRezultat.Name = "tbRezultat";
            this.tbRezultat.Size = new System.Drawing.Size(844, 20);
            this.tbRezultat.TabIndex = 2;
            // 
            // gbPostopek
            // 
            this.gbPostopek.Controls.Add(this.lblRezultat);
            this.gbPostopek.Controls.Add(this.tbRezultat);
            this.gbPostopek.Controls.Add(this.lvPostopek);
            this.gbPostopek.Location = new System.Drawing.Point(20, 247);
            this.gbPostopek.Name = "gbPostopek";
            this.gbPostopek.Size = new System.Drawing.Size(902, 332);
            this.gbPostopek.TabIndex = 6;
            this.gbPostopek.TabStop = false;
            this.gbPostopek.Text = "Postopek";
            // 
            // lvPostopek
            // 
            this.lvPostopek.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderP1,
            this.columnHeaderP2,
            this.columnHeaderP3,
            this.columnHeaderP4,
            this.columnHeaderP5,
            this.columnHeaderP6,
            this.columnHeaderP7});
            this.lvPostopek.Location = new System.Drawing.Point(7, 19);
            this.lvPostopek.Name = "lvPostopek";
            this.lvPostopek.Size = new System.Drawing.Size(889, 263);
            this.lvPostopek.TabIndex = 1;
            this.lvPostopek.UseCompatibleStateImageBehavior = false;
            this.lvPostopek.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderP6
            // 
            this.columnHeaderP6.Text = "E3Cnt";
            // 
            // columnHeaderF4
            // 
            this.columnHeaderF4.Text = "h(x)";
            // 
            // columnHeaderF3
            // 
            this.columnHeaderF3.Text = "l(x)";
            // 
            // columnHeaderF1
            // 
            this.columnHeaderF1.Text = "Symbol";
            // 
            // lvTabela
            // 
            this.lvTabela.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderF1,
            this.columnHeaderF2,
            this.columnHeaderF3,
            this.columnHeaderF4});
            this.lvTabela.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTabela.Location = new System.Drawing.Point(3, 16);
            this.lvTabela.Name = "lvTabela";
            this.lvTabela.Size = new System.Drawing.Size(391, 210);
            this.lvTabela.TabIndex = 0;
            this.lvTabela.UseCompatibleStateImageBehavior = false;
            this.lvTabela.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderF2
            // 
            this.columnHeaderF2.Text = "Freq";
            // 
            // gbFrekvence
            // 
            this.gbFrekvence.Controls.Add(this.lvTabela);
            this.gbFrekvence.Location = new System.Drawing.Point(525, 12);
            this.gbFrekvence.Name = "gbFrekvence";
            this.gbFrekvence.Size = new System.Drawing.Size(397, 229);
            this.gbFrekvence.TabIndex = 5;
            this.gbFrekvence.TabStop = false;
            this.gbFrekvence.Text = "Tabela frekvenc";
            // 
            // btnDekodirajText
            // 
            this.btnDekodirajText.Location = new System.Drawing.Point(89, 19);
            this.btnDekodirajText.Name = "btnDekodirajText";
            this.btnDekodirajText.Size = new System.Drawing.Size(75, 23);
            this.btnDekodirajText.TabIndex = 4;
            this.btnDekodirajText.Text = "Dekodiraj";
            this.btnDekodirajText.UseVisualStyleBackColor = true;
            this.btnDekodirajText.Click += new System.EventHandler(this.BtnDekodirajTextClick);
            // 
            // btnDekodirajDatoteko
            // 
            this.btnDekodirajDatoteko.Location = new System.Drawing.Point(386, 18);
            this.btnDekodirajDatoteko.Name = "btnDekodirajDatoteko";
            this.btnDekodirajDatoteko.Size = new System.Drawing.Size(107, 23);
            this.btnDekodirajDatoteko.TabIndex = 3;
            this.btnDekodirajDatoteko.Text = "Dekodiraj datoteko";
            this.btnDekodirajDatoteko.UseVisualStyleBackColor = true;
            this.btnDekodirajDatoteko.Click += new System.EventHandler(this.BtnDekodirajDatotekoClick);
            // 
            // btnKodirajDatoteko
            // 
            this.btnKodirajDatoteko.Location = new System.Drawing.Point(287, 18);
            this.btnKodirajDatoteko.Name = "btnKodirajDatoteko";
            this.btnKodirajDatoteko.Size = new System.Drawing.Size(93, 23);
            this.btnKodirajDatoteko.TabIndex = 2;
            this.btnKodirajDatoteko.Text = "Kodiraj datoteko";
            this.btnKodirajDatoteko.UseVisualStyleBackColor = true;
            this.btnKodirajDatoteko.Click += new System.EventHandler(this.btnKodirajDatoteko_Click);
            // 
            // btnKodirajText
            // 
            this.btnKodirajText.Location = new System.Drawing.Point(8, 19);
            this.btnKodirajText.Name = "btnKodirajText";
            this.btnKodirajText.Size = new System.Drawing.Size(75, 23);
            this.btnKodirajText.TabIndex = 1;
            this.btnKodirajText.Text = "Kodiraj";
            this.btnKodirajText.UseVisualStyleBackColor = true;
            this.btnKodirajText.Click += new System.EventHandler(this.BtnKodirajTextClick);
            // 
            // gbVhod
            // 
            this.gbVhod.Controls.Add(this.cbBiti);
            this.gbVhod.Controls.Add(this.btnDekodirajText);
            this.gbVhod.Controls.Add(this.btnDekodirajDatoteko);
            this.gbVhod.Controls.Add(this.btnKodirajDatoteko);
            this.gbVhod.Controls.Add(this.btnKodirajText);
            this.gbVhod.Controls.Add(this.tbText);
            this.gbVhod.Location = new System.Drawing.Point(20, 12);
            this.gbVhod.Name = "gbVhod";
            this.gbVhod.Size = new System.Drawing.Size(499, 229);
            this.gbVhod.TabIndex = 4;
            this.gbVhod.TabStop = false;
            this.gbVhod.Text = "Vhod";
            // 
            // cbBiti
            // 
            this.cbBiti.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBiti.FormattingEnabled = true;
            this.cbBiti.Items.AddRange(new object[] {
            "8",
            "16",
            "32",
            "64"});
            this.cbBiti.Location = new System.Drawing.Point(171, 20);
            this.cbBiti.Name = "cbBiti";
            this.cbBiti.Size = new System.Drawing.Size(110, 21);
            this.cbBiti.TabIndex = 5;
            this.cbBiti.SelectedIndexChanged += new System.EventHandler(this.CbBitiSelectedIndexChanged);
            // 
            // tbText
            // 
            this.tbText.Location = new System.Drawing.Point(9, 48);
            this.tbText.Multiline = true;
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(484, 172);
            this.tbText.TabIndex = 0;
            this.tbText.Text = "Krasna si bistra hci planin";
            // 
            // AritmeticnoKodiranje
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 593);
            this.Controls.Add(this.gbPostopek);
            this.Controls.Add(this.gbFrekvence);
            this.Controls.Add(this.gbVhod);
            this.Name = "AritmeticnoKodiranje";
            this.Text = "Aritmetično kodiranje (RM-1)";
            this.gbPostopek.ResumeLayout(false);
            this.gbPostopek.PerformLayout();
            this.gbFrekvence.ResumeLayout(false);
            this.gbVhod.ResumeLayout(false);
            this.gbVhod.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeaderP7;
        private System.Windows.Forms.ColumnHeader columnHeaderP5;
        private System.Windows.Forms.ColumnHeader columnHeaderP4;
        private System.Windows.Forms.ColumnHeader columnHeaderP3;
        private System.Windows.Forms.ColumnHeader columnHeaderP2;
        private System.Windows.Forms.ColumnHeader columnHeaderP1;
        private System.Windows.Forms.Label lblRezultat;
        private System.Windows.Forms.TextBox tbRezultat;
        private System.Windows.Forms.GroupBox gbPostopek;
        private System.Windows.Forms.ListView lvPostopek;
        private System.Windows.Forms.ColumnHeader columnHeaderP6;
        private System.Windows.Forms.ColumnHeader columnHeaderF4;
        private System.Windows.Forms.ColumnHeader columnHeaderF3;
        private System.Windows.Forms.ColumnHeader columnHeaderF1;
        private System.Windows.Forms.ListView lvTabela;
        private System.Windows.Forms.ColumnHeader columnHeaderF2;
        private System.Windows.Forms.GroupBox gbFrekvence;
        private System.Windows.Forms.Button btnDekodirajText;
        private System.Windows.Forms.Button btnDekodirajDatoteko;
        private System.Windows.Forms.Button btnKodirajDatoteko;
        private System.Windows.Forms.Button btnKodirajText;
        private System.Windows.Forms.GroupBox gbVhod;
        private System.Windows.Forms.ComboBox cbBiti;
        private System.Windows.Forms.TextBox tbText;
    }
}

