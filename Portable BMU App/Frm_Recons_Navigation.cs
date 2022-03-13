using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;

namespace Portable_BMU_App
{
    public partial class Frm_Recons_Navigation : Form
    {
        public Frm_Recons_Navigation()
        {
            InitializeComponent();
            
            frm_Recons_Navigation = this;
        }

        //--------------------------------- from mainform PPP----------------------------------//
        public static Frm_Recons_Navigation frm_Recons_Navigation;
        public static Mat srcG = null;              // For removing musle -----By Song sheng
        public static Mat showMat = null;           // For brightness and contrast -----By Li linlin
        public static int SelectedColor = (int)ColorMap.Bone; 
        public static short[][,] volume;
        public static short[][,] volume_cut;
        public static ArrayList top_points = new ArrayList();
        public static ArrayList bottom_points = new ArrayList();
        public Mat coronalImgGlobal = null;
        public string OpenFilePath = string.Empty;
        /// <summary>
        /// the following is for segmentation line
        /// </summary>
        // The "size" of an object for mouse over purposes.
        private const int object_radius = 3;

        // We're over an object if the distance squared
        // between the mouse and the object is less than this.
        private const int over_dist_squared = object_radius * object_radius;

        // The points that make up the line segments.
        private List<System.Drawing.Point> Pt1 = new List<System.Drawing.Point>();
        private List<System.Drawing.Point> Pt2 = new List<System.Drawing.Point>();

        // Points for the new line.
        private bool IsDrawing = false;
        private System.Drawing.Point NewPt1, NewPt2;


        //Current size of pic box
        private int picCanvas_ImageWidth, picCanvas_ImageHeight;
        // The current scale.
        private float ImageScale = 1.0f;

        public enum ColorMap
        {
            Bone,
            Jet,
            Gray
        }

        //--------------------------------- from mainform PRP----------------------------------//
        // --------------------------- Display paras ------------------------------//
        public static double LATERAL_Resolution = 0;
        public static int Image_Width = 0;
        public static int Image_Height = 0;
        OpenCvSharp.Size dSizeFor3DDisplay;
        Thread threadRefreshNavigation;

        public static Vector3 boxOrg = new Vector3(0f, 0f, 0f);  // BoxSize and calibration Matrix
        public static int boxWidth = 0;                                               // BoxSize and calibration Matrix
        public static int boxHeight = 0;                                              // BoxSize and calibration Matrix
        public static int boxDepth = 0;                                               // BoxSize and calibration Matrix
        public static Matrix4x4 calibrationMatrix = new Matrix4x4();                    // BoxSize and calibration Matrix
        public static float voxelDepth = 0f;
        public static float voxelWidth = 0f;
        public static float voxelHeight = 0f;

        byte[][,] boxV = new byte[boxDepth][,];
        Mat coronalImage = null;
        Mat saggitalImage = null;
        bool navigationKeyFlag = false;

        int ppi = 96;  // displauy
        int visualHeight = 0;
        int visualWidth = 0;


        //Navigation 2, Use scale
        public void NavigationProcess()
        {
            int rectSize = 8;

            Mat coronalImageColor = new Mat();
            Cv2.ApplyColorMap(coronalImage, coronalImageColor, ColormapTypes.Bone);



            //if (SelectedColor == (int)ColorMap.Bone)
            //{
            //    Cv2.ApplyColorMap(coronalImage, coronalImageColor, ColormapTypes.Bone);
            //}
            //else if (SelectedColor == (int)ColorMap.Jet)
            //{
            //    Cv2.ApplyColorMap(coronalImage, coronalImageColor, ColormapTypes.Jet);
            //}
            //else if (SelectedColor == (int)ColorMap.Gray)
            //{
            //    coronalImageColor = coronalImage;
            //}

            //Mat saggitalImageColor = new Mat();
            //Cv2.ApplyColorMap(saggitalImage, saggitalImageColor, ColormapTypes.Bone);
            byte[,] b8Frame = new byte[boxHeight, boxWidth];
            MainForm.g4Flag2 = false;
            while (true)
            {

                float[] xyzArray = new float[3]; float[] quaternionArray = new float[4];
                Object thisLock = new Object();
                bool isNavigation = false;

                lock (thisLock)
                {

                    if (MainForm.g4Flag2 == true)
                    {
                        Array.Copy(MainForm.sourceXYZ2, xyzArray, MainForm.sourceXYZ.Length);
                        Array.Copy(MainForm.sourceAER2, quaternionArray, MainForm.sourceAER.Length);
                        //Console.WriteLine("x:{0},y:{1},z:{2}",xyzArray[0],xyzArray[1],xyzArray[2]);
                        //savePreScanGPS.Add(xyzArray);
                        //savePreScanGPS.Add(quaternionArray);
                        isNavigation = true;
                        MainForm.g4Flag2 = false;
                    }


                }
                if (isNavigation)
                {
                    Vector3 tempVector = new Vector3(xyzArray[0] * 10, xyzArray[1] * 10, xyzArray[2] * 10);
                    Vector3 sensor2Location = Vector3.Transform(tempVector, calibrationMatrix);
                    sensor2Location.X = 0f - sensor2Location.X; 

                    Vector3 sensor2InVolume = sensor2Location - boxOrg;
                    int indexY = (int)Math.Round(sensor2InVolume.Y / voxelWidth);
                    int indexZ = (int)Math.Round(sensor2InVolume.Z / voxelDepth);
                    int indexX = (int)Math.Round(sensor2InVolume.X / voxelHeight);


                    Console.WriteLine("x,y,z is {0},{1},{2}", indexX, indexY, indexZ);

                    if (indexZ >= 0 && indexZ < boxDepth)
                    {
                        //float scaleFactor = (float)dSizeFor3DDisplay.Width / boxWidth;
                        //int visualHeight = (int)Math.Round((float)boxDepth * 2 * scaleFactor);
                        //int visualWidth = dSizeFor3DDisplay.Width;

                        //if (visualHeight > dSizeFor3DDisplay.Height)
                        //{
                        //    visualHeight = dSizeFor3DDisplay.Height;
                        //}

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
                        

                        navigationCoronalImage = null;
                        GC.Collect();

                    }

                }
            }
        }



        public void PictureBoxShow3D(Bitmap bitmapG)
        {
            this.real3DPictureBox.Image = bitmapG;
        }


        private void buttonNavigation_Click(object sender, EventArgs e)
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

        private void Frm_Recons_Navigation_Load(object sender, EventArgs e)
        {
            dSizeFor3DDisplay = new OpenCvSharp.Size(real3DPictureBox.Width, real3DPictureBox.Height);
        }

        private void Frm_Recons_Navigation_SizeChanged(object sender, EventArgs e)
        {
            dSizeFor3DDisplay = new OpenCvSharp.Size(real3DPictureBox.Width, real3DPictureBox.Height);

        }

        private void toolStripButtonNavigation_Click(object sender, EventArgs e)
        {

        }

        void NavigationStart()
        {
            // Start clarius imaging if not before navigation
            if (!MainForm.isClariusImaging)
                UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            // Initial navigation thread and start
            //saveNavigation.Clear();

            threadRefreshNavigation = new Thread(NavigationProcess);
            Thread.Sleep(20);

            threadRefreshNavigation.Start();
            Console.WriteLine("Start Navigation...");
            this.buttonNavigation.BackgroundImage = Properties.Resources.navigationG;
        }

        void NavigationStop()
        {
            // Freeze clarius imaging if not after navition
            if (MainForm.isClariusImaging)
                UnitInterFaceV733.clariusUserFunction(UnitInterFaceV733.USER_FN_TOGGLE_FREEZE, 0, null);

            Thread.Sleep(50);
            if (!threadRefreshNavigation.Join(2000))
            {
                threadRefreshNavigation.Abort();
            }
            Console.WriteLine("Stop Navigation!");
            this.buttonNavigation.BackgroundImage = Properties.Resources.navigation;
            navigationKeyFlag = false;
        }

        public void top_showSag_closed(object sender, FormClosedEventArgs e)
        {
            Show_Saggital bottom_showSag = new Show_Saggital();
            bottom_showSag.InitializeComponent();
            short[,] saggitalImage = VolumeProjection.SaggitalProjection(Frm_Recons_Navigation.volume);
            byte[,] byteG = ConvertFile.ShortArrayToByte(saggitalImage);
            Mat srcG = ConvertFile.ArrayToMat(byteG);
            //Cv2.ImShow("img", srcG);
            Bitmap bitmap = ConvertFile.MatToBitmap(srcG);
            bottom_showSag.pictureBox1.Image = bitmap;
            bottom_showSag.FormClosed += bottom_showSag_closed;
            bottom_showSag.Text = "Bottom";
            bottom_showSag.Show(this);
        }

        /// <summary>
        /// Songsheng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bottom_showSag_closed(object sender, FormClosedEventArgs e)
        {
            //Array.Copy(volume,volume_cut,volume.Length* volume[0].GetLength(0)* volume[0].GetLength(1));



            //MainForm.volume.CopyTo(volume_cut,0); ;
            int height = volume[0].GetLength(0);
            int width = volume[0].GetLength(1);
            int depth = volume.Length;

            volume_cut = new short[depth][,];
            for (int i = 0; i < depth; i++)
            {
                volume_cut[i] = new short[height, width];
            }

            for (int k = 0; k < volume.Length; k++)
            {
                for (int i = 0; i < volume[0].GetLength(0); i++)
                {
                    for (int j = 0; j < volume[0].GetLength(1); j++)
                    {
                        volume_cut[k][i, j] = volume[k][i, j];
                    }
                }
            }



            for (int i = 0; i < top_points.Count; i++)
            {
                if (i == 0)
                {
                    for (int sag_x = 0; sag_x < Convert.ToInt16(((PointF)top_points[i]).X); sag_x++)
                    {
                        int sag_y = Convert.ToInt16(((PointF)top_points[i]).Y);
                        for (int j = 0; j < width; j++)
                        {
                            for (int k = 0; k < sag_y; k++)
                            {
                                volume_cut[sag_x][k, j] = 0;
                            }
                        }
                    }
                    continue;
                }



                for (int sag_x = Convert.ToInt16(((PointF)top_points[i - 1]).X); sag_x < Convert.ToInt16(((PointF)top_points[i]).X); sag_x++)
                {
                    float y_2 = ((PointF)top_points[i]).Y;
                    float y_1 = ((PointF)top_points[i - 1]).Y;
                    float x_2 = ((PointF)top_points[i]).X;
                    float x_1 = ((PointF)top_points[i - 1]).X;

                    //int sag_y = Convert.ToInt16(((((PointF)top_points[i]).Y - ((PointF)top_points[i-1]).Y)/ (((PointF)top_points[i]).X - ((PointF)top_points[i - 1]).X)) * (Convert.ToSinglesag_x - ((PointF)top_points[i - 1]).X)) +((PointF)top_points[i-1]).Y); 
                    //int sag_y = Convert.ToInt16(((y_2 - y_1) / (x_2 - x_1)) * (sag_x - x_1) + y_1 );                    
                    int sag_y = Convert.ToInt16(((y_2 - y_1) / (x_2 - x_1)) * (sag_x - x_1) + y_1);

                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < sag_y; k++)
                        {
                            volume_cut[sag_x][k, j] = 0;
                        }
                    }
                }
                if (i == top_points.Count - 1)
                {
                    for (int sag_x = Convert.ToInt16(((PointF)top_points[i]).X); sag_x < depth; sag_x++)
                    {
                        int sag_y = Convert.ToInt16(((PointF)top_points[i]).Y);
                        for (int j = 0; j < width; j++)
                        {
                            for (int k = 0; k < sag_y; k++)
                            {
                                volume_cut[sag_x][k, j] = 0;
                            }
                        }
                    }
                    continue;
                }

            }

            for (int i = 0; i < bottom_points.Count; i++)
            {
                if (i == 0)
                {
                    for (int sag_x = 0; sag_x < Convert.ToInt16(((PointF)bottom_points[i]).X); sag_x++)
                    {
                        int sag_y = Convert.ToInt16(((PointF)bottom_points[i]).Y);
                        for (int j = 0; j < width; j++)
                        {
                            for (int k = sag_y; k < height; k++)
                            {
                                volume_cut[sag_x][k, j] = 0;
                            }
                        }
                    }
                    continue;
                }


                for (int sag_x = Convert.ToInt16(((PointF)bottom_points[i - 1]).X); sag_x < Convert.ToInt16(((PointF)bottom_points[i]).X); sag_x++)
                {
                    float y_2 = ((PointF)bottom_points[i]).Y;
                    float y_1 = ((PointF)bottom_points[i - 1]).Y;
                    float x_2 = ((PointF)bottom_points[i]).X;
                    float x_1 = ((PointF)bottom_points[i - 1]).X;

                    //int sag_y = Convert.ToInt16(((((PointF)bottom_points[i]).Y - ((PointF)bottom_points[i-1]).Y)/ (((PointF)bottom_points[i]).X - ((PointF)bottom_points[i - 1]).X)) * (Convert.ToSinglesag_x - ((PointF)bottom_points[i - 1]).X)) +((PointF)bottom_points[i-1]).Y); 
                    int sag_y = Convert.ToInt16(((y_2 - y_1) / (x_2 - x_1)) * (sag_x - x_1) + y_1);
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = sag_y; k < height; k++)
                        {
                            volume_cut[sag_x][k, j] = 0;
                        }
                    }
                }

                if (i == bottom_points.Count - 1)
                {
                    for (int sag_x = Convert.ToInt16(((PointF)bottom_points[i]).X); sag_x < depth; sag_x++)
                    {
                        int sag_y = Convert.ToInt16(((PointF)bottom_points[i]).Y);
                        for (int j = 0; j < width; j++)
                        {
                            for (int k = sag_y; k < height; k++)
                            {
                                volume_cut[sag_x][k, j] = 0;
                            }
                        }
                    }
                    continue;
                }
            }


            // Display
            Console.WriteLine(">> Default ppi is 96");
            float boxWidthScaleFactor = (float)  ppi/ (2*25.4f);  // Transfer to ppi; (mm/25.4)*96; 1 inch -> 25.4mm; ppi: pixel per inch
            float boxDepthScaleFactor = (float)  ppi / (25.4f);  // Transfer to ppi; (mm/25.4)*96; 1 inch -> 25.4mm; ppi: pixel per inch


            visualHeight = (int)Math.Round((float)boxDepthScaleFactor * boxDepth);
            visualWidth =(int)Math.Round( (float) boxWidthScaleFactor * boxWidth);

            

            if (visualHeight > dSizeFor3DDisplay.Height)
            {
                visualHeight = dSizeFor3DDisplay.Height;
                float ppiTemp = (float) visualHeight * 25.4f / boxDepth ;
                boxWidthScaleFactor = (float)ppiTemp / (2 * 25.4f);  // Transfer to ppi; (mm/25.4)*96; 1 inch -> 25.4mm; ppi: pixel per inch
                visualWidth = (int)Math.Round((float)boxWidthScaleFactor * boxWidth);
                Console.WriteLine(">> Change the ppi to {0} to fit the volume size",ppiTemp);

            }
            if (visualWidth > dSizeFor3DDisplay.Width)
                visualWidth = dSizeFor3DDisplay.Width;


            Console.WriteLine(visualWidth);
            Console.WriteLine(visualHeight);
            short[,] coronalImageShort = VolumeProjection.CoronalProjection2(volume_cut);
            //short[,] coronalImage = SaggitalProjection(volume);
            byte[,] byteG = ConvertFile.ShortArrayToByte(coronalImageShort);
            srcG = ConvertFile.ArrayToMat(byteG);
            coronalImage = srcG;
            showMat = srcG;

            DisplayImage();
            //Cv2.ImShow("img", srcG);
            //OpenCvSharp.Size dsize = new OpenCvSharp.Size(visualWidth, visualHeight);
            //Mat dstG = new Mat();
            //Cv2.Resize(srcG,dstG, dsize);
            //Bitmap bitmap = ConvertFile.MatToBitmap(dstG);



            //this.real3DPictureBox.Image = bitmap;
        }



        private void BrightnessBar_Scroll(object sender, EventArgs e)
        {
            Mat newMat = new Mat();
            updateBrightnessContrast(srcG, newMat, BrightnessBar.Value, ContrastBar.Value);
            showMat = newMat;

            DisplayImage(showMat);

            newMat = null;
            GC.Collect();

        }

        public void updateBrightnessContrast(Mat src, Mat modifiedSrc, int brightness, int contrast)
        {
            brightness = brightness - 100;
            contrast = contrast - 100;

            double alpha, beta;
            if (contrast > 0)
            {
                double delta = 127f * contrast / 100f;
                alpha = 255f / (255f - delta * 2);
                beta = alpha * (brightness - delta);
            }
            else
            {
                double delta = -128f * contrast / 100;
                alpha = (256f - delta * 2) / 255f;
                beta = alpha * brightness + delta;
            }
            src.ConvertTo(modifiedSrc, src.Type(), alpha, beta);
        }



        private void ContrastBar_Scroll(object sender, EventArgs e)
        {
            Mat newMat = new Mat();
            updateBrightnessContrast(srcG, newMat, BrightnessBar.Value, ContrastBar.Value);

            showMat = newMat;
            DisplayImage(showMat);

            newMat = null;
            GC.Collect();
        }

        private void toolStripButtonBone_Click(object sender, EventArgs e)
        {
            SelectedColor = (int)ColorMap.Bone;

            this.toolStripButtonJet.Checked = false;
            this.toolStripButtonGray.Checked = false;
            this.toolStripButtonBone.Checked = true;

            DisplayImage();
        }



        private void toolStripButtonJet_Click(object sender, EventArgs e)
        {
            SelectedColor = (int)ColorMap.Jet;

            this.toolStripButtonJet.Checked = true; ;        
            this.toolStripButtonGray.Checked = false;
            this.toolStripButtonBone.Checked = false;

            DisplayImage();
        }

        private void toolStripButtonGray_Click(object sender, EventArgs e)
        {
            SelectedColor = (int)ColorMap.Gray;

            this.toolStripButtonJet.Checked = false;
            this.toolStripButtonGray.Checked = true;
            this.toolStripButtonBone.Checked = false;

            DisplayImage();
        }

        void DisplayImage()
        {

            Mat showDstG = new Mat();

            if (SelectedColor == (int)ColorMap.Bone)
            {
                Cv2.ApplyColorMap(showMat, showDstG, ColormapTypes.Bone);
            }
            else if (SelectedColor == (int)ColorMap.Jet)
            {
                Cv2.ApplyColorMap(showMat, showDstG, ColormapTypes.Jet);
            }
            else if (SelectedColor == (int)ColorMap.Gray)
            {
                showDstG = showMat;
            }


            OpenCvSharp.Size dsize = new OpenCvSharp.Size(visualWidth, visualHeight);
            Mat dstG = new Mat();
            Cv2.Resize(showDstG, dstG, dsize);

            Bitmap bitmap = ConvertFile.MatToBitmap(dstG);
            this.real3DPictureBox.Image = bitmap;
        }

        void DisplayImage(Mat mat)
        {

            Mat showDstG = new Mat();

            if (SelectedColor == (int)ColorMap.Bone)
            {
                Cv2.ApplyColorMap(mat, showDstG, ColormapTypes.Bone);
            }
            else if (SelectedColor == (int)ColorMap.Jet)
            {
                Cv2.ApplyColorMap(mat, showDstG, ColormapTypes.Jet);
            }
            else if (SelectedColor == (int)ColorMap.Gray)
            {
                showDstG = mat;

            }

            OpenCvSharp.Size dsize = new OpenCvSharp.Size(visualWidth, visualHeight);
            Mat dstG = new Mat();
            Cv2.Resize(showDstG, dstG, dsize);
            Bitmap bitmap = ConvertFile.MatToBitmap(dstG);
            this.real3DPictureBox.Image = bitmap;
        }
    }
}
