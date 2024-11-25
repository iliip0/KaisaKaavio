namespace KaisaKaavio
{
    partial class UusiKilpailuPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UusiKilpailuPopup));
            this.uusiKilpailuButton = new System.Windows.Forms.Button();
            this.peruutaButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.kilpailunTyyppiComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.kilpailunNimiTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // uusiKilpailuButton
            // 
            this.uusiKilpailuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uusiKilpailuButton.Location = new System.Drawing.Point(463, 128);
            this.uusiKilpailuButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uusiKilpailuButton.Name = "uusiKilpailuButton";
            this.uusiKilpailuButton.Size = new System.Drawing.Size(279, 60);
            this.uusiKilpailuButton.TabIndex = 0;
            this.uusiKilpailuButton.Text = "Luo uusi kilpailu!";
            this.uusiKilpailuButton.UseVisualStyleBackColor = true;
            this.uusiKilpailuButton.Click += new System.EventHandler(this.uusiKilpailuButton_Click);
            // 
            // peruutaButton
            // 
            this.peruutaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.peruutaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.peruutaButton.Location = new System.Drawing.Point(18, 128);
            this.peruutaButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Size = new System.Drawing.Size(240, 60);
            this.peruutaButton.TabIndex = 1;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.UseVisualStyleBackColor = true;
            this.peruutaButton.Click += new System.EventHandler(this.peruutaButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Kilpailun tyyppi:";
            // 
            // kilpailunTyyppiComboBox
            // 
            this.kilpailunTyyppiComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.kilpailunTyyppiComboBox.FormattingEnabled = true;
            this.kilpailunTyyppiComboBox.Items.AddRange(new object[] {
            "Viikkokilpailu (40min per peli, 3.kierros pudari)",
            "RG kilpailu (60min per peli, tuplakaavio loppuun asti)"});
            this.kilpailunTyyppiComboBox.Location = new System.Drawing.Point(156, 35);
            this.kilpailunTyyppiComboBox.Name = "kilpailunTyyppiComboBox";
            this.kilpailunTyyppiComboBox.Size = new System.Drawing.Size(586, 28);
            this.kilpailunTyyppiComboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Kilpailun nimi:";
            // 
            // kilpailunNimiTextBox
            // 
            this.kilpailunNimiTextBox.Location = new System.Drawing.Point(156, 78);
            this.kilpailunNimiTextBox.Name = "kilpailunNimiTextBox";
            this.kilpailunNimiTextBox.Size = new System.Drawing.Size(586, 26);
            this.kilpailunNimiTextBox.TabIndex = 5;
            this.kilpailunNimiTextBox.Text = "Kaisakilpailu";
            this.kilpailunNimiTextBox.TextChanged += new System.EventHandler(this.kilpailunNimiTextBox_TextChanged);
            // 
            // UusiKilpailuPopup
            // 
            this.AcceptButton = this.uusiKilpailuButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(760, 207);
            this.Controls.Add(this.kilpailunNimiTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.kilpailunTyyppiComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.peruutaButton);
            this.Controls.Add(this.uusiKilpailuButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(780, 250);
            this.MinimumSize = new System.Drawing.Size(780, 250);
            this.Name = "UusiKilpailuPopup";
            this.Text = "Luo uusi Kaisa kilpailu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button uusiKilpailuButton;
        private System.Windows.Forms.Button peruutaButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox kilpailunTyyppiComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox kilpailunNimiTextBox;
    }
}