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
    public partial class FacePay : Form
    {

        //Declararation of all variables, vectors and haarcascades
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

        public FacePay()
        {
            InitializeComponent();
            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                //Load of previus trainned faces and labels for each image
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
                //MessageBox.Show(e.ToString());
                MessageBox.Show("Nothing in binary database, please add at least a face(Simply train the prototype with the Add Face Button).", "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnGetDetails_Click(object sender, EventArgs e)
        {
            TransactionWithPay();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPayAmount.Text = "";
            txtPayeeAccountNo.Text = "";
            txtPayeeIFSC.Text = "";
            txtPayeeName.Text = "";
            txtPayeeName.Focus();
        }

        private void btnPayAmount_Click(object sender, EventArgs e)
        {
            if (txtSecurePin.Text != "")
            {
                TransferAmount();
            }
            else
            {
                MessageBox.Show("Please Enter Secure Pin.");
                txtSecurePin.Focus();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(txtPayAmount.Text!="" && txtPayeeAccountNo.Text!="" && txtPayeeIFSC.Text!="" & txtPayeeName.Text!="")
            {
                grbFaceDetect.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Initialize the capture device
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event
            Application.Idle += new EventHandler(FrameGrabber);
            button1.Enabled = false;
        }
        void FrameGrabber(object sender, EventArgs e)
        {
            label3.Text = "0";
            //label4.Text = "";
            NamePersons.Add("");


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            gray = currentFrame.Convert<Gray, Byte>();

            //Face Detector
            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
          face,
          1.2,
          10,
          Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
          new Size(20, 20));

            //Action for each element detected
            foreach (MCvAvgComp f in facesDetected[0])
            {
                t = t + 1;
                result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //draw the face detected in the 0th (gray) channel with blue color
                currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                if (trainingImages.ToArray().Length != 0)
                {
                    //TermCriteria for face recognition with numbers of trained images like maxIteration
                    MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                    //Eigen face recognizer
                    EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                       trainingImages.ToArray(),
                       labels.ToArray(),
                       3000,
                       ref termCrit);

                    name = recognizer.Recognize(result);

                    //Draw the label for each face detected and recognized
                    currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                }

                NamePersons[t - 1] = name;
                NamePersons.Add("");


                //Set the number of faces detected on the scene
                label3.Text = facesDetected[0].Length.ToString();

            }
            t = 0;

            //Names concatenation of persons recognized
            for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
            {
                names = names + NamePersons[nnn] + ", ";
            }
            //Show the faces procesed and recognized
            imageBoxFrameGrabber.Image = currentFrame;
            label4.Text = names;

            names = "";
            //Clear the list(vector) of names
            NamePersons.Clear();

        }

        void TransactionWithPay()
        {
            if (label4.Text != "")
            {
                
                string AccountNo = label4.Text.Split('~')[1].Trim().ToString();
                
                if (AccountNo != "")
                {
                    SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-8MTP45M\MSSQLSERVER01;Initial Catalog=IDEOLOGY_BANK;Integrated Security=True");

                    string SQL = "select * from ACCOUNT_REGISTRATION A, AccountMaster C where A.Account_No='"+AccountNo+"' and A.ID=C.RegistrationID";

                    SqlCommand cmd = new SqlCommand(SQL, conn);

                    cmd.CommandType = CommandType.Text;
                    conn.Open();

                    SqlDataReader DR = cmd.ExecuteReader();
                    if (DR.Read())
                    {
                        int Total_Amount = Convert.ToInt32(DR["TotalAmount"].ToString());
                        txtAccountNumber.Text = DR["Account_No"].ToString();
                        txtIFSC.Text= DR["IFSC"].ToString();
                        textBox1.Text= DR["FIRST_NAME"].ToString()+" "+ DR["MIDDLE_NAME"].ToString()+" "+ DR["LAST_NAME"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Sorry...!, No Record Found.");
                    }
                }
            }
        }

        void TransferAmount()
        {

        }
    }
}
