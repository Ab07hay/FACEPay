using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;

namespace MultiFaceRec
{
    public partial class FacePayTransaction : Form
    {
       
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;

        private void btn_Click(object sender, EventArgs e)
        {
            if (txtSecurePin.Text != "")
            {
                TransactionWithPay();
            }
            else
            {
                MessageBox.Show("Please Enter Secure Pin.");
                txtSecurePin.Focus();
            }
        }

        private void FacePayTransaction_Load(object sender, EventArgs e)
        {
            txtAccountNumber.Text = Login.AccountNumber.ToString();
            txtIFSC.Text = Login.IFSC.ToString();
            textBox1.Text = Login.FirstName.ToString() + " " + Login.LastName.ToString();

        }

        public FacePayTransaction()
        {
            InitializeComponent();
            
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels + 1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }

            }
            catch (Exception e)
            {
                
                MessageBox.Show("Nothing in binary database, please add at least a face(Simply train the prototype with the Add Face Button).", "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (  txtPayeeAccountNo.Text!="" && txtPayAmount.Text!="")
            {
                
                grabber = new Capture();
                grabber.QueryFrame();
                
             Application.Idle += new EventHandler(FrameGrabber);
                button1.Enabled = false;
            }
            else
            {
                MessageBox.Show("Please Enter Payee Details.");
            }
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            label3.Text = "0";
            
            NamePersons.Add("");


            
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            
            gray = currentFrame.Convert<Gray, Byte>();

            
            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
          face,
          1.2,
          10,
          Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
          new Size(20, 20));

            
            foreach (MCvAvgComp f in facesDetected[0])
            {
                t = t + 1;
                result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                
                currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                if (trainingImages.ToArray().Length != 0)
                {
                   
                    MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

               
                    EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                       trainingImages.ToArray(),
                       labels.ToArray(),
                       3000,
                       ref termCrit);

                    name = recognizer.Recognize(result);

              
                    currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                }

                NamePersons[t - 1] = name;
                NamePersons.Add("");


          
                label3.Text = facesDetected[0].Length.ToString();

                /*
                //Set the region of interest on the faces

                gray.ROI = f.rect;
                MCvAvgComp[][] eyesDetected = gray.DetectHaarCascade(
                   eye,
                   1.1,
                   10,
                   Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                   new Size(20, 20));
                gray.ROI = Rectangle.Empty;

                foreach (MCvAvgComp ey in eyesDetected[0])
                {
                    Rectangle eyeRect = ey.rect;
                    eyeRect.Offset(f.rect.X, f.rect.Y);
                    currentFrame.Draw(eyeRect, new Bgr(Color.Blue), 2);
                }
                 */

            }
            t = 0;

         
            for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
            {
                names = names + NamePersons[nnn] + ", ";
            }
           
            imageBoxFrameGrabber.Image = currentFrame;
            label4.Text = names;
            
            names = "";
      
            NamePersons.Clear();

        }


        void TransactionWithPay()
        {


            MessageBox.Show("successfullllll");
            //if (label4.Text != "")
            //{
            //    string AccountNo = label4.Text.Split('~')[1].Trim().ToString();
            //    if (AccountNo == txtAccountNumber.Text)
            //    {
            //        int Account_ID = 0;
            //        SqlConnection conn = new SqlConnection("@Data Source=DESKTOP-8MTP45M\MSSQLSERVER01;Initial Catalog=IDEOLOGY_BANK;Integrated Security=True");

            //        string SQL = "select * from ACCOUNT_REGISTRATION R, AccountMaster A where R.Account_No='" + AccountNo + "' and R.ID=A.RegistrationID";

            //        SqlCommand cmd = new SqlCommand(SQL, conn);

            //        cmd.CommandType = CommandType.Text;
            //        conn.Open();

            //        SqlDataReader DR = cmd.ExecuteReader();
            //        if (DR.Read())
            //        {

            //            int Total_Amount = Convert.ToInt32(DR["TotalAmount"].ToString());
            //            Account_ID = Convert.ToInt32(DR["ID"].ToString());
            //            DR.Close();
            //            if (Convert.ToInt32(txtPayAmount.Text) < Total_Amount)
            //            {
            //                int Balance = Total_Amount - Convert.ToInt32(txtPayAmount.Text);
            //                string UpdateSQL = "update AccountMaster set TotalAmount='" + Balance + "', BalanceAmount='" + Balance + "', TransactionDate='" + DateTime.Today.ToShortDateString() + "' where RegistrationID='" + Account_ID + "' ";
            //                SqlCommand cmdupdate = new SqlCommand(UpdateSQL, conn);
            //                cmdupdate.CommandType = CommandType.Text;
            //                int i = cmdupdate.ExecuteNonQuery();
            //                if (i == 1)
            //                {
            //                    //Inserting Amount to table to whom we paying
            //                    string InsertAccuntDetails = "insert into AccountDetails values ('" + Balance + "','" + txtPayAmount.Text + "','" + "Self" + "','" + Balance + "','" + DateTime.Today.ToShortDateString() + "','" + Account_ID + "')";
            //                    SqlCommand InsertCMD = new SqlCommand(InsertAccuntDetails, conn);
            //                    InsertCMD.CommandType = CommandType.Text;
            //                    int j = InsertCMD.ExecuteNonQuery();
            //                    if (j == 1)
            //                    {
            //                        MessageBox.Show("Transaction Successfull");
            //                        base.Close();
            //                        Dashboard obj = new Dashboard();
            //                        obj.Show();
            //                    }
            //                    else
            //                    {
            //                        MessageBox.Show("Transaction Fail");
            //                    }

            //                }
            //                else
            //                    MessageBox.Show("Insufficient Amount, Please try with minimum amount.");




            //            }
            //            else
            //            {
            //                MessageBox.Show("Please Enter Valid Pin, Try Again");
            //            }
            //        }
                }
            }
        }
        

        

//    }
//}
