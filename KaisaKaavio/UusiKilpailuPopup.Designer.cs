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
            this.label1 = new System.Windows.Forms.Label();
            this.kilpailunTyyppiComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.kilpailunNimiTextBox = new System.Windows.Forms.TextBox();
            this.rankingCheckBox = new System.Windows.Forms.CheckBox();
            this.rankingComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.uusiKilpailuLajiComboBox = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.kaavioComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.kilpaSarjaComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.kilpasarjaLabel = new System.Windows.Forms.Label();
            this.alkamisAikaDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.lajiSplitContainer = new System.Windows.Forms.SplitContainer();
            this.alaLajiComboBox = new System.Windows.Forms.ComboBox();
            this.lajiPictureBox = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.peliAikaCheckBox = new System.Windows.Forms.CheckBox();
            this.peliaikaLabel = new System.Windows.Forms.Label();
            this.peliAikaNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.tavoiteNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tavoiteLabel = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.uusiKilpailuButton = new System.Windows.Forms.Button();
            this.peruutaButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lajiSplitContainer)).BeginInit();
            this.lajiSplitContainer.Panel1.SuspendLayout();
            this.lajiSplitContainer.Panel2.SuspendLayout();
            this.lajiSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lajiPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peliAikaNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tavoiteNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(311, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 24);
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
            "Viikkokilpailu",
            "Avoin turnaus",
            "SBiL RG osakilpailu",
            "SBiL SM kilpailu"});
            this.kilpailunTyyppiComboBox.Location = new System.Drawing.Point(476, 45);
            this.kilpailunTyyppiComboBox.Name = "kilpailunTyyppiComboBox";
            this.kilpailunTyyppiComboBox.Size = new System.Drawing.Size(479, 32);
            this.kilpailunTyyppiComboBox.TabIndex = 3;
            this.kilpailunTyyppiComboBox.SelectedIndexChanged += new System.EventHandler(this.kilpailunTyyppiComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(311, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Kilpailun nimi:";
            // 
            // kilpailunNimiTextBox
            // 
            this.kilpailunNimiTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kilpailunNimiTextBox.Location = new System.Drawing.Point(476, 159);
            this.kilpailunNimiTextBox.Name = "kilpailunNimiTextBox";
            this.kilpailunNimiTextBox.Size = new System.Drawing.Size(479, 29);
            this.kilpailunNimiTextBox.TabIndex = 5;
            this.kilpailunNimiTextBox.Text = "Kaisakilpailu";
            this.kilpailunNimiTextBox.TextChanged += new System.EventHandler(this.kilpailunNimiTextBox_TextChanged);
            this.kilpailunNimiTextBox.Validated += new System.EventHandler(this.kilpailunNimiTextBox_Validated);
            // 
            // rankingCheckBox
            // 
            this.rankingCheckBox.AutoSize = true;
            this.rankingCheckBox.Location = new System.Drawing.Point(295, 14);
            this.rankingCheckBox.Name = "rankingCheckBox";
            this.rankingCheckBox.Size = new System.Drawing.Size(338, 28);
            this.rankingCheckBox.TabIndex = 6;
            this.rankingCheckBox.Text = "Liitä kilpailu viikkokisarankingsarjaan?";
            this.rankingCheckBox.UseVisualStyleBackColor = true;
            this.rankingCheckBox.CheckedChanged += new System.EventHandler(this.rankingCheckBox_CheckedChanged);
            // 
            // rankingComboBox
            // 
            this.rankingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rankingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rankingComboBox.FormattingEnabled = true;
            this.rankingComboBox.Items.AddRange(new object[] {
            "Kuukausittainen ranking sarja",
            "Kolmen kuukauden välein vaihtuva sarja",
            "Puolen vuoden välein vaihtuva sarja",
            "Vuoden kestävä sarja"});
            this.rankingComboBox.Location = new System.Drawing.Point(639, 12);
            this.rankingComboBox.Name = "rankingComboBox";
            this.rankingComboBox.Size = new System.Drawing.Size(316, 32);
            this.rankingComboBox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(311, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 24);
            this.label3.TabIndex = 9;
            this.label3.Text = "Laji:";
            // 
            // uusiKilpailuLajiComboBox
            // 
            this.uusiKilpailuLajiComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uusiKilpailuLajiComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uusiKilpailuLajiComboBox.FormattingEnabled = true;
            this.uusiKilpailuLajiComboBox.Location = new System.Drawing.Point(0, 0);
            this.uusiKilpailuLajiComboBox.Name = "uusiKilpailuLajiComboBox";
            this.uusiKilpailuLajiComboBox.Size = new System.Drawing.Size(206, 32);
            this.uusiKilpailuLajiComboBox.TabIndex = 10;
            this.uusiKilpailuLajiComboBox.SelectionChangeCommitted += new System.EventHandler(this.uusiKilpailuLajiComboBox_SelectionChangeCommitted);
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
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.uusiKilpailuButton);
            this.splitContainer1.Panel2.Controls.Add(this.peruutaButton);
            this.splitContainer1.Panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.Size = new System.Drawing.Size(962, 450);
            this.splitContainer1.SplitterDistance = 358;
            this.splitContainer1.TabIndex = 11;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer2.Panel2.Controls.Add(this.rankingCheckBox);
            this.splitContainer2.Panel2.Controls.Add(this.rankingComboBox);
            this.splitContainer2.Panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer2.Size = new System.Drawing.Size(962, 358);
            this.splitContainer2.SplitterDistance = 296;
            this.splitContainer2.TabIndex = 9;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer3.Panel1.Controls.Add(this.kaavioComboBox);
            this.splitContainer3.Panel1.Controls.Add(this.label6);
            this.splitContainer3.Panel1.Controls.Add(this.kilpaSarjaComboBox);
            this.splitContainer3.Panel1.Controls.Add(this.label3);
            this.splitContainer3.Panel1.Controls.Add(this.label1);
            this.splitContainer3.Panel1.Controls.Add(this.label4);
            this.splitContainer3.Panel1.Controls.Add(this.kilpasarjaLabel);
            this.splitContainer3.Panel1.Controls.Add(this.label2);
            this.splitContainer3.Panel1.Controls.Add(this.kilpailunTyyppiComboBox);
            this.splitContainer3.Panel1.Controls.Add(this.kilpailunNimiTextBox);
            this.splitContainer3.Panel1.Controls.Add(this.alkamisAikaDateTimePicker);
            this.splitContainer3.Panel1.Controls.Add(this.lajiSplitContainer);
            this.splitContainer3.Panel1.Controls.Add(this.lajiPictureBox);
            this.splitContainer3.Panel1.ForeColor = System.Drawing.Color.White;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pictureBox3);
            this.splitContainer3.Panel2.Controls.Add(this.peliAikaCheckBox);
            this.splitContainer3.Panel2.Controls.Add(this.peliaikaLabel);
            this.splitContainer3.Panel2.Controls.Add(this.peliAikaNumericUpDown);
            this.splitContainer3.Panel2.Controls.Add(this.label5);
            this.splitContainer3.Panel2.Controls.Add(this.tavoiteNumericUpDown);
            this.splitContainer3.Panel2.Controls.Add(this.tavoiteLabel);
            this.splitContainer3.Size = new System.Drawing.Size(962, 296);
            this.splitContainer3.SplitterDistance = 202;
            this.splitContainer3.TabIndex = 24;
            // 
            // kaavioComboBox
            // 
            this.kaavioComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kaavioComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.kaavioComboBox.FormattingEnabled = true;
            this.kaavioComboBox.Items.AddRange(new object[] {
            "Tuplakaavio loppuun asti",
            "Pudotuspelit 2. kierroksesta alkaen",
            "Pudotuspelit 3. kierroksesta alkaen"});
            this.kaavioComboBox.Location = new System.Drawing.Point(476, 83);
            this.kaavioComboBox.Name = "kaavioComboBox";
            this.kaavioComboBox.Size = new System.Drawing.Size(479, 32);
            this.kaavioComboBox.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(311, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 24);
            this.label6.TabIndex = 20;
            this.label6.Text = "Kaavio:";
            // 
            // kilpaSarjaComboBox
            // 
            this.kilpaSarjaComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kilpaSarjaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.kilpaSarjaComboBox.FormattingEnabled = true;
            this.kilpaSarjaComboBox.Location = new System.Drawing.Point(708, 121);
            this.kilpaSarjaComboBox.Name = "kilpaSarjaComboBox";
            this.kilpaSarjaComboBox.Size = new System.Drawing.Size(247, 32);
            this.kilpaSarjaComboBox.TabIndex = 15;
            this.kilpaSarjaComboBox.SelectedIndexChanged += new System.EventHandler(this.kilpaSarjaComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(311, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 24);
            this.label4.TabIndex = 12;
            this.label4.Text = "Alkamispäivä:";
            // 
            // kilpasarjaLabel
            // 
            this.kilpasarjaLabel.AutoSize = true;
            this.kilpasarjaLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.kilpasarjaLabel.Location = new System.Drawing.Point(607, 123);
            this.kilpasarjaLabel.Name = "kilpasarjaLabel";
            this.kilpasarjaLabel.Size = new System.Drawing.Size(95, 24);
            this.kilpasarjaLabel.TabIndex = 14;
            this.kilpasarjaLabel.Text = "Kilpasarja:";
            // 
            // alkamisAikaDateTimePicker
            // 
            this.alkamisAikaDateTimePicker.CustomFormat = "d.M.yyyy";
            this.alkamisAikaDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.alkamisAikaDateTimePicker.Location = new System.Drawing.Point(476, 122);
            this.alkamisAikaDateTimePicker.Name = "alkamisAikaDateTimePicker";
            this.alkamisAikaDateTimePicker.Size = new System.Drawing.Size(111, 29);
            this.alkamisAikaDateTimePicker.TabIndex = 13;
            this.alkamisAikaDateTimePicker.ValueChanged += new System.EventHandler(this.alkamisAikaDateTimePicker_ValueChanged);
            // 
            // lajiSplitContainer
            // 
            this.lajiSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lajiSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.lajiSplitContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lajiSplitContainer.IsSplitterFixed = true;
            this.lajiSplitContainer.Location = new System.Drawing.Point(476, 7);
            this.lajiSplitContainer.Name = "lajiSplitContainer";
            // 
            // lajiSplitContainer.Panel1
            // 
            this.lajiSplitContainer.Panel1.Controls.Add(this.uusiKilpailuLajiComboBox);
            // 
            // lajiSplitContainer.Panel2
            // 
            this.lajiSplitContainer.Panel2.Controls.Add(this.alaLajiComboBox);
            this.lajiSplitContainer.Size = new System.Drawing.Size(479, 35);
            this.lajiSplitContainer.SplitterDistance = 206;
            this.lajiSplitContainer.TabIndex = 16;
            // 
            // alaLajiComboBox
            // 
            this.alaLajiComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alaLajiComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.alaLajiComboBox.FormattingEnabled = true;
            this.alaLajiComboBox.Location = new System.Drawing.Point(0, 0);
            this.alaLajiComboBox.Name = "alaLajiComboBox";
            this.alaLajiComboBox.Size = new System.Drawing.Size(269, 32);
            this.alaLajiComboBox.TabIndex = 0;
            // 
            // lajiPictureBox
            // 
            this.lajiPictureBox.BackColor = System.Drawing.Color.Black;
            this.lajiPictureBox.BackgroundImage = global::KaisaKaavio.Properties.Resources.KaisaMV;
            this.lajiPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.lajiPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lajiPictureBox.Location = new System.Drawing.Point(3, 3);
            this.lajiPictureBox.Name = "lajiPictureBox";
            this.lajiPictureBox.Size = new System.Drawing.Size(288, 192);
            this.lajiPictureBox.TabIndex = 11;
            this.lajiPictureBox.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::KaisaKaavio.Properties.Resources.KilpailuInfo32;
            this.pictureBox3.Location = new System.Drawing.Point(10, 11);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(41, 42);
            this.pictureBox3.TabIndex = 24;
            this.pictureBox3.TabStop = false;
            // 
            // peliAikaCheckBox
            // 
            this.peliAikaCheckBox.AutoSize = true;
            this.peliAikaCheckBox.Location = new System.Drawing.Point(295, 47);
            this.peliAikaCheckBox.Name = "peliAikaCheckBox";
            this.peliAikaCheckBox.Size = new System.Drawing.Size(157, 28);
            this.peliAikaCheckBox.TabIndex = 23;
            this.peliAikaCheckBox.Text = "Peliaika rajattu?";
            this.peliAikaCheckBox.UseVisualStyleBackColor = true;
            this.peliAikaCheckBox.CheckedChanged += new System.EventHandler(this.peliAikaCheckBox_CheckedChanged);
            // 
            // peliaikaLabel
            // 
            this.peliaikaLabel.AutoSize = true;
            this.peliaikaLabel.Location = new System.Drawing.Point(793, 48);
            this.peliaikaLabel.Name = "peliaikaLabel";
            this.peliaikaLabel.Size = new System.Drawing.Size(85, 24);
            this.peliaikaLabel.TabIndex = 22;
            this.peliaikaLabel.Text = "minuuttia";
            // 
            // peliAikaNumericUpDown
            // 
            this.peliAikaNumericUpDown.Location = new System.Drawing.Point(708, 46);
            this.peliAikaNumericUpDown.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this.peliAikaNumericUpDown.Name = "peliAikaNumericUpDown";
            this.peliAikaNumericUpDown.Size = new System.Drawing.Size(79, 29);
            this.peliAikaNumericUpDown.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(311, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 24);
            this.label5.TabIndex = 17;
            this.label5.Text = "Pelataan:";
            // 
            // tavoiteNumericUpDown
            // 
            this.tavoiteNumericUpDown.Location = new System.Drawing.Point(708, 9);
            this.tavoiteNumericUpDown.Name = "tavoiteNumericUpDown";
            this.tavoiteNumericUpDown.Size = new System.Drawing.Size(79, 29);
            this.tavoiteNumericUpDown.TabIndex = 18;
            // 
            // tavoiteLabel
            // 
            this.tavoiteLabel.AutoSize = true;
            this.tavoiteLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tavoiteLabel.Location = new System.Drawing.Point(793, 11);
            this.tavoiteLabel.Name = "tavoiteLabel";
            this.tavoiteLabel.Size = new System.Drawing.Size(75, 24);
            this.tavoiteLabel.TabIndex = 19;
            this.tavoiteLabel.Text = "voittoon";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::KaisaKaavio.Properties.Resources.Ranking32;
            this.pictureBox2.Location = new System.Drawing.Point(10, 9);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(41, 38);
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // uusiKilpailuButton
            // 
            this.uusiKilpailuButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uusiKilpailuButton.Image = global::KaisaKaavio.Properties.Resources.Ok;
            this.uusiKilpailuButton.Location = new System.Drawing.Point(691, 5);
            this.uusiKilpailuButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uusiKilpailuButton.Name = "uusiKilpailuButton";
            this.uusiKilpailuButton.Size = new System.Drawing.Size(263, 74);
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
            this.peruutaButton.Location = new System.Drawing.Point(4, 5);
            this.peruutaButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.peruutaButton.Name = "peruutaButton";
            this.peruutaButton.Size = new System.Drawing.Size(186, 74);
            this.peruutaButton.TabIndex = 1;
            this.peruutaButton.Text = "Peruuta";
            this.peruutaButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.peruutaButton.UseVisualStyleBackColor = true;
            this.peruutaButton.Click += new System.EventHandler(this.peruutaButton_Click);
            // 
            // UusiKilpailuPopup
            // 
            this.AcceptButton = this.uusiKilpailuButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.peruutaButton;
            this.ClientSize = new System.Drawing.Size(962, 450);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(982, 493);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(982, 493);
            this.Name = "UusiKilpailuPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Luo uusi kilpailu";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.UusiKilpailuPopup_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.lajiSplitContainer.Panel1.ResumeLayout(false);
            this.lajiSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lajiSplitContainer)).EndInit();
            this.lajiSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lajiPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peliAikaNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tavoiteNumericUpDown)).EndInit();
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
        private System.Windows.Forms.ComboBox rankingComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox uusiKilpailuLajiComboBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DateTimePicker alkamisAikaDateTimePicker;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox lajiPictureBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox kilpaSarjaComboBox;
        private System.Windows.Forms.Label kilpasarjaLabel;
        private System.Windows.Forms.SplitContainer lajiSplitContainer;
        private System.Windows.Forms.ComboBox alaLajiComboBox;
        private System.Windows.Forms.Label tavoiteLabel;
        private System.Windows.Forms.NumericUpDown tavoiteNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.CheckBox peliAikaCheckBox;
        private System.Windows.Forms.Label peliaikaLabel;
        private System.Windows.Forms.NumericUpDown peliAikaNumericUpDown;
        private System.Windows.Forms.ComboBox kaavioComboBox;
        private System.Windows.Forms.Label label6;
    }
}