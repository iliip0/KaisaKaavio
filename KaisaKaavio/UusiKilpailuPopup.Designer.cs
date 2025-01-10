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
            this.rankingCheckBox = new System.Windows.Forms.CheckBox();
            this.rankingLabel = new System.Windows.Forms.Label();
            this.rankingComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.uusiKilpailuLajiComboBox = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.alkamisAikaDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.kilpasarjaLabel = new System.Windows.Forms.Label();
            this.kilpaSarjaComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // uusiKilpailuButton
            // 
            this.uusiKilpailuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uusiKilpailuButton.Image = global::KaisaKaavio.Properties.Resources.Ok;
            this.uusiKilpailuButton.Location = new System.Drawing.Point(575, 6);
            this.uusiKilpailuButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uusiKilpailuButton.Name = "uusiKilpailuButton";
            this.uusiKilpailuButton.Size = new System.Drawing.Size(203, 60);
            this.uusiKilpailuButton.TabIndex = 0;
            this.uusiKilpailuButton.Text = "Luo uusi kilpailu!";
            this.uusiKilpailuButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.uusiKilpailuButton.UseVisualStyleBackColor = true;
            this.uusiKilpailuButton.Click += new System.EventHandler(this.uusiKilpailuButton_Click);
            // 
            // peruutaButton
            // 
            this.peruutaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.peruutaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.peruutaButton.Image = global::KaisaKaavio.Properties.Resources.Peruuta;
            this.peruutaButton.Location = new System.Drawing.Point(4, 6);
            this.peruutaButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Size = new System.Drawing.Size(131, 60);
            this.peruutaButton.TabIndex = 1;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.peruutaButton.UseVisualStyleBackColor = true;
            this.peruutaButton.Click += new System.EventHandler(this.peruutaButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 55);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Kilpailun tyyppi:";
            // 
            // kilpailunTyyppiComboBox
            // 
            this.kilpailunTyyppiComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kilpailunTyyppiComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.kilpailunTyyppiComboBox.FormattingEnabled = true;
            this.kilpailunTyyppiComboBox.Items.AddRange(new object[] {
            "Viikkokilpailu (3.kierros pudari)",
            "Avoin kilpailu (tuplakaavio loppuun asti)",
            "Kaisan RG kilpailu (tuplakaavio loppuun asti, 1-8 sijoitettua pelaajaa)",
            "Kaisan SM kilpailu (tuplakaavio loppuun asti, 1-24 sijoitettua pelaajaa)"});
            this.kilpailunTyyppiComboBox.Location = new System.Drawing.Point(250, 52);
            this.kilpailunTyyppiComboBox.Name = "kilpailunTyyppiComboBox";
            this.kilpailunTyyppiComboBox.Size = new System.Drawing.Size(522, 28);
            this.kilpailunTyyppiComboBox.TabIndex = 3;
            this.kilpailunTyyppiComboBox.SelectedIndexChanged += new System.EventHandler(this.kilpailunTyyppiComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Kilpailun nimi:";
            // 
            // kilpailunNimiTextBox
            // 
            this.kilpailunNimiTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kilpailunNimiTextBox.Location = new System.Drawing.Point(250, 86);
            this.kilpailunNimiTextBox.Name = "kilpailunNimiTextBox";
            this.kilpailunNimiTextBox.Size = new System.Drawing.Size(522, 26);
            this.kilpailunNimiTextBox.TabIndex = 5;
            this.kilpailunNimiTextBox.Text = "Kaisakilpailu";
            this.kilpailunNimiTextBox.TextChanged += new System.EventHandler(this.kilpailunNimiTextBox_TextChanged);
            this.kilpailunNimiTextBox.Validated += new System.EventHandler(this.kilpailunNimiTextBox_Validated);
            // 
            // rankingCheckBox
            // 
            this.rankingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rankingCheckBox.AutoSize = true;
            this.rankingCheckBox.Location = new System.Drawing.Point(85, 18);
            this.rankingCheckBox.Name = "rankingCheckBox";
            this.rankingCheckBox.Size = new System.Drawing.Size(311, 24);
            this.rankingCheckBox.TabIndex = 6;
            this.rankingCheckBox.Text = "Kilpailu on viikkokisa ranking osakilpailu?";
            this.rankingCheckBox.UseVisualStyleBackColor = true;
            this.rankingCheckBox.CheckedChanged += new System.EventHandler(this.rankingCheckBox_CheckedChanged);
            // 
            // rankingLabel
            // 
            this.rankingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rankingLabel.AutoSize = true;
            this.rankingLabel.Location = new System.Drawing.Point(81, 51);
            this.rankingLabel.Name = "rankingLabel";
            this.rankingLabel.Size = new System.Drawing.Size(163, 20);
            this.rankingLabel.TabIndex = 7;
            this.rankingLabel.Text = "Ranking sarjan tyyppi:";
            // 
            // rankingComboBox
            // 
            this.rankingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rankingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankingComboBox.FormattingEnabled = true;
            this.rankingComboBox.Items.AddRange(new object[] {
            "Kuukausittainen ranking sarja",
            "Kolmen kuukauden välein vaihtuva sarja",
            "Puolen vuoden välein vaihtuva sarja",
            "Vuoden kestävä sarja"});
            this.rankingComboBox.Location = new System.Drawing.Point(250, 48);
            this.rankingComboBox.Name = "rankingComboBox";
            this.rankingComboBox.Size = new System.Drawing.Size(522, 28);
            this.rankingComboBox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Laji:";
            // 
            // uusiKilpailuLajiComboBox
            // 
            this.uusiKilpailuLajiComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uusiKilpailuLajiComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uusiKilpailuLajiComboBox.FormattingEnabled = true;
            this.uusiKilpailuLajiComboBox.Location = new System.Drawing.Point(250, 18);
            this.uusiKilpailuLajiComboBox.Name = "uusiKilpailuLajiComboBox";
            this.uusiKilpailuLajiComboBox.Size = new System.Drawing.Size(522, 28);
            this.uusiKilpailuLajiComboBox.TabIndex = 10;
            this.uusiKilpailuLajiComboBox.SelectedIndexChanged += new System.EventHandler(this.uusiKilpailuLajiComboBox_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.uusiKilpailuButton);
            this.splitContainer1.Panel2.Controls.Add(this.peruutaButton);
            this.splitContainer1.Size = new System.Drawing.Size(786, 334);
            this.splitContainer1.SplitterDistance = 255;
            this.splitContainer1.TabIndex = 11;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.kilpaSarjaComboBox);
            this.splitContainer2.Panel1.Controls.Add(this.kilpasarjaLabel);
            this.splitContainer2.Panel1.Controls.Add(this.alkamisAikaDateTimePicker);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.kilpailunNimiTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.uusiKilpailuLajiComboBox);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.kilpailunTyyppiComboBox);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer2.Panel2.Controls.Add(this.rankingCheckBox);
            this.splitContainer2.Panel2.Controls.Add(this.rankingComboBox);
            this.splitContainer2.Panel2.Controls.Add(this.rankingLabel);
            this.splitContainer2.Size = new System.Drawing.Size(786, 255);
            this.splitContainer2.SplitterDistance = 166;
            this.splitContainer2.TabIndex = 9;
            // 
            // alkamisAikaDateTimePicker
            // 
            this.alkamisAikaDateTimePicker.CustomFormat = "d.M.yyyy";
            this.alkamisAikaDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.alkamisAikaDateTimePicker.Location = new System.Drawing.Point(250, 118);
            this.alkamisAikaDateTimePicker.Name = "alkamisAikaDateTimePicker";
            this.alkamisAikaDateTimePicker.Size = new System.Drawing.Size(111, 26);
            this.alkamisAikaDateTimePicker.TabIndex = 13;
            this.alkamisAikaDateTimePicker.ValueChanged += new System.EventHandler(this.alkamisAikaDateTimePicker_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Alkamispäivämäärä:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KaisaKaavio.Properties.Resources.KilpailuInfo32;
            this.pictureBox1.Location = new System.Drawing.Point(4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(54, 50);
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::KaisaKaavio.Properties.Resources.Ranking32;
            this.pictureBox2.Location = new System.Drawing.Point(4, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(54, 50);
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // kilpasarjaLabel
            // 
            this.kilpasarjaLabel.AutoSize = true;
            this.kilpasarjaLabel.Location = new System.Drawing.Point(392, 121);
            this.kilpasarjaLabel.Name = "kilpasarjaLabel";
            this.kilpasarjaLabel.Size = new System.Drawing.Size(81, 20);
            this.kilpasarjaLabel.TabIndex = 14;
            this.kilpasarjaLabel.Text = "Kilpasarja:";
            // 
            // kilpaSarjaComboBox
            // 
            this.kilpaSarjaComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kilpaSarjaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.kilpaSarjaComboBox.FormattingEnabled = true;
            this.kilpaSarjaComboBox.Location = new System.Drawing.Point(479, 118);
            this.kilpaSarjaComboBox.Name = "kilpaSarjaComboBox";
            this.kilpaSarjaComboBox.Size = new System.Drawing.Size(293, 28);
            this.kilpaSarjaComboBox.TabIndex = 15;
            this.kilpaSarjaComboBox.SelectedIndexChanged += new System.EventHandler(this.kilpaSarjaComboBox_SelectedIndexChanged);
            // 
            // UusiKilpailuPopup
            // 
            this.AcceptButton = this.uusiKilpailuButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(786, 334);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(806, 377);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(806, 377);
            this.Name = "UusiKilpailuPopup";
            this.Text = "Luo uusi kilpailu";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uusiKilpailuButton;
        private System.Windows.Forms.Button peruutaButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox kilpailunTyyppiComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox kilpailunNimiTextBox;
        private System.Windows.Forms.CheckBox rankingCheckBox;
        private System.Windows.Forms.Label rankingLabel;
        private System.Windows.Forms.ComboBox rankingComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox uusiKilpailuLajiComboBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DateTimePicker alkamisAikaDateTimePicker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox kilpaSarjaComboBox;
        private System.Windows.Forms.Label kilpasarjaLabel;
    }
}