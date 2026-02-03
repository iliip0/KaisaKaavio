namespace KaisaKaavio.ManuaalinenHaku
{
    partial class ManuaalinenHaku
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManuaalinenHaku));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.haeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.vastustajaComboBox = new System.Windows.Forms.ComboBox();
            this.hakijaComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.haetutPelitDataGridView = new System.Windows.Forms.DataGridView();
            this.pelaajanNimi1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pelaajanNimi2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tilanne = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.poistaPeliColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.peliBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tilanneLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.haetutPelitDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peliBindingSource)).BeginInit();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tilanneLabel);
            this.splitContainer1.Panel2.Controls.Add(this.okButton);
            this.splitContainer1.Size = new System.Drawing.Size(936, 499);
            this.splitContainer1.SplitterDistance = 426;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.haeButton);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.vastustajaComboBox);
            this.groupBox2.Controls.Add(this.hakijaComboBox);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 317);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(912, 102);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Hae uusi peli:";
            // 
            // haeButton
            // 
            this.haeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.haeButton.BackColor = System.Drawing.Color.LightGreen;
            this.haeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.haeButton.Location = new System.Drawing.Point(817, 25);
            this.haeButton.Name = "haeButton";
            this.haeButton.Size = new System.Drawing.Size(89, 71);
            this.haeButton.TabIndex = 5;
            this.haeButton.Text = "Hae!";
            this.haeButton.UseVisualStyleBackColor = false;
            this.haeButton.Click += new System.EventHandler(this.haeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(26, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Vastustaja:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(58, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Hakija:";
            // 
            // vastustajaComboBox
            // 
            this.vastustajaComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vastustajaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.vastustajaComboBox.FormattingEnabled = true;
            this.vastustajaComboBox.Location = new System.Drawing.Point(121, 62);
            this.vastustajaComboBox.Name = "vastustajaComboBox";
            this.vastustajaComboBox.Size = new System.Drawing.Size(690, 28);
            this.vastustajaComboBox.TabIndex = 1;
            this.vastustajaComboBox.SelectedIndexChanged += new System.EventHandler(this.vastustajaComboBox_SelectedIndexChanged);
            // 
            // hakijaComboBox
            // 
            this.hakijaComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hakijaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hakijaComboBox.FormattingEnabled = true;
            this.hakijaComboBox.Location = new System.Drawing.Point(121, 28);
            this.hakijaComboBox.Name = "hakijaComboBox";
            this.hakijaComboBox.Size = new System.Drawing.Size(690, 28);
            this.hakijaComboBox.TabIndex = 0;
            this.hakijaComboBox.SelectedIndexChanged += new System.EventHandler(this.hakijaComboBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.haetutPelitDataGridView);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(16);
            this.groupBox1.Size = new System.Drawing.Size(912, 301);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Haetut pelit:";
            // 
            // haetutPelitDataGridView
            // 
            this.haetutPelitDataGridView.AllowUserToAddRows = false;
            this.haetutPelitDataGridView.AllowUserToDeleteRows = false;
            this.haetutPelitDataGridView.AllowUserToResizeColumns = false;
            this.haetutPelitDataGridView.AllowUserToResizeRows = false;
            this.haetutPelitDataGridView.AutoGenerateColumns = false;
            this.haetutPelitDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.haetutPelitDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.haetutPelitDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pelaajanNimi1DataGridViewTextBoxColumn,
            this.pelaajanNimi2DataGridViewTextBoxColumn,
            this.Tilanne,
            this.poistaPeliColumn});
            this.haetutPelitDataGridView.DataSource = this.peliBindingSource;
            this.haetutPelitDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.haetutPelitDataGridView.Location = new System.Drawing.Point(16, 35);
            this.haetutPelitDataGridView.Name = "haetutPelitDataGridView";
            this.haetutPelitDataGridView.ReadOnly = true;
            this.haetutPelitDataGridView.RowHeadersVisible = false;
            this.haetutPelitDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.haetutPelitDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.haetutPelitDataGridView.Size = new System.Drawing.Size(880, 250);
            this.haetutPelitDataGridView.TabIndex = 0;
            this.haetutPelitDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.haetutPelitDataGridView_CellContentClick);
            // 
            // pelaajanNimi1DataGridViewTextBoxColumn
            // 
            this.pelaajanNimi1DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.pelaajanNimi1DataGridViewTextBoxColumn.DataPropertyName = "PelaajanNimi1";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.pelaajanNimi1DataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.pelaajanNimi1DataGridViewTextBoxColumn.HeaderText = "Hakija";
            this.pelaajanNimi1DataGridViewTextBoxColumn.Name = "pelaajanNimi1DataGridViewTextBoxColumn";
            this.pelaajanNimi1DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pelaajanNimi2DataGridViewTextBoxColumn
            // 
            this.pelaajanNimi2DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.pelaajanNimi2DataGridViewTextBoxColumn.DataPropertyName = "PelaajanNimi2";
            this.pelaajanNimi2DataGridViewTextBoxColumn.HeaderText = "Vastustaja";
            this.pelaajanNimi2DataGridViewTextBoxColumn.Name = "pelaajanNimi2DataGridViewTextBoxColumn";
            this.pelaajanNimi2DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Tilanne
            // 
            this.Tilanne.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Tilanne.DataPropertyName = "TilanneTekstiManuaalisessaHaussa";
            this.Tilanne.HeaderText = "Pelin tilanne";
            this.Tilanne.MinimumWidth = 256;
            this.Tilanne.Name = "Tilanne";
            this.Tilanne.ReadOnly = true;
            this.Tilanne.Width = 256;
            // 
            // poistaPeliColumn
            // 
            this.poistaPeliColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.poistaPeliColumn.HeaderText = "";
            this.poistaPeliColumn.MinimumWidth = 128;
            this.poistaPeliColumn.Name = "poistaPeliColumn";
            this.poistaPeliColumn.ReadOnly = true;
            this.poistaPeliColumn.Text = "Poista";
            this.poistaPeliColumn.ToolTipText = "Peruuta tämä haku";
            this.poistaPeliColumn.UseColumnTextForButtonValue = true;
            this.poistaPeliColumn.Width = 128;
            // 
            // peliBindingSource
            // 
            this.peliBindingSource.DataSource = typeof(KaisaKaavio.Peli);
            // 
            // tilanneLabel
            // 
            this.tilanneLabel.AutoSize = true;
            this.tilanneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tilanneLabel.Location = new System.Drawing.Point(22, 25);
            this.tilanneLabel.Name = "tilanneLabel";
            this.tilanneLabel.Size = new System.Drawing.Size(57, 20);
            this.tilanneLabel.TabIndex = 1;
            this.tilanneLabel.Text = "label3";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(748, 14);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(174, 41);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Sulje";
            this.okButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Tilanne";
            this.dataGridViewTextBoxColumn1.HeaderText = "Pelin tilanne";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 256;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 256;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Tilanne";
            this.dataGridViewTextBoxColumn2.HeaderText = "Pelin tilanne";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 256;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 256;
            // 
            // ManuaalinenHaku
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 499);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(763, 479);
            this.Name = "ManuaalinenHaku";
            this.Text = "Manuaalinen pelien haku";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.haetutPelitDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peliBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button haeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox vastustajaComboBox;
        private System.Windows.Forms.ComboBox hakijaComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView haetutPelitDataGridView;
        private System.Windows.Forms.BindingSource peliBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn pelaajanNimi1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pelaajanNimi2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tilanne;
        private System.Windows.Forms.DataGridViewButtonColumn poistaPeliColumn;
        private System.Windows.Forms.Label tilanneLabel;
    }
}