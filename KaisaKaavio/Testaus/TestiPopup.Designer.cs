namespace KaisaKaavio.Testaus
{
    partial class TestiPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestiPopup));
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.testaaButton = new System.Windows.Forms.Button();
            this.peruutaButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mcAsetuksetGroupBox = new System.Windows.Forms.GroupBox();
            this.mcMaxPelaajiaNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mcMinPelaajiaNnumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mcKisojaNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mcCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mcAsetuksetGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mcMaxPelaajiaNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mcMinPelaajiaNnumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mcKisojaNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.Location = new System.Drawing.Point(210, 27);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(53, 20);
            this.numericUpDown1.TabIndex = 0;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pöytien (samanaikaisten pelien) määrä:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(20, 62);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(219, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Pelaa pelit satunnaisessa järjestyksessä?";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 99);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Testiajon asetukset:";
            // 
            // testaaButton
            // 
            this.testaaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.testaaButton.Image = global::KaisaKaavio.Properties.Resources.Ok;
            this.testaaButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.testaaButton.Location = new System.Drawing.Point(155, 370);
            this.testaaButton.Name = "testaaButton";
            this.testaaButton.Size = new System.Drawing.Size(117, 39);
            this.testaaButton.TabIndex = 4;
            this.testaaButton.Text = "Testaa!";
            this.testaaButton.UseVisualStyleBackColor = true;
            this.testaaButton.Click += new System.EventHandler(this.testaaButton_Click);
            // 
            // peruutaButton
            // 
            this.peruutaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.peruutaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.peruutaButton.Image = global::KaisaKaavio.Properties.Resources.Peruuta;
            this.peruutaButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.peruutaButton.Location = new System.Drawing.Point(12, 370);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Size = new System.Drawing.Size(127, 39);
            this.peruutaButton.TabIndex = 5;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.mcAsetuksetGroupBox);
            this.groupBox2.Controls.Add(this.mcCheckBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 117);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(269, 247);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Monte Carlo testaus:";
            // 
            // mcAsetuksetGroupBox
            // 
            this.mcAsetuksetGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcAsetuksetGroupBox.Controls.Add(this.mcMaxPelaajiaNumericUpDown);
            this.mcAsetuksetGroupBox.Controls.Add(this.mcMinPelaajiaNnumericUpDown);
            this.mcAsetuksetGroupBox.Controls.Add(this.mcKisojaNumericUpDown);
            this.mcAsetuksetGroupBox.Controls.Add(this.label5);
            this.mcAsetuksetGroupBox.Controls.Add(this.label4);
            this.mcAsetuksetGroupBox.Controls.Add(this.label3);
            this.mcAsetuksetGroupBox.Location = new System.Drawing.Point(9, 116);
            this.mcAsetuksetGroupBox.Name = "mcAsetuksetGroupBox";
            this.mcAsetuksetGroupBox.Size = new System.Drawing.Size(251, 125);
            this.mcAsetuksetGroupBox.TabIndex = 2;
            this.mcAsetuksetGroupBox.TabStop = false;
            this.mcAsetuksetGroupBox.Text = "Asetukset:";
            this.mcAsetuksetGroupBox.Visible = false;
            // 
            // mcMaxPelaajiaNumericUpDown
            // 
            this.mcMaxPelaajiaNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcMaxPelaajiaNumericUpDown.Location = new System.Drawing.Point(156, 81);
            this.mcMaxPelaajiaNumericUpDown.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.mcMaxPelaajiaNumericUpDown.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.mcMaxPelaajiaNumericUpDown.Name = "mcMaxPelaajiaNumericUpDown";
            this.mcMaxPelaajiaNumericUpDown.Size = new System.Drawing.Size(86, 20);
            this.mcMaxPelaajiaNumericUpDown.TabIndex = 5;
            this.mcMaxPelaajiaNumericUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // mcMinPelaajiaNnumericUpDown
            // 
            this.mcMinPelaajiaNnumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcMinPelaajiaNnumericUpDown.Location = new System.Drawing.Point(156, 55);
            this.mcMinPelaajiaNnumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.mcMinPelaajiaNnumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mcMinPelaajiaNnumericUpDown.Name = "mcMinPelaajiaNnumericUpDown";
            this.mcMinPelaajiaNnumericUpDown.Size = new System.Drawing.Size(86, 20);
            this.mcMinPelaajiaNnumericUpDown.TabIndex = 4;
            this.mcMinPelaajiaNnumericUpDown.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // mcKisojaNumericUpDown
            // 
            this.mcKisojaNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mcKisojaNumericUpDown.Location = new System.Drawing.Point(156, 29);
            this.mcKisojaNumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mcKisojaNumericUpDown.Name = "mcKisojaNumericUpDown";
            this.mcKisojaNumericUpDown.Size = new System.Drawing.Size(86, 20);
            this.mcKisojaNumericUpDown.TabIndex = 3;
            this.mcKisojaNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Pelaajia enintään / kisa:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Pelaajia vähintään / kisa:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Testattavien kisojen määrä:";
            // 
            // mcCheckBox
            // 
            this.mcCheckBox.AutoSize = true;
            this.mcCheckBox.Location = new System.Drawing.Point(20, 93);
            this.mcCheckBox.Name = "mcCheckBox";
            this.mcCheckBox.Size = new System.Drawing.Size(129, 17);
            this.mcCheckBox.TabIndex = 1;
            this.mcCheckBox.Text = "Aja Monte Carlo testi?";
            this.mcCheckBox.UseVisualStyleBackColor = true;
            this.mcCheckBox.CheckedChanged += new System.EventHandler(this.mcCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 52);
            this.label2.TabIndex = 0;
            this.label2.Text = "Mikäli käytössä, testiajossa luodaan X satunnaista \r\nkilpailua ja pelataan ne läp" +
    "i satunnaisesti. \r\nLopussa katsotaan monessako kilpailussa \r\ntuli uusintaottelu " +
    "ennen finaalia";
            // 
            // TestiPopup
            // 
            this.AcceptButton = this.testaaButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(284, 421);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.peruutaButton);
            this.Controls.Add(this.testaaButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(304, 464);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(304, 464);
            this.Name = "TestiPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Testikaavioiden ajo:";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mcAsetuksetGroupBox.ResumeLayout(false);
            this.mcAsetuksetGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mcMaxPelaajiaNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mcMinPelaajiaNnumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mcKisojaNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button testaaButton;
        private System.Windows.Forms.Button peruutaButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox mcAsetuksetGroupBox;
        private System.Windows.Forms.CheckBox mcCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown mcMaxPelaajiaNumericUpDown;
        private System.Windows.Forms.NumericUpDown mcMinPelaajiaNnumericUpDown;
        private System.Windows.Forms.NumericUpDown mcKisojaNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}