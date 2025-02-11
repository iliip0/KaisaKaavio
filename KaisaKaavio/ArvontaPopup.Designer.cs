namespace KaisaKaavio
{
    partial class ArvontaPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArvontaPopup));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.okButton = new System.Windows.Forms.Button();
            this.peruutaButton = new System.Windows.Forms.Button();
            this.vaakaSplitContainer = new System.Windows.Forms.SplitContainer();
            this.pelaajatSplitContainer = new System.Windows.Forms.SplitContainer();
            this.paikatSplitContainer = new System.Windows.Forms.SplitContainer();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pelaajatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.paikatRichTextBox = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.virheRichTextBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vaakaSplitContainer)).BeginInit();
            this.vaakaSplitContainer.Panel1.SuspendLayout();
            this.vaakaSplitContainer.Panel2.SuspendLayout();
            this.vaakaSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pelaajatSplitContainer)).BeginInit();
            this.pelaajatSplitContainer.Panel1.SuspendLayout();
            this.pelaajatSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paikatSplitContainer)).BeginInit();
            this.paikatSplitContainer.Panel1.SuspendLayout();
            this.paikatSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.DarkGreen;
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Panel1.ForeColor = System.Drawing.Color.White;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(745, 514);
            this.splitContainer1.SplitterDistance = 99;
            this.splitContainer1.TabIndex = 0;
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
            this.splitContainer2.Panel1.Controls.Add(this.vaakaSplitContainer);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.virheRichTextBox);
            this.splitContainer2.Panel2.Controls.Add(this.peruutaButton);
            this.splitContainer2.Panel2.Controls.Add(this.okButton);
            this.splitContainer2.Panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer2.Size = new System.Drawing.Size(745, 411);
            this.splitContainer2.SplitterDistance = 345;
            this.splitContainer2.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Image = global::KaisaKaavio.Properties.Resources.Ok;
            this.okButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.okButton.Location = new System.Drawing.Point(536, 3);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.okButton.Size = new System.Drawing.Size(202, 52);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Arvo kaavio!";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // peruutaButton
            // 
            this.peruutaButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.peruutaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.peruutaButton.Image = global::KaisaKaavio.Properties.Resources.Peruuta;
            this.peruutaButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.peruutaButton.Location = new System.Drawing.Point(3, 3);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.peruutaButton.Size = new System.Drawing.Size(171, 52);
            this.peruutaButton.TabIndex = 1;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.UseVisualStyleBackColor = true;
            // 
            // vaakaSplitContainer
            // 
            this.vaakaSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.vaakaSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vaakaSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.vaakaSplitContainer.Name = "vaakaSplitContainer";
            // 
            // vaakaSplitContainer.Panel1
            // 
            this.vaakaSplitContainer.Panel1.Controls.Add(this.pelaajatSplitContainer);
            // 
            // vaakaSplitContainer.Panel2
            // 
            this.vaakaSplitContainer.Panel2.Controls.Add(this.paikatSplitContainer);
            this.vaakaSplitContainer.Size = new System.Drawing.Size(745, 345);
            this.vaakaSplitContainer.SplitterDistance = 382;
            this.vaakaSplitContainer.TabIndex = 0;
            // 
            // pelaajatSplitContainer
            // 
            this.pelaajatSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pelaajatSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pelaajatSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.pelaajatSplitContainer.IsSplitterFixed = true;
            this.pelaajatSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.pelaajatSplitContainer.Name = "pelaajatSplitContainer";
            this.pelaajatSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // pelaajatSplitContainer.Panel1
            // 
            this.pelaajatSplitContainer.Panel1.BackColor = System.Drawing.Color.PaleTurquoise;
            this.pelaajatSplitContainer.Panel1.Controls.Add(this.pelaajatRichTextBox);
            this.pelaajatSplitContainer.Panel1.Controls.Add(this.pictureBox2);
            this.pelaajatSplitContainer.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pelaajatSplitContainer.Size = new System.Drawing.Size(382, 345);
            this.pelaajatSplitContainer.SplitterDistance = 63;
            this.pelaajatSplitContainer.TabIndex = 0;
            // 
            // paikatSplitContainer
            // 
            this.paikatSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.paikatSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paikatSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.paikatSplitContainer.IsSplitterFixed = true;
            this.paikatSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.paikatSplitContainer.Name = "paikatSplitContainer";
            this.paikatSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // paikatSplitContainer.Panel1
            // 
            this.paikatSplitContainer.Panel1.BackColor = System.Drawing.Color.Lavender;
            this.paikatSplitContainer.Panel1.Controls.Add(this.paikatRichTextBox);
            this.paikatSplitContainer.Panel1.Controls.Add(this.pictureBox3);
            this.paikatSplitContainer.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paikatSplitContainer.Size = new System.Drawing.Size(359, 345);
            this.paikatSplitContainer.SplitterDistance = 63;
            this.paikatSplitContainer.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::KaisaKaavio.Properties.Resources.Osallistujat32;
            this.pictureBox2.Location = new System.Drawing.Point(14, 9);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(50, 42);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::KaisaKaavio.Properties.Resources.Pelipaikat;
            this.pictureBox3.Location = new System.Drawing.Point(9, 9);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(45, 42);
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // pelaajatRichTextBox
            // 
            this.pelaajatRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pelaajatRichTextBox.BackColor = System.Drawing.Color.PaleTurquoise;
            this.pelaajatRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pelaajatRichTextBox.Location = new System.Drawing.Point(74, 3);
            this.pelaajatRichTextBox.Name = "pelaajatRichTextBox";
            this.pelaajatRichTextBox.Size = new System.Drawing.Size(301, 53);
            this.pelaajatRichTextBox.TabIndex = 1;
            this.pelaajatRichTextBox.Text = "Pelaajien sijoittaminen sekä ennalta määrätyt pelipaikat:";
            // 
            // paikatRichTextBox
            // 
            this.paikatRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.paikatRichTextBox.BackColor = System.Drawing.Color.Lavender;
            this.paikatRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.paikatRichTextBox.Location = new System.Drawing.Point(71, 3);
            this.paikatRichTextBox.Name = "paikatRichTextBox";
            this.paikatRichTextBox.Size = new System.Drawing.Size(281, 53);
            this.paikatRichTextBox.TabIndex = 1;
            this.paikatRichTextBox.Text = "Pelipaikkojen pöytämäärät sekä pelaajamäärät:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.Color.DarkGreen;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(59, 10);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(672, 73);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KaisaKaavio.Properties.Resources.Tiedoksi;
            this.pictureBox1.Location = new System.Drawing.Point(3, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(54, 50);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // virheRichTextBox
            // 
            this.virheRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.virheRichTextBox.BackColor = System.Drawing.Color.Salmon;
            this.virheRichTextBox.ForeColor = System.Drawing.Color.White;
            this.virheRichTextBox.Location = new System.Drawing.Point(181, 4);
            this.virheRichTextBox.Name = "virheRichTextBox";
            this.virheRichTextBox.Size = new System.Drawing.Size(349, 51);
            this.virheRichTextBox.TabIndex = 2;
            this.virheRichTextBox.Text = "Virhe!";
            this.virheRichTextBox.Visible = false;
            // 
            // ArvontaPopup
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(745, 514);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArvontaPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Arvo kaavio";
            this.TopMost = true;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.vaakaSplitContainer.Panel1.ResumeLayout(false);
            this.vaakaSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vaakaSplitContainer)).EndInit();
            this.vaakaSplitContainer.ResumeLayout(false);
            this.pelaajatSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pelaajatSplitContainer)).EndInit();
            this.pelaajatSplitContainer.ResumeLayout(false);
            this.paikatSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.paikatSplitContainer)).EndInit();
            this.paikatSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button peruutaButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.SplitContainer vaakaSplitContainer;
        private System.Windows.Forms.SplitContainer pelaajatSplitContainer;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.SplitContainer paikatSplitContainer;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.RichTextBox pelaajatRichTextBox;
        private System.Windows.Forms.RichTextBox paikatRichTextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox virheRichTextBox;
    }
}