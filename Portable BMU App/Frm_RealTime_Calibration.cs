using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portable_BMU_App
{
    public partial class Frm_RealTime_Calibration : Form
    {
        public static List<float[]> saveSensor1Data = new List<float[]>();
        public static List<float[]> saveSensor2Data = new List<float[]>();
        bool isStartButtonClicked = false;

        public Frm_RealTime_Calibration()
        {
            InitializeComponent();
        }



        public void GetSensorsData()
        {
            Console.WriteLine("Start pre scanning...");

            UnitInterFaceV733.G4Listener_GetSinglePno(MainForm.g4NewInfo);

            while (true)
            {
                float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
                float[] xyzArray2 = new float[3]; float[] quaternionArray2 = new float[4];

                Object thisLock = new Object();

                lock (thisLock)
                {

                    if (MainForm.g4Flag == true)
                    {
                        Array.Copy(MainForm.sourceXYZ, xyzArray, MainForm.sourceXYZ.Length);
                        Array.Copy(MainForm.sourceAER, quaternionArray, MainForm.sourceAER.Length);
                        //Console.WriteLine("x:{0},y:{1},z:{2}",xyzArray[0],xyzArray[1],xyzArray[2]);
                        saveSensor1Data.Add(xyzArray);
                        saveSensor1Data.Add(quaternionArray);
                        MainForm.g4Flag = false;
                    }
                    if (MainForm.g4Flag2 == true)
                    {
                        Array.Copy(MainForm.sourceXYZ2, xyzArray, MainForm.sourceXYZ.Length);
                        Array.Copy(MainForm.sourceAER2, quaternionArray, MainForm.sourceAER.Length);
                        //Console.WriteLine("x:{0},y:{1},z:{2}",xyzArray[0],xyzArray[1],xyzArray[2]);
                        saveSensor1Data.Add(xyzArray);
                        saveSensor1Data.Add(quaternionArray);
                        MainForm.g4Flag2 = false;
                    }                    
                }
            }
        }


        /// <summary>
        /// Save location information from navigation G4 to a .gps file
        /// </summary>
        /// <param name="fileName"></param>
        void SaveCalibrationG4Data(string sensor1FileName, string sensor2FileName)
        {
            // Sensor1
            Console.WriteLine("Save Sensor1 data...");

            int couts = saveSensor1Data.Count / 2;

            float[][] array = saveSensor1Data.ToArray();    // Convert to jagged array

            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(sensor1FileName, FileMode.Create)))
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
            Console.WriteLine("Saved Sensor1 GPS data count: {0}, finished!", couts);

            // Sensor2
            Console.WriteLine("Save Sensor2 data...");

            int couts2 = saveSensor2Data.Count / 2;

            float[][] array2 = saveSensor2Data.ToArray();    // Convert to jagged array

            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(sensor2FileName, FileMode.Create)))
            {
                binaryWriter.Write(couts);


                for (int i = 0; i < array2.Length; i++)
                {
                    var arr = array2[i];
                    foreach (var ele in arr)
                    {
                        binaryWriter.Write(ele);
                    }
                }
            }
            Console.WriteLine("Saved Sensor2 GPS data count: {0}, finished!", couts2);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string sensor1 = @"C:\Users\Chenhb\Desktop\Calibration Data\sensor1.g4gps";
            string sensor2 = @"C:\Users\Chenhb\Desktop\Calibration Data\sensor2.g4gps";

            SaveCalibrationG4Data(sensor1, sensor2);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            isStartButtonClicked = !isStartButtonClicked;
            if (isStartButtonClicked)
            {
                timer1.Interval = 1000; // 这里确定定时器间隔时间(ms)
                timer1.Enabled = true; // 使能
                timer1.Start(); // 启动定时器
                this.buttonStart.Text = "STOP";
            }
            else
            {
                timer1.Stop();
                this.buttonStart.Text = "START";
            }

        }
    }
}
