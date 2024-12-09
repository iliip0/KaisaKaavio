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

        public TestiPopup()
        {
            InitializeComponent();
        }

        private void testaaButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
