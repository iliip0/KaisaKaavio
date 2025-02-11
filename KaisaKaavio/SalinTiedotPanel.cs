using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    public partial class SalinTiedotPanel : UserControl
    {
        public SalinTiedotPanel()
        {
            InitializeComponent();

            rooliDataGridViewTextBoxColumn.DataSource = Enum.GetValues(typeof(ToimitsijanRooli));
        }

        public void EndEditing()
        {
            try
            {
                if (this.poydatDataGridView.IsCurrentCellInEditMode)
                {
                    this.poydatDataGridView.EndEdit();
                }

                if (this.saliLinkitDataGridView.IsCurrentCellInEditMode)
                {
                    this.saliLinkitDataGridView.EndEdit();
                }

                if (this.toimitsijatDataGridView.IsCurrentCellInEditMode)
                {
                    this.toimitsijatDataGridView.EndEdit();
                }
            }
            catch 
            {
            }
        }
    }
}
