namespace KaisaKaavio.Integraatio
{
    partial class LataaOnlineKilpailuPopup
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LataaOnlineKilpailuPopup));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.salasanaTextBox = new System.Windows.Forms.TextBox();
            this.naytaSalasanaCheckBox = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.kilpailutDataGridView = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.peruutaButton = new System.Windows.Forms.Button();
            this.pvmDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nimiDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kilpailutBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kilpailutDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kilpailutBindingSource)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.naytaSalasanaCheckBox);
            this.splitContainer1.Panel1.Controls.Add(this.salasanaTextBox);
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Panel1.ForeColor = System.Drawing.Color.White;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(821, 517);
            this.splitContainer1.SplitterDistance = 70;
            this.splitContainer1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.Color.DarkGreen;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(585, 32);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "Lataa kilpailu KaisaKaavio.fi serveriltä salasanan perusteella:";
            // 
            // salasanaTextBox
            // 
            this.salasanaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.salasanaTextBox.Enabled = false;
            this.salasanaTextBox.Location = new System.Drawing.Point(602, 3);
            this.salasanaTextBox.MaxLength = 8;
            this.salasanaTextBox.Name = "salasanaTextBox";
            this.salasanaTextBox.Size = new System.Drawing.Size(212, 29);
            this.salasanaTextBox.TabIndex = 2;
            this.salasanaTextBox.UseSystemPasswordChar = true;
            this.salasanaTextBox.TextChanged += new System.EventHandler(this.salasanaTextBox_TextChanged);
            // 
            // naytaSalasanaCheckBox
            // 
            this.naytaSalasanaCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.naytaSalasanaCheckBox.AutoSize = true;
            this.naytaSalasanaCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.naytaSalasanaCheckBox.Location = new System.Drawing.Point(686, 38);
            this.naytaSalasanaCheckBox.Name = "naytaSalasanaCheckBox";
            this.naytaSalasanaCheckBox.Size = new System.Drawing.Size(128, 20);
            this.naytaSalasanaCheckBox.TabIndex = 3;
            this.naytaSalasanaCheckBox.Text = "Näytä salasana?";
            this.naytaSalasanaCheckBox.UseVisualStyleBackColor = true;
            this.naytaSalasanaCheckBox.CheckedChanged += new System.EventHandler(this.naytaSalasanaCheckBox_CheckedChanged);
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
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.peruutaButton);
            this.splitContainer2.Panel2.Controls.Add(this.okButton);
            this.splitContainer2.Size = new System.Drawing.Size(821, 443);
            this.splitContainer2.SplitterDistance = 379;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.kilpailutDataGridView);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(811, 369);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Valitse kilpailu:";
            // 
            // kilpailutDataGridView
            // 
            this.kilpailutDataGridView.AllowUserToAddRows = false;
            this.kilpailutDataGridView.AllowUserToDeleteRows = false;
            this.kilpailutDataGridView.AllowUserToResizeColumns = false;
            this.kilpailutDataGridView.AllowUserToResizeRows = false;
            this.kilpailutDataGridView.AutoGenerateColumns = false;
            this.kilpailutDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.kilpailutDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pvmDataGridViewTextBoxColumn,
            this.nimiDataGridViewTextBoxColumn});
            this.kilpailutDataGridView.DataSource = this.kilpailutBindingSource;
            this.kilpailutDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kilpailutDataGridView.Location = new System.Drawing.Point(3, 18);
            this.kilpailutDataGridView.MultiSelect = false;
            this.kilpailutDataGridView.Name = "kilpailutDataGridView";
            this.kilpailutDataGridView.ReadOnly = true;
            this.kilpailutDataGridView.RowHeadersVisible = false;
            this.kilpailutDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.kilpailutDataGridView.Size = new System.Drawing.Size(805, 348);
            this.kilpailutDataGridView.TabIndex = 0;
            this.kilpailutDataGridView.SelectionChanged += new System.EventHandler(this.kilpailutDataGridView_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Voit asettaa kilpailulle salasanan \'Online-asetukset\' välilehdeltä";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.Color.LightGreen;
            this.okButton.Enabled = false;
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Image = global::KaisaKaavio.Properties.Resources.Ok;
            this.okButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.okButton.Location = new System.Drawing.Point(582, 3);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.okButton.Size = new System.Drawing.Size(232, 50);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Lataa kilpailu";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Visible = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // peruutaButton
            // 
            this.peruutaButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.peruutaButton.BackColor = System.Drawing.Color.LightCoral;
            this.peruutaButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.peruutaButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.peruutaButton.ForeColor = System.Drawing.Color.White;
            this.peruutaButton.Image = global::KaisaKaavio.Properties.Resources.Peruuta;
            this.peruutaButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.peruutaButton.Location = new System.Drawing.Point(3, 3);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.peruutaButton.Size = new System.Drawing.Size(189, 50);
            this.peruutaButton.TabIndex = 1;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.UseVisualStyleBackColor = false;
            this.peruutaButton.Click += new System.EventHandler(this.peruutaButton_Click);
            // 
            // pvmDataGridViewTextBoxColumn
            // 
            this.pvmDataGridViewTextBoxColumn.DataPropertyName = "Pvm";
            this.pvmDataGridViewTextBoxColumn.HeaderText = "Pvm";
            this.pvmDataGridViewTextBoxColumn.MinimumWidth = 110;
            this.pvmDataGridViewTextBoxColumn.Name = "pvmDataGridViewTextBoxColumn";
            this.pvmDataGridViewTextBoxColumn.ReadOnly = true;
            this.pvmDataGridViewTextBoxColumn.Width = 110;
            // 
            // nimiDataGridViewTextBoxColumn
            // 
            this.nimiDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nimiDataGridViewTextBoxColumn.DataPropertyName = "Nimi";
            this.nimiDataGridViewTextBoxColumn.HeaderText = "Nimi";
            this.nimiDataGridViewTextBoxColumn.Name = "nimiDataGridViewTextBoxColumn";
            this.nimiDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // kilpailutBindingSource
            // 
            this.kilpailutBindingSource.AllowNew = false;
            this.kilpailutBindingSource.DataSource = typeof(KaisaKaavio.Tyypit.KilpailuTietue);
            // 
            // LataaOnlineKilpailuPopup
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(821, 517);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(837, 556);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(837, 556);
            this.Name = "LataaOnlineKilpailuPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lataa kilpailu KaisaKaavio.fi sivustolta:";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.LataaOnlineKilpailuPopup_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kilpailutDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kilpailutBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox naytaSalasanaCheckBox;
        private System.Windows.Forms.TextBox salasanaTextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button peruutaButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView kilpailutDataGridView;
        private System.Windows.Forms.BindingSource kilpailutBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn pvmDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nimiDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label label1;
    }
}