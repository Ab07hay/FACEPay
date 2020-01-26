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
    public partial class Login : Form
    {
        public static string Username = string.Empty;
        public static string AccountType = string.Empty;
        public static string FirstName = string.Empty;
        public static string LastName = string.Empty;
        public static int AccountNumber = 0;
        public static int RegistrationID = 0;
        public static string IFSC = "";
        public static string BankName = "";
        public static string TransferLimit = "";
        public static string TotalAmount = "";
        public static string BalanceAmount = "";
        public static string LoginType = "";

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-8MTP45M\MSSQLSERVER01;Initial Catalog=IDEOLOGY_BANK;Integrated Security=True");

            if (txtUsername.Text!="" && txtPassword.Text!="" && cmbAccountType.SelectedItem.ToString() == "Other")
            {
                string SQL = "select * from UserLogin R, ACCOUNT_REGISTRATION A, AccountMaster C where R.Username='" + txtUsername.Text + "' and R.Password='" + txtPassword.Text + "' and R.RegID=A.ID and R.RegID=C.RegistrationID";

                SqlCommand cmd = new SqlCommand(SQL, conn);

                cmd.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader DR = cmd.ExecuteReader();
                if (DR.Read())
                {
                    Username = DR["Username"].ToString();
                    FirstName = DR["FirstName"].ToString();
                    LastName = DR["LastName"].ToString();
                    RegistrationID = Convert.ToInt32(DR["RegID"].ToString());
                    AccountNumber = Convert.ToInt32(DR["Account_Number"].ToString());
                    IFSC = DR["IFSC"].ToString();
                    BankName = DR["Bank_Name"].ToString();
                    BalanceAmount = DR["BalanceAmount"].ToString();
                    TransferLimit = DR["Transfer_Limit"].ToString();
                    LoginType = cmbAccountType.SelectedItem.ToString();
                    DR.Close();
                    Dashboard DBorad = new Dashboard();
                    DBorad.Show();
                    base.Hide();
                }
                else
                {
                    MessageBox.Show("Please Enter Correct Credentials, Try Again");
                    ClearControll();
                }
            }
            else if (txtUsername.Text != "" && txtPassword.Text != "" && cmbAccountType.SelectedItem.ToString() == "Admin")
            {
                string SQL2 = "select * from UserLogin R where R.Username='" + txtUsername.Text + "' and R.Password='" + txtPassword.Text + "'";

                SqlCommand cmd2 = new SqlCommand(SQL2, conn);

                cmd2.CommandType = CommandType.Text;
                conn.Open();

                SqlDataReader DR = cmd2.ExecuteReader();
                if (DR.Read())
                {
                    Username = DR["Username"].ToString();
                    FirstName = DR["FirstName"].ToString();
                    LastName = DR["LastName"].ToString();
                    RegistrationID = Convert.ToInt32(DR["RegID"].ToString());
                    AccountNumber = Convert.ToInt32(DR["Account_Number"].ToString());
                    IFSC = DR["IFSC"].ToString();
                    BankName = DR["Bank_Name"].ToString();
                   
                    LoginType = cmbAccountType.SelectedItem.ToString();
                    Dashboard DBorad = new Dashboard();
                    DBorad.Show();
                    base.Hide();
                }
                else
                {
                    MessageBox.Show("Please Enter Correct Credentials, Try Again");
                    ClearControll();
                }
            }
            else
            {
                MessageBox.Show("Please Enter Username and Password.");
                ClearControll();
                
            }
        }
        void ClearControll()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtUsername.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControll();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
