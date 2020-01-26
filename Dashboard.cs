using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiFaceRec
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void btnNewRegi_Click(object sender, EventArgs e)
        {
            OpenAccount OC = new OpenAccount();
            OC.MdiParent = this;
            OC.TopLevel = false;
            OC.WindowState = FormWindowState.Maximized;
            this.panelBody.Controls.Add(OC);
            OC.Show();
        }

        private void btnTransferAmount_Click(object sender, EventArgs e)
        {
            FacePayTransaction OC = new FacePayTransaction();
            OC.MdiParent = this;
            OC.TopLevel = false;
            OC.WindowState = FormWindowState.Maximized;
            this.panelBody.Controls.Add(OC);
            OC.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            lblName.Text = Login.FirstName + " " + Login.LastName;
            lblBankType.Text = Login.AccountType.ToString();
            lblIFSC.Text = Login.IFSC.ToString();
            lblAccountNo.Text = Login.AccountNumber.ToString();
            if(Login.LoginType.ToString()=="Admin")
            {
                lblLoginType.Text = "Admin Dashboard";
                btnNewRegi.Enabled = true;
                btnTransferAmount.Enabled = false;
                btnFacePay.Enabled = false;
            }
            else
            {
                lblLoginType.Text = "User Dashboard";
                btnNewRegi.Enabled = false;
                btnTransferAmount.Enabled = true;
                btnFacePay.Enabled = true;
            }
        }

        private void btnFacePay_Click(object sender, EventArgs e)
        {
            FacePay OC = new FacePay();
            OC.MdiParent = this;
            OC.TopLevel = false;
            OC.WindowState = FormWindowState.Maximized;
            this.panelBody.Controls.Add(OC);
            OC.Show();
        }

        private void lblLoginType_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelBody_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelBody_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
