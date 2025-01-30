using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Testaus
{
    public partial class TestiPopup : Form
    {
        public bool SatunnainenPeliJarjestys { get { return this.checkBox1.Checked; } }
        public int PoytienMaara { get { return (int)this.numericUpDown1.Value; } }

        public bool MonteCarloTestaus { get { return this.mcCheckBox.Checked; } }
        public int MonteCarloKisoja { get { return (int)this.mcKisojaNumericUpDown.Value; } }
        public int MonteCarloMinPelaajia { get { return (int)this.mcMinPelaajiaNnumericUpDown.Value; } }
        public int MonteCarloMaxPelaajia { get { return (int)this.mcMaxPelaajiaNumericUpDown.Value; } }

        public TestiPopup()
        {
            InitializeComponent();
        }

        private void testaaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void mcCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mcAsetuksetGroupBox.Visible = mcCheckBox.Checked;
        }
    }
}
