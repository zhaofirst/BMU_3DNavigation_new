using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;
using MathWorks.MATLAB.NET.Arrays;
using MatlabFunction;
using System.IO;

namespace Portable_BMU_App
{
    public partial class Frm_RealTime_ShowLocationCalibration : Form
    {
        public Frm_RealTime_ShowLocationCalibration()
        {
            InitializeComponent();
        }

        public static List<float[]> saveGPS = new List<float[]>();

        // GPS
        GetRect getRect = new GetRect();
        public US_GPS_ENTRY myGpsEntries = new US_GPS_ENTRY();
        public Rect3[] myRectangles = new Rect3[3];
        public Vector3[] myCalibrationPoints = new Vector3[3];
        public static Vector3 gpsTop;
        public static Vector3 gpsBottom;
        public Matrix4x4 calibrationMatrix = new Matrix4x4();

        //Voxel Size
        public float voxelWidth = 1f;
        public float voxelHeight = 1f;
        public float voxelDepth = 2f;

        const double Radian2Degree = 57.295779513082323;
        public static bool InverseZFlag = false;
        public void Calibaration()
        {
            // A rect vertexs were denoted as abcd
            Vector3 orgPoint = this.myRectangles[0].v[0];
            Vector3 bPoint = this.myRectangles[0].v[1];
            Vector3 dPoint = this.myRectangles[0].v[3];
            //Vector3 orgPoint = new Vector3(219.3985f,79.3590f,-139.8762f);
            //Vector3 bPoint = new Vector3(259.4648f, 50.5138f, -88.8002f);
            //Vector3 dPoint = new Vector3(253.4263f, 160.2313f,-120.9184f);

            Vector3[] calibrationPoints = new Vector3[3];
            calibrationPoints[0] = orgPoint;
            calibrationPoints[1] = dPoint;
            calibrationPoints[2] = bPoint;

            // Calibration Data
            calibrationMatrix = Matrix4x4.Identity;

            // Get data
            //Vector3[] calibrationPoints = this.myGpsLocation;

            #region **[1 0 0]
            // Define Vector in GPS space
            Vector3 xGPS = bPoint - orgPoint;
            xGPS = Vector3.Normalize(xGPS);
            Console.WriteLine(xGPS);
            //Define Vector in screen space
            Vector3 xDirection = new Vector3(1f, 0f, 0f);

            // Do transformation
            float degreeNum = (float)(Math.Acos(Vector3.Dot(xGPS, xDirection)) * 57.295779513082323);      //求和x轴夹角，并转换为角度
            Console.WriteLine("\t Calibration: Angle{{upVector(1,0,0)}} = {0} degree", degreeNum);
            Vector3[] mySreenGPS = new Vector3[3];
            if (degreeNum > 2 && xGPS.Length() > 0)
            {
                Matrix4x4 M = this.BuildRotateFtomTO(xGPS, xDirection);
                for (int i = 0; i < 3; i++)
                {
                    Vector3 tempVector = calibrationPoints[i];
                    mySreenGPS[i] = Vector3.Transform(tempVector, M);
                }
                calibrationMatrix = calibrationMatrix * M;
            }
            xGPS = mySreenGPS[2] - mySreenGPS[0];
            xGPS = Vector3.Normalize(xGPS);
            degreeNum = (float)(Math.Acos(Vector3.Dot(xGPS, xDirection)) * 57.295779513082323);      //变换后：求和x轴夹角，并转换为角度
            Console.WriteLine("\t After Calibration: Angle{{upVector(1,0,0)}} = {0} degree", degreeNum);
            #endregion

            #region **[0 1 0]

            //Define Vector in screen space
            Vector3 yDirection = new Vector3(0f, 1f, 0f);

            // Define Vector in GPS space
            Vector3 yGPS = mySreenGPS[1] - mySreenGPS[0];
            yGPS = Vector3.Normalize(yGPS);
            yGPS.X = 0;  // 投影到yz平面

            // Do transformation
            Console.WriteLine();
            degreeNum = (float)(Math.Acos(Vector3.Dot(yGPS, yDirection)) * 57.295779513082323);      //求和y轴夹角，并转换为角度
            Console.WriteLine("\t Calibration: Angle{{upVector(0,0,1)}} = {0} degree", degreeNum);
            Vector3[] mySreenGPS2 = new Vector3[3];
            if (degreeNum > 2 && yGPS.Length() > 0)
            {
                Matrix4x4 M = this.BuildRotateFtomTO(yGPS, yDirection);
                for (int i = 0; i < 3; i++)
                {
                    Vector3 tempVector = mySreenGPS[i];
                    mySreenGPS2[i] = Vector3.Transform(tempVector, M);
                }
                calibrationMatrix = calibrationMatrix * M;
            }

            yGPS = mySreenGPS2[1] - mySreenGPS2[0];
            yGPS = Vector3.Normalize(yGPS);
            degreeNum = (float)(Math.Acos(Vector3.Dot(yGPS, yDirection)) * 57.295779513082323);      //变换后：求和y轴夹角，并转换为角度
            Console.WriteLine("\t After Calibration: Angle{{upVector(0,1,0)}} = {0} degree", degreeNum);

            xGPS = mySreenGPS2[2] - mySreenGPS2[0];
            xGPS = Vector3.Normalize(xGPS);
            degreeNum = (float)(Math.Acos(Vector3.Dot(xGPS, xDirection)) * 57.295779513082323);      //变换后：求和x轴夹角，并转换为角度
            Console.WriteLine("\t After Calibration: Angle{{upVector(1,0,0)}} = {0} degree", degreeNum);
            #endregion

        }

        // 根据四个点的坐标计算BoxSize
        public void CalculateBoxSize(out Vector3 boxOrg, out int boxWidth, out int boxHeight, out int boxDepth)
        {
            // Define paras
            //Vector3 boxOrg = new Vector3();
            //int boxWidth, boxHeight, boxDepth;

            //// Calibration Three rects by using calibrationMatrix
            //for (int i = 0; i < this.myRectangles.Length; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        Vector3 tempVector = this.myRectangles[i].v[j];
            //        this.myRectangles[i].v[j] = Vector3.Transform(tempVector, calibrationMatrix);
            //    }
            //}

            Vector3 minVector = this.myRectangles[0].v[0];
            Vector3 maxVector = this.myRectangles[0].v[0];
            //Console.WriteLine("请选择四个校准点！");
            for (int i = 0; i < this.myRectangles.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 tempVector = this.myRectangles[i].v[j];
                    minVector = Vector3.Min(tempVector, minVector);
                    maxVector = Vector3.Max(tempVector, maxVector);
                }
            }
            boxOrg = minVector;
            boxWidth = 2 + (int)Math.Round(Math.Abs(minVector.X - maxVector.X) / voxelWidth);
            boxHeight = 2 + (int)Math.Round(Math.Abs(minVector.Y - maxVector.Y) / voxelHeight);
            boxDepth = 2 + (int)Math.Round(Math.Abs(minVector.Z - maxVector.Z) / voxelDepth);

            // Display in label
            this.labelWidth.Visible = true; this.labelWidth.Text = boxWidth.ToString();
            this.labelHeight.Visible = true; this.labelHeight.Text = boxHeight.ToString();
            this.labelDepth.Visible = true; this.labelDepth.Text = boxDepth.ToString();

            // Send this para into Frm_RealTime_ReconsMainForm
            MainForm.boxDepth = boxDepth;
            MainForm.boxWidth = boxWidth;
            MainForm.boxHeight = boxHeight;
            MainForm.boxOrg = boxOrg;
            MainForm.calibrationMatrix = calibrationMatrix;
        }

        // 根据四个点的坐标计算BoxSize
        public void CalculateCalibrationBoxSize(out int boxHeight)
        {
            Vector3 minVector = this.myRectangles[0].v[0];
            Vector3 maxVector = this.myRectangles[0].v[0];
            //Console.WriteLine("请选择四个校准点！");
            for (int i = 0; i < this.myRectangles.Length; i++)    
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 tempVector = this.myRectangles[i].v[j];
                    minVector = Vector3.Min(tempVector, minVector);
                    maxVector = Vector3.Max(tempVector, maxVector);
                }
            }
            boxHeight = (int)Math.Round(Math.Abs(minVector.Y - maxVector.Y) / voxelHeight);
        }

        /// <summary>
        /// Builds a matrix that rotates from one vector to another
        /// </summary>
        /// <param name="fromVector"></param>
        /// <param name="toVector"></param>
        /// <returns></returns>
        public Matrix4x4 BuildRotateFtomTO(Vector3 fromVector, Vector3 toVector)
        {
            Matrix4x4 transformMatrix = new Matrix4x4();
            transformMatrix = Matrix4x4.Identity;

            Vector3 fNormalize = Vector3.Normalize(fromVector);
            Vector3 tNormalize = Vector3.Normalize(toVector);
            Vector3 tfVector = Vector3.Cross(tNormalize, fNormalize);
            Vector3 tfVectorNormalize = new Vector3();

            if (tfVector.Length() != 0f)
            {
                tfVectorNormalize = Vector3.Normalize(tfVector);

                float cosValue = Vector3.Dot(fNormalize, tNormalize);  //the value cos Angle
                float degreeAngle = (float)Math.Acos(cosValue);      // Angle
                Vector3 vtVector = tfVectorNormalize * (1 - cosValue);

                transformMatrix.M11 = vtVector.X * tfVectorNormalize.X + cosValue;
                transformMatrix.M22 = vtVector.Y * tfVectorNormalize.Y + cosValue;
                transformMatrix.M33 = vtVector.Z * tfVectorNormalize.Z + cosValue;

                vtVector.X = vtVector.X * tfVectorNormalize.Y;
                vtVector.Z = vtVector.Z * tfVectorNormalize.X;
                vtVector.Y = vtVector.Y * tfVectorNormalize.Z;

                transformMatrix.M12 = vtVector.X - tfVector.Z;
                transformMatrix.M13 = vtVector.Z + tfVector.Y;
                transformMatrix.M14 = 0;

                transformMatrix.M21 = vtVector.X + tfVector.Z;
                transformMatrix.M23 = vtVector.Y - tfVector.X;
                transformMatrix.M24 = 0;

                transformMatrix.M31 = vtVector.Z - tfVector.Y;
                transformMatrix.M32 = vtVector.Y + tfVector.X;
                transformMatrix.M34 = 0;

                transformMatrix.M41 = 0;
                transformMatrix.M42 = 0;
                transformMatrix.M43 = 0;
                transformMatrix.M44 = 1;
            }
            return transformMatrix;

        }

        private void buttonOrigin_Click(object sender, EventArgs e)
        {
            OriginOperation();
        }

        public void OriginOperation()
        {
            // Get current coordinate
            // 当这个按键按下时，获取当前的一个GPS信息即可
            float[] xyzArray = new float[3];
            float[] aerArray = new float[4];

            object synObj = new object();
            lock (this)
            {
                Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length);
                Array.Copy(MainForm.sourceAER, aerArray, MainForm.sourceAER.Length);

            }

            saveGPS.Add(xyzArray);
            saveGPS.Add(aerArray);
            //this.labelOrigin.Visible = true;
            //this.labelOrigin.Text = MainForm.sourceXYZ[0].ToString("#0.0") + "," + MainForm.sourceXYZ[1].ToString("#0.0") +
            //    "," + MainForm.sourceXYZ[2].ToString("#0.0");

            this.labelOrigin.Visible = true;
            this.labelOrigin.Text = xyzArray[0].ToString("#0.0") + "," + xyzArray[1].ToString("#0.0") +
                "," + xyzArray[2].ToString("#0.0");

            getRect = new GetRect(xyzArray, aerArray,MainForm.LATERAL_Resolution);

            this.myRectangles[0] = new Rect3();
            this.myRectangles[0].v = getRect.GetRectangle();
            //foreach (var ele in this.myRectangles)
            //{
            //    Console.WriteLine(ele);

            //}
            // Use Origin point to get calibration matrix
            Calibaration();


            // Calibration Three rects by using calibrationMatrix
            for (int j = 0; j < 4; j++)
            {
                Vector3 tempVector = this.myRectangles[0].v[j];
                this.myRectangles[0].v[j] = Vector3.Transform(tempVector, calibrationMatrix);
            }

            float[,] arr = new float[3, 4];
            for (int i = 0; i < 4; i++)
            {
                arr[0, i] = this.myRectangles[0].v[i].X;
                arr[1, i] = this.myRectangles[0].v[i].Y;
                arr[2, i] = this.myRectangles[0].v[i].Z;
            }
            MWNumericArray aaa = arr;

            myCalibrationPoints[0] = Vector3.Transform(new Vector3(xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10), calibrationMatrix);
            MWNumericArray xyzData = new double[3] { myCalibrationPoints[0].X, myCalibrationPoints[0].Y, myCalibrationPoints[0].Z };
            //MWNumericArray xyzData = new double[3] { xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10 };

            MainForm.callMatlabFunction.drawRect(aaa, xyzData);
  
        }

        private void buttonXDirection_Click(object sender, EventArgs e)
        {
            ZDirectionOperation();
        }

        public void ZDirectionOperation()
        {
            // Get current coordinate
            // 当这个按键按下时，获取当前的一个GPS信息即可
            //byte[] gpsArray;
            //Frm_RealTime_ReconsMainForm.gpsQueue.TryDequeue(out gpsArray);
            //this.myGpsEntries = (US_GPS_ENTRY)BytesToStruct(gpsArray, typeof(US_GPS_ENTRY));

            float[] xyzArray = new float[3]; float[] aerArray = new float[4];

            object synObj = new object();
            lock (this)
            {
                Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length);
                Array.Copy(MainForm.sourceAER, aerArray, MainForm.sourceAER.Length);

            }

            saveGPS.Add(xyzArray);
            saveGPS.Add(aerArray);

            this.labelxDirection.Visible = true;
            this.labelxDirection.Text = xyzArray[0].ToString("#0.0") + "," + xyzArray[1].ToString("#0.0") +
                "," + xyzArray[2].ToString("#0.0");

            getRect = new GetRect(xyzArray, aerArray,MainForm.LATERAL_Resolution);

            this.myRectangles[1] = new Rect3();
            this.myRectangles[1].v = getRect.GetRectangle();

            // Calibration Three rects by using calibrationMatrix
            for (int j = 0; j < 4; j++)
            {
                Vector3 tempVector = this.myRectangles[1].v[j];
                this.myRectangles[1].v[j] = Vector3.Transform(tempVector, calibrationMatrix);
            }

            float[,] arr = new float[3, 4];
            for (int i = 0; i < 4; i++)
            {
                arr[0, i] = this.myRectangles[1].v[i].X;
                arr[1, i] = this.myRectangles[1].v[i].Y;
                arr[2, i] = this.myRectangles[1].v[i].Z;
            }
            MWNumericArray aaa = arr;

            myCalibrationPoints[1] = Vector3.Transform(new Vector3(xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10), calibrationMatrix);
            MWNumericArray xyzData = new double[3] { myCalibrationPoints[1].X, myCalibrationPoints[1].Y, myCalibrationPoints[1].Z };
            //MWNumericArray xyzData = new double[3] { xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10 };

            MainForm.callMatlabFunction.drawRect(aaa, xyzData);
        }

        private void buttonYDirection_Click(object sender, EventArgs e)
        {
            XDirectionOperation();
        }

        public void XDirectionOperation()
        {
            // Get current coordinate
            // 当这个按键按下时，获取当前的一个GPS信息即可
            //byte[] gpsArray;
            //Frm_RealTime_ReconsMainForm.gpsQueue.TryDequeue(out gpsArray);
            //this.myGpsEntries = (US_GPS_ENTRY)BytesToStruct(gpsArray, typeof(US_GPS_ENTRY));
            float[] xyzArray = new float[3]; float[] aerArray = new float[4];

            object synObj = new object();
            lock (this)
            {
                Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length);
                Array.Copy(MainForm.sourceAER, aerArray, MainForm.sourceAER.Length);

            }

            saveGPS.Add(xyzArray);
            saveGPS.Add(aerArray);

            this.labelyDirection.Visible = true;
            this.labelyDirection.Text = xyzArray[0].ToString("#0.0") + "," + xyzArray[1].ToString("#0.0") +
                "," + xyzArray[2].ToString("#0.0");

            getRect = new GetRect(xyzArray, aerArray,MainForm.LATERAL_Resolution);
            this.myRectangles[2] = new Rect3();
            this.myRectangles[2].v = getRect.GetRectangle();

            // Calibration Three rects by using calibrationMatrix
            for (int j = 0; j < 4; j++)
            {
                Vector3 tempVector = this.myRectangles[2].v[j];
                this.myRectangles[2].v[j] = Vector3.Transform(tempVector, calibrationMatrix);
            }

            float[,] arr = new float[3, 4];
            for (int i = 0; i < 4; i++)
            {
                arr[0, i] = this.myRectangles[2].v[i].X;
                arr[1, i] = this.myRectangles[2].v[i].Y;
                arr[2, i] = this.myRectangles[2].v[i].Z;
            }
            MWNumericArray aaa = arr;

            myCalibrationPoints[2] = Vector3.Transform(new Vector3(xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10), calibrationMatrix);
            MWNumericArray xyzData = new double[3] { myCalibrationPoints[2].X, myCalibrationPoints[2].Y, myCalibrationPoints[2].Z };
            //MWNumericArray xyzData = new double[3] { xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10 };

            MainForm.callMatlabFunction.drawRect(aaa, xyzData);
        }

        //private void buttonCalculateBox_Click(object sender, EventArgs e)
        //{
        //    CalculateBoxSize(out Vector3 boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
        //    this.labelBoxWidth.Visible = true;
        //    this.labelBoxDepth.Visible = true;
        //    this.labelBoxHeight.Visible = true;

        //    this.labelBoxWidth.Text = boxWidth.ToString();
        //    this.labelBoxHeight.Text = boxHeight.ToString();
        //    this.labelBoxDepth.Text = boxDepth.ToString();
        //    Frm_RealTime_ReconsMainForm.boxDepth = boxDepth;
        //    Frm_RealTime_ReconsMainForm.boxWidth = boxWidth;
        //    Frm_RealTime_ReconsMainForm.boxHeight = boxHeight;
        //    Frm_RealTime_ReconsMainForm.boxOrg = boxOrg;

        //}



        /// <summary>
        /// 将字节转化为结构体
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length) return null;

            //分配结构体大小的空间
            IntPtr structptr = Marshal.AllocHGlobal(size);
            //将byte数组拷贝到分配好的空间
            Marshal.Copy(bytes, 0, structptr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structptr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structptr);
            return obj;

        }

        private void Frm_RealTime_ShowLocationCalibration_Load(object sender, EventArgs e)
        {
            this.labelOrigin.Visible = false;
            this.labelxDirection.Visible = false;
            this.labelyDirection.Visible = false;

            this.labelDepth.Visible = false;
            this.labelWidth.Visible = false;
            this.labelHeight.Visible = false;
            MainForm.callMatlabFunction.initialCoordinate();
        }

        // Do calibration according to the data
        private void buttonCalibration_Click(object sender, EventArgs e)
        {
            CalibrationOperation();
        }

        public void CalibrationOperation()
        {
            // Calculate z direction to determine whether inverse z or not.
            Vector3 OriPoint = myCalibrationPoints[0];
            Vector3 TopPoint = myCalibrationPoints[0];
            TopPoint.Z = myCalibrationPoints[1].Z;
            Vector3 zDirection = Vector3.Normalize(TopPoint-OriPoint);
            float degreeNumber = (float)(Math.Cos(Vector3.Dot(zDirection, new Vector3 (0, 0, 1 )))* Radian2Degree);
            //if (degreeNumber != 0)
            //{
            //    InverseZFlag = true;
            //    for (int i = 0; i < myRectangles.Length; i++)
            //    {
            //        for (int j = 0; j < 4; j++)
            //        {
            //            myRectangles[i].v[j].Z = 0 - myRectangles[i].v[j].Z;
            //        }
            //        myCalibrationPoints[i].Z = -myCalibrationPoints[i].Z;
            //    }
            //}

            SaveGPSData();
            // Define paras
            Vector3 boxOrg = new Vector3();
            int boxWidth, boxHeight, boxDepth;
            CalculateBoxSize(out boxOrg, out boxWidth, out boxHeight, out boxDepth);
            MainForm.isCalibration = true;

            float[,,] arr = new float[3, 3, 4];
            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < 4; i++)
                {
                    arr[k, 0, i] = this.myRectangles[k].v[i].X;
                    arr[k, 1, i] = this.myRectangles[k].v[i].Y;
                    arr[k, 2, i] = this.myRectangles[k].v[i].Z;

                }
            }
            MWNumericArray aaa = arr;

            float[,] locationArray = new float[3,3];
            for (int i = 0; i < 3; i++)
            {
                locationArray[0, i] = myCalibrationPoints[i].X;
                locationArray[1, i] = myCalibrationPoints[i].Y;
                locationArray[2, i] = myCalibrationPoints[i].Z;
            }
            MWNumericArray bbb = locationArray;

            MainForm.callMatlabFunction.calculateBoxSize(aaa,bbb);
            //float[,,] srr = new float[,,]
            // Refresh App setting
            Properties.Settings.Default.isCalibration = true;
            Properties.Settings.Default.CalibrationMatrix = calibrationMatrix;
            Properties.Settings.Default.boxOrg = boxOrg;
            Properties.Settings.Default.boxDepth = boxDepth;
            Properties.Settings.Default.boxWidth = boxWidth;
            Properties.Settings.Default.boxHeight = boxHeight;
            Properties.Settings.Default.Save();
        }

        public static void SaveGPSData()
        {

            int num = saveGPS.Count / 2;
            byte[] numByte = BitConverter.GetBytes(num);

            const string fileName = @"C:\Users\BMU\Desktop\G4Data\us.gps";
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                foreach (var ele in numByte)
                {
                    binaryWriter.Write(ele);
                }

                foreach (var gpss in saveGPS)
                {
                    foreach (var gps in gpss)
                    {
                        byte[] temp = BitConverter.GetBytes(gps);
                        foreach (var element in temp)
                        {
                            binaryWriter.Write(element);
                        }
                    }

                }

            }
            Console.WriteLine("G4 number:{0}", num);
            //Console.WriteLine("Img number:{0}", saveImg.Count);

            Console.WriteLine("Finished");
        }

        private void buttonOrigin_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show("tt","sss");
            //if (e.KeyCode == Keys.Left)
            //{
            //    buttonOrigin_Click(sender,e);
            //}
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                OriginOperation();
            }
            else if (keyData == Keys.Right)
            {
                XDirectionOperation();
            }
            else if (keyData == Keys.Up)
            {
                ZDirectionOperation();
            }
            else if (keyData == Keys.Enter)
            {
                CalibrationOperation();
            }
            return true;
            //return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
