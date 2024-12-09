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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
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
            this.testaaButton.Location = new System.Drawing.Point(155, 128);
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
            this.peruutaButton.Location = new System.Drawing.Point(12, 128);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Size = new System.Drawing.Size(127, 39);
            this.peruutaButton.TabIndex = 5;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.UseVisualStyleBackColor = true;
            // 
            // TestiPopup
            // 
            this.AcceptButton = this.testaaButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(284, 179);
            this.Controls.Add(this.peruutaButton);
            this.Controls.Add(this.testaaButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TestiPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Testikaavioiden ajo:";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button testaaButton;
        private System.Windows.Forms.Button peruutaButton;
    }
}