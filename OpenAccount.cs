using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace MultiFaceRec
{
    public partial class OpenAccount : Form
    {
        public static string FullName = string.Empty;
        public static string AccountNo = string.Empty;
        public static string IFSC = string.Empty;
        public OpenAccount()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            SqlConnection Conn = new SqlConnection(@"Data Source=DESKTOP-8MTP45M\MSSQLSERVER01;Initial Catalog=IDEOLOGY_BANK;Integrated Security=True");
            
            StringBuilder sqlstr = new StringBuilder();
            
            sqlstr.Append("Insert into ACCOUNT_REGISTRATION");
            sqlstr.Append(" values ('"+txtFName.Text+ "','"+txtMiddleName.Text+ "','"+txtLName.Text+ "','"+ Convert.ToDateTime(dtpBirthDate.Text)+ "','"+cmbGender.SelectedItem.ToString()+"',");
            sqlstr.Append(" '"+txtAdd1.Text+ "','"+txtAdd2.Text+ "','"+txtarea.Text+ "','"+txtcity.Text+ "','"+txtTahsil.Text+ "','"+txtDistrict.Text+ "','"+txtState.Text+ "','"+txtCountry.Text+"', ");
            sqlstr.Append(" '" + txtTelphone.Text + "','" + txtMobile1.Text + "','" + txtMobile2.Text + "','" + txtEmail.Text + "', ");
            sqlstr.Append(" '" + cmbAccountType.SelectedItem.ToString() + "','" + txtNominee.Text + "','" + txtAdhar.Text + "','" + txtPAN.Text + "','" + txtTransferLimit.Text + "','" + txtAccountNo.Text + "','" + DateTime.Today.ToShortDateString() + "','" + "Y" + "', ");
            sqlstr.Append(" '" + "N" + "','" + txtIFSC.Text + "','" + txtBranchDetails.Text + "' )");
            Conn.Open();

            SqlCommand cmd = new SqlCommand(sqlstr.ToString(),Conn);
            cmd.CommandType = CommandType.Text;
            int i = cmd.ExecuteNonQuery();
            if (i == 1)
            {
                int RegID = 0;
                string SQL = "select ID from ACCOUNT_REGISTRATION where First_Name='" + txtFName.Text + "' and Middle_Name='" + txtMiddleName.Text + "' and Last_Name='" + txtLName.Text + "'";
                SqlCommand cmd1 = new SqlCommand(SQL,Conn);
                cmd1.CommandType = CommandType.Text;
                SqlDataReader DR = cmd1.ExecuteReader();
                if (DR.Read())
                {
                    RegID =Convert.ToInt32(DR["ID"].ToString());
                    if(RegID!=0)
                    {
                        DR.Close();
                        string SQL1="Insert into ACCOUNTMASTER values ('" + txtOpeningAmount.Text + "','" + txtOpeningAmount.Text + "','" + DateTime.Today.ToShortDateString() + "','" + cmbAccountType.SelectedItem.ToString() + "', '" + RegID + "')";
                        SqlCommand cmd2 = new SqlCommand(SQL1, Conn);
                        cmd2.CommandType = CommandType.Text;
                        int j = cmd2.ExecuteNonQuery();

                        if(j==1)
                        {
                            
                            string UserName =  txtFName.Text + "." + txtLName.Text;
                            
                            string YearString = (Convert.ToDateTime(dtpBirthDate.Text)).ToString().Split('/')[2].Trim().ToString();
                            string BirthYear = YearString.Split(' ')[0].Trim().ToString();
                            string Password = txtFName.Text + "" + BirthYear;

                            string SQL2 = "Insert into UserLogin values ('" + txtFName.Text + "','" + txtLName.Text + "','" + cmbGender.SelectedItem.ToString()+ "','" + Convert.ToDateTime(dtpBirthDate.Text) + "','" + UserName + "','" + Password + "','"+txtMobile1.Text+ "','"+txtCountry.Text+"','" + txtAccountNo.Text + "','" + "Ideology Bank" + "','" + txtIFSC.Text + "','" + txtSecurePin.Text + "','" + "Y" + "','"+ RegID + "')";
                            SqlCommand cmd3 = new SqlCommand(SQL2, Conn);
                            cmd3.CommandType = CommandType.Text;
                            int k = cmd3.ExecuteNonQuery();
                            if(k==1)
                            {
                                FullName = txtFName.Text + " " + txtMiddleName.Text + " " + txtLName.Text;
                                IFSC = txtIFSC.Text;
                                AccountNo = txtAccountNo.Text;
                                MessageBox.Show("Mr. " + txtFName.Text + " " + txtLName.Text + " has been registerd successfully. Your Username= "+ UserName+" And Password="+Password+" And SecurePin="+txtSecurePin.Text);
                                AccountHolderImage AHI = new AccountHolderImage();
                                AHI.Show();
                                base.Hide();
                            }
                        }

                       
                    }
                }
               
            }
            else
                MessageBox.Show("Record Not Added");
            
        }

        private void OpenAccount_Load(object sender, EventArgs e)
        {
            RandomAccountNumber();
            SecurePin();
        }

        void RandomAccountNumber()
        {
            Random Random = new Random();
            int AccountNo = Random.Next(32000000, 33000000);
            txtAccountNo.Text = AccountNo.ToString();
        }

        void SecurePin()
        {
            Random Random = new Random();
            int SecurePin = Random.Next(0000, 9999);
            txtSecurePin.Text = SecurePin.ToString();
        }

        void Password()
        {
           
        }

        private void txtIFSC_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

    }
}
