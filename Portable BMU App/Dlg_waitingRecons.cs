using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portable_BMU_App
{
    public partial class Dlg_waitingRecons : Form
    {
        public delegate void DelegateCancelButtonClick();
        public event DelegateCancelButtonClick CancelButonClick;

        public Dlg_waitingRecons()
        {
            InitializeComponent();
            this.progressBarWaiting.Style = ProgressBarStyle.Marquee;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dialogResult = MessageBox.Show("Are you sure to cancel?", "Cancel Current Process", messButton);

            if (dialogResult == DialogResult.OK)
            {
                if (CancelButonClick != null)
                {
                    CancelButonClick();
                }
            }
            else
            {
                return;
            }

        }
    }
}
