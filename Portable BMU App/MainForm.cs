using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;
//using MatlabFunction;
//using MathWorks.MATLAB.NET.Arrays;
namespace Portable_BMU_App
{
    public partial class MainForm : Form
    {
        public static UnitInterFaceV733.ChooseScannerType ChooseScannerType = new UnitInterFaceV733.ChooseScannerType();
        string defaultTextinMainForm = "Portable Real-time Processing App";
        // Units flag
        UInt32 clariusPort = 5828;
        string clariusIPAddress = "192.168.1.1";
        bool isG4Connected = false;
        bool isClariusConnected = false;
        public static bool isAllUnitsConencted = false;
        public static bool isClariusImaging = false;
        int keyPressCounts = 0;
        bool navigationKeyFlag = false;
        bool g4cFileFlag = false;

        // Path
        public string savePath = string.Empty;
        public string saveDataName = string.Empty;
        public string gpsConfigFilePath = string.Empty;
        public string g4cFilePathStandingPosition = string.Empty;
        public string g4cFilePathPronePosition = string.Empty;
        const string g4cExtern = ".g4c";
        const string g4cFileNameStandingPosition = "standing";
        const string g4cFileNamePronePosition = "prone";

        // Selected Saved Data Type
        public SavedDataType savedDataType = new SavedDataType();
        public string selectedPosition = null;   // default selected method is VNN in radioButton Group
        public bool isSavedData = false;
        // --------------------------- Recons paras ------------------------------//
        ReconstructionMethods2 reconstructionMethods;    //Recons Methods
        Thread threadRefreshUI;                     // Define a thread to diaplay 2D frame and reconstruced 3D projection image
        Thread threadRefreshNavigation;
        Thread threadImagingTrasverse;
        static USDataCollection2 usDataCollectionForRealTime = new USDataCollection2(); // Data Set
        public static MainForm mainForm;
        public const long BUFFERSIZE = 640 * 480;
        public static byte[] sourceB8 = new byte[BUFFERSIZE];
        public static float[] sourceXYZ = new float[3];
        public static float[] sourceAER = new float[4];
        public static float[] sourceXYZ2 = new float[3];
        public static float[] sourceAER2 = new float[4];

        public static Vector3 boxOrg = new Vector3(264.1527f, -137.4425f, -171.0527f);  // BoxSize and calibration Matrix
        public static int boxWidth = 310;                                               // BoxSize and calibration Matrix
        public static int boxHeight = 429;                                              // BoxSize and calibration Matrix
        public static int boxDepth = 389;                                               // BoxSize and calibration Matrix
        public static Matrix4x4 calibrationMatrix = new Matrix4x4();                    // BoxSize and calibration Matrix

        public static float voxelDepth = 1f;
        public static float voxelWidth = 0.5f;
        public static float voxelHeight = 0.5f;

        public Rect3 mRectangles = new Rect3();
        public static List<float[]> saveGPS = new List<float[]>();
        public static List<float[]> saveNavigation = new List<float[]>();

        public static List<byte[]> saveClarius = new List<byte[]>();
        byte[][,] boxV = new byte[boxDepth][,];
        Mat coronalImage = null;
        Mat saggitalImage = null;

        // --------------------------- Display paras ------------------------------//
        public static double LATERAL_Resolution = 0;
        public static int Image_Width = 0;
        public static int Image_Height = 0;
        OpenCvSharp.Size dSizeFor2DDisplay;
        OpenCvSharp.Size dSizeFor3DDisplay;
        OpenCvSharp.Size dSizeForSagittal;

        /// Some flags
        public static bool g4Flag = false;
        public static bool g4Flag2 = false;

        public static bool saveFlag = false;
        public static bool isCalibration = false;
        static bool isRecons = false;

        // --------------------------- Navigation paras ------------------------------//
        private const int RectSize = 20;
        private bool IsDrawing = false;
        private bool IsOver = false;
        private const int object_radius = RectSize / 2;         // The "size" of an object for mouse over purposes.
        private System.Drawing.Point MarkerLocation = new System.Drawing.Point(-RectSize / 2, -RectSize / 2);
        private System.Drawing.Point StartPointTrasverse = new System.Drawing.Point(-RectSize / 2, -RectSize / 2);
        private System.Drawing.Point EndPointTrasverse = new System.Drawing.Point(-RectSize / 2, -RectSize / 2);
        public int SegmentMuscleLevel = 0;
        // We're over an object if the distance squared
        // between the mouse and the object is less than this.
        private const int over_dist_squared = object_radius * object_radius;


        /// Clarius Equipment
        //private static UnitInterFace.ClariusNewProcessedImageFnCallBack clariusProcessedImageCallback = null;
        //private static UnitInterFace.ClariusNewRawImageCallback clariusNewRawImageCallBack = null;
        //private static UnitInterFace.ClariusErrorCallback clariusErrorCallback = null;
        //private static UnitInterFace.ClariusProgressCallback clariusProgressCallback = null;
        //private static UnitInterFace.ClariusFreezeCallback clariusFreezeCallback = null;
        //private static UnitInterFace.ClariusButtonCallback clariusButtonCallback = null;

        private static UnitInterFaceV733.ClariusNewProcessedImageFnCallBack clariusProcessedImageCallback = null;
        private static UnitInterFaceV733.ClariusNewRawImageCallback clariusNewRawImageCallBack = null;
        private static UnitInterFaceV733.ClariusErrorCallback clariusErrorCallback = null;
        private static UnitInterFaceV733.ClariusProgressCallback clariusProgressCallback = null;
        private static UnitInterFaceV733.ClariusFreezeCallback clariusFreezeCallback = null;
        private static UnitInterFaceV733.ClariusButtonCallback clariusButtonCallback = null;

        #region Control Related

        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += Form_Closing;
        }

        public void ToolStripButtonPreScanStatus(bool isEnable)
        {
            if (isEnable)
            {
                this.toolStripButtonPreScan.Enabled = true;
            }
            else
            {
                this.toolStripButtonPreScan.Enabled = false;
            }
        }

        void SomeInitializeWhenFormLoading()
        {
            // ------------ Setting initial status of Controls --------------------//
            this.buttonSegmentTrasverse.Enabled = false;
            this.toolStripButtonStartRecon.Enabled = false;
            this.toolStripButtonClariusStatus.Enabled = false;
            this.toolStripButtonPreScan.Enabled = false;
            this.toolStripButtonNavigation.Enabled = false;
            this.toolStripButtonReconsWithSelection.Enabled = true;
            this.toolStripLabelReconsWithSelection.Enabled = false;
            this.showConsoleToolStripMenuItem.Checked = false;
            //FreeConsole();

            // ------------ Initial Some paras -----------------------------------//
            dSizeFor2DDisplay = new OpenCvSharp.Size(pictureBox2DDisplay.Width, pictureBox2DDisplay.Height);
            dSizeFor3DDisplay = new OpenCvSharp.Size(real3DPictureBox.Width, real3DPictureBox.Height);
            dSizeForSagittal = new OpenCvSharp.Size(pictureBoxSagittalPlane.Width, pictureBoxSagittalPlane.Height);
            Console.WriteLine();


            // ----------- Get the scanner type ---------------------------------//
            ChooseScannerType = Properties.Settings.Default.ScannerType;
            Console.WriteLine(">>--- Scanner Type: {0} ---<<", ChooseScannerType.ToString());
            if (ChooseScannerType == UnitInterFaceV733.ChooseScannerType.Clarius)
            {
                clariusC3ToolStripMenuItem.Checked = true;
            }
            else
            {
                clariusC3HDToolStripMenuItem.Checked = true;

            }
            this.Text = defaultTextinMainForm + " —— " + ChooseScannerType.ToString();

            //Console.WriteLine("{0},{1}", pictureBoxSagittalPlane.Width, pictureBoxSagittalPlane.Height);

            //string[] path = new string[1];
            //path[0] = @"C:\Polhemus\PDI\PDI_140\Lib\x64";
            //AddEnvironmentPaths(path);

            // ----------- Initial Units, G4 and Clarius ------------------------//
            InitializeG4();
            InitializeClarius();

            // ----------- Load App Settings -----------------------------------//
            // ----------- if last experiment has done the calibration, we can use the calbration matrix generated by the last experiment ---//
            selectedPosition = Properties.Settings.Default.ScanPosition;
            if (selectedPosition == radioButtonPronePosition.Text)
            {
                radioButtonPronePosition.Checked = true;
            }
            else
            {
                radioButtonStandingPosition.Checked = true;
            }
            Console.WriteLine(">> Selected position mode: {0}", selectedPosition);
            this.gpsConfigFilePath = Properties.Settings.Default.gpsConfigFilePath;
            CheckGPSConfigurationFilePath();

            isCalibration = Properties.Settings.Default.isCalibration;
            //isCalibration = false;
            if (isCalibration)
            {
                Console.WriteLine(">> Load last data...");
                boxOrg = Properties.Settings.Default.boxOrg;
                boxWidth = Properties.Settings.Default.boxWidth;
                boxHeight = Properties.Settings.Default.boxHeight;
                boxDepth = Properties.Settings.Default.boxDepth;
                calibrationMatrix = Properties.Settings.Default.CalibrationMatrix;
                Console.WriteLine("  BoxWidth is: {0}, BoxHeight is: {1}, BoxDepth is: {2}", boxWidth, boxHeight, boxDepth);
                //Console.WriteLine(calibrationMatrix);

            }
            threadRefreshUI = new Thread(RealTimeReconstruction_TestAnyPosition);
            threadRefreshNavigation = new Thread(NavigationProcess);

        }

        // --------------------------- Control processing -------------------------------//
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            UnitInterFaceV733.clariusDestroyCast();


            if (isAllUnitsConencted)
            {
                DisconnectAllUnits();
            }
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {


            if (this.g4cFileFlag)
            {
                if (!isAllUnitsConencted)
                {
                    ConnectUnits();
                }
                else
                {

                    DisconnectAllUnits();
                }
            }
            else
            {
                MessageBox.Show("Please select the path for the GPS config file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //string str1 = Process.GetCurrentProcess().MainModule.FileName;//可获得当前执行的exe的文件名。
            //string str2 = Environment.CurrentDirectory;
            //string str3 = Directory.GetCurrentDirectory();//获取应用程序的当前工作目录
            //Console.WriteLine(str1);
            //Console.WriteLine(str2);
            //Console.WriteLine(str3);

            SomeInitializeWhenFormLoading();
        }

        private void toolStripButtonSaveData_Click(object sender, EventArgs e)
        {
            isSavedData = false; ;

            if (saveClarius.Count > 0 && saveGPS.Count > 0)
            {
                // Open save UI and select path to save
                Frm_RealTime_SaveUI frm_RealTime_SaveUI = new Frm_RealTime_SaveUI(this);
                frm_RealTime_SaveUI.ShowDialog();

                SaveAllAcquiredData();
                isSavedData = true;

            }
            else
            {
                MessageBox.Show("No Data Collection", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                isSavedData = false; ;


            }
        }

        private void toolStripButtonPreScan_Click(object sender, EventArgs e)
        {
            if (!isClariusImaging)
            {
                Portable_BMU_App.UnitInterFaceV733.clariusUserFunction(Portable_BMU_App.UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);
            }
            Frm_RealTime_PreScan frm_RealTime_PreScan = new Frm_RealTime_PreScan();
            frm_RealTime_PreScan.ShowDialog();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            dSizeFor2DDisplay = new OpenCvSharp.Size(pictureBox2DDisplay.Width, pictureBox2DDisplay.Height);

            dSizeFor3DDisplay = new OpenCvSharp.Size(real3DPictureBox.Width, real3DPictureBox.Height);
            dSizeForSagittal = new OpenCvSharp.Size(pictureBoxSagittalPlane.Width, pictureBoxSagittalPlane.Height);

        }

        private void toolStripButtonClariusStatus_Click(object sender, EventArgs e)
        {
            Portable_BMU_App.UnitInterFaceV733.clariusUserFunction(Portable_BMU_App.UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;  // 表示该事件已处理。用来屏蔽按钮对键盘空格的响应

            if (e.KeyData == Keys.Space)
            {
                this.toolStripButtonStartRecon.PerformClick();
            }
            else if (e.KeyData == Keys.N)
            {
                this.toolStripButtonNavigation.PerformClick();
            }
        }

        private void toolStripTextBoxPortSetting_TextChanged(object sender, EventArgs e)
        {
            string port = toolStripTextBoxPortSetting.Text;
            clariusPort = Convert.ToUInt32(port);
            Console.WriteLine(clariusPort);
        }

        private void toolStripTextBoxPortSetting_Validated(object sender, EventArgs e)
        {
            if (toolStripTextBoxPortSetting.Text == null | toolStripTextBoxPortSetting.Text == "")
            {
                MessageBox.Show("The port cannot be empty ！", "Please Set port");
            }
            else
            {
                string port = toolStripTextBoxPortSetting.Text;
                clariusPort = Convert.ToUInt32(port);
            }
        }

        private void userManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + @"\User Manual.chm");
        }

        // 启动控制台
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool AllocConsole();

        // 释放控制台
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool FreeConsole();

        private void showConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showConsoleToolStripMenuItem.Checked == true)
            {
                FreeConsole();
                showConsoleToolStripMenuItem.Checked = false;
            }
            else
            {
                AllocConsole();
                showConsoleToolStripMenuItem.Checked = true;
            }
        }
        #endregion


        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    //threadRefreshUI.IsBackground = true;

        //    // Detect a key was pressed
        //    if (keyData == Keys.Space)
        //    {
        //        ReconstrutionStartStopControl();
        //    }
        //    return true;
        //    //return base.ProcessCmdKey(ref msg, keyData);
        //}


        #region Units Related

        /// G4 Equipment
        public static void g4NewInfo(uint v1, uint v2, ref UnitInterFaceV733.G4_SENSORDATA g4Info)
        {
            //saveGPS.Add(g4Info.pos);
            //saveGPS.Add(g4Info.ori);
            //Console.WriteLine("[+]Show G4 ID is :" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n");


            Object thisLock = new Object();
            lock (thisLock)
            {
                if (g4Info.nSnsID == 0) // If there has two sensor
                {
                    Array.Copy(g4Info.pos, sourceXYZ, g4Info.pos.Length);
                    Array.Copy(g4Info.ori, sourceAER, g4Info.ori.Length);
                    g4Flag = true;
                    //Console.WriteLine("G4");

                }
                else
                {
                    Array.Copy(g4Info.pos, sourceXYZ2, g4Info.pos.Length);
                    Array.Copy(g4Info.ori, sourceAER2, g4Info.ori.Length);
                    g4Flag2 = true;
                }
            }
            //Console.Write("SensorID is : {0},FrameC is {1}", g4Info.nSnsID, v2);
            //Console.Write("{0}, {1}, {2}", sourceXYZ[0].ToString("0.00"), sourceXYZ[1].ToString("0.00"), sourceXYZ[2].ToString("0.00"));
            //Console.WriteLine();
            //Console.Write(" | ");
            //Console.WriteLine("{0}, {1}, {2},{3}", sourceAER[0].ToString("0.00"), sourceAER[1].ToString("0.00"), sourceAER[2].ToString("0.00"), sourceAER[3].ToString("0.00"));
            //Console.WriteLine("Here");
        }

        void InitializeG4()
        {
            // Initial G4  
            if ((UnitInterFaceV733.Initialize()))
            {
                Console.WriteLine("Initial G4 Successfully!");
            }
        }

        void ConnetG4()
        {
            // file path
            //string g_G4CFilePath = @"C:\Users\Chenhb\Desktop\Debug_Fig\0723.g4c";
            string g_G4CFilePath = string.Empty;

            if (selectedPosition == radioButtonStandingPosition.Text)
            {
                g_G4CFilePath = this.g4cFilePathStandingPosition;
            }
            else if (selectedPosition == radioButtonPronePosition.Text)
            {
                g_G4CFilePath = this.g4cFilePathPronePosition;

            }
            else
            {
                MessageBox.Show("Please set the gps config file(*.g4c) path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //g_G4CFilePath = @"C:\Users\BMU\Desktop\Debug_Fig\0723.g4c";
            //Connect To Tracker
            if (!(UnitInterFaceV733.Connect(g_G4CFilePath)))
            {
                Console.WriteLine("Connect GPS failed...");
                isG4Connected = false;
                MessageBox.Show("Connect GPS Failed!", "ERROR");
                return;

            }
            //Configure Tracker
            else if (!(UnitInterFaceV733.SetupDevice()))
            {
                isG4Connected = false;
                MessageBox.Show("Connect GPS Failed!", "ERROR");
                return;
            }
            else
            {
                Console.WriteLine("Connect GPS Successfully!");
                isG4Connected = true;
            }
        }

        /// <summary>
        /// 添加环境变量路径，以引用G4的库
        /// </summary>
        /// <param name="paths"></param>
        static void AddEnvironmentPaths(IEnumerable<string> paths)
        {
            var path = new[] { Environment.GetEnvironmentVariable("PATH") ?? string.Empty };

            string newPath = string.Join(Path.PathSeparator.ToString(), path.Concat(paths));

            Environment.SetEnvironmentVariable("PATH", newPath);

        }

        public void ConnectUnits()
        {
            if (!isG4Connected)
            {
                ConnetG4();
            }

            if (isG4Connected)
            {
                if (!isClariusConnected)
                    ConnectClarius();
            }

            if (isG4Connected && isClariusConnected)
            {
                isAllUnitsConencted = true;
                // Change to connected icon
                this.groupBoxPosition.Enabled = false;
                this.toolStripButtonConnectUnits.Image = Properties.Resources.connected;
                this.toolStripButtonConnectUnits.Text = "Disconnect";

                this.toolStripLabelConnectUnits.Text = "Connected";
                this.toolStripButtonStartRecon.Enabled = true;
                this.toolStripButtonPreScan.Enabled = true;
                this.toolStripButtonClariusStatus.Enabled = true;
                this.buttonSegmentTrasverse.Enabled = true;
                Portable_BMU_App.UnitInterFaceV733.clariusUserFunction(Portable_BMU_App.UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            }
        }

        public void DisconnectAllUnits()
        {
            // IF the following threads are aborted
            if (threadRefreshUI.ThreadState == System.Threading.ThreadState.Running)
            {
                Console.WriteLine("threadRefreshUI");
                if (!threadRefreshUI.Join(2000))
                {
                    threadRefreshUI.Abort();
                }
            }

            if (threadRefreshNavigation.ThreadState == System.Threading.ThreadState.Running)
            {
                Console.WriteLine("threadRefreshNavigation");

                if (!threadRefreshNavigation.Join(2000))
                {
                    threadRefreshNavigation.Abort();
                }
            }

            if (isClariusImaging)
            {
                Portable_BMU_App.UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);
            }

            if (Portable_BMU_App.UnitInterFaceV733.clariusDisconnect(null) == 0)
            {
                Console.WriteLine("Clarius Disconnected!");
                isClariusConnected = false;

            }

            if (Portable_BMU_App.UnitInterFaceV733.DisConnect() == true)
            {
                Console.WriteLine("G4 Disconnected!");
                isG4Connected = false;
            }

            if (!isClariusConnected && !isG4Connected)
            {
                isAllUnitsConencted = false;
                // change to disconnected icon
                this.groupBoxPosition.Enabled = true;
                this.toolStripButtonConnectUnits.Image = Properties.Resources.disconnected2;
                this.toolStripLabelConnectUnits.Text = "No Connection";
                this.toolStripButtonConnectUnits.Text = "Connect";

                //this.toolStripButtonSaveData.Enabled = false;
                this.toolStripButtonStartRecon.Enabled = false;
                this.toolStripButtonClariusStatus.Enabled = false;
                this.toolStripButtonPreScan.Enabled = false;
                this.toolStripButtonNavigation.Enabled = false;
                this.buttonSegmentTrasverse.Enabled = false;

            }
        }

        void InitializeClarius()
        {
            // initialize with callbacks
            int argc = 5;
            string ipAddress = "192.168.1.1";
            //UInt32 port = 46771;
            UInt32 port = 5828;
            string portString = port.ToString();
            string[] argv = new string[5]; ;
            const int width = 640;
            const int height = 480;

            argv[0] = "xxx";
            argv[1] = "--address";
            argv[2] = ipAddress;
            argv[3] = "--port";
            argv[4] = portString;
            string keydir = "/tmp/";
            //IntPtr keydirPointer = Marshal.StringToHGlobalAnsi(keydir);


            clariusProcessedImageCallback = new Portable_BMU_App.UnitInterFaceV733.ClariusNewProcessedImageFnCallBack(newImageFn);
            clariusNewRawImageCallBack = new Portable_BMU_App.UnitInterFaceV733.ClariusNewRawImageCallback(newRawImageFn);

            //ClariusNewFrameCallback clariusNewFrameCallback = new ClariusNewFrameCallback(newImageFn);
            clariusErrorCallback = new Portable_BMU_App.UnitInterFaceV733.ClariusErrorCallback(errorFn);
            clariusProgressCallback = new Portable_BMU_App.UnitInterFaceV733.ClariusProgressCallback(progressFn);
            clariusFreezeCallback = new Portable_BMU_App.UnitInterFaceV733.ClariusFreezeCallback(freezeFn);
            clariusButtonCallback = new Portable_BMU_App.UnitInterFaceV733.ClariusButtonCallback(buttonFn);
            Portable_BMU_App.UnitInterFaceV733.ClariusReturnCallback clariusReturnCallback = new Portable_BMU_App.UnitInterFaceV733.ClariusReturnCallback(returnFn);

            if (Portable_BMU_App.UnitInterFaceV733.clariusInitCast(argc, argv, keydir, clariusProcessedImageCallback, clariusNewRawImageCallBack, clariusFreezeCallback, clariusButtonCallback, clariusProgressCallback, clariusErrorCallback, null, width, height) < 0)
            {
                Console.WriteLine("Could not initialize listener!");
                isClariusConnected = false;
                return;
            }
            else
            {
                Console.WriteLine("Initial Clarius Successfully!");
            }
        }

        void ConnectClarius()
        {
            string ipAddress = clariusIPAddress;

            UInt32 port = clariusPort;
            string portString = port.ToString();
            string[] argv = new string[5]; ;


            argv[0] = "xxx";
            argv[1] = "--address";
            argv[2] = ipAddress;
            argv[3] = "--port";
            argv[4] = portString;

            if (UnitInterFaceV733.clariusConnect(argv[2], port, null) < 0)
            {
                Console.WriteLine("Failed to connect SCanner!");
                MessageBox.Show("Connect Scanner Failed!", "ERROR");

                isClariusConnected = false;
                return;

            }
            else
            {
                Console.WriteLine("Connect Scanner Successfully!");
                isClariusConnected = true;
            }
        }

        void newImageFn(IntPtr buffPointer, ref Portable_BMU_App.UnitInterFaceV733.ClariusProcessedImageInfo nfo, int npos, ref Portable_BMU_App.UnitInterFaceV733.ClariusPosInfo pos)
        {
            //Console.WriteLine("Clarius");

            //saveFlag = false;
            //Console.WriteLine("[+]Show Clarius ID is :" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n");
            int size = 640 * 480 * (4);
            byte[] buff = new byte[size];
            Marshal.Copy(buffPointer, buff, 0, size);

            byte[] b8FrameVector = new byte[480 * 640];

            for (int i = 0; i < size / 4; i++)
            {
                int idx = 4 * i;
                b8FrameVector[i] = buff[idx];
            }

            //Console.WriteLine(nfo.micronsPerPixel);
            LATERAL_Resolution = nfo.micronsPerPixel;
            Image_Width = nfo.width;
            Image_Height = nfo.height;

            Object thisLock = new Object();
            lock (thisLock)
            {

                Array.Copy(b8FrameVector, sourceB8, b8FrameVector.Length);

                saveFlag = true;

            }

            UnitInterFaceV733.G4Listener_GetSinglePno(g4NewInfo);


        }

        void returnFn(int retCode)
        {

        }

        void freezeFn(int val)
        {
            if (Convert.ToBoolean(val))
            {
                Console.WriteLine("Frozen");
                isClariusImaging = false;

            }
            else
            {
                Console.WriteLine("Imaging");
                isClariusImaging = true;
            }

            ChangeClariusStatus();
        }

        private delegate void ChangeClariusStatusDelegate();
        void ChangeClariusStatus()
        {
            if (this.InvokeRequired)
            {
                ChangeClariusStatusDelegate changeClariusStatusDelegate = ChangeClariusStatus;
                this.Invoke(changeClariusStatusDelegate);

                return;
            }

            if (isClariusImaging)
            {
                this.toolStripButtonClariusStatus.Text = "Frozen";
                this.toolStripButtonClariusStatus.Image = Properties.Resources.imaging;
                this.toolStripLabelClariusStatus.Text = "Imaging";
            }
            else
            {
                this.toolStripButtonClariusStatus.Text = "Imaging";
                this.toolStripButtonClariusStatus.Image = Properties.Resources.frozen;
                this.toolStripLabelClariusStatus.Text = "Frozen";
            }
        }

        void errorFn(string err)
        {
            Console.WriteLine("error" + err);
        }

        void progressFn(int progress)
        {
            Console.WriteLine("download:" + progress);
        }

        void newRawImageFn(IntPtr buffPointer, ref Portable_BMU_App.UnitInterFaceV733.ClariusRawImageInfo nfo, int npos, ref Portable_BMU_App.UnitInterFaceV733.ClariusPosInfo pos)
        {
            //Console.WriteLine("...");

            //saveFlag = false;
            //Console.WriteLine("[+]Show Clarius ID is :" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n");
            //int size = 640 * 480 * 4;
            //byte[] buff = new byte[size];
            //Marshal.Copy(buffPointer, buff, 0, size);

            //byte[] b8FrameVector = new byte[480 * 640];

            //for (int i = 0; i < size / 4; i++)
            //{
            //    int idx = 4 * i;
            //    b8FrameVector[i] = buff[idx];
            //}

            //LATERAL_Resolution = nfo.micronsPerPixel;
            //Image_Width = nfo.width;
            //Image_Height = nfo.height;

            //Object thisLock = new Object();
            //lock (thisLock)
            //{
            //    Array.Copy(b8FrameVector, sourceB8, b8FrameVector.Length);
            //}


            ////UnitInterFace.G4Listener_GetSinglePno(g4NewInfo);

            //g4Flag = true;
            //saveFlag = true;

        }

        void buttonFn(int btn, int clicks)
        {
            if (Convert.ToBoolean(btn))
                Console.WriteLine("down button pressed, clicks:" + clicks);
            else
                Console.WriteLine("up");
            Console.WriteLine("up button pressed, clicks:" + clicks);
        }

        #endregion

        #region Save Data

        void SaveImage(string filename)
        {
            //string filename = @"C:\Users\BMU\Desktop\ImgFromBMU\coronalImage2.jpg";
            Console.Write("Save Image... ");
            Cv2.ImWrite(filename, coronalImage);
            Console.WriteLine("success!");
        }

        public void SaveAllAcquiredData()
        {




            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // Save data according to selected data type
            string[] typeNames = Enum.GetNames(typeof(SavedDataType));
            int lengthOfSavedDataType = typeNames.Length;

            if (this.savePath != string.Empty)
            {
                Directory.CreateDirectory(this.savePath);
                if (Convert.ToBoolean(savedDataType & SavedDataType.Volume))
                {
                    string fileNameMat = Path.Combine(this.savePath, this.saveDataName) + "_RealTime.mat";
                    Frm_Recons_DataExport.SaveVolume2Mat(fileNameMat, boxV, this.saveDataName, voxelWidth, voxelHeight, voxelDepth, true);
                    //reconstructionMethods.SaveVolumeToMat(boxV, boxWidth, boxHeight, boxDepth, fileNameMat);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.TransVerseImage))
                {
                    string fileNameClarius = Path.Combine(this.savePath, this.saveDataName) + ".clarius";
                    SaveClariusData(fileNameClarius);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.LocationData))
                {
                    string fileNameG4 = Path.Combine(this.savePath, this.saveDataName) + ".g4gps";
                    SaveG4Data(fileNameG4);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.CoronalImage))
                {
                    string fileNameJpg = Path.Combine(this.savePath, this.saveDataName) + ".jpg";
                    SaveImage(fileNameJpg);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.NavigationData))
                {
                    string fileNameNaGPS = Path.Combine(this.savePath, this.saveDataName) + ".nagps";
                    SaveNavigationG4Data(fileNameNaGPS);
                }

            }


            this.Cursor = System.Windows.Forms.Cursors.Arrow;

        }

        public void SaveAllAcquiredData(string savePath, string saveDataName)
        {




            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // Save data according to selected data type
            string[] typeNames = Enum.GetNames(typeof(SavedDataType));
            int lengthOfSavedDataType = typeNames.Length;

            if (savePath != string.Empty)
            {
                Directory.CreateDirectory(savePath);
                if (Convert.ToBoolean(savedDataType & SavedDataType.Volume))
                {
                    string fileNameMat = Path.Combine(savePath, saveDataName) + "_RealTime.mat";
                    Frm_Recons_DataExport.SaveVolume2Mat(fileNameMat, boxV, this.saveDataName, voxelWidth, voxelHeight, voxelDepth, true);
                    //reconstructionMethods.SaveVolumeToMat(boxV, boxWidth, boxHeight, boxDepth, fileNameMat);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.TransVerseImage))
                {
                    string fileNameClarius = Path.Combine(savePath, saveDataName) + ".clarius";
                    SaveClariusData(fileNameClarius);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.LocationData))
                {
                    string fileNameG4 = Path.Combine(savePath, saveDataName) + ".g4gps";
                    SaveG4Data(fileNameG4);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.CoronalImage))
                {
                    string fileNameJpg = Path.Combine(savePath, saveDataName) + ".jpg";
                    SaveImage(fileNameJpg);
                }

                if (Convert.ToBoolean(savedDataType & SavedDataType.NavigationData))
                {
                    string fileNameNaGPS = Path.Combine(savePath, saveDataName) + ".nagps";
                    SaveNavigationG4Data(fileNameNaGPS);
                }

            }


            this.Cursor = System.Windows.Forms.Cursors.Arrow;

        }


        /// <summary>
        /// Save location information from G4 to a .gps file
        /// </summary>
        /// <param name="fileName"></param>
        void SaveG4Data(string fileName)
        {

            Console.Write("Save G4 data... ");

            int couts = saveGPS.Count / 2;

            float[][] array = saveGPS.ToArray();    // Convert to jagged array

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
            Console.WriteLine(" count: {0}, finished!", couts);

        }

        /// <summary>
        /// Save location information from navigation G4 to a .gps file
        /// </summary>
        /// <param name="fileName"></param>
        void SaveNavigationG4Data(string fileName)
        {

            Console.Write("Save Navigation data...  ");

            int couts = saveNavigation.Count / 2;

            float[][] array = saveNavigation.ToArray();    // Convert to jagged array

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
            Console.WriteLine("count: {0}, finished!", couts);

        }

        /// <summary>
        /// save clarius 2D frame to .clarius file
        /// </summary>
        void SaveClariusData(string fileName)
        {
            Console.Write("Save Clarius image...");
            int couts = saveClarius.Count;
            byte[][] array = saveClarius.ToArray();    // Convert to jagged array
            int imgWidth = Image_Width;
            int imgHeight = Image_Height;
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                binaryWriter.Write(couts);
                binaryWriter.Write(imgWidth);
                binaryWriter.Write(imgHeight);
                binaryWriter.Write(LATERAL_Resolution);

                for (int i = 0; i < array.Length; i++)
                {
                    var arr = array[i];
                    foreach (var ele in arr)
                    {
                        binaryWriter.Write(ele);
                    }
                }
            }
            Console.WriteLine(" counts: {0}, finished!", couts);

        }

        #endregion

        #region GPS Configuration
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Determine which position is selected
            RadioButton radioButton = sender as RadioButton;
            selectedPosition = radioButton.Text;
            Console.WriteLine("Selected position mode: {0}", radioButton.Text);
            Properties.Settings.Default.ScanPosition = selectedPosition;
            Properties.Settings.Default.Save();
        }

        void CheckGPSConfigurationFilePath()
        {
            /*------ Find if g4C file in gpsConfigFilePath path  --------*/
            if (this.gpsConfigFilePath != string.Empty)
            {
                Console.WriteLine("GPS configuration file path: {0}", this.gpsConfigFilePath);

                string[] filesInFolder = null;
                try
                {
                    filesInFolder = Directory.GetFiles(this.gpsConfigFilePath);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return;
                }

                List<string> g4cFileFullPath = new List<string>();
                g4cFileFlag = false;

                foreach (string file in filesInFolder)
                {
                    string extension = Path.GetExtension(file);
                    if (extension == g4cExtern)
                    {
                        g4cFileFlag = true;
                        g4cFileFullPath.Add(file);
                        //Console.WriteLine(file);
                    }
                }
                if (!g4cFileFlag)
                {
                    MessageBox.Show("No GPS cpnfiguration(.g4c) file in this folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.g4cFilePathStandingPosition = string.Empty;
                this.g4cFilePathPronePosition = string.Empty;

                for (int i = 0; i < g4cFileFullPath.Count; i++)
                {
                    string filename = Path.GetFileNameWithoutExtension(g4cFileFullPath[i]);
                    //Console.WriteLine(filename);
                    if (filename == g4cFileNameStandingPosition)
                    {
                        this.g4cFilePathStandingPosition = g4cFileFullPath[i];
                    }
                    if (filename == g4cFileNamePronePosition)
                    {
                        this.g4cFilePathPronePosition = g4cFileFullPath[i];
                    }

                }
                if (this.g4cFilePathStandingPosition == string.Empty || this.g4cFilePathPronePosition == string.Empty)
                {
                    MessageBox.Show("No stading/prone cpnfiguration(.g4c) file in this folder!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    Properties.Settings.Default.gpsConfigFilePath = this.gpsConfigFilePath;
                    Properties.Settings.Default.Save();
                }


            }
            else
            {
                MessageBox.Show("Please select the path for the GPS config file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void gPSConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.gpsConfigFilePath = string.Empty;
            /*------ Prompts user to select a folder where the GPS Config file is ------*/
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Pleae select the path of GPS config file  ";
            folderBrowserDialog.ShowNewFolderButton = false;                  // Forbide create new folder
            folderBrowserDialog.SelectedPath = @"D:\";
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;


            this.gpsConfigFilePath = folderBrowserDialog.SelectedPath;

            CheckGPSConfigurationFilePath();
        }

        #endregion

        #region Reconstruction Process
        private void toolStripButtonStartRecon_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("...");
            ReconstrutionStartStopControl();
        }

        void ReconstrutionStartStopControl()
        {
            keyPressCounts++;
            if (keyPressCounts == 1)
            {
                ReconstrutionStart();
            }
            if (keyPressCounts == 2)
            {
                ReconstrutionStop();
            }
        }

        void ReconstrutionStart()
        {
            saveClarius.Clear();
            saveGPS.Clear();
            g4Flag = false;
            g4Flag2 = false;

            isSavedData = false;

            isRecons = true;
            if (!isClariusImaging)
                UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            Console.WriteLine("Start Reconstruction...");

            this.toolStripButtonStartRecon.Image = Properties.Resources.stop;
            this.toolStripButtonStartRecon.Text = "Stop Real Time Reconstruction";
            this.toolStripLabelStartRecons.Text = "Stop";
            this.toolStripButtonSaveData.Enabled = false;
            this.toolStripButtonNavigation.Enabled = false;
            this.toolStripButtonPostProcessing.Enabled = false;

            // Input usData collection and selected recons method
            usDataCollectionForRealTime.voxelWidth = voxelWidth;
            usDataCollectionForRealTime.voxelHeight = voxelHeight;
            usDataCollectionForRealTime.voxelDepth = voxelDepth;
            reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.RealTimePNN, usDataCollectionForRealTime);

            StartReconstruction();
        }
        void ReconstrutionStop()
        {
            isRecons = false;

            if (isClariusImaging)
                UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            Thread.Sleep(50);
            if (!threadRefreshUI.Join(2000))
            {
                threadRefreshUI.Abort();
            }
            Console.WriteLine("Stop Reconstruction!");

            this.toolStripButtonStartRecon.Image = Properties.Resources.start;
            this.toolStripButtonStartRecon.Text = "Start Real Time Reconstruction";
            this.toolStripLabelStartRecons.Text = "Start";
            this.toolStripButtonSaveData.Enabled = true;
            this.toolStripButtonNavigation.Enabled = true;
            this.toolStripButtonPostProcessing.Enabled = true;

            keyPressCounts = 0;


            g4Flag = false;
            g4Flag2 = false;


        }

        public void StartReconstruction()
        {
            //isCalibration = false;
            // 如果校准已完成，则开始实时重建。否则开启另一个线程，只显示二维图像和
            if (isCalibration)
            {
                threadRefreshUI = new Thread(RealTimeReconstruction);
                //threadRefreshUI.IsBackground = true;

                Thread.Sleep(20);
                reconstructionMethods.mboxDepth = boxDepth;
                reconstructionMethods.mboxWidth = boxWidth;
                reconstructionMethods.mboxHeight = boxHeight;
                reconstructionMethods.mboxOrg = boxOrg;
                Console.WriteLine("dep,wid,hei is {0},{1},{2}", boxDepth, boxWidth, boxHeight);
                // 点击开始按钮。打开重建线程
                Console.WriteLine("Start 3D reconstruction...");


                threadRefreshUI.Start();
            }
            else
            {
                threadRefreshUI = new Thread(RealTimeDisplay2DImage);
                threadRefreshUI.IsBackground = true;
                Thread.Sleep(20);
                Console.WriteLine("Start 2D imaging...");
                threadRefreshUI.Start();
            }
        }

        public void RealTimeDisplay2DImage()
        {
            // Get Data
            int arraySize = 480 * 640;
            byte[,] b8Frame = new byte[480, 640];

            while (true)
            {
                float[] xyzArray = new float[3];
                float[] aerArray = new float[4];

                //Thread.Sleep(5);
                byte[] b8ByteArray = new byte[BUFFERSIZE];

                bool reconsFlag = false;
                Object thisLock = new Object();
                lock (thisLock)
                {
                    if (g4Flag)
                    {
                        Array.Copy(sourceB8, b8ByteArray, sourceB8.Length);
                        //Array.Copy(sourceXYZ, xyzArray, sourceXYZ.Length);
                        //Array.Copy(sourceAER, aerArray, sourceAER.Length);
                        //Console.Write("{0}, {1}, {2}", xyzArray[0].ToString("0.00"), xyzArray[1].ToString("0.00"), xyzArray[2].ToString("0.00"));
                        //Console.WriteLine();
                        reconsFlag = true;
                        //saveGPS.Add(xyzArray);
                        //saveGPS.Add(aerArray);
                        //saveClarius.Add(b8ByteArray);
                    }

                }

                if (reconsFlag)
                {
                    //Console.WriteLine("Come in reconstruction");
                    DateTime beforeTime = DateTime.Now;

                    //将一维数组转换为二维数组,得到Frame
                    for (int j = 0; j < arraySize; j++)
                    {
                        b8Frame[j / 640, j % 640] = b8ByteArray[j];
                    }

                    // 镜像翻转二维图像
                    for (int i = 0; i < 480; i++)
                    {
                        for (int j = 0; j < 640 / 2; j++)
                        {
                            byte temp = b8Frame[i, j];
                            b8Frame[i, j] = b8Frame[i, 640 - j - 1];
                            b8Frame[i, 640 - j - 1] = temp;
                        }
                    }

                    Mat srcG2D = ConvertFile.ArrayToMat(b8Frame);
                    Mat dstG2D = new Mat();
                    Cv2.Resize(srcG2D, dstG2D, dSizeFor2DDisplay);
                    Cv2.ApplyColorMap(dstG2D, dstG2D, ColormapTypes.Jet);
                    Bitmap bitmapG2D = ConvertFile.MatToBitmap(dstG2D);


                    this.pictureBox2DDisplay.Invoke((MethodInvoker)delegate { PictureBoxShow2D(bitmapG2D); });
                    dstG2D = null;

                    reconsFlag = false;
                }

            }
        }

        public void RealTimeReconstruction_TestAnyPosition()
        {

            // Get Data
            int arraySize = 480 * 640;
            byte[,] b8Frame = new byte[480, 640];
            byte[,] lastCoronal = new byte[boxDepth, boxWidth];
            byte[,] lastSagittal = new byte[boxDepth, boxHeight];

            byte[,] coronalProjection = new byte[boxDepth, boxWidth];
            byte[,] sagittalProjection = new byte[boxDepth, boxHeight];

            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxWidth; k2++)
                {
                    coronalProjection[k1, k2] = 0;
                }
            }


            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxHeight; k2++)
                {
                    lastSagittal[k1, k2] = 0;
                }
            }

            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxHeight; k2++)
                {
                    sagittalProjection[k1, k2] = 0;
                }
            }


            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxWidth; k2++)
                {
                    lastCoronal[k1, k2] = 0;
                }
            }

            byte[][,] boxValues = new byte[boxDepth][,];
            for (int ik = 0; ik < boxDepth; ik++)
            {
                boxValues[ik] = new byte[boxHeight, boxWidth];
            }

            while (isRecons)
            {
                bool reconsFlag = false;
                byte[] clariusByteArray = new byte[BUFFERSIZE];
                float[] xyzArray = new float[3]; float[] aerArray = new float[4];

                //Console.WriteLine(g4Flag);
                Object thisLock = new Object();
                lock (thisLock)
                {
                    if (g4Flag == true)
                    {
                        Array.Copy(sourceB8, clariusByteArray, sourceB8.Length);
                        Array.Copy(sourceXYZ, xyzArray, sourceXYZ.Length);
                        Array.Copy(sourceAER, aerArray, sourceAER.Length);

                        saveGPS.Add(xyzArray);
                        saveGPS.Add(aerArray);
                        saveClarius.Add(clariusByteArray);

                        g4Flag = false;
                        reconsFlag = true;
                    }


                    //saveGPS.Add(gpsByteArray);
                    //saveB8.Add(b8ByteArray);
                }

                if (reconsFlag)
                {
                    //DateTime beforeTime = DateTime.Now;

                    //将一维数组转换为二维数组,得到Frame
                    for (int j = 0; j < arraySize; j++)
                    {
                        b8Frame[j / 640, j % 640] = clariusByteArray[j];
                    }

                    // 镜像翻转二维图像
                    for (int i = 0; i < 480; i++)
                    {
                        for (int j = 0; j < 640 / 2; j++)
                        {
                            byte temp = b8Frame[i, j];
                            b8Frame[i, j] = b8Frame[i, 640 - j - 1];
                            b8Frame[i, 640 - j - 1] = temp;
                        }
                    }

                    Mat srcG2D = ConvertFile.ArrayToMat(b8Frame);
                    Mat dstG2D = new Mat();
                    Cv2.Resize(srcG2D, dstG2D, dSizeFor2DDisplay);
                    Cv2.ApplyColorMap(dstG2D, dstG2D, ColormapTypes.Jet);
                    Bitmap bitmapG2D = ConvertFile.MatToBitmap(dstG2D);
                    dstG2D = null;

                    this.pictureBox2DDisplay.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow2D(bitmapG2D); }));


                    // Remove Muscle
                    if (SegmentMuscleLevel != 0)
                    {
                        for (int j = 0; j < SegmentMuscleLevel; j++)
                        {
                            for (int k = 0; k < 640; k++)
                            {
                                b8Frame[j, k] = 0;
                            }
                        }
                    }


                    G4GetRects_Pre getRect = new G4GetRects_Pre(xyzArray, aerArray, LATERAL_Resolution);
                    this.mRectangles.v = getRect.GetRectangle();
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.mRectangles.v[i];
                        this.mRectangles.v[i] = Vector3.Transform(tempVector, calibrationMatrix);
                    }



                    // 传入XYZ和AER
                    //G = reconstructionMethods.RealTimePNN_V1(this.m_Rectangles, b8Frame, boxValues);
                    //boxValues = reconstructionMethods.RealTimePNN_V1(this.mRectangles, b8Frame, boxValues, out coronalProjection);
                    //coronalProjection = reconstructionMethods.GetMatrixsMax(coronalProjection, lastCoronal);
                    boxValues = reconstructionMethods.RealTimePNN_Coronal_Sagittal_WyHx(this.mRectangles, b8Frame, boxValues, out sagittalProjection, out coronalProjection);
                    sagittalProjection = reconstructionMethods.GetMatrixsMax(sagittalProjection, lastSagittal);
                    coronalProjection = reconstructionMethods.GetMatrixsMax(coronalProjection, lastCoronal);


                    Mat srcG = ConvertFile.ArrayToMat(sagittalProjection);
                    Mat dstG = new Mat();
                    Cv2.Resize(srcG, dstG, dSizeForSagittal);

                    Cv2.ApplyColorMap(dstG, dstG, ColormapTypes.Bone);
                    //Mat dstG2 = new Mat();
                    //Cv2.Flip(dstG, dstG2, FlipMode.Y);
                    Bitmap bitmapG = ConvertFile.MatToBitmap(dstG);
                    this.pictureBoxSagittalPlane.BeginInvoke(new MethodInvoker(delegate { PictureBoxShowSagittal(bitmapG); }));


                    Mat srcG2 = ConvertFile.ArrayToMat(coronalProjection);
                    Mat dstG2 = new Mat();
                    Cv2.Resize(srcG2, dstG2, dSizeFor3DDisplay);

                    Cv2.ApplyColorMap(dstG2, dstG2, ColormapTypes.Bone);
                    //Mat dstG2 = new Mat();
                    //Cv2.Flip(dstG2, dstG2, FlipMode.Y);
                    Bitmap bitmapG2 = ConvertFile.MatToBitmap(dstG2);
                    this.real3DPictureBox.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(bitmapG2); }));


                    // 同步调用
                    //this.real3DPictureBox.Invoke((MethodInvoker)delegate { PictureBoxShow3D(bitmapG); });

                    lastSagittal = sagittalProjection;
                    lastCoronal = coronalProjection;

                    coronalImage = srcG2;
                    saggitalImage = srcG;
                    boxV = boxValues;
                    reconsFlag = false;

                    //DateTime afterTime = DateTime.Now;
                    //TimeSpan ts = afterTime.Subtract(beforeTime);
                    //Console.WriteLine("1: {0}", ts.TotalMilliseconds);
                    dstG = null;
                    dstG2 = null;
                    srcG = null;
                    srcG2 = null;
                    GC.Collect();
                    //Console.WriteLine("[+]Show Recons is :" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n");

                }

            }
        }

        // Using scale to fit the normal depth-height ratio
        public void RealTimeReconstruction()
        {
            //int visualHeight = boxDepth * 2;
            //int visualWidthForSaggital = boxHeight;
            //int visualWidthForCoronal = boxWidth;

            // Get Data
            int arraySize = 480 * 640;
            byte[,] b8Frame = new byte[480, 640];
            byte[,] lastCoronal = new byte[boxDepth, boxWidth];
            byte[,] lastSagittal = new byte[boxDepth, boxHeight];

            byte[,] coronalProjection = new byte[boxDepth, boxWidth];
            byte[,] sagittalProjection = new byte[boxDepth, boxHeight];

            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxWidth; k2++)
                {
                    coronalProjection[k1, k2] = 0;
                }
            }


            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxHeight; k2++)
                {
                    lastSagittal[k1, k2] = 0;
                }
            }

            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxHeight; k2++)
                {
                    sagittalProjection[k1, k2] = 0;
                }
            }


            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxWidth; k2++)
                {
                    lastCoronal[k1, k2] = 0;
                }
            }

            byte[][,] boxValues = new byte[boxDepth][,];
            for (int ik = 0; ik < boxDepth; ik++)
            {
                boxValues[ik] = new byte[boxHeight, boxWidth];
            }

            while (isRecons)
            {
                bool reconsFlag = false;
                byte[] clariusByteArray = new byte[BUFFERSIZE];
                float[] xyzArray = new float[3]; float[] aerArray = new float[4];

                Object thisLock = new Object();
                lock (thisLock)
                {
                    if (g4Flag == true)
                    {
                        Array.Copy(sourceB8, clariusByteArray, sourceB8.Length);
                        Array.Copy(sourceXYZ, xyzArray, sourceXYZ.Length);
                        Array.Copy(sourceAER, aerArray, sourceAER.Length);

                        saveGPS.Add(xyzArray);
                        saveGPS.Add(aerArray);
                        saveClarius.Add(clariusByteArray);

                        g4Flag = false;
                        reconsFlag = true;
                    }
                }

                if (reconsFlag)
                {
                    //将一维数组转换为二维数组,得到Frame
                    for (int j = 0; j < arraySize; j++)
                    {
                        b8Frame[j / 640, j % 640] = clariusByteArray[j];
                    }

                    // 镜像翻转二维图像
                    for (int i = 0; i < 480; i++)
                    {
                        for (int j = 0; j < 640 / 2; j++)
                        {
                            byte temp = b8Frame[i, j];
                            b8Frame[i, j] = b8Frame[i, 640 - j - 1];
                            b8Frame[i, 640 - j - 1] = temp;
                        }
                    }

                    Mat srcG2D = ConvertFile.ArrayToMat(b8Frame);
                    Mat dstG2D = new Mat();
                    Cv2.Resize(srcG2D, dstG2D, dSizeFor2DDisplay);
                    Cv2.ApplyColorMap(dstG2D, dstG2D, ColormapTypes.Jet);
                    Bitmap bitmapG2D = ConvertFile.MatToBitmap(dstG2D);
                    dstG2D = null;

                    this.pictureBox2DDisplay.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow2D(bitmapG2D); }));


                    // segment Muscle
                    if (SegmentMuscleLevel != 0)
                    {
                        for (int j = 0; j < SegmentMuscleLevel; j++)
                        {
                            for (int k = 0; k < 640; k++)
                            {
                                b8Frame[j, k] = 0;
                            }
                        }
                    }


                    G4GetRects_Pre getRect = new G4GetRects_Pre(xyzArray, aerArray, LATERAL_Resolution);
                    this.mRectangles.v = getRect.GetRectangle();
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.mRectangles.v[i];
                        this.mRectangles.v[i] = Vector3.Transform(tempVector, calibrationMatrix);
                    }

                    boxValues = reconstructionMethods.RealTimePNN_Coronal_Sagittal_WyHx(this.mRectangles, b8Frame, boxValues, out sagittalProjection, out coronalProjection);
                    sagittalProjection = reconstructionMethods.GetMatrixsMax(sagittalProjection, lastSagittal);
                    coronalProjection = reconstructionMethods.GetMatrixsMax(coronalProjection, lastCoronal);

                    float scaleFactor = (float)dSizeForSagittal.Width / boxWidth;
                    int visualHeight = (int)Math.Round((float)boxDepth * 2 * scaleFactor);
                    int visualWidth = dSizeForSagittal.Width;

                    if (visualHeight > dSizeForSagittal.Height)
                    {
                        visualHeight = dSizeForSagittal.Height;
                    }
                    //Console.WriteLine("vw,vh is {0},{1}",visualWidth,visualHeight);
                    Mat srcG = ConvertFile.ArrayToMat(sagittalProjection);
                    Mat dstG = new Mat();
                    Cv2.Resize(srcG, dstG, new OpenCvSharp.Size(visualWidth, visualHeight));

                    Cv2.ApplyColorMap(dstG, dstG, ColormapTypes.Bone);
                    //Mat dstG2 = new Mat();
                    //Cv2.Flip(dstG, dstG2, FlipMode.Y);
                    Bitmap bitmapG = ConvertFile.MatToBitmap(dstG);
                    this.pictureBoxSagittalPlane.BeginInvoke(new MethodInvoker(delegate { PictureBoxShowSagittal(bitmapG); }));


                    Mat srcG2 = ConvertFile.ArrayToMat(coronalProjection);
                    Mat dstG2 = new Mat();
                    Cv2.Resize(srcG2, dstG2, new OpenCvSharp.Size(visualWidth, visualHeight));

                    Cv2.ApplyColorMap(dstG2, dstG2, ColormapTypes.Bone);
                    //Mat dstG2 = new Mat();
                    //Cv2.Flip(dstG2, dstG2, FlipMode.Y);
                    Bitmap bitmapG2 = ConvertFile.MatToBitmap(dstG2);
                    this.real3DPictureBox.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(bitmapG2); }));


                    // 同步调用
                    //this.real3DPictureBox.Invoke((MethodInvoker)delegate { PictureBoxShow3D(bitmapG); });

                    lastSagittal = sagittalProjection;
                    lastCoronal = coronalProjection;

                    coronalImage = srcG2;
                    saggitalImage = srcG;
                    boxV = boxValues;
                    reconsFlag = false;

                    //DateTime afterTime = DateTime.Now;
                    //TimeSpan ts = afterTime.Subtract(beforeTime);
                    //Console.WriteLine("1: {0}", ts.TotalMilliseconds);
                    dstG = null;
                    dstG2 = null;
                    srcG = null;
                    srcG2 = null;
                    GC.Collect();
                    //Console.WriteLine("[+]Show Recons is :" + Thread.CurrentThread.ManagedThreadId.ToString() + "\n");

                }

            }
        }
        public void PictureBoxShow2D(Bitmap bitmapG)
        {
            this.pictureBox2DDisplay.Image = bitmapG;
        }
        public void PictureBoxShow3D(Bitmap bitmapG)
        {
            this.real3DPictureBox.Image = bitmapG;
        }

        public void PictureBoxShowSagittal(Bitmap bitmapG)
        {
            this.pictureBoxSagittalPlane.Image = bitmapG;
        }

        #endregion

        #region Navigation Process

        private void toolStripButtonNavigation_Click(object sender, EventArgs e)
        {
            // Navigation V1.0 only in the main panel
            NavigationStartStopControl();

        }

        void NavigationStartStopControl()
        {
            navigationKeyFlag = !navigationKeyFlag;
            if (navigationKeyFlag)
            {

                NavigationStart();
            }
            else
            {
                NavigationStop();
            }
        }

        void NavigationStart()
        {
            // Start clarius imaging if not before navigation
            if (!isClariusImaging)
                UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            // Initial navigation thread and start
            saveNavigation.Clear();

            threadRefreshNavigation = new Thread(NavigationProcess);
            Thread.Sleep(20);

            threadRefreshNavigation.Start();
            Console.WriteLine("Start Navigation...");
            this.toolStripButtonNavigation.Image = Properties.Resources.navigationG;
        }

        void NavigationStop()
        {
            // Freeze clarius imaging if not after navition
            if (isClariusImaging)
                UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            Thread.Sleep(50);
            if (!threadRefreshNavigation.Join(2000))
            {
                threadRefreshNavigation.Abort();
            }
            Console.WriteLine("Stop Navigation!");
            this.toolStripButtonNavigation.Image = Properties.Resources.navigation;
            navigationKeyFlag = false;
        }

        //Navigation 2, Use scale
        public void NavigationProcess()
        {
            int rectSize = 8;

            Mat coronalImageColor = new Mat();
            Cv2.ApplyColorMap(coronalImage, coronalImageColor, ColormapTypes.Bone);
            Mat saggitalImageColor = new Mat();
            Cv2.ApplyColorMap(saggitalImage, saggitalImageColor, ColormapTypes.Bone);
            byte[,] b8Frame = new byte[boxHeight, boxWidth];
            g4Flag2 = false;
            while (true)
            {

                float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
                Object thisLock = new Object();
                bool isNavigation = false;
                lock (thisLock)
                {

                    if (g4Flag2 == true)
                    {
                        Array.Copy(sourceXYZ2, xyzArray, sourceXYZ2.Length);
                        Array.Copy(sourceAER2, quaternionArray, sourceAER2.Length);

                        saveNavigation.Add(xyzArray);
                        saveNavigation.Add(quaternionArray);
                        g4Flag2 = false;
                        isNavigation = true;
                    }

                }
                if (isNavigation)
                {
                    Vector3 tempVector = new Vector3(xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10);
                    Vector3 sensor2Location = Vector3.Transform(tempVector, calibrationMatrix); ;

                    Vector3 sensor2InVolume = sensor2Location - boxOrg;
                    int indexY = (int)Math.Round(sensor2InVolume.Y / voxelWidth);
                    int indexZ = (int)Math.Round(sensor2InVolume.Z / voxelDepth);
                    int indexX = (int)Math.Round(sensor2InVolume.X / voxelHeight);


                    Console.WriteLine("y,z is {0},{1}", indexY, indexZ);

                    if (indexZ >= 0 && indexZ < boxDepth)
                    {
                        float scaleFactor = (float)dSizeForSagittal.Width / boxWidth;
                        int visualHeight = (int)Math.Round((float)boxDepth * 2 * scaleFactor);
                        int visualWidth = dSizeForSagittal.Width;

                        if (visualHeight > dSizeForSagittal.Height)
                        {
                            visualHeight = dSizeForSagittal.Height;
                        }

                        /// ---- Coronal ---- ///
                        // Resize coronal image according to 3DDisplay Control Size
                        Mat navigationCoronalImage = coronalImageColor.Clone();
                        Mat dstGCoronal = new Mat();
                        Cv2.Resize(navigationCoronalImage, dstGCoronal, new OpenCvSharp.Size(visualWidth, visualHeight));

                        // Transform pixel coordinate in coronal image to resized image and Draw Pointer

                        int resizedCoronalIndex_Y = (int)(indexY * ((float)visualWidth / boxWidth));
                        int resizedCoronalIndex_Z = (int)(indexZ * ((float)visualHeight / boxDepth));
                        //Console.WriteLine("resizedY, resizedZ, {0},{1}", resizedCoronalIndex_Y, resizedCoronalIndex_Z);
                        OpenCvSharp.Point startPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y, resizedCoronalIndex_Z);
                        OpenCvSharp.Point endPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y + rectSize, resizedCoronalIndex_Z + rectSize);
                        Cv2.Rectangle(dstGCoronal, startPoint, endPoint, OpenCvSharp.Scalar.Red, -1);

                        // Display 


                        Bitmap bitmapG2 = ConvertFile.MatToBitmap(dstGCoronal);
                        this.real3DPictureBox.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(bitmapG2); }));

                        /// ---- Sagittal -----///
                        // Resize sagittal image according to 3DDisplay Control Size
                        Mat navigationSaggitalImage = saggitalImageColor.Clone();
                        Mat dstGSaggital = new Mat();
                        Cv2.Resize(navigationSaggitalImage, dstGSaggital, new OpenCvSharp.Size(visualWidth, visualHeight));

                        // Transform pixel coordinate in sagittal image to resized image and Draw Pointer
                        int resizedSagittalIndex_Y = (int)(indexX * ((float)visualWidth / boxHeight));
                        int resizedSagittalIndex_Z = (int)(indexZ * ((float)visualHeight / boxDepth));
                        OpenCvSharp.Point startPoint2 = new OpenCvSharp.Point(resizedSagittalIndex_Y, resizedSagittalIndex_Z);
                        OpenCvSharp.Point endPoint2 = new OpenCvSharp.Point(resizedSagittalIndex_Y + rectSize, resizedSagittalIndex_Z + rectSize);
                        Cv2.Rectangle(dstGSaggital, startPoint2, endPoint2, OpenCvSharp.Scalar.Red, -1);

                        // Display 
                        Bitmap saggitalBitmap = ConvertFile.MatToBitmap(dstGSaggital);
                        this.pictureBoxSagittalPlane.BeginInvoke(new MethodInvoker(delegate { PictureBoxShowSagittal(saggitalBitmap); }));

                        /// ---- Transverse -----///
                        // Resize sagittal image according to 3DDisplay Control Size
                        //BitConverter.GetBytes(])
                        b8Frame = (byte[,])(boxV[indexZ].Clone());
                        Mat srcGTransverse = ConvertFile.ArrayToMat(b8Frame);
                        Mat dstGTransverse = new Mat();
                        Cv2.ApplyColorMap(srcGTransverse, dstGTransverse, ColormapTypes.Jet);
                        Cv2.Resize(dstGTransverse, dstGTransverse, dSizeFor2DDisplay);

                        // Transform pixel coordinate in transverse image to resized image and Draw Pointer
                        int resizedTransverseIndex_Y = (int)(indexY * ((float)dSizeFor2DDisplay.Width / boxWidth));
                        int resizedTransverseIndex_X = (int)(indexX * ((float)dSizeFor2DDisplay.Height / boxHeight));
                        OpenCvSharp.Point startPoint3 = new OpenCvSharp.Point(resizedTransverseIndex_Y, resizedTransverseIndex_X);
                        OpenCvSharp.Point endPoint3 = new OpenCvSharp.Point(resizedTransverseIndex_Y + rectSize, resizedTransverseIndex_X + rectSize);
                        Cv2.Rectangle(dstGTransverse, startPoint3, endPoint3, OpenCvSharp.Scalar.Red, -1);
                        //Cv2.Flip(dstGTransverse, dstGTransverse, FlipMode.X);
                        Bitmap tramsverseBitmap = ConvertFile.MatToBitmap(dstGTransverse);
                        dstGTransverse = null;

                        this.pictureBox2DDisplay.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow2D(tramsverseBitmap); }));

                        isNavigation = false;

                        srcGTransverse = null;
                        dstGTransverse = null;
                        dstGCoronal = null;
                        dstGSaggital = null;
                        navigationCoronalImage = null;
                        navigationSaggitalImage = null;
                        GC.Collect();

                    }

                }
            }
        }
        //Navigation 2, Use scale
        public void NavigationProcess2()
        {
            int rectSize = 8;

            Mat coronalImageColor = new Mat();
            Cv2.ApplyColorMap(coronalImage, coronalImageColor, ColormapTypes.Bone);
            Mat saggitalImageColor = new Mat();
            Cv2.ApplyColorMap(saggitalImage, saggitalImageColor, ColormapTypes.Bone);
            byte[,] b8Frame = new byte[boxHeight, boxWidth];
            g4Flag2 = false;
            while (true)
            {

                float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
                Object thisLock = new Object();
                bool isNavigation = false;
                lock (thisLock)
                {

                    if (g4Flag2 == true)
                    {
                        Array.Copy(sourceXYZ2, xyzArray, sourceXYZ2.Length);
                        Array.Copy(sourceAER2, quaternionArray, sourceAER2.Length);

                        saveNavigation.Add(xyzArray);
                        saveNavigation.Add(quaternionArray);
                        g4Flag2 = false;
                        isNavigation = true;
                    }

                }
                if (isNavigation)
                {
                    Vector3 tempVector = new Vector3(xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10);
                    Vector3 sensor2Location = Vector3.Transform(tempVector, calibrationMatrix); ;

                    Vector3 sensor2InVolume = sensor2Location - boxOrg;
                    int indexY = (int)Math.Round(sensor2InVolume.Y / voxelWidth);
                    int indexZ = (int)Math.Round(sensor2InVolume.Z / voxelDepth);
                    int indexX = (int)Math.Round(sensor2InVolume.X / voxelHeight);

                    Console.WriteLine("y,z is {0},{1}", indexY, indexZ);

                    if (indexZ >= 0 && indexZ < boxDepth)
                    {
                        /// ---- Coronal ---- ///
                        // Resize coronal image according to 3DDisplay Control Size
                        Mat navigationCoronalImage = coronalImageColor.Clone();
                        Mat dstGCoronal = new Mat();
                        Cv2.Resize(navigationCoronalImage, dstGCoronal, dSizeFor3DDisplay);

                        // Transform pixel coordinate in coronal image to resized image and Draw Pointer

                        int resizedCoronalIndex_Y = (int)(indexY * ((float)dSizeFor3DDisplay.Width / boxWidth));
                        int resizedCoronalIndex_Z = (int)(indexZ * ((float)dSizeFor3DDisplay.Height / boxDepth));
                        //Console.WriteLine("resizedY, resizedZ, {0},{1}",resizedCoronalIndex_Y,resizedCoronalIndex_Z);
                        OpenCvSharp.Point startPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y, resizedCoronalIndex_Z);
                        OpenCvSharp.Point endPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y + rectSize, resizedCoronalIndex_Z + rectSize);
                        Cv2.Rectangle(dstGCoronal, startPoint, endPoint, OpenCvSharp.Scalar.Red, -1);

                        // Display 
                        Bitmap bitmapG2 = ConvertFile.MatToBitmap(dstGCoronal);
                        this.real3DPictureBox.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(bitmapG2); }));

                        /// ---- Sagittal -----///
                        // Resize sagittal image according to 3DDisplay Control Size
                        Mat navigationSaggitalImage = saggitalImageColor.Clone();
                        Mat dstGSaggital = new Mat();
                        Cv2.Resize(navigationSaggitalImage, dstGSaggital, dSizeForSagittal);

                        // Transform pixel coordinate in sagittal image to resized image and Draw Pointer
                        int resizedSagittalIndex_Y = (int)(indexX * ((float)dSizeForSagittal.Width / boxHeight));
                        int resizedSagittalIndex_Z = (int)(indexZ * ((float)dSizeForSagittal.Height / boxDepth));
                        OpenCvSharp.Point startPoint2 = new OpenCvSharp.Point(resizedSagittalIndex_Y, resizedSagittalIndex_Z);
                        OpenCvSharp.Point endPoint2 = new OpenCvSharp.Point(resizedSagittalIndex_Y + rectSize, resizedSagittalIndex_Z + rectSize);
                        Cv2.Rectangle(dstGSaggital, startPoint2, endPoint2, OpenCvSharp.Scalar.Red, -1);

                        // Display 
                        Bitmap saggitalBitmap = ConvertFile.MatToBitmap(dstGSaggital);
                        this.pictureBoxSagittalPlane.BeginInvoke(new MethodInvoker(delegate { PictureBoxShowSagittal(saggitalBitmap); }));

                        /// ---- Transverse -----///
                        // Resize sagittal image according to 3DDisplay Control Size
                        b8Frame = (byte[,])boxV[indexZ].Clone();
                        Mat srcGTransverse = ConvertFile.ArrayToMat(b8Frame);
                        Mat dstGTransverse = new Mat();
                        Cv2.ApplyColorMap(srcGTransverse, dstGTransverse, ColormapTypes.Jet);
                        Cv2.Resize(dstGTransverse, dstGTransverse, dSizeFor2DDisplay);

                        // Transform pixel coordinate in transverse image to resized image and Draw Pointer
                        int resizedTransverseIndex_Y = (int)(indexY * ((float)dSizeFor2DDisplay.Width / boxWidth));
                        int resizedTransverseIndex_X = (int)(indexX * ((float)dSizeFor2DDisplay.Height / boxHeight));
                        OpenCvSharp.Point startPoint3 = new OpenCvSharp.Point(resizedTransverseIndex_Y, resizedTransverseIndex_X);
                        OpenCvSharp.Point endPoint3 = new OpenCvSharp.Point(resizedTransverseIndex_Y + rectSize, resizedTransverseIndex_X + rectSize);
                        Cv2.Rectangle(dstGTransverse, startPoint3, endPoint3, OpenCvSharp.Scalar.Red, -1);
                        //Cv2.Flip(dstGTransverse, dstGTransverse, FlipMode.X);
                        Bitmap tramsverseBitmap = ConvertFile.MatToBitmap(dstGTransverse);
                        dstGTransverse = null;

                        this.pictureBox2DDisplay.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow2D(tramsverseBitmap); }));

                        isNavigation = false;

                        srcGTransverse = null;
                        dstGTransverse = null;
                        dstGCoronal = null;
                        dstGSaggital = null;
                        navigationCoronalImage = null;
                        navigationSaggitalImage = null;
                        GC.Collect();

                    }

                }
            }
        }

        // Add marke in navigation UI
        private void Marker_Click(object sender, EventArgs e)
        {
            // Remove ContextMenuStrip from picBox when Marker event is triggered
            this.real3DPictureBox.ContextMenuStrip = null;

            // Start Mouse click event
            this.real3DPictureBox.MouseDown += real3DPictureBoxMouseClick;
            this.real3DPictureBox.MouseMove += real3DPictureBoxMouseMove;

            // Change the shape of cursor
            real3DPictureBox.Cursor = Cursors.Cross;
        }


        private void real3DPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (IsDrawing)
            {
                if (IsOver)
                {
                    e.Graphics.FillEllipse(Brushes.Red, new Rectangle(MarkerLocation.X - RectSize / 2, MarkerLocation.Y - RectSize / 2, RectSize, RectSize));
                }
                else
                {
                    e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(MarkerLocation.X - RectSize / 2, MarkerLocation.Y - RectSize / 2, RectSize, RectSize));
                }
            }
        }

        private void real3DPictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("current, Marker, {0},{1}",e.Location,MarkerLocation);
            // Check the moving mouse is over the Marker Location.
            if (FindDistanceToPointSquared(e.Location, MarkerLocation) < over_dist_squared)
            {
                // We're over the Marker Location.
                IsOver = true;
                this.real3DPictureBox.Refresh();

                //Console.WriteLine("over");
            }
            else
            {
                IsOver = false;
                this.real3DPictureBox.Refresh();

            }
        }

        private void real3DPictureBoxMouseClick(object sender, MouseEventArgs e)
        {
            // Record the location for marker  to draw

            if (e.Button == MouseButtons.Left)
            {
                // Draw if mouse is NOT over the Marker Location
                if (!IsOver)
                {
                    MarkerLocation = new System.Drawing.Point(e.X, e.Y);
                    //this.real3DPictureBox.MouseMove += real3DPictureBoxMouseMove;

                    IsDrawing = true;
                    this.real3DPictureBox.Refresh();
                }
                // Cancel draw if mouse is over the Marker Location
                else
                {
                    IsDrawing = false;
                    this.real3DPictureBox.Refresh();
                    IsOver = false;
                    //this.real3DPictureBox.MouseMove -= real3DPictureBoxMouseMove;
                    MarkerLocation = new System.Drawing.Point(-RectSize / 2, -RectSize / 2);

                }

            }
            // Cancel mouse event and re-add ContextMenuStrip into picBox 
            else if (e.Button == MouseButtons.Right)
            {
                this.real3DPictureBox.MouseDown -= real3DPictureBoxMouseClick;
                this.real3DPictureBox.MouseMove -= real3DPictureBoxMouseMove;
                this.real3DPictureBox.ContextMenuStrip = contextMenuStripCoronalView;
                real3DPictureBox.Cursor = Cursors.Arrow;

            }
        }

        // Calculate the distance squared between two points.
        private int FindDistanceToPointSquared(System.Drawing.Point pt1, System.Drawing.Point pt2)
        {
            int dx = pt1.X - pt2.X;
            int dy = pt1.Y - pt2.Y;
            return dx * dx + dy * dy;
        }

        #endregion

        #region Remove Muscle in Trasverse
        bool IsOver_Trasverse = false;
        bool IsDrawing_Trasverse = false;
        private void toolStripMenuItemTrasverseView_Click(object sender, EventArgs e)
        {
            // Remove ContextMenuStrip from picBox when Marker event is triggered
            this.pictureBox2DDisplay.ContextMenuStrip = null;

            // Start Mouse click event
            this.pictureBox2DDisplay.MouseDown += pictureBox2DDisplayMouseClick;
            this.pictureBox2DDisplay.MouseMove += pictureBox2DDisplayMouseMove;

            // Change the shape of cursor
            pictureBox2DDisplay.Cursor = Cursors.Cross;
        }

        const int LineWidth_Trasverse = 5;
        private void pictureBox2DDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (IsDrawing_Trasverse)
            {
                //System.Drawing.Point startPoint = new System.Drawing.Point(0,MarkerLocation_Trasverse.Y);
                //System.Drawing.Point endPoint = new System.Drawing.Point(dSizeFor2DDisplay.Width-1, MarkerLocation_Trasverse.Y);

                if (IsOver_Trasverse)
                {
                    Pen pen = new Pen(Color.Red, LineWidth_Trasverse);

                    e.Graphics.DrawLine(pen, StartPointTrasverse, EndPointTrasverse);

                    //e.Graphics.FillEllipse(Brushes.Red, new Rectangle(MarkerLocation_Trasverse.X - RectSize / 2, MarkerLocation_Trasverse.Y - RectSize / 2, RectSize, RectSize));
                }
                else
                {
                    Pen pen = new Pen(Color.Green, LineWidth_Trasverse);

                    e.Graphics.DrawLine(pen, StartPointTrasverse, EndPointTrasverse);
                    //e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(MarkerLocation_Trasverse.X - RectSize / 2, MarkerLocation_Trasverse.Y - RectSize / 2, RectSize, RectSize));
                }
            }
        }

        private void pictureBox2DDisplayMouseMove(object sender, MouseEventArgs e)
        {
            System.Drawing.PointF closest;
            //Console.WriteLine("move");


            // Check the moving mouse is over the Marker Location.
            if (FindDistanceToSegmentSquared(e.Location, StartPointTrasverse, EndPointTrasverse, out closest) < over_dist_squared)
            {
                // We're over the Marker Location.
                IsOver_Trasverse = true;
                this.pictureBox2DDisplay.Refresh();

            }
            else
            {
                IsOver_Trasverse = false;
                this.pictureBox2DDisplay.Refresh();

            }
        }

        private void pictureBox2DDisplayMouseClick(object sender, MouseEventArgs e)
        {
            // Record the location for marker  to draw

            if (e.Button == MouseButtons.Left)
            {
                // Draw if mouse is NOT over the Marker Location
                if (!IsOver_Trasverse)
                {

                    //MarkerLocation_Trasverse = new System.Drawing.Point(e.X, e.Y);                   
                    StartPointTrasverse = new System.Drawing.Point(0, e.Y);
                    EndPointTrasverse = new System.Drawing.Point(dSizeFor2DDisplay.Width - 1, e.Y);
                    SegmentMuscleLevel = (int)((float)e.Y / dSizeFor2DDisplay.Height * 480);
                    //Console.WriteLine(RemoveMuscleLevel);
                    IsDrawing_Trasverse = true;
                    this.pictureBox2DDisplay.Refresh();
                }
                // Cancel draw if mouse is over the Marker Location
                else
                {
                    IsDrawing_Trasverse = false;
                    this.pictureBox2DDisplay.Refresh();
                    IsOver_Trasverse = false;
                    StartPointTrasverse = new System.Drawing.Point(-RectSize / 2, -RectSize / 2);
                    EndPointTrasverse = new System.Drawing.Point(-RectSize / 2, -RectSize / 2);
                    SegmentMuscleLevel = 0;
                }

            }
            // Cancel mouse event and re-add ContextMenuStrip into picBox 
            else if (e.Button == MouseButtons.Right)
            {
                this.pictureBox2DDisplay.MouseDown -= pictureBox2DDisplayMouseClick;
                this.pictureBox2DDisplay.MouseMove -= pictureBox2DDisplayMouseMove;
                this.pictureBox2DDisplay.ContextMenuStrip = contextMenuStripTrasverseView;
                pictureBox2DDisplay.Cursor = Cursors.Arrow;

            }
        }

        // Calculate the distance squared between
        // point pt and the segment p1 --> p2.
        private double FindDistanceToSegmentSquared(System.Drawing.Point pt, System.Drawing.Point p1, System.Drawing.Point p2, out System.Drawing.PointF closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return dx * dx + dy * dy;
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new System.Drawing.PointF(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new System.Drawing.PointF(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new System.Drawing.PointF(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return dx * dx + dy * dy;
        }

        bool IsButtonSegmentTrasverseCliked = false;
        private void buttonSegmentTrasverse_Click(object sender, EventArgs e)
        {
            IsButtonSegmentTrasverseCliked = !IsButtonSegmentTrasverseCliked;
            if (IsButtonSegmentTrasverseCliked)
            {
                // Start Mouse click event
                this.pictureBox2DDisplay.MouseDown += pictureBox2DDisplayMouseClick;
                this.pictureBox2DDisplay.MouseMove += pictureBox2DDisplayMouseMove;

                // Change the shape of cursor
                pictureBox2DDisplay.Cursor = Cursors.Cross;

                if (!isClariusImaging)
                    UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);
                buttonSegmentTrasverse.BackColor = SystemColors.Info;
                threadImagingTrasverse = new Thread(RealTimeDisplay2DImage);
                threadImagingTrasverse.IsBackground = true;
                Thread.Sleep(20);
                Console.WriteLine("Start Segmentation...");
                threadImagingTrasverse.Start();


            }
            else
            {
                this.pictureBox2DDisplay.MouseDown -= pictureBox2DDisplayMouseClick;
                this.pictureBox2DDisplay.MouseMove -= pictureBox2DDisplayMouseMove;
                pictureBox2DDisplay.Cursor = Cursors.Arrow;

                if (isClariusImaging)
                    UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);
                buttonSegmentTrasverse.BackColor = SystemColors.Control;

                Thread.Sleep(50);
                if (!threadImagingTrasverse.Join(2000))
                {
                    threadImagingTrasverse.Abort();
                }
                Console.WriteLine("Stop Segmentation!");
            }

        }

        #endregion



        #region Post-Processing
        Rect3[] GetRectsFromGPSData()
        {
            int count = saveGPS.Count / 2;
            Rect3[] rectsForPstProcessing = new Rect3[count];
            G4GetRects_Pre getRect = new G4GetRects_Pre();

            Console.WriteLine(count);
            float[][] gpsArray = saveGPS.ToArray();
            for (int i = 0; i < count; i++)
            {
                float[] xyz = gpsArray[i * 2];               // xyz
                float[] quaternion = gpsArray[i * 2 + 1];             // quatern

                getRect = new G4GetRects_Pre(xyz, quaternion, MainForm.LATERAL_Resolution);
                rectsForPstProcessing[i] = new Rect3();
                rectsForPstProcessing[i].v = getRect.GetRectangle();
            }
            return rectsForPstProcessing;
        }

        byte[][,] GetByteArraysFromClariusData()
        {
            int count = saveClarius.Count;
            byte[][,] mFrames = new byte[count][,];
            int readBytesSize = Image_Height * Image_Width;
            //Console.WriteLine("count {0}",count);
            int idx = 0;
            foreach (var ele in saveClarius)
            {
                byte[,] array2 = new byte[Image_Height, Image_Width];
                //将一维数组转换为二维数组
                for (int j = 0; j < readBytesSize; j++)
                {
                    array2[j / Image_Width, j % Image_Width] = ele[j];
                }
                // Mirror the frame data
                for (int iHeight = 0; iHeight < Image_Height; iHeight++)
                {
                    for (int jWidth = 0; jWidth < Image_Width / 2; jWidth++)
                    {
                        byte temp = array2[iHeight, jWidth];
                        array2[iHeight, jWidth] = array2[iHeight, Image_Width - jWidth - 1];
                        array2[iHeight, Image_Width - jWidth - 1] = temp;
                    }
                }
                mFrames[idx++] = array2;
                //Console.WriteLine(idx);
            }

            return mFrames;
        }

        private void toolStripButtonPostProcessing_Click(object sender, EventArgs e)
        {
            //GetByteArraysFromClariusData();

            if (saveClarius.Count > 0 && saveGPS.Count > 0)
            {
                Transformation transformation = new Transformation();
                transformation.rects = GetRectsFromGPSData();

                for (int i = 0; i < 2; i++)
                {
                    if (this.checkBoxPlumbLine.Checked)
                    {
                        transformation.CoordinateTransformationForPortableUsingMIASWithPlumbLine();
                    }
                    else
                    {
                        transformation.CoordinateTransformationForPortableUsingMIAS();
                    }
                }
                transformation.InverseGPS_X();
                byte[][,] mFrames = GetByteArraysFromClariusData();

                USDataCollection2 usDataCollectionForPostProcessing = new USDataCollection2(saveClarius.Count, mFrames,
                    mFrames, transformation.rects, this.saveDataName, this.savePath);

                Frm_Recons_DataExport frm_Recons_DataExport = new Frm_Recons_DataExport(usDataCollectionForPostProcessing);
                frm_Recons_DataExport.Show();

            }
            else
            {
                MessageBox.Show("No Data Collection", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        #endregion

        private void clariusC3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!clariusC3ToolStripMenuItem.Checked)
            {
                clariusC3ToolStripMenuItem.Checked = true;
                clariusC3HDToolStripMenuItem.Checked = false;

                ChooseScannerType = UnitInterFaceV733.ChooseScannerType.Clarius;
                Properties.Settings.Default.ScannerType = ChooseScannerType;
                Properties.Settings.Default.Save();

            }

            MessageBox.Show(ChooseScannerType.ToString(), "Current selected scanner type", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Text = defaultTextinMainForm + " —— " + ChooseScannerType.ToString();
            Console.WriteLine(">> Scanner Type: {0}", ChooseScannerType.ToString());

        }

        private void clariusC3NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!clariusC3HDToolStripMenuItem.Checked)
            {
                clariusC3HDToolStripMenuItem.Checked = true;
                clariusC3ToolStripMenuItem.Checked = false;

                ChooseScannerType = UnitInterFaceV733.ChooseScannerType.Clarius_HD;
                Properties.Settings.Default.ScannerType = ChooseScannerType;
                Properties.Settings.Default.Save();


            }
            MessageBox.Show(ChooseScannerType.ToString(), "Current selected scanner type", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Text = defaultTextinMainForm + " —— " + ChooseScannerType.ToString();
            Console.WriteLine(">> Scanner Type: {0}", ChooseScannerType.ToString());
        }


        #region navigation with double sweep
        private void toolStripButtonReconsWithSelection_Click(object sender, EventArgs e)
        {
            //Frm_Recons_Navigation frm_Recons_Navigation = new Frm_Recons_Navigation();
            //frm_Recons_Navigation.Show();
            Frm_Recons_DataSelect frm_Recons_DataSelect = new Frm_Recons_DataSelect(this);
            frm_Recons_DataSelect.Show();
        }

        #endregion

        private void radioButtonStandingPosition_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
