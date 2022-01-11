using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;

namespace Portable_BMU_App
{
    public partial class Frm_RealTime_PreScan : Form
    {
        public float LATERAL_X = 0.148f;
        public float LATERAL_Y = 0.148f;

        public float mvoxelDepth = 1f;
        public float mvoxelWidth = 0.5f;
        public float mvoxelHeight = 0.5f;
        public int adjustParasForVolume = 1;

        public static List<float[]> savePreScanGPS = new List<float[]>();
        G4GetRects_Pre getRect = new G4GetRects_Pre();
        public string preScanFileName = null;
        public static string savePath = null;
        public byte[][,] mFrames;
        Rect3[] myRectangles;
        Vector3 PlumbLineTop = new Vector3();
        Vector3 PlumbLineBottom = new Vector3();
        bool isUsedTopBottom = false;
        public Frm_RealTime_PreScan()
        {
            InitializeComponent();
            this.buttonStart.BackColor = SystemColors.Control;
        }


        public void GetSavePath()
        {
            string pathString = @"D:\Users\Chenhb\Documents\G4Data";  // The path to store the G4 and Clarius data
            string folderName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm"); // Use current time as tye name of folder and 

            string filePath = Path.Combine(pathString, folderName);
            Directory.CreateDirectory(filePath);
            savePath = Path.Combine(filePath, folderName);
            preScanFileName = savePath + ".pregps";
            Console.WriteLine(preScanFileName);

            // write the export dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "GPS Files (*.g4gps)|*.pregps|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "pregps";
            saveFileDialog.RestoreDirectory = false;                       // false, do not remember the last folder location you opened
            saveFileDialog.InitialDirectory = filePath;
            saveFileDialog.FileName = folderName + ".pregps";                 // set fileName according to the currently dataSet

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Directory.CreateDirectory(filePath);                          // Create the file folder according to current time after saveDialog confirming
        }

        public void PreScan()
        {
            Console.WriteLine("Start pre scanning...");

            while (true)
            {
                float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
                Object thisLock = new Object();

                lock (thisLock)
                {

                    if (MainForm.g4Flag == true)
                    {
                        Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length);
                        Array.Copy(MainForm.sourceAER, quaternionArray, MainForm.sourceAER.Length);
                        //Console.WriteLine("x:{0},y:{1},z:{2}",xyzArray[0],xyzArray[1],xyzArray[2]);
                        savePreScanGPS.Add(xyzArray);
                        savePreScanGPS.Add(quaternionArray);
                        MainForm.g4Flag = false;
                    }


                }
            }


        }

        void SaveGPSDataTest()
        {
            int num = 2;

            float[] aa = new float[3];
            aa[0] = 1.2f;
            aa[1] = 1.3f;
            aa[2] = 1.4f;
            savePreScanGPS.Add(aa);

            float[] a2 = new float[4];
            a2[0] = 2.2f;
            a2[1] = 2.3f;
            a2[2] = 2.4f;
            a2[3] = 2.5f;
            savePreScanGPS.Add(a2);

            float[] a3 = new float[3];
            a3[0] = 3.2f;
            a3[1] = 3.3f;
            a3[2] = 3.4f;
            savePreScanGPS.Add(a3);

            float[] a4 = new float[4];
            a4[0] = 4.2f;
            a4[1] = 4.3f;
            a4[2] = 4.4f;
            a4[3] = 4.5f;
            savePreScanGPS.Add(a4);

            //byte[] bb = BitConverter.GetBytes(num);

            //saveGPS.Add(aa);
            float[][] array = savePreScanGPS.ToArray();

            //Console.WriteLine(array.Length);
            for (int i = 0; i < num; i++)
            {
                float[] xyz = array[i * 2];
                float[] ori = array[i * 2 + 1];
                for (int k = 0; k < 3; k++)
                    Console.WriteLine(xyz[k]);
                for (int j = 0; j < 4; j++)
                    Console.WriteLine(ori[j]);

                Console.WriteLine();
            }
            //Console.WriteLine(array.Length);
            //Console.WriteLine(array[0].Length);
            //const string fileName = @"C:\Users\BMU\Desktop\testest\us57.gps";
            //using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            //{
            //    binaryWriter.Write(num);
            //    for (int i = 0; i < array.Length; i++)
            //    {
            //        var arr = array[i];
            //        foreach (var ele in arr)
            //        {
            //            binaryWriter.Write(ele);
            //        }
            //    }
            //}

            //Console.WriteLine("Finished");
        }


        void SaveGPSData(string fileName)
        {
            int num = savePreScanGPS.Count / 2;
            byte[] numByte = BitConverter.GetBytes(num);

            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                foreach (var ele in numByte)
                {
                    binaryWriter.Write(ele);
                }

                foreach (var gpss in savePreScanGPS)
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

            Console.WriteLine("Finished");
        }

        /// <summary>
        /// Save location information from G4 to a .gps file
        /// </summary>
        /// <param name="fileName"></param>
        void SaveG4Data(string fileName)
        {

            Console.WriteLine("Save G4 data...");

            int couts = savePreScanGPS.Count / 2;
            float[][] array = savePreScanGPS.ToArray();    // Convert to jagged array

            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                binaryWriter.Write(couts);
                for (int i = 0; i < array.Length; i++)
                {
                    var arr = array[i];
                    foreach (var ele in arr)
                    {
                        binaryWriter.Write(ele);
                    }
                }
            }
            Console.WriteLine("Saved count: {0}, finished!", couts);

        }

        void GetRectsFromPreScanData()
        {
            int cout = savePreScanGPS.Count / 2;
            myRectangles = new Rect3[cout];

            Console.WriteLine(cout);
            float[][] gpsArray = savePreScanGPS.ToArray();
            for (int i = 0; i < cout; i++)
            {
                float[] xyz = gpsArray[i * 2];               // xyz
                float[] quaternion = gpsArray[i * 2 + 1];             // quatern

                getRect = new G4GetRects_Pre(xyz, quaternion, MainForm.LATERAL_Resolution);
                this.myRectangles[i] = new Rect3();
                this.myRectangles[i].v = getRect.GetRectangle();
            }
        }

        /// <summary>
        /// Calculate calibration matrix through the pre-scan location data
        /// </summary>
        public void CalculateCalibrationMatrix()
        {
            // Read G4 gps Data and calculate calibration matrix
            GetRectsFromPreScanData();

            Transformation transformation = new Transformation();
            transformation.voxelDepth = mvoxelDepth;
            transformation.voxelHeight = mvoxelHeight;
            transformation.voxelWidth = mvoxelWidth;
            transformation.rects = this.myRectangles;
            if (this.checkBoxPlumbLine.Checked)
            {
                MainForm.calibrationMatrix = transformation.GetCalibrationMatrixWyHxForPlumbLine();
            }
            else
            {
                MainForm.calibrationMatrix = transformation.GetCalibrationMatrixWyHx();
            }

            for (int k = 0; k < transformation.rects.Length; k++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 tempVector = new Vector3();
                    tempVector = transformation.rects[k].v[i];
                    transformation.rects[k].v[i] = Vector3.Transform(tempVector, MainForm.calibrationMatrix);
                }
            }

            Vector3 boxOrg = new Vector3();
            int adjustParas = adjustParasForVolume;
            transformation.CalculateBoxSizeWyHx(adjustParas, out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);

            Console.WriteLine("Pre-scan Completed!");
            Console.WriteLine("boxWidth, boxHeight, boxDepth are : {0}*{1}*{2}", boxWidth, boxHeight, boxDepth);
            Console.WriteLine("voxel size is: {0}*{1}*{2} ", voxelWidth, voxelHeight, voxelDepth);
            Console.WriteLine();

            // Transfer these paras to mainform after pre-scan
            MainForm.boxWidth = boxWidth;
            MainForm.boxHeight = boxHeight;
            MainForm.boxDepth = boxDepth;
            MainForm.boxOrg = boxOrg;
            MainForm.isCalibration = true;
            MainForm.voxelDepth = mvoxelDepth;
            MainForm.voxelHeight = mvoxelHeight;
            MainForm.voxelWidth = mvoxelWidth;

            // Refresh App setting
            Properties.Settings.Default.isCalibration = true;
            Properties.Settings.Default.CalibrationMatrix = MainForm.calibrationMatrix;
            Properties.Settings.Default.boxOrg = boxOrg;
            Properties.Settings.Default.boxDepth = boxDepth;
            Properties.Settings.Default.boxWidth = boxWidth;
            Properties.Settings.Default.boxHeight = boxHeight;
            Properties.Settings.Default.Save();
        }

        int keyPressCounts = 0;
        Thread threadRefreshUI;

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{

        //    // Detect a key was pressed
        //    if (keyData == Keys.Left)
        //    {
        //        PreScanStartStopControl();
        //    }
        //    return true;
        //    //return base.ProcessCmdKey(ref msg, keyData);
        //}

        public void PreScanStartStopControl()
        {
            keyPressCounts++;
            if (keyPressCounts == 1)
            {

                // For the first press, start GPS data acquisition
                //GetSavePath();
                savePreScanGPS.Clear();
                this.buttonStart.BackColor = SystemColors.Info;
                //Thread.Sleep(20);
                threadRefreshUI = new Thread(PreScan);

                threadRefreshUI.Start();
            }
            if (keyPressCounts == 2)
            {
                // For the second press, end GPS data acquisition and save it, then calculate calibration matrix. 
                if (!threadRefreshUI.Join(1500))
                {
                    threadRefreshUI.Abort();
                }

                Thread.Sleep(20);

                this.buttonStart.BackColor = SystemColors.Control;

                CalculateCalibrationMatrix();
                keyPressCounts = 0;
                Console.WriteLine("End Pre-scanning!");


                this.Dispose();
                GC.Collect();
            }
        }

        public void ReadG4(string path)
        {
            //string path = @"C:\Users\BMU\Desktop\Test\us57.gps";
            BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
            Console.WriteLine("[+]Reading {0}", path);

            int gpsEntriesCount = binaryReader.ReadInt32();
            Console.WriteLine("gps Cout is :{0}", gpsEntriesCount);
            //Rect3[] myRectangles;

            //myRectangles = new Rect3[gpsEntriesCount];
            myRectangles = new Rect3[gpsEntriesCount];
            // Read G4 data and transform to rect
            for (int i = 0; i < gpsEntriesCount; i++)
            {
                float[] g4XYZ = new float[3];
                float[] g4Quaternion = new float[4];

                for (int j = 0; j < g4XYZ.Length; j++)
                {
                    g4XYZ[j] = binaryReader.ReadSingle();

                }

                for (int j = 0; j < g4Quaternion.Length; j++)
                {
                    g4Quaternion[j] = binaryReader.ReadSingle();

                }

                getRect = new G4GetRects_Pre(g4XYZ, g4Quaternion, MainForm.LATERAL_Resolution);
                myRectangles[i] = new Rect3();
                myRectangles[i].v = getRect.GetRectangle();
            }

            binaryReader.Close();

            Console.WriteLine("-------- Read gps done --------");
            Console.WriteLine("\n");

        }


        public void ReadClarius(string path)
        {
            BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
            Console.WriteLine("[+]Reading {0}", path);

            int frameCounts = binaryReader.ReadInt32();
            int frameWidth = binaryReader.ReadInt32();
            int frameHeight = binaryReader.ReadInt32();
            int reso = binaryReader.ReadInt32();

            Console.WriteLine("Frame Cout is :{0}, w*h: {1}*{2}", frameCounts, frameWidth, frameHeight);
            Console.WriteLine("Resolution is: {0}", reso);
            mFrames = new byte[frameCounts][,];

            int readBytesSize = frameWidth * frameHeight;
            for (int i = 0; i < frameCounts; i++)
            {
                byte[] arr = binaryReader.ReadBytes(readBytesSize);
                byte[,] array2 = new byte[frameHeight, frameWidth];
                //将一维数组转换为二维数组
                for (int j = 0; j < readBytesSize; j++)
                {
                    array2[j / frameWidth, j % frameWidth] = arr[j];
                }
                this.mFrames[i] = array2;
            }
            binaryReader.Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("start..");
            //GetSavePath();
            PreScanStartStopControl();
        }

        private void voxelWidth_ValueChanged(object sender, EventArgs e)
        {
            mvoxelWidth = (float)voxelWidth.Value;
        }

        private void voxelHeight_ValueChanged(object sender, EventArgs e)
        {
            mvoxelHeight = (float)voxelHeight.Value;
        }

        private void voxelDepth_ValueChanged(object sender, EventArgs e)
        {
            mvoxelDepth = (float)voxelDepth.Value;
        }


        private void Frm_RealTime_PreScan_Load(object sender, EventArgs e)
        {
            mvoxelDepth = (float)voxelDepth.Value;
            mvoxelHeight = (float)voxelHeight.Value;
            mvoxelWidth = (float)voxelWidth.Value;
            adjustParasForVolume = (int)numericUpDownAdjustParas.Value;
            this.labelTop.Visible = false;
            this.labelBottom.Visible = false;
            this.buttonTop.Visible = false;
            this.buttonBottom.Visible = false;
            isUsedTopBottom = false;
            this.checkBoxPlumbLine.Checked = false;  // Default using Plumb Line
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            adjustParasForVolume = (int)numericUpDownAdjustParas.Value;
        }

        private void Frm_RealTime_PreScan_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            if (e.KeyData == Keys.Space)
            {
                this.buttonStart.PerformClick();
            }
        }

        private void buttonTop_Click(object sender, EventArgs e)
        {
            // Get current coordinate
            // 当这个按键按下时，获取当前的一个GPS信息即可
            float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
            Object thisLock = new Object();

            lock (thisLock)
            {

                Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length); this.labelTop.Visible = true;
                this.labelTop.Text = (xyzArray[0] * 10).ToString("#0.0") + "," + (xyzArray[1] * 10).ToString("#0.0") +
                 "," + (xyzArray[2] * 10).ToString("#0.0");
            }
            PlumbLineTop.X = xyzArray[0] * 10;
            PlumbLineTop.Y = xyzArray[1] * 10;
            PlumbLineTop.Z = xyzArray[2] * 10;

        }

        private void buttonBottom_Click(object sender, EventArgs e)
        {
            // Get current coordinate
            // 当这个按键按下时，获取当前的一个GPS信息即可
            float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
            Object thisLock = new Object();

            lock (thisLock)
            {

                Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length);
                this.labelBottom.Visible = true;
                this.labelBottom.Text = (xyzArray[0] * 10).ToString("#0.0") + "," + (xyzArray[1] * 10).ToString("#0.0") +
                 "," + (xyzArray[2] * 10).ToString("#0.0");
            }

            PlumbLineBottom.X = xyzArray[0] * 10;
            PlumbLineBottom.Y = xyzArray[1] * 10;
            PlumbLineBottom.Z = xyzArray[2] * 10;

            isUsedTopBottom = true; // set the flag of isUsedPlumbLine after the bottom button is cliked.
        }

    }
}
