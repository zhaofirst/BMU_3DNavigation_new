﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;
using csmatio.io;
using csmatio.types;
using System.IO;
using Point = OpenCvSharp.Point;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Threading;






namespace Portable_BMU_App
{

    public partial class Frm_RealTime_Navigation_ZLX : Form
    {
        //----------------------- ----------------------------Start of parameter from Frm_RealTime_Navigation------------------ --------//


        // Points for the new line.
        private bool IsDrawing = false;
        private System.Drawing.Point NewPt1, NewPt2;


        //Current size of pic box
        private int picCanvas_ImageWidth, picCanvas_ImageHeight;
        // The current scale.
        private float ImageScale = 1.0f;


        //--------------------------------- from mainform PRP----------------------------------//
        // --------------------------- Display paras ------------------------------//
        public static double LATERAL_Resolution = 0;
        public static int Image_Width = 0;
        public static int Image_Height = 0;
        OpenCvSharp.Size dSizeFor3DDisplay;
        Thread threadRefreshNavigation;

        public static Vector3 boxOrg = new Vector3(0f, 0f, 0f);  // BoxSize and calibration Matrix
        public static int boxWidth = 0;                                               // BoxSize and calibration Matrix    原始图像三数据
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

        int ppi = 96;  // display
        int visualHeight = 0;
        int visualWidth = 0;

        //------------------------------------------End of parameter from Frm_RealTime_Navigation--------------------------//


        #region


        public Frm_RealTime_Navigation_ZLX()
        {
            InitializeComponent();
            //mainForm = this;
        }



        /// Volume 为short类型的三维建模数据
        /// depth 为脊柱高度
        /// saggital 
        /// 



        //   public static Frm_RealTime_Navigation_ZLX frm_RealTime_Navigation_ZLX;

        public int increasX;
        public int increasY;
        public int increasZ;

        public int Brightnessvalue = 100;
        public int Contrastvalue = 100;
        public Mat SagittalProjectionMat;
        public Mat CoronalProjectionMat;
        public static short[][,] volume;
        public static short[][,] volume_guan;
        public static short[][,] volume_shi;
        public static short[][,] volume_heng;
        public static int Original_Depth;
        public static int Original_Width;
        public static int Original_Height;



        // 用作记录没个pictureBox上图片的全局变量
        private Mat Picguan_clone;
        private Mat Picheng_clone;
        private Mat Picshi_clone;
        private Mat PicSagPro_clone;
        private Mat PicCorPro_clone;



        public static Mat volume_color;
        public static short[][,] volume_cut;
        public static MainForm mainForm;
        public static Mat srcG = null;
        public static Mat showMat = null;
        public static int SelectedColor = (int)ColorMap.Bone;
        //public static short[][,] volume_shi;
        public static MLInt16 mlSquares;
        public static int dataCount;

        public static int numofguan = 0;
        public static int numofshi = 0;
        public static int numofheng = 0;
        //

        public static ArrayList top_points = new ArrayList();
        public static ArrayList bottom_points = new ArrayList();
        public Mat coronalImgGlobal = null;
        public string OpenFilePath = string.Empty;
        private const int object_radius = 3;

        // We're over an object if the distance squared
        // between the mouse and the object is less than this.
        private const int over_dist_squared = object_radius * object_radius;

        // The points that make up the line segments.
        private List<System.Drawing.Point> Pt1 = new List<System.Drawing.Point>();
        private List<System.Drawing.Point> Pt2 = new List<System.Drawing.Point>();
        // Points for the new line.



        //Current size of pic box


        public enum ColorMap
        {
            Bone,
            Jet,
            Gray
        }
        /// <summary>
        /// 这里更改了原来chb的代码 
        /// 原来是按照H= getlength(1) W = getLength(0)这样来的
        /// 现在代码得到了H方向的最大值
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public short[] GetMatrixMax(short[,] matrix)
        {
            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            short[] rowVector = new short[matrixWidth];

            for (int i = 0; i < matrixWidth; i++)
            {
                short max = 0;
                for (int j = 0; j < matrixHeight; j++)
                {
                    short v = matrix[j, i];
                    if (v > max)
                    {
                        max = v;
                    }
                }
                //找到一列的最大值，放入行向量
                rowVector[i] = max;
            }
            return rowVector;
        }

        /// <summary>
        /// 这里更改了原来chb的代码 
        /// 原来是按照H= getlength(1) W = getLength(0)这样来的
        /// 现在代码得到了W的最大值，方便了saggital上面的投影
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public short[] GetMatrixMax2(short[,] matrix)
        {
            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            short[] rowVector = new short[matrixHeight];


            for (int i = 0; i < matrixHeight; i++)
            {
                short max = 0;
                for (int j = 0; j < matrixWidth; j++)
                {
                    short v = matrix[i, j];
                    if (v > max)
                    {
                        max = v;
                    }
                }
                //找到一列的最大值，放入行向量
                //rowVector[matrixWidth - i - 1 ] = max;
                rowVector[i] = max;

            }
            return rowVector;
        }

        /// <summary>
        /// 获得saggital面投影 (矢状）
        /// 这里
        /// </summary>
        public short[,] SaggitalProjection(short[][,] vol)
        {
            int depth = vol.Length;
            int width = vol[0].GetLength(1);
            int height = vol[0].GetLength(0);

            short[,] dMatrix = new short[depth, height];

            for (int k1 = 0; k1 < height; k1++)
            {
                for (int k2 = 0; k2 < depth; k2++)
                {
                    dMatrix[k2, k1] = 0;
                }
            }

            for (int k = 0; k < depth; k++)
            {

                short[,] bb = (short[,])vol[k].Clone();

                short[] c = this.GetMatrixMax2(bb);

                for (int m = 0; m < height; m++)
                {
                    //dMatrix[m, k] = c[height - 1 - m];
                    dMatrix[k, m] = c[m];

                }

            }
            return dMatrix;

        }

        /// <summary>
        /// 获得Coronal面投影 (冠状面）
        /// </summary>
        /// 
        public short[,] CoronalProjection(short[][,] vol)
        {
            int depth = vol.Length;
            int width = vol[0].GetLength(1);
            int height = vol[0].GetLength(0);

            short[,] dMatrix = new short[depth, width];
            for (int k1 = 0; k1 < width; k1++)
            {
                for (int k2 = 0; k2 < depth; k2++)
                {
                    dMatrix[k2, k1] = 0;
                }
            }
            for (int k = 0; k < depth; k++)
            {

                short[,] bb = (short[,])vol[k].Clone();

                short[] c = this.GetMatrixMax(bb);

                for (int m = 0; m < width; m++)
                {

                    //dMatrix[m, k] = c[height - 1 - m];
                    dMatrix[k, m] = c[m];

                }

            }
            return dMatrix;

        }

        /// <summary>


        private void InitializeMyScrollBar()
        {
            /// guan
            //HScrollBar hscroll_Coronal = new HScrollBar();

            //hscroll_Coronal.Dock = DockStyle.Bottom;
            hscroll_Coronal.Maximum = volume[0].GetLength(0);
            //this.hscroll_Coronal.Maximum += this.hscroll_Coronal.LargeChange;


            /// shi
            //HScrollBar hscroll_Sagittal = new HScrollBar();


            //hscroll_Sagittal.Dock = DockStyle.Bottom;
            hscroll_Sagittal.Maximum = volume[0].GetLength(1);
            Console.WriteLine(hscroll_Sagittal.Maximum);
            //this.hscroll_Sagittal.Maximum += this.hscroll_Sagittal.LargeChange;

            Console.WriteLine(hscroll_Sagittal.Maximum);
            /// heng
            //HScrollBar hscroll_transverse = new HScrollBar();


            //hscroll_transverse.Dock = DockStyle.Bottom;
            hscroll_transverse.Maximum = volume.Length;
            //this.hscroll_transverse.Maximum += this.hscroll_transverse.LargeChange;


            // Add the scroll bar to the form.
            //Controls.Add(hscroll_Coronal);
        }

        // picture 1 
        // 冠状面
        // height
        private void hscroll_Coronal_Scroll(object sender, ScrollEventArgs e)
        {
            if (volume == null)
            {

            }
            else
            {


                numofguan = hscroll_Coronal.Value;
                texdown_guan.Text = string.Format("{0}/{1}", numofguan + 1, Original_Height);
                hscroll_Coronal.Maximum = volume[0].GetLength(0) - 2;
                hscroll_Coronal.Maximum += hscroll_Coronal.LargeChange;

                UpdataFivePictureBox_Zoom_And_Mark(); //输出显示 并更新信息


            }

        }
        private void texdown_guan_KeyDown(object sender, KeyEventArgs e)
        {

            if (volume == null)
            {
                MessageBox.Show("You have not entered data！");
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string inputnow = texdown_shi.Text;
                    int input_number = int.Parse(inputnow) - 1;

                    int depth = volume.Length;
                    int height = volume[0].GetLength(0);
                    int width = volume[0].GetLength(1);
                    if (input_number <= height && input_number >= 0)
                    {
                        string tex_shi = string.Format("{0}/{1}", input_number + 1, height);
                        texdown_shi.Text = tex_shi;

                        // 输出一个冠状面
                        short[,] shi = new short[depth, width];
                        for (int i = 0; i < depth; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                shi[i, j] = volume[i][input_number, j];
                            }
                        }
                        // 结束输出

                        Mat picshiMat = new Mat();
                        picshiMat = Out_ShorttoMat(PicSag, shi, 2);


                        // 输出克隆面 /// 
                        Picshi_clone = picshiMat;
                        ///////////////

                        Mat brightnessMat = new Mat();
                        updateBrightnessContrast(picshiMat, brightnessMat, Brightnessvalue, Contrastvalue);
                        Bitmap showbitmap = new Bitmap(ConvertFile.MatToBitmap(brightnessMat));
                        PicSag.Image = showbitmap;

                    }
                    else
                    {
                        MessageBox.Show("输入错误");
                    }
                }



            }
        }

        //
        // 冠状面结束




        // picture 2
        // 矢状面 
        // 
        private void hscroll_Sagittal_Scroll(object sender, ScrollEventArgs e)
        {
            if (volume == null)
            {

            }
            else
            {
                numofshi = hscroll_Sagittal.Value;

                int depth = volume.Length;
                int height = volume[0].GetLength(0);
                int width = volume[0].GetLength(1);

                texdown_shi.Text = string.Format("{0}/{1}", numofshi + 1, width);
                hscroll_Sagittal.Maximum = volume[0].GetLength(1) - 2;
                hscroll_Sagittal.Maximum += hscroll_Sagittal.LargeChange;



                UpdataFivePictureBox_Zoom_And_Mark(); //输出显示 并更新信息



            }

        }
        private void texdown_shi_KeyDown(object sender, KeyEventArgs e)
        {

            if (volume == null)
            {
                MessageBox.Show("You have not entered data！");
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string inputnow = texdown_shi.Text;
                    int input_number = int.Parse(inputnow) - 1;

                    int depth = volume.Length;
                    int height = volume[0].GetLength(0);
                    int width = volume[0].GetLength(1);
                    if (input_number <= height && input_number >= 0)
                    {
                        string tex_shi = string.Format("{0}/{1}", input_number + 1, height);
                        texdown_shi.Text = tex_shi;

                        // 输出一个冠状面
                        short[,] shi = new short[depth, width];
                        for (int i = 0; i < depth; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                shi[i, j] = volume[i][input_number, j];
                            }
                        }
                        // 结束输出

                        Mat picshiMat = new Mat();
                        picshiMat = Out_ShorttoMat(PicSag, shi, 2);


                        // 输出克隆面 /// 
                        Picshi_clone = picshiMat;
                        ///////////////

                        Mat brightnessMat = new Mat();
                        updateBrightnessContrast(picshiMat, brightnessMat, Brightnessvalue, Contrastvalue);
                        Bitmap showbitmap = new Bitmap(ConvertFile.MatToBitmap(brightnessMat));
                        PicSag.Image = showbitmap;

                    }
                    else
                    {
                        MessageBox.Show("输入错误");
                    }
                }



            }
        }


        // 
        // 矢状面结束
        /// <summary>
        /// //////////////



        // picture 3
        //横断面 depth
        //

        private void hscroll_Transverse_Scroll(object sender, ScrollEventArgs e)
        {
            if (volume == null)
            {

            }
            else
            {
                numofheng = hscroll_transverse.Value;

                int depth = volume.Length;
                int height = volume[0].GetLength(0);
                int width = volume[0].GetLength(1);

                texdown_heng.Text = string.Format("{0}/{1}", numofheng + 1, depth);
                hscroll_transverse.Maximum = volume.Length - 2;
                hscroll_transverse.Maximum += hscroll_transverse.LargeChange;


                UpdataFivePictureBox_Zoom_And_Mark();




            }

        }
        private void texdown_heng_KeyDown(object sender, KeyEventArgs e)
        {

            if (volume == null)
            {
                MessageBox.Show("You have not entered data！");
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string inputnow = texdown_heng.Text;
                    int input_number = int.Parse(inputnow) - 1;

                    int depth = volume.Length;
                    int height = volume[0].GetLength(0);
                    int width = volume[0].GetLength(1);
                    if (input_number <= height && input_number >= 0)
                    {
                        string tex_heng = string.Format("{0}/{1}", input_number + 1, depth);
                        texdown_heng.Text = tex_heng;
                        hscroll_transverse.Value = input_number;

                        // 输出一个横断面
                        short[,] heng = new short[height, width];
                        heng = volume[input_number];

                        Mat pichengMat = new Mat();
                        pichengMat = Out_ShorttoMat(PicTra, heng, 0);


                        // 输出克隆面 /// 
                        Picheng_clone = pichengMat;
                        ///////////////

                        Mat brightnessMat = new Mat();
                        updateBrightnessContrast(pichengMat, brightnessMat, Brightnessvalue, Contrastvalue);
                        Bitmap showbitmap = new Bitmap(ConvertFile.MatToBitmap(brightnessMat));
                        PicTra.Image = showbitmap;

                    }
                    else
                    {
                        MessageBox.Show("输入错误");
                    }
                }



            }
        }


        #region 导航部分

        private void ThreeD_Navigation_Standing(int x, int y, int z)
        {


            Console.WriteLine("threed duanle  ", boxDepth, boxHeight, boxWidth);
            Console.WriteLine("threed duanle 3", Properties.Settings.Default.ScanPosition);
            int rectSize = 3;
            int indexX = x - 1;
            int indexY = y - 1;
            int indexZ = z - 1;


            // ----Coronal_Pro---- ///.



            Mat brightnessMat_Cor_Pro = new Mat();
            updateBrightnessContrast(CoronalProjectionMat, brightnessMat_Cor_Pro, Brightnessvalue, Contrastvalue);

            Mat CoronalImageColor_Pro = new Mat();
            Cv2.ApplyColorMap(brightnessMat_Cor_Pro, CoronalImageColor_Pro, ColormapTypes.Bone);
            Mat navigationCoronalImage = CoronalImageColor_Pro.Clone();
            Mat dstGCoronal = new Mat();
            float visual_Height_guan_Pro = (float)PicProCor.Width * Original_Depth * 2 / Original_Width;
            int visualHeight_guan_Pro = (int)Math.Round((float)visual_Height_guan_Pro);
            OpenCvSharp.Size dsize_Cornal = new OpenCvSharp.Size(PicProCor.Width, visualHeight_guan_Pro);
            Cv2.Resize(navigationCoronalImage, dstGCoronal, dsize_Cornal);
            int resizedCoronalIndex_Y = (int)(indexY * ((float)PicProCor.Width / boxWidth));
            int resizedCoronalIndex_Z = (int)(indexZ * ((float)visualHeight_guan_Pro / boxDepth));
            OpenCvSharp.Point startPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y, resizedCoronalIndex_Z);
            OpenCvSharp.Point endPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y + rectSize, resizedCoronalIndex_Z + rectSize);
            Cv2.Rectangle(dstGCoronal, startPoint, endPoint, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapCor = ConvertFile.MatToBitmap(dstGCoronal);
            this.PicProCor.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicProCor, bitmapCor); }));
            navigationCoronalImage = null;



            // ----Saggital_Pro---- ///.


            Mat brightnessMat_Sag_Pro = new Mat();
            updateBrightnessContrast(SagittalProjectionMat, brightnessMat_Sag_Pro, Brightnessvalue, Contrastvalue);



            Mat SaggitalImageColor_Pro = new Mat();
            Cv2.ApplyColorMap(brightnessMat_Sag_Pro, SaggitalImageColor_Pro, ColormapTypes.Bone);
            Mat navigationSaggImage = SaggitalImageColor_Pro.Clone();
            Mat dstGSagg = new Mat();
            float visual_Height_shi_Pro = (float)PicProSag.Width * Original_Depth * 2 / Original_Height;
            int visualHeight_shi_Pro = (int)Math.Round((float)visual_Height_shi_Pro);
            OpenCvSharp.Size dsize_Sag = new OpenCvSharp.Size(PicProSag.Width, visualHeight_shi_Pro);
            Cv2.Resize(navigationSaggImage, dstGSagg, dsize_Sag);
            int resizedSaggIndex_X = (int)(indexX * ((float)PicProSag.Width / boxHeight));
            int resizedSaggIndex_Z = (int)(indexZ * ((float)visualHeight_shi_Pro / boxDepth));
            //OpenCvSharp.Point startPoint2 = new OpenCvSharp.Point(resizedSaggIndex_X, resizedSaggIndex_Z);
            //OpenCvSharp.Point endPoint2 = new OpenCvSharp.Point(resizedSaggIndex_X + rectSize, resizedSaggIndex_Z + rectSize);


            OpenCvSharp.Point startPoint2 = new OpenCvSharp.Point(PicSag.Width - resizedSaggIndex_X - rectSize, resizedSaggIndex_Z);
            OpenCvSharp.Point endPoint2 = new OpenCvSharp.Point(PicSag.Width - resizedSaggIndex_X, resizedSaggIndex_Z + rectSize);


            Cv2.Rectangle(dstGSagg, startPoint2, endPoint2, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapSagg = ConvertFile.MatToBitmap(dstGSagg);
            this.PicProSag.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicProSag, bitmapSagg); }));
            navigationSaggImage = null;


            // ----Coronal---- ///.





            short[,] guan = new short[Original_Depth, Original_Width];
            guan = volume_guan[indexX];
            short[][,] test = volume_guan;
            short[][,] test2 = volume_heng;
            Mat Guan_Mat = Out_ShorttoMat(PicCor, guan, 1);

            Mat brightnessMat_Cor = new Mat();
            updateBrightnessContrast(Guan_Mat, brightnessMat_Cor, Brightnessvalue, Contrastvalue);


            Mat GuanImageColor = new Mat();
            Cv2.ApplyColorMap(brightnessMat_Cor, GuanImageColor, ColormapTypes.Bone);
            Mat navigationGuanImage = GuanImageColor.Clone();
            Mat dstGGuan = new Mat();
            float visual_Height_Guan = (float)PicCor.Width * Original_Depth * 2 / Original_Width;
            int visualHeight_Guan = (int)Math.Round((float)visual_Height_Guan);
            OpenCvSharp.Size dsize_guan = new OpenCvSharp.Size(PicCor.Width, visualHeight_Guan);
            Cv2.Resize(navigationGuanImage, dstGGuan, dsize_guan);
            int resizedGuanIndex_Y = (int)(indexY * ((float)PicCor.Width / boxWidth));
            int resizedGuanIndex_Z = (int)(indexZ * ((float)visualHeight_Guan / boxDepth));
            OpenCvSharp.Point startPoint3 = new OpenCvSharp.Point(resizedGuanIndex_Y, resizedGuanIndex_Z);
            OpenCvSharp.Point endPoint3 = new OpenCvSharp.Point(resizedGuanIndex_Y + rectSize, resizedGuanIndex_Z + rectSize);
            Cv2.Rectangle(dstGGuan, startPoint3, endPoint3, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapGuan = ConvertFile.MatToBitmap(dstGGuan);
            this.PicCor.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicCor, bitmapGuan); }));
            navigationGuanImage = null;
            string tex_guan = string.Format("{0}/{1}", indexX + 1, Original_Height);
            texdown_guan.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_guan, tex_guan); }));


            // ----Saggital---- ///.

            short[,] shi = new short[Original_Depth, Original_Width];
            shi = volume_shi[indexY];
            Mat Shi_Mat = Out_ShorttoMat(PicSag, shi, 2);

            Mat brightnessMat_Sag = new Mat();
            updateBrightnessContrast(Shi_Mat, brightnessMat_Sag, Brightnessvalue, Contrastvalue);



            Mat ShiImageColor = new Mat();
            Cv2.ApplyColorMap(brightnessMat_Sag, ShiImageColor, ColormapTypes.Bone);
            Mat navigationShiImage = ShiImageColor.Clone();
            Mat dstGShi = new Mat();
            float visual_Height_Shi = (float)PicSag.Width * Original_Depth * 2 / Original_Height;
            int visualHeight_Shi = (int)Math.Round((float)visual_Height_Shi);
            OpenCvSharp.Size dsize_shi = new OpenCvSharp.Size(PicSag.Width, visualHeight_Shi);
            Cv2.Resize(navigationShiImage, dstGShi, dsize_shi);
            int resizedShiIndex_X = (int)(indexX * ((float)PicSag.Width / boxHeight));
            int resizedShiIndex_Z = (int)(indexZ * ((float)visualHeight_Shi / boxDepth));
            //OpenCvSharp.Point startPoint4 = new OpenCvSharp.Point(resizedShiIndex_X, resizedShiIndex_Z);
            OpenCvSharp.Point startPoint4 = new OpenCvSharp.Point(PicSag.Width - resizedShiIndex_X - rectSize, resizedShiIndex_Z);
            OpenCvSharp.Point endPoint4 = new OpenCvSharp.Point(PicSag.Width - resizedShiIndex_X, resizedShiIndex_Z + rectSize);
            Cv2.Rectangle(dstGShi, startPoint4, endPoint4, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapShi = ConvertFile.MatToBitmap(dstGShi);
            this.PicSag.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicSag, bitmapShi); }));
            navigationShiImage = null;
            string tex_shi = string.Format("{0}/{1}", indexY + 1, Original_Width);
            texdown_guan.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_shi, tex_shi); }));



            // ----Cross----///

            short[,] heng = new short[Original_Height, Original_Width];
            heng = volume_heng[indexZ];
            Mat Heng_Mat = Out_ShorttoMat(PicTra, heng, 0);

            Mat brightnessMat_Cross = new Mat();
            updateBrightnessContrast(Heng_Mat, brightnessMat_Cross, Brightnessvalue, Contrastvalue);


            Mat HengImageColor = new Mat();
            Cv2.ApplyColorMap(brightnessMat_Cross, HengImageColor, ColormapTypes.Bone);
            Mat navigationHengImage = HengImageColor.Clone();
            Mat dstGHeng = new Mat();
            float visual_Height_Heng = (float)PicTra.Width * Original_Height / Original_Height;
            int visualHeight_Heng = (int)Math.Round((float)visual_Height_Heng);
            OpenCvSharp.Size dsize_heng = new OpenCvSharp.Size(PicTra.Width, visualHeight_Heng);
            Cv2.Resize(navigationHengImage, dstGHeng, dsize_heng);
            int resizedHengIndex_X = (int)(indexX * ((float)PicTra.Width / boxHeight));
            int resizedHengIndex_Y = (int)(indexY * ((float)visualHeight_Heng / boxWidth));
            OpenCvSharp.Point startPoint5 = new OpenCvSharp.Point(resizedHengIndex_Y, visualHeight_Heng - resizedHengIndex_X - rectSize);
            OpenCvSharp.Point endPoint5 = new OpenCvSharp.Point(resizedHengIndex_Y + rectSize, visualHeight_Heng - resizedHengIndex_X);
            Cv2.Rectangle(dstGHeng, startPoint5, endPoint5, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapHeng = ConvertFile.MatToBitmap(dstGHeng);
            this.PicTra.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicTra, bitmapHeng); }));
            navigationHengImage = null;
            string tex_heng = string.Format("{0}/{1}", indexZ + 1, Original_Depth);
            texdown_guan.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_heng, tex_heng); }));
            /////////////////////
            GC.Collect();
        }



        private void ThreeD_Navigation_Prone(int x, int y, int z)
        {
            int rectSize = 3;
            int indexX = x - 1;
            int indexY = y - 1;
            int indexZ = z - 1;
            // ----Coronal_Pro---- ///.
            Mat CoronalImageColor_Pro = new Mat();
            Cv2.ApplyColorMap(CoronalProjectionMat, CoronalImageColor_Pro, ColormapTypes.Bone);
            Mat navigationCoronalImage = CoronalImageColor_Pro.Clone();
            Mat dstGCoronal = new Mat();
            float visual_Height_guan_Pro = (float)PicProCor.Width * Original_Depth * 2 / Original_Width;
            int visualHeight_guan_Pro = (int)Math.Round((float)visual_Height_guan_Pro);
            OpenCvSharp.Size dsize_Cornal = new OpenCvSharp.Size(PicProCor.Width, visualHeight_guan_Pro);
            Cv2.Resize(navigationCoronalImage, dstGCoronal, dsize_Cornal);
            int resizedCoronalIndex_Y = (int)(indexY * ((float)PicProCor.Width / boxWidth));
            int resizedCoronalIndex_Z = (int)(indexZ * ((float)visualHeight_guan_Pro / boxDepth));
            OpenCvSharp.Point startPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y, resizedCoronalIndex_Z);
            OpenCvSharp.Point endPoint = new OpenCvSharp.Point(resizedCoronalIndex_Y + rectSize, resizedCoronalIndex_Z + rectSize);
            Cv2.Rectangle(dstGCoronal, startPoint, endPoint, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapCor = ConvertFile.MatToBitmap(dstGCoronal);
            this.PicProCor.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicProCor, bitmapCor); }));
            navigationCoronalImage = null;



            // ----Saggital_Pro---- ///.

            Mat SaggitalImageColor_Pro = new Mat();
            Cv2.ApplyColorMap(SagittalProjectionMat, SaggitalImageColor_Pro, ColormapTypes.Bone);
            Mat navigationSaggImage = SaggitalImageColor_Pro.Clone();
            Mat dstGSagg = new Mat();
            float visual_Height_shi_Pro = (float)PicProSag.Width * Original_Depth * 2 / Original_Height;
            int visualHeight_shi_Pro = (int)Math.Round((float)visual_Height_shi_Pro);
            OpenCvSharp.Size dsize_Sag = new OpenCvSharp.Size(PicProSag.Width, visualHeight_shi_Pro);
            Cv2.Resize(navigationSaggImage, dstGSagg, dsize_Sag);
            int resizedSaggIndex_X = (int)(indexX * ((float)PicProSag.Width / boxHeight));
            int resizedSaggIndex_Z = (int)(indexZ * ((float)visualHeight_shi_Pro / boxDepth));
            OpenCvSharp.Point startPoint2 = new OpenCvSharp.Point(resizedSaggIndex_X, resizedSaggIndex_Z);
            OpenCvSharp.Point endPoint2 = new OpenCvSharp.Point(resizedSaggIndex_X + rectSize, resizedSaggIndex_Z + rectSize);
            Cv2.Rectangle(dstGSagg, startPoint2, endPoint2, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapSagg = ConvertFile.MatToBitmap(dstGSagg);
            this.PicProSag.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicProSag, bitmapSagg); }));
            navigationSaggImage = null;


            // ----Coronal---- ///.


            short[,] guan = new short[Original_Depth, Original_Width];
            guan = volume_guan[indexX];
            short[][,] test = volume_guan;
            short[][,] test2 = volume_heng;
            Mat Guan_Mat = Out_ShorttoMat(PicCor, guan, 1);
            Mat GuanImageColor = new Mat();
            Cv2.ApplyColorMap(Guan_Mat, GuanImageColor, ColormapTypes.Bone);
            Mat navigationGuanImage = GuanImageColor.Clone();
            Mat dstGGuan = new Mat();
            float visual_Height_Guan = (float)PicCor.Width * Original_Depth * 2 / Original_Width;
            int visualHeight_Guan = (int)Math.Round((float)visual_Height_Guan);
            OpenCvSharp.Size dsize_guan = new OpenCvSharp.Size(PicCor.Width, visualHeight_Guan);
            Cv2.Resize(navigationGuanImage, dstGGuan, dsize_guan);
            int resizedGuanIndex_Y = (int)(indexY * ((float)PicCor.Width / boxWidth));
            int resizedGuanIndex_Z = (int)(indexZ * ((float)visualHeight_Guan / boxDepth));
            OpenCvSharp.Point startPoint3 = new OpenCvSharp.Point(resizedGuanIndex_Y, resizedGuanIndex_Z);
            OpenCvSharp.Point endPoint3 = new OpenCvSharp.Point(resizedGuanIndex_Y + rectSize, resizedGuanIndex_Z + rectSize);
            Cv2.Rectangle(dstGGuan, startPoint3, endPoint3, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapGuan = ConvertFile.MatToBitmap(dstGGuan);
            this.PicCor.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicCor, bitmapGuan); }));
            navigationGuanImage = null;
            string tex_guan = string.Format("{0}/{1}", indexX + 1, Original_Height);
            texdown_guan.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_guan, tex_guan); }));


            // ----Saggital---- ///.

            short[,] shi = new short[Original_Depth, Original_Width];
            shi = volume_shi[indexY];
            Mat Shi_Mat = Out_ShorttoMat(PicSag, shi, 2);


            Mat ShiImageColor = new Mat();
            Cv2.ApplyColorMap(Shi_Mat, ShiImageColor, ColormapTypes.Bone);
            Mat navigationShiImage = ShiImageColor.Clone();
            Mat dstGShi = new Mat();
            float visual_Height_Shi = (float)PicSag.Width * Original_Depth * 2 / Original_Height;
            int visualHeight_Shi = (int)Math.Round((float)visual_Height_Shi);
            OpenCvSharp.Size dsize_shi = new OpenCvSharp.Size(PicSag.Width, visualHeight_Shi);
            Cv2.Resize(navigationShiImage, dstGShi, dsize_shi);
            int resizedShiIndex_X = (int)(indexX * ((float)PicSag.Width / boxHeight));
            int resizedShiIndex_Z = (int)(indexZ * ((float)visualHeight_Shi / boxDepth));
            //OpenCvSharp.Point startPoint4 = new OpenCvSharp.Point(resizedShiIndex_X, resizedShiIndex_Z);
            OpenCvSharp.Point startPoint4 = new OpenCvSharp.Point(resizedShiIndex_X, resizedShiIndex_Z);
            OpenCvSharp.Point endPoint4 = new OpenCvSharp.Point(resizedShiIndex_X + rectSize, resizedShiIndex_Z + rectSize);
            Cv2.Rectangle(dstGShi, startPoint4, endPoint4, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapShi = ConvertFile.MatToBitmap(dstGShi);
            this.PicSag.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicSag, bitmapShi); }));
            navigationShiImage = null;
            string tex_shi = string.Format("{0}/{1}", indexY + 1, Original_Width);
            texdown_guan.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_shi, tex_shi); }));



            // ----Cross----///

            short[,] heng = new short[Original_Height, Original_Width];
            heng = volume_heng[indexZ];
            Mat Heng_Mat = Out_ShorttoMat(PicTra, heng, 0);
            Mat HengImageColor = new Mat();
            Cv2.ApplyColorMap(Heng_Mat, HengImageColor, ColormapTypes.Bone);
            Mat navigationHengImage = HengImageColor.Clone();
            Mat dstGHeng = new Mat();
            float visual_Height_Heng = (float)PicTra.Width * Original_Height / Original_Width;
            int visualHeight_Heng = (int)Math.Round((float)visual_Height_Heng);
            OpenCvSharp.Size dsize_heng = new OpenCvSharp.Size(PicTra.Width, visualHeight_Heng);
            Cv2.Resize(navigationHengImage, dstGHeng, dsize_heng);
            int resizedHengIndex_X = (int)(indexX * ((float)PicTra.Width / boxHeight));
            int resizedHengIndex_Y = (int)(indexY * ((float)visualHeight_Heng / boxWidth));
            OpenCvSharp.Point startPoint5 = new OpenCvSharp.Point(resizedHengIndex_Y, resizedHengIndex_X);
            OpenCvSharp.Point endPoint5 = new OpenCvSharp.Point(resizedHengIndex_Y + rectSize, resizedHengIndex_X + rectSize);
            Cv2.Rectangle(dstGHeng, startPoint5, endPoint5, OpenCvSharp.Scalar.Red, -1);
            Bitmap bitmapHeng = ConvertFile.MatToBitmap(dstGHeng);
            this.PicTra.BeginInvoke(new MethodInvoker(delegate { PictureBoxShow3D(PicTra, bitmapHeng); }));
            navigationHengImage = null;
            string tex_heng = string.Format("{0}/{1}", indexZ + 1, Original_Depth);
            texdown_guan.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_heng, tex_heng); }));
            /////////////////////
            GC.Collect();
        }

        #endregion


        /// <summary>
        ///  用于图片short 转换成可以resize的Mat类型

        private Mat Out_ShorttoMat(object sender, short[,] pic_input, int parameter)
        {
            int Pic_Width;
            int Pic_Height;
            Pic_Width = ((PictureBox)sender).Width;
            Pic_Height = ((PictureBox)sender).Height;
            byte[,] byteG2 = ConvertFile.ShortArrayToByte(pic_input);
            Mat srcG2 = ConvertFile.ArrayToMat(byteG2);

            switch (parameter)
            {
                case 0:
                    float visual_Height_heng = (float)Pic_Width * Original_Height / Original_Width;
                    int visualHeight_heng = (int)Math.Round((float)visual_Height_heng);
                    OpenCvSharp.Size dsize_heng = new OpenCvSharp.Size(Pic_Width, visualHeight_heng);
                    Mat Visualpic_heng = new Mat();
                    Cv2.Resize(srcG2, Visualpic_heng, dsize_heng);
                    return Visualpic_heng;

                //break;
                case 1:
                    float visual_Height_guan = (float)Pic_Width * Original_Depth * 2 / Original_Width;
                    int visualHeight_guan = (int)Math.Round((float)visual_Height_guan);
                    OpenCvSharp.Size dsize_guan = new OpenCvSharp.Size(Pic_Width, visualHeight_guan);
                    Mat Visualpic_guan = new Mat();
                    Cv2.Resize(srcG2, Visualpic_guan, dsize_guan);
                    return Visualpic_guan;
                //break;
                case 2:
                    float visual_Height_shi = (float)Pic_Width * Original_Depth * 2 / Original_Height;
                    int visualHeight_shi = (int)Math.Round((float)visual_Height_shi);
                    OpenCvSharp.Size dsize_shi = new OpenCvSharp.Size(Pic_Width, visualHeight_shi);
                    Mat Visualpic_shi = new Mat();
                    Cv2.Resize(srcG2, Visualpic_shi, dsize_shi);
                    return Visualpic_shi;


                default:
                    Console.WriteLine(" Invalid input");
                    return null;


            }
        }

        /// <summary>
        ///  Display ZLX
        /// </summary> 按比例显示图像到picbox上
        /// 输入为 short[,](volume 中读取数据） 输出为显示好的bitmap
        /// 参数parameter代表选择哪个面的显示 ，0表示横断面,1表冠状面,2表矢状面.
        private void Display_zlx(object sender, short[,] pic_input, int parameter)
        {
            int Pic_Width;
            int Pic_Height;
            Pic_Width = ((PictureBox)sender).Width;
            Pic_Height = ((PictureBox)sender).Height;
            byte[,] byteG2 = ConvertFile.ShortArrayToByte(pic_input);
            Mat srcG2 = ConvertFile.ArrayToMat(byteG2);

            switch (parameter)
            {
                case 0:
                    float visual_Height_heng = (float)Pic_Width * Original_Height / Original_Width;
                    int visualHeight_heng = (int)Math.Round((float)visual_Height_heng);
                    OpenCvSharp.Size dsize_heng = new OpenCvSharp.Size(Pic_Width, visualHeight_heng);
                    Mat Visualpic_heng = new Mat();
                    Cv2.Resize(srcG2, Visualpic_heng, dsize_heng);
                    Bitmap Visualpic_hengBitmap = ConvertFile.MatToBitmap(Visualpic_heng);
                    ((PictureBox)sender).Image = Visualpic_hengBitmap;
                    break;
                case 1:
                    float visual_Height_guan = (float)Pic_Width * Original_Depth * 2 / Original_Width;
                    int visualHeight_guan = (int)Math.Round((float)visual_Height_guan);
                    OpenCvSharp.Size dsize_guan = new OpenCvSharp.Size(Pic_Width, visualHeight_guan);
                    Mat Visualpic_guan = new Mat();
                    Cv2.Resize(srcG2, Visualpic_guan, dsize_guan);
                    Bitmap Visualpic_guanBitmap = ConvertFile.MatToBitmap(Visualpic_guan);
                    //Visualpic_guanBitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                    ((PictureBox)sender).Image = Visualpic_guanBitmap;
                    break;
                case 2:
                    float visual_Height_shi = (float)Pic_Width * Original_Depth * 2 / Original_Height;
                    int visualHeight_shi = (int)Math.Round((float)visual_Height_shi);
                    OpenCvSharp.Size dsize_shi = new OpenCvSharp.Size(Pic_Width, visualHeight_shi);
                    Mat Visualpic_shi = new Mat();
                    Cv2.Resize(srcG2, Visualpic_shi, dsize_shi);
                    Bitmap Visualpic_shiBitmap = ConvertFile.MatToBitmap(Visualpic_shi);
                    //Visualpic_shiBitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                    ((PictureBox)sender).Image = Visualpic_shiBitmap;
                    break;

                default:
                    Console.WriteLine(" Invalid input");
                    break;

            }


        }
        // 限制输入为数字
        private void tBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
        }
        //结束

        /// <summary>
        /// chb 寻找二维map图片列或者行最大值函数
        /// getMatrixMax 寻找Width 最大值
        /// </summary>



        private void num_increaseX_ValueChanged(object sender, EventArgs e)
        {
            increasX = Convert.ToInt32(((NumericUpDown)sender).Value);
        }
        private void num_increaseY_ValueChanged(object sender, EventArgs e)
        {
            increasY = Convert.ToInt32(((NumericUpDown)sender).Value);
        }

        private void num_increaseZ_ValueChanged(object sender, EventArgs e)
        {
            increasZ = Convert.ToInt32(((NumericUpDown)sender).Value);
        }



        #region 对比度部分

        private void BrightnessBarContrastBar_Scroll(object sender, EventArgs e)
        {

            Contrastvalue = ContrastBar.Value;
            Brightnessvalue = BrightnessBar.Value;
            UpdataFivePictureBox_Zoom_And_Mark();

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
        #endregion




        #region 数据输入部分

        public void toolStripMenuItemOpen_Click(object sender, EventArgs e)
        {

            // Prompts user to select a folder 
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Supported files (*.jpg,*.bmp,*.png,*.mat)|*.jpg;*.bmp;*.png;*.mat|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                string filePath = openFileDialog.FileName;
                Direction.Text = openFileDialog.FileName;
                this.OpenFilePath = filePath;
                Console.WriteLine("Volume File: {0}", this.OpenFilePath);


                //return;
                var mfr = new MatFileReader(filePath);
                dataCount = mfr.Data.Count();
                Console.WriteLine(mfr.MatFileHeader.ToString());

                mlSquares = (mfr.Content["usData"] as MLInt16);
                int height = mlSquares.Dimensions[0];
                int width = mlSquares.Dimensions[1];
                int depth = mlSquares.Dimensions[2];

                PicSag.Width = (int)(PicTra.Width * height / width);  // 使得Sag面的宽度保持和Tra面的高度一致
                int poX = (int)((panel3.Width - PicSag.Width) / 2);
                int poY = 63;
                PicSag.Location = new System.Drawing.Point(poX, PicSag.Location.Y);
                PicProSag.Width = (int)(PicTra.Width * height / width);  // 使得Sag面的宽度保持和Tra面的高度一致
                PicProSag.Location = new System.Drawing.Point(poX, PicProSag.Location.Y);
                Console.WriteLine(mlSquares);

                Console.WriteLine("Volume size, height*width*depth : {0}, {1},{2}", height, width, depth);
                byte[] tempArray = mlSquares.RealByteBuffer.Array();


                volume = new short[depth][,];
                //short[,] aa; 
                for (int i = 0; i < depth; i++)
                {
                    volume[i] = new short[height, width];
                    //aa = volume[i];
                }

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int idx = y + (x + z * width) * height;
                            short val = tempArray[idx * 2];

                            if (dataCount > 2)
                            {
                                if (val < 0)
                                    volume[z][y, x] = short.MaxValue;
                                else
                                    volume[z][y, x] = (short)val;
                            }
                            else
                            {
                                if (val < 0)
                                    volume[z][height - 1 - y, x] = short.MaxValue;
                                else
                                    volume[z][height - 1 - y, x] = (short)val;
                            }

                        }
                    }
                }






                Original_Depth = volume.Length;
                Original_Height = volume[0].GetLength(0);
                Original_Width = volume[0].GetLength(1);

                //这里完成3个面的切片
                volume_guan = new short[Original_Height][,];
                for (int i = 0; i < Original_Height; i++)
                {
                    volume_guan[i] = new short[Original_Depth, Original_Width];
                    //aa = volume[i];
                }
                for (int m = 0; m < Original_Height; m++)
                {
                    for (int i = 0; i < depth; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            volume_guan[m][i, j] = volume[i][m, j];
                        }
                    }
                }

                volume_shi = new short[Original_Width][,];
                for (int i = 0; i < Original_Width; i++)
                {
                    volume_shi[i] = new short[Original_Depth, Original_Height];
                    //aa = volume[i];
                }
                for (int m = 0; m < Original_Width; m++)
                {
                    for (int i = 0; i < depth; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            volume_shi[m][i, j] = volume[i][j, m];
                        }
                    }
                }




                volume_heng = new short[Original_Depth][,];
                for (int i = 0; i < Original_Depth; i++)
                {
                    volume_heng[i] = volume[i];
                }



                /// 初始化以及各个面显示图像

                InitializeMyScrollBar();

                // SaggitalProjection


                short[,] SaggitalImage_zlx = SaggitalProjection(volume);
                byte[,] byteG2 = ConvertFile.ShortArrayToByte(SaggitalImage_zlx);
                SagittalProjectionMat = ConvertFile.ArrayToMat(byteG2);

                PicSagPro_clone = Out_ShorttoMat(PicProSag, SaggitalImage_zlx, 2);
                Display_zlx(PicProSag, SaggitalImage_zlx, 2);

                // CoronalProjection


                short[,] CoronalImage_zlx = CoronalProjection(volume);
                byte[,] byteG22 = ConvertFile.ShortArrayToByte(CoronalImage_zlx);
                CoronalProjectionMat = ConvertFile.ArrayToMat(byteG22);
                PicCorPro_clone = Out_ShorttoMat(PicProCor, CoronalImage_zlx, 1);
                Display_zlx(PicProCor, CoronalImage_zlx, 1);

                float midHf = Original_Height / 2;
                int midHint = (int)Math.Round(midHf);

                float midDf = Original_Depth / 2;
                int midDint = (int)Math.Round(midDf);

                float midWf = Original_Width / 2;
                int midWint = (int)Math.Round(midWf);


                //Guan
                short[,] guan = volume_guan[midHint];
                Mat picguanMat = new Mat();
                picguanMat = Out_ShorttoMat(PicCor, guan, 1);
                Picguan_clone = picguanMat;
                string tex_guan = string.Format("{0}/{1}", midHint + 1, Original_Height);
                hscroll_Coronal.Value = midHint;
                texdown_guan.Text = tex_guan;
                PicCor.Image = ConvertFile.MatToBitmap(picguanMat);


                //Shi 
                short[,] shi = volume_shi[midWint];
                Mat picshiMat = new Mat();
                picshiMat = Out_ShorttoMat(PicSag, shi, 2);
                Picshi_clone = picshiMat;
                hscroll_Sagittal.Value = midWint;
                string tex_shi = string.Format("{0}/{1}", midWint + 1, Original_Width);
                texdown_shi.Text = tex_shi;
                PicSag.Image = ConvertFile.MatToBitmap(picshiMat);


                //heng 
                short[,] heng = volume_heng[midDint];
                Mat pichengMat = new Mat();
                pichengMat = Out_ShorttoMat(PicTra, heng, 0);
                Picheng_clone = pichengMat;
                hscroll_transverse.Value = midDint;
                string tex_heng = string.Format("{0}/{1}", midDint + 1, Original_Depth);
                texdown_heng.Text = tex_heng;
                PicTra.Image = ConvertFile.MatToBitmap(pichengMat);





                Console.WriteLine(dataCount);




            }


        }

        public void Initialize_five_Picture()
        {


            Original_Depth = volume.Length;
            Original_Height = volume[0].GetLength(0);
            Original_Width = volume[0].GetLength(1);

            int depth = Original_Depth;
            int width = Original_Width;
            int height = Original_Height;

      
            //这里完成3个面的切片
            volume_guan = new short[Original_Height][,];
            for (int i = 0; i < Original_Height; i++)
            {
                volume_guan[i] = new short[Original_Depth, Original_Width];
                //aa = volume[i];
            }
            for (int m = 0; m < Original_Height; m++)
            {
                for (int i = 0; i < depth; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        volume_guan[m][i, j] = volume[i][m, j];
                    }
                }
            }




            volume_shi = new short[Original_Width][,];
            for (int i = 0; i < Original_Width; i++)
            {
                volume_shi[i] = new short[Original_Depth, Original_Height];
                //aa = volume[i];
            }
            for (int m = 0; m < Original_Width; m++)
            {
                for (int i = 0; i < depth; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        volume_shi[m][i, j] = volume[i][j, m];
                    }
                }
            }


            volume_heng = new short[Original_Depth][,];
            for (int i = 0; i < Original_Depth; i++)
            {
                volume_heng[i] = volume[i];
            }



            /// 初始化以及各个面显示图像

            InitializeMyScrollBar();

            // SaggitalProjection




            short[,] SaggitalImage_zlx = SaggitalProjection(volume);
            byte[,] byteG2 = ConvertFile.ShortArrayToByte(SaggitalImage_zlx);
            SagittalProjectionMat = ConvertFile.ArrayToMat(byteG2);

            PicSagPro_clone = Out_ShorttoMat(PicProSag, SaggitalImage_zlx, 2);
            Display_zlx(PicProSag, SaggitalImage_zlx, 2);

            // CoronalProjection


            short[,] CoronalImage_zlx = CoronalProjection(volume);
            byte[,] byteG22 = ConvertFile.ShortArrayToByte(CoronalImage_zlx);
            CoronalProjectionMat = ConvertFile.ArrayToMat(byteG22);
            PicCorPro_clone = Out_ShorttoMat(PicProCor, CoronalImage_zlx, 1);
            Display_zlx(PicProCor, CoronalImage_zlx, 1);



            float midHf = Original_Height / 2;
            int midHint = (int)Math.Round(midHf);

            float midDf = Original_Depth / 2;
            int midDint = (int)Math.Round(midDf);

            float midWf = Original_Width / 2;
            int midWint = (int)Math.Round(midWf);


            //Guan
            short[,] guan = volume_guan[midHint];
            Mat picguanMat = new Mat();
            picguanMat = Out_ShorttoMat(PicCor, guan, 1);
            Picguan_clone = picguanMat;
            string tex_guan = string.Format("{0}/{1}", midWint + 1, Original_Height);
            hscroll_Coronal.Value = midHint;
            texdown_guan.Text = tex_guan;
            PicCor.Image = ConvertFile.MatToBitmap(picguanMat);


            //Shi 
            short[,] shi = volume_shi[midWint];
            Mat picshiMat = new Mat();
            picshiMat = Out_ShorttoMat(PicSag, shi, 2);
            Picshi_clone = picshiMat;
            hscroll_Sagittal.Value = midWint;
            string tex_shi = string.Format("{0}/{1}", midWint + 1, Original_Width);
            texdown_shi.Text = tex_shi;
            PicSag.Image = ConvertFile.MatToBitmap(picshiMat);


            //heng 
            short[,] heng = volume_heng[midDint];
            Mat pichengMat = new Mat();
            pichengMat = Out_ShorttoMat(PicTra, heng, 0);
            Picheng_clone = pichengMat;
            hscroll_transverse.Value = midDint;
            string tex_heng = string.Format("{0}/{1}", midDint + 1, Original_Depth);
            texdown_heng.Text = tex_heng;
            PicTra.Image = ConvertFile.MatToBitmap(pichengMat);





            PicSag.Width = (int)(PicTra.Width * height / width);  // 使得Sag面的宽度保持和Tra面的高度一致
            int poX = (int)((panel3.Width - PicSag.Width) / 2);
            int poY = 63;
            PicSag.Location = new System.Drawing.Point(poX, poY);
            PicProSag.Width = (int)(PicTra.Width * height / width);  // 使得Sag面的宽度保持和Tra面的高度一致
            PicProSag.Location = new System.Drawing.Point(poX, poY);
            //ThreeD_Navigation(200, 100, 90);
            Console.WriteLine(dataCount);
        }

        #endregion


        #region 图片缩放与标记  注意这里记得在Design中给每个pictureBox 添加滚轮控件函数
        //该部分目前有个bug，每次Click原地都会自动加一，猜测是由于int float类型转换的时候计算不精准出现的问题。

        // Zoom Button //

        int Click_Depth = 0; // 记录标记点在Volume中的位置
        int Click_Width = 0;
        int Click_Height = 0;

        Mat PictureCor_Original_Zoom = new Mat(); //原始图像
        Mat PictureSag_Original_Zoom = new Mat(); //原始图像
        Mat PictureTra_Original_Zoom = new Mat(); //原始图像
        Mat PictureProCor_Original_Zoom = new Mat(); //原始图像
        Mat PictureProSag_Original_Zoom = new Mat(); //原始图像

        System.Drawing.Point LocationCor_Point_Zoom = new System.Drawing.Point(0, 0);  //记录放缩位置在原始图像中的位置
        System.Drawing.Point LocationSag_Point_Zoom = new System.Drawing.Point(0, 0);  //记录放缩位置在原始图像中的位置
        System.Drawing.Point LocationTra_Point_Zoom = new System.Drawing.Point(0, 0);  //记录放缩位置在原始图像中的位置
        System.Drawing.Point LocationProCor_Point_Zoom = new System.Drawing.Point(0, 0);  //记录放缩位置在原始图像中的位置
        System.Drawing.Point LocationProSag_Point_Zoom = new System.Drawing.Point(0, 0);  //记录放缩位置在原始图像中的位置


        bool MarkExist = false; //判断是否存在Mark

        System.Drawing.Point ShowPosition_Cor_Point_Mark = new System.Drawing.Point(0, 0);  //记录标记位置在要显示的图像中的位置
        System.Drawing.Point ShowPosition_Sag_Point_Mark = new System.Drawing.Point(0, 0);  //记录标记位置在要显示的图像中的位置
        System.Drawing.Point ShowPosition_Tra_Point_Mark = new System.Drawing.Point(0, 0);  //记录标记位置在要显示的图像中的位置
        System.Drawing.Point ShowPosition_ProCor_Point_Mark = new System.Drawing.Point(0, 0);  //记录标记位置在要显示的图像中的位置
        System.Drawing.Point ShowPosition_ProSag_Point_Mark = new System.Drawing.Point(0, 0);  //记录标记位置在要显示的图像中的位置



        // 放缩用放缩因子是否大于一判断图片是否处于放缩状态
        double Cor_Scale_Zoom = 1; //记录放缩倍数 （>=1）
        double Sag_Scale_Zoom = 1; //记录放缩倍数 （>=1）
        double Tra_Scale_Zoom = 1; //记录放缩倍数 （>=1）
        double ProCor_Scale_Zoom = 1; //记录放缩倍数 （>=1）
        double ProSag_Scale_Zoom = 1; //记录放缩倍数 （>=1）




        // 辅助功能函数 //
        void UpdataTextAndScroll() //更新TextBox的值和Scroll的值
        {
            string tex_guan = string.Format("{0}/{1}", Click_Height + 1, Original_Height);
            hscroll_Coronal.Value = Click_Height;
            texdown_guan.Text = tex_guan;

            hscroll_transverse.Value = Click_Depth;
            string tex_heng = string.Format("{0}/{1}", Click_Depth + 1, Original_Depth);
            texdown_heng.Text = tex_heng;

            hscroll_Sagittal.Value = Click_Width;
            string tex_shi = string.Format("{0}/{1}", Click_Width + 1, Original_Width);
            texdown_shi.Text = tex_shi;


        }
        void TextToMat_Zoom()//依据TextBox上的数值从volume读取五个mat的图像 （初始化上述的原始图像五个参数）
        {
            int Tran = System.Convert.ToInt32(texdown_heng.Text.Substring(0, texdown_heng.Text.IndexOf("/")));
            int Sag = System.Convert.ToInt32(texdown_shi.Text.Substring(0, texdown_shi.Text.IndexOf("/")));
            int Cor = System.Convert.ToInt32(texdown_guan.Text.Substring(0, texdown_guan.Text.IndexOf("/")));


            short[,] Transverse = volume[Tran];
            short[,] Sagittal = volume_shi[Sag];
            short[,] Coronal = volume_guan[Cor];

            PictureCor_Original_Zoom = Out_ShorttoMat(PicCor, Coronal, 1);
            PictureSag_Original_Zoom = Out_ShorttoMat(PicSag, Sagittal, 2);
            PictureTra_Original_Zoom = Out_ShorttoMat(PicTra, Transverse, 0);


            Mat SagPro = new Mat();
            SagPro = SagittalProjectionMat.Clone();
            Mat CorPro = new Mat();
            CorPro = CoronalProjectionMat.Clone();

            OpenCvSharp.Size dsize_shi_Pro = new OpenCvSharp.Size(PicSag.Image.Width, PicSag.Image.Height);
            OpenCvSharp.Size dsize_guan_Pro = new OpenCvSharp.Size(PicCor.Image.Width, PicCor.Image.Height);

            Cv2.Resize(SagPro, PictureProSag_Original_Zoom, dsize_shi_Pro);
            Cv2.Resize(CorPro, PictureProCor_Original_Zoom, dsize_guan_Pro);

        }

        void VolumePosition_To_ShowpicturePositon()//将Mark在Volume中坐标换成要显示的图片中的坐标
        {
            Click_Depth = System.Convert.ToInt32(texdown_heng.Text.Substring(0, texdown_heng.Text.IndexOf("/")));
            Click_Width = System.Convert.ToInt32(texdown_shi.Text.Substring(0, texdown_shi.Text.IndexOf("/")));
            Click_Height = System.Convert.ToInt32(texdown_guan.Text.Substring(0, texdown_guan.Text.IndexOf("/")));


            int PicDepth = (int)(((double)Click_Depth / Original_Depth) * PicCor.Image.Height);
            int PicWidth = (int)(((double)Click_Width / Original_Width) * PicCor.Image.Width);
            int PicHeight = (int)(((double)Click_Height / Original_Height) * PicSag.Image.Width); //从Volume中的位置转化为原始显示图像中的位置

            //Cor
            int CorX = (int)(PicWidth * Cor_Scale_Zoom - LocationCor_Point_Zoom.X * (Cor_Scale_Zoom - 1));
            int CorY = (int)(PicDepth * Cor_Scale_Zoom - LocationCor_Point_Zoom.Y * (Cor_Scale_Zoom - 1));
            ShowPosition_Cor_Point_Mark = new System.Drawing.Point(CorX, CorY);

            //ProCor
            int ProCorX = (int)(PicWidth * ProCor_Scale_Zoom - LocationProCor_Point_Zoom.X * (ProCor_Scale_Zoom - 1));
            int ProCorY = (int)(PicDepth * ProCor_Scale_Zoom - LocationProCor_Point_Zoom.Y * (ProCor_Scale_Zoom - 1));
            ShowPosition_ProCor_Point_Mark = new System.Drawing.Point(ProCorX, ProCorY);
            //Sag
            int SagX = (int)(PicHeight * Sag_Scale_Zoom - LocationSag_Point_Zoom.X * (Sag_Scale_Zoom - 1));
            int SagY = (int)(PicDepth * Sag_Scale_Zoom - LocationSag_Point_Zoom.Y * (Sag_Scale_Zoom - 1));
            ShowPosition_Sag_Point_Mark = new System.Drawing.Point(SagX, SagY);
            //ProSag
            int ProSagX = (int)(PicHeight * ProSag_Scale_Zoom - LocationProSag_Point_Zoom.X * (ProSag_Scale_Zoom - 1));
            int ProSagY = (int)(PicDepth * ProSag_Scale_Zoom - LocationProSag_Point_Zoom.Y * (ProSag_Scale_Zoom - 1));
            ShowPosition_ProSag_Point_Mark = new System.Drawing.Point(ProSagX, ProSagY);
            //Tra
            int TraX = (int)(PicWidth * Tra_Scale_Zoom - LocationTra_Point_Zoom.X * (Tra_Scale_Zoom - 1));
            int TraY = (int)(PicHeight * Tra_Scale_Zoom - LocationTra_Point_Zoom.Y * (Tra_Scale_Zoom - 1));
            ShowPosition_Tra_Point_Mark = new System.Drawing.Point(TraX, TraY);
        }

        void RedCross(PictureBox sender, System.Drawing.Point p)//给图像画上红叉 并显示图像
        {
            Bitmap OrBitMap = new Bitmap(sender.Image);
            Mat OrMat = new Mat();
            OrMat = ConvertFile.BitmapToMat(OrBitMap);
            OrMat = OrMat.CvtColor(ColorConversionCodes.RGBA2RGB, 3);
            OpenCvSharp.Point startPointY = new OpenCvSharp.Point(0, p.Y);
            OpenCvSharp.Point endPointY = new OpenCvSharp.Point(sender.Image.Width, p.Y);
            Cv2.Line(OrMat, startPointY, endPointY, OpenCvSharp.Scalar.Red, 1);

            OpenCvSharp.Point startPointX = new OpenCvSharp.Point(p.X, 0);
            OpenCvSharp.Point endPointX = new OpenCvSharp.Point(p.X, sender.Image.Height);
            Cv2.Line(OrMat, startPointX, endPointX, OpenCvSharp.Scalar.Red, 1);
            Mat NewMat = new Mat();
            updateBrightnessContrast(OrMat, NewMat, Brightnessvalue, Contrastvalue);
            Bitmap NewBitMap = ConvertFile.MatToBitmap(NewMat);
            //NewBitMap 
            sender.Image = NewBitMap;
        }


        void UpdataFivePictureBox_Zoom_And_Mark()//更新五个picbox参数，采用先放缩后标点的方式 并在标记时更新对比度和亮度   ///该函数以用于全局图像的更新Date: March 4
        {
            TextToMat_Zoom();
            VolumePosition_To_ShowpicturePositon();
            //Cor
            ShowZoomPictureCorMat();
            ShowZoomPictureSagMat();
            ShowZoomPictureTraMat();
            ShowZoomPictureProCorMat();
            ShowZoomPictureProSagMat();
            Console.WriteLine(MarkExist);
            if (MarkExist)
            {
                RedCross(PicCor, ShowPosition_Cor_Point_Mark);
                RedCross(PicSag, ShowPosition_Sag_Point_Mark);
                RedCross(PicTra, ShowPosition_Tra_Point_Mark);
                RedCross(PicProCor, ShowPosition_ProCor_Point_Mark);
                RedCross(PicProSag, ShowPosition_ProSag_Point_Mark);
            }
            else
            {
                //Cor
                Bitmap PicCorBitmap = new Bitmap(PicCor.Image);
                Mat PicCorMat = new Mat();
                PicCorMat = ConvertFile.BitmapToMat(PicCorBitmap);
                Mat NewMatCor = new Mat();
                updateBrightnessContrast(PicCorMat, NewMatCor, Brightnessvalue, Contrastvalue);
                Bitmap NewBitMapCor = ConvertFile.MatToBitmap(NewMatCor);
                PicCor.Image = NewBitMapCor;
                //Sag
                Bitmap PicSagBitmap = new Bitmap(PicSag.Image);
                Mat PicSagMat = new Mat();
                PicSagMat = ConvertFile.BitmapToMat(PicSagBitmap);
                Mat NewMatSag = new Mat();
                updateBrightnessContrast(PicSagMat, NewMatSag, Brightnessvalue, Contrastvalue);
                Bitmap NewBitMapSag = ConvertFile.MatToBitmap(NewMatSag);
                PicSag.Image = NewBitMapSag;
                //Tra
                Bitmap PicTraBitmap = new Bitmap(PicTra.Image);
                Mat PicTraMat = new Mat();
                PicTraMat = ConvertFile.BitmapToMat(PicTraBitmap);
                Mat NewMatTra = new Mat();
                updateBrightnessContrast(PicTraMat, NewMatTra, Brightnessvalue, Contrastvalue);
                Bitmap NewBitMapTra = ConvertFile.MatToBitmap(NewMatTra);
                PicTra.Image = NewBitMapTra;
                //SagPro
                Bitmap PicProSagBitmap = new Bitmap(PicProSag.Image);
                Mat PicProSagMat = new Mat();
                PicProSagMat = ConvertFile.BitmapToMat(PicProSagBitmap);
                Mat NewMatProSag = new Mat();
                updateBrightnessContrast(PicProSagMat, NewMatProSag, Brightnessvalue, Contrastvalue);
                Bitmap NewBitMapProSag = ConvertFile.MatToBitmap(NewMatProSag);
                PicProSag.Image = NewBitMapProSag;
                //CorPro
                Bitmap PicProCorBitmap = new Bitmap(PicProCor.Image);
                Mat PicProCorMat = new Mat();
                PicProCorMat = ConvertFile.BitmapToMat(PicProCorBitmap);
                Mat NewMatProCor = new Mat();
                updateBrightnessContrast(PicProCorMat, NewMatProCor, Brightnessvalue, Contrastvalue);
                Bitmap NewBitMapProCor = ConvertFile.MatToBitmap(NewMatProCor);
                PicProCor.Image = NewBitMapProCor;
            }


        }

        private void Mark_Clean_Button_Click(object sender, EventArgs e)//用于删去Mark
        {
            MarkExist = false;
            UpdataFivePictureBox_Zoom_And_Mark();
        }


        /* Cor面 */
        void ShowZoomPictureCorMat()// 图像显示 用到的参数：Mat PicOriginalMat,Point Location_Zoom,int ZoomScale
        {
            Mat OutMat = new Mat();
            OpenCvSharp.Size ZoomDsize = new OpenCvSharp.Size();
            System.Drawing.Point p = LocationCor_Point_Zoom;


            int ZoomWidth = (int)Convert.ToInt32(PictureCor_Original_Zoom.Width * Cor_Scale_Zoom);
            int ZoomHeight = (int)Convert.ToInt32(PictureCor_Original_Zoom.Height * Cor_Scale_Zoom);
            Mat ZoomTempMat = new Mat();
            if (Cor_Scale_Zoom <= 1)
            {
                Cor_Scale_Zoom = 1;
                OutMat = PictureCor_Original_Zoom;
                //updateBrightnessContrast(PictureCor_Original_Zoom, OutMat, Brightnessvalue, Contrastvalue);
            }
            else
            {
                ZoomDsize = new OpenCvSharp.Size(ZoomWidth, ZoomHeight);
                Cv2.Resize(PictureCor_Original_Zoom, ZoomTempMat, ZoomDsize, 0, 0, InterpolationFlags.Cubic);
                Rect rect = new Rect((int)(Cor_Scale_Zoom * p.X) - p.X, (int)((Cor_Scale_Zoom - 1) * p.Y), PictureCor_Original_Zoom.Width, PictureCor_Original_Zoom.Height);
                OutMat = new Mat(ZoomTempMat, rect);
                //updateBrightnessContrast(OutMat, OutMat, Brightnessvalue, Contrastvalue);  //改变对比度
            }
            Bitmap bitmapFinal = ConvertFile.MatToBitmap(OutMat);
            PicCor.Image = bitmapFinal;
            //return OutMat;
        }
        void LocationChange_Cor_Zoom(System.Drawing.Point p_using)//坐标变换：从pintureBox中的坐标变换到未放缩的图像中的坐标 并更新放缩位置(用于滚轮）
        {
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicCor.Height - PictureCor_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            LocationCor_Point_Zoom.X = (int)((LocationCor_Point_Zoom.X * (Cor_Scale_Zoom - 1) + p_using.X) / Cor_Scale_Zoom);
            LocationCor_Point_Zoom.Y = (int)((LocationCor_Point_Zoom.Y * (Cor_Scale_Zoom - 1) + p_using.Y) / Cor_Scale_Zoom);
        }
        System.Drawing.Point LocationToOrPictureChange_Cor_Mark(System.Drawing.Point p_using)//坐标变换 从pintureBox中的坐标变换到未放缩的图像中的坐标 并输出坐标位置（用于鼠标左点击）
        {
            System.Drawing.Point Outp = new System.Drawing.Point();
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicCor.Height - PictureCor_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            Outp.X = (int)((LocationCor_Point_Zoom.X * (Cor_Scale_Zoom - 1) + p_using.X) / Cor_Scale_Zoom);
            Outp.Y = (int)((LocationCor_Point_Zoom.Y * (Cor_Scale_Zoom - 1) + p_using.Y) / Cor_Scale_Zoom);
            return Outp;
        }
        void ZoomCorFunction_MouseWheel(object sender, MouseEventArgs e) //更新放缩位置与放缩大小
        {
            int i = e.Delta * SystemInformation.MouseWheelScrollLines;
            Cor_Scale_Zoom = Cor_Scale_Zoom + i / 3600.0; //表示当前乘子
            System.Drawing.Point p_using = e.Location;
            LocationChange_Cor_Zoom(p_using); //更新放缩位置

            UpdataFivePictureBox_Zoom_And_Mark();//更新图像

        }
        private void Mark_CorFunction_MouseClick(object sender, MouseEventArgs e) //更新Mark点在volume中的位置（更新Click_Depth,Click_Height,Click_width的参数）
        {

            if (e.Button == MouseButtons.Left)
            {

                MarkExist = true;
                System.Drawing.Point orP = e.Location;
                System.Drawing.Point p = LocationToOrPictureChange_Cor_Mark(orP);
                Click_Depth = (int)((double)p.Y / PicCor.Image.Height * Original_Depth);
                Click_Height = System.Convert.ToInt32(texdown_guan.Text.Substring(0, texdown_guan.Text.IndexOf("/")));
                Click_Width = (int)((double)p.X / PicCor.Image.Width * Original_Width);
                UpdataTextAndScroll(); //更新TextBox的值和Scroll的值
                UpdataFivePictureBox_Zoom_And_Mark();//更新图像




            }

        }


        /* Sag面 */
        void ShowZoomPictureSagMat()// 图像显示 用到的参数：Mat PiSagiginalMat,Point Location_Zoom,int ZoomScale
        {
            Mat OutMat = new Mat();
            OpenCvSharp.Size ZoomDsize = new OpenCvSharp.Size();
            System.Drawing.Point p = LocationSag_Point_Zoom;


            int ZoomWidth = (int)Convert.ToInt32(PictureSag_Original_Zoom.Width * Sag_Scale_Zoom);
            int ZoomHeight = (int)Convert.ToInt32(PictureSag_Original_Zoom.Height * Sag_Scale_Zoom);
            Mat ZoomTempMat = new Mat();
            if (Sag_Scale_Zoom <= 1)
            {
                Sag_Scale_Zoom = 1;
                OutMat = PictureSag_Original_Zoom;
                //updateBrightnessContrast(PictureSag_Original_Zoom, OutMat, Brightnessvalue, Contrastvalue);
            }
            else
            {
                ZoomDsize = new OpenCvSharp.Size(ZoomWidth, ZoomHeight);
                Cv2.Resize(PictureSag_Original_Zoom, ZoomTempMat, ZoomDsize, 0, 0, InterpolationFlags.Cubic);
                Rect rect = new Rect((int)(Sag_Scale_Zoom * p.X) - p.X, (int)((Sag_Scale_Zoom - 1) * p.Y), PictureSag_Original_Zoom.Width, PictureSag_Original_Zoom.Height);
                OutMat = new Mat(ZoomTempMat, rect);
                //updateBrightnessContrast(OutMat, OutMat, Brightnessvalue, Contrastvalue);  //改变对比度
            }
            Bitmap bitmapFinal = ConvertFile.MatToBitmap(OutMat);
            PicSag.Image = bitmapFinal;
            //return OutMat;
        }
        void LocationChange_Sag_Zoom(System.Drawing.Point p_using)//坐标变换：从pintureBox中的坐标变换到未放缩的图像中的坐标 并更新放缩位置
        {
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicSag.Height - PictureSag_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            LocationSag_Point_Zoom.X = (int)((LocationSag_Point_Zoom.X * (Sag_Scale_Zoom - 1) + p_using.X) / Sag_Scale_Zoom);
            LocationSag_Point_Zoom.Y = (int)((LocationSag_Point_Zoom.Y * (Sag_Scale_Zoom - 1) + p_using.Y) / Sag_Scale_Zoom);
        }
        System.Drawing.Point LocationToOrPictureChange_Sag_Mark(System.Drawing.Point p_using)//坐标变换 从pintureBox中的坐标变换到未放缩的图像中的坐标 并输出坐标位置
        {
            System.Drawing.Point Outp = new System.Drawing.Point();
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicSag.Height - PictureSag_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            Outp.X = (int)((LocationSag_Point_Zoom.X * (Sag_Scale_Zoom - 1) + p_using.X) / Sag_Scale_Zoom);
            Outp.Y = (int)((LocationSag_Point_Zoom.Y * (Sag_Scale_Zoom - 1) + p_using.Y) / Sag_Scale_Zoom);
            return Outp;
        }
        void ZoomSagFunction_MouseWheel(object sender, MouseEventArgs e) //更新放缩位置与放缩大小
        {
            int i = e.Delta * SystemInformation.MouseWheelScrollLines;
            Sag_Scale_Zoom = Sag_Scale_Zoom + i / 3600.0; //表示当前乘子
            System.Drawing.Point p_using = e.Location;
            LocationChange_Sag_Zoom(p_using); //更新放缩位置
            UpdataFivePictureBox_Zoom_And_Mark();//更新图像
        }
        private void Mark_SagFunction_MouseClick(object sender, MouseEventArgs e) //更新Mark点在volume中的位置（更新Click_Depth,Click_Height,Click_width的参数）
        {
            if (e.Button == MouseButtons.Left)
            {
                MarkExist = true;
                System.Drawing.Point orP = e.Location;
                System.Drawing.Point p = LocationToOrPictureChange_Sag_Mark(orP);
                Click_Depth = (int)((double)p.Y / PicSag.Image.Height * Original_Depth);
                Click_Height = (int)((double)p.X / PicSag.Image.Width * Original_Height);
                Click_Width = System.Convert.ToInt32(texdown_shi.Text.Substring(0, texdown_shi.Text.IndexOf("/")));
                UpdataTextAndScroll(); //更新TextBox的值和Scroll的值
                UpdataFivePictureBox_Zoom_And_Mark();//更新图像
            }
        }
        /* Tra面 */
        void ShowZoomPictureTraMat()// 图像显示 用到的参数：Mat PiTraiginalMat,Point Location_Zoom,int ZoomScale
        {
            Mat OutMat = new Mat();
            OpenCvSharp.Size ZoomDsize = new OpenCvSharp.Size();
            System.Drawing.Point p = LocationTra_Point_Zoom;


            int ZoomWidth = (int)Convert.ToInt32(PictureTra_Original_Zoom.Width * Tra_Scale_Zoom);
            int ZoomHeight = (int)Convert.ToInt32(PictureTra_Original_Zoom.Height * Tra_Scale_Zoom);
            Mat ZoomTempMat = new Mat();
            if (Tra_Scale_Zoom <= 1)
            {
                Tra_Scale_Zoom = 1;
                OutMat = PictureTra_Original_Zoom;
                //updateBrightnessContrast(PictureTra_Original_Zoom, OutMat, Brightnessvalue, Contrastvalue);
            }
            else
            {
                ZoomDsize = new OpenCvSharp.Size(ZoomWidth, ZoomHeight);
                Cv2.Resize(PictureTra_Original_Zoom, ZoomTempMat, ZoomDsize, 0, 0, InterpolationFlags.Cubic);
                Rect rect = new Rect((int)(Tra_Scale_Zoom * p.X) - p.X, (int)((Tra_Scale_Zoom - 1) * p.Y), PictureTra_Original_Zoom.Width, PictureTra_Original_Zoom.Height);
                OutMat = new Mat(ZoomTempMat, rect);
                //updateBrightnessContrast(OutMat, OutMat, Brightnessvalue, Contrastvalue);  //改变对比度
            }
            Bitmap bitmapFinal = ConvertFile.MatToBitmap(OutMat);
            PicTra.Image = bitmapFinal;
            //return OutMat;
        }
        void LocationChange_Tra_Zoom(System.Drawing.Point p_using)//坐标变换：从pintureBox中的坐标变换到未放缩的图像中的坐标 并更新放缩位置
        {
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicTra.Height - PictureTra_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            LocationTra_Point_Zoom.X = (int)((LocationTra_Point_Zoom.X * (Tra_Scale_Zoom - 1) + p_using.X) / Tra_Scale_Zoom);
            LocationTra_Point_Zoom.Y = (int)((LocationTra_Point_Zoom.Y * (Tra_Scale_Zoom - 1) + p_using.Y) / Tra_Scale_Zoom);
        }
        System.Drawing.Point LocationToOrPictureChange_Tra_Mark(System.Drawing.Point p_using)//坐标变换 从pintureBox中的坐标变换到未放缩的图像中的坐标 并输出坐标位置
        {
            System.Drawing.Point Outp = new System.Drawing.Point();
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicTra.Height - PictureTra_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            Outp.X = (int)((LocationTra_Point_Zoom.X * (Tra_Scale_Zoom - 1) + p_using.X) / Tra_Scale_Zoom);
            Outp.Y = (int)((LocationTra_Point_Zoom.Y * (Tra_Scale_Zoom - 1) + p_using.Y) / Tra_Scale_Zoom);
            return Outp;
        }
        void ZoomTraFunction_MouseWheel(object sender, MouseEventArgs e) //更新放缩位置与放缩大小
        {
            int i = e.Delta * SystemInformation.MouseWheelScrollLines;
            Tra_Scale_Zoom = Tra_Scale_Zoom + i / 3600.0; //表示当前乘子
            System.Drawing.Point p_using = e.Location;
            LocationChange_Tra_Zoom(p_using); //更新放缩位置
            UpdataFivePictureBox_Zoom_And_Mark();//更新图像

        }
        private void Mark_TraFunction_MouseClick(object sender, MouseEventArgs e) //更新Mark点在volume中的位置（更新Click_Depth,Click_Height,Click_width的参数）
        {

            if (e.Button == MouseButtons.Left)
            {
                MarkExist = true;
                System.Drawing.Point orP = e.Location;
                System.Drawing.Point p = LocationToOrPictureChange_Tra_Mark(orP);

                Click_Depth = System.Convert.ToInt32(texdown_heng.Text.Substring(0, texdown_heng.Text.IndexOf("/")));

                Click_Height = (int)((double)p.Y / PicTra.Image.Height * Original_Height);

                Click_Width = (int)((double)p.X / PicTra.Image.Width * Original_Width);

                UpdataTextAndScroll(); //更新TextBox的值和Scroll的值
                UpdataFivePictureBox_Zoom_And_Mark();//更新图像

            }
        }
        /* ProCor面 */
        void ShowZoomPictureProCorMat()// 图像显示 用到的参数：Mat PiProcOriginalMat,Point Location_Zoom,int ZoomScale
        {
            Mat OutMat = new Mat();
            OpenCvSharp.Size ZoomDsize = new OpenCvSharp.Size();
            System.Drawing.Point p = LocationProCor_Point_Zoom;


            int ZoomWidth = (int)Convert.ToInt32(PictureProCor_Original_Zoom.Width * ProCor_Scale_Zoom);
            int ZoomHeight = (int)Convert.ToInt32(PictureProCor_Original_Zoom.Height * ProCor_Scale_Zoom);
            Mat ZoomTempMat = new Mat();
            if (ProCor_Scale_Zoom <= 1)
            {
                ProCor_Scale_Zoom = 1;
                OutMat = PictureProCor_Original_Zoom;
                //updateBrightnessContrast(PictureProCor_Original_Zoom, OutMat, Brightnessvalue, Contrastvalue);
            }
            else
            {
                ZoomDsize = new OpenCvSharp.Size(ZoomWidth, ZoomHeight);
                Cv2.Resize(PictureProCor_Original_Zoom, ZoomTempMat, ZoomDsize, 0, 0, InterpolationFlags.Cubic);
                Rect rect = new Rect((int)(ProCor_Scale_Zoom * p.X) - p.X, (int)((ProCor_Scale_Zoom - 1) * p.Y), PictureProCor_Original_Zoom.Width, PictureProCor_Original_Zoom.Height);
                OutMat = new Mat(ZoomTempMat, rect);
                //updateBrightnessContrast(OutMat, OutMat, Brightnessvalue, Contrastvalue);  //改变对比度
            }
            Bitmap bitmapFinal = ConvertFile.MatToBitmap(OutMat);
            PicProCor.Image = bitmapFinal;
            //return OutMat;
        }
        void LocationChange_ProCor_Zoom(System.Drawing.Point p_using)//坐标变换：从pintureBox中的坐标变换到未放缩的图像中的坐标 并更新放缩位置(用于滚轮）
        {
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicProCor.Height - PictureProCor_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            LocationProCor_Point_Zoom.X = (int)((LocationProCor_Point_Zoom.X * (ProCor_Scale_Zoom - 1) + p_using.X) / ProCor_Scale_Zoom);
            LocationProCor_Point_Zoom.Y = (int)((LocationProCor_Point_Zoom.Y * (ProCor_Scale_Zoom - 1) + p_using.Y) / ProCor_Scale_Zoom);
        }
        System.Drawing.Point LocationToOrPictureChange_ProCor_Mark(System.Drawing.Point p_using)//坐标变换 从pintureBox中的坐标变换到未放缩的图像中的坐标 并输出坐标位置（用于鼠标左点击）
        {
            System.Drawing.Point Outp = new System.Drawing.Point();
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicProCor.Height - PictureProCor_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            Outp.X = (int)((LocationProCor_Point_Zoom.X * (ProCor_Scale_Zoom - 1) + p_using.X) / ProCor_Scale_Zoom);
            Outp.Y = (int)((LocationProCor_Point_Zoom.Y * (ProCor_Scale_Zoom - 1) + p_using.Y) / ProCor_Scale_Zoom);
            return Outp;
        }
        void ZoomProCorFunction_MouseWheel(object sender, MouseEventArgs e) //更新放缩位置与放缩大小
        {
            int i = e.Delta * SystemInformation.MouseWheelScrollLines;
            ProCor_Scale_Zoom = ProCor_Scale_Zoom + i / 3600.0; //表示当前乘子
            System.Drawing.Point p_using = e.Location;
            LocationChange_ProCor_Zoom(p_using); //更新放缩位置

            UpdataFivePictureBox_Zoom_And_Mark();//更新图像

        }
        private void Mark_ProCorFunction_MouseClick(object sender, MouseEventArgs e) //更新Mark点在volume中的位置（更新Click_Depth,Click_Height,Click_width的参数）
        {

            if (e.Button == MouseButtons.Left)
            {

                MarkExist = true;
                System.Drawing.Point orP = e.Location;
                System.Drawing.Point p = LocationToOrPictureChange_ProCor_Mark(orP);
                Click_Depth = (int)((double)p.Y / PicProCor.Image.Height * Original_Depth);
                //Click_Height = System.Convert.ToInt32(texdown_guan.Text.Substring(0, texdown_guan.Text.IndexOf("/")));
                Click_Width = (int)((double)p.X / PicProCor.Image.Width * Original_Width);
                UpdataTextAndScroll(); //更新TextBox的值和Scroll的值
                UpdataFivePictureBox_Zoom_And_Mark();//更新图像

            }

        }
        /* ProSag面 */
        void ShowZoomPictureProSagMat()// 图像显示 用到的参数：Mat PiProSagiginalMat,Point Location_Zoom,int ZoomScale
        {
            Mat OutMat = new Mat();
            OpenCvSharp.Size ZoomDsize = new OpenCvSharp.Size();
            System.Drawing.Point p = LocationProSag_Point_Zoom;


            int ZoomWidth = (int)Convert.ToInt32(PictureProSag_Original_Zoom.Width * ProSag_Scale_Zoom);
            int ZoomHeight = (int)Convert.ToInt32(PictureProSag_Original_Zoom.Height * ProSag_Scale_Zoom);
            Mat ZoomTempMat = new Mat();
            if (ProSag_Scale_Zoom <= 1)
            {
                ProSag_Scale_Zoom = 1;
                OutMat = PictureProSag_Original_Zoom;
                //updateBrightnessContrast(PictureProSag_Original_Zoom, OutMat, Brightnessvalue, Contrastvalue);
            }
            else
            {
                ZoomDsize = new OpenCvSharp.Size(ZoomWidth, ZoomHeight);
                Cv2.Resize(PictureProSag_Original_Zoom, ZoomTempMat, ZoomDsize, 0, 0, InterpolationFlags.Cubic);
                Rect rect = new Rect((int)(ProSag_Scale_Zoom * p.X) - p.X, (int)((ProSag_Scale_Zoom - 1) * p.Y), PictureProSag_Original_Zoom.Width, PictureProSag_Original_Zoom.Height);
                OutMat = new Mat(ZoomTempMat, rect);
                //updateBrightnessContrast(OutMat, OutMat, Brightnessvalue, Contrastvalue);  //改变对比度
            }
            Bitmap bitmapFinal = ConvertFile.MatToBitmap(OutMat);
            PicProSag.Image = bitmapFinal;
            //return OutMat;
        }
        void LocationChange_ProSag_Zoom(System.Drawing.Point p_using)//坐标变换：从pintureBox中的坐标变换到未放缩的图像中的坐标 并更新放缩位置
        {
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicProSag.Height - PictureProSag_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            LocationProSag_Point_Zoom.X = (int)((LocationProSag_Point_Zoom.X * (ProSag_Scale_Zoom - 1) + p_using.X) / ProSag_Scale_Zoom);
            LocationProSag_Point_Zoom.Y = (int)((LocationProSag_Point_Zoom.Y * (ProSag_Scale_Zoom - 1) + p_using.Y) / ProSag_Scale_Zoom);
        }
        System.Drawing.Point LocationToOrPictureChange_ProSag_Mark(System.Drawing.Point p_using)//坐标变换 从pintureBox中的坐标变换到未放缩的图像中的坐标 并输出坐标位置
        {
            System.Drawing.Point Outp = new System.Drawing.Point();
            int compensation_Y_Value = 0;
            compensation_Y_Value = (PicProSag.Height - PictureProSag_Original_Zoom.Height) / 2; //记录补偿缺口大小
            p_using.Y = p_using.Y - compensation_Y_Value;
            Outp.X = (int)((LocationProSag_Point_Zoom.X * (ProSag_Scale_Zoom - 1) + p_using.X) / ProSag_Scale_Zoom);
            Outp.Y = (int)((LocationProSag_Point_Zoom.Y * (ProSag_Scale_Zoom - 1) + p_using.Y) / ProSag_Scale_Zoom);
            return Outp;
        }
        void ZoomProSagFunction_MouseWheel(object sender, MouseEventArgs e) //更新放缩位置与放缩大小
        {
            int i = e.Delta * SystemInformation.MouseWheelScrollLines;
            ProSag_Scale_Zoom = ProSag_Scale_Zoom + i / 3600.0; //表示当前乘子
            System.Drawing.Point p_using = e.Location;
            LocationChange_ProSag_Zoom(p_using); //更新放缩位置
            UpdataFivePictureBox_Zoom_And_Mark();//更新图像
        }
        private void Mark_ProSagFunction_MouseClick(object sender, MouseEventArgs e) //更新Mark点在volume中的位置（更新Click_Depth,Click_Height,Click_width的参数）
        {
            if (e.Button == MouseButtons.Left)
            {
                MarkExist = true;
                System.Drawing.Point orP = e.Location;
                System.Drawing.Point p = LocationToOrPictureChange_ProSag_Mark(orP);
                Click_Depth = (int)((double)p.Y / PicProSag.Image.Height * Original_Depth);
                Click_Height = (int)((double)p.X / PicProSag.Image.Width * Original_Height);
                Click_Width = System.Convert.ToInt32(texdown_shi.Text.Substring(0, texdown_shi.Text.IndexOf("/")));
                UpdataTextAndScroll(); //更新TextBox的值和Scroll的值
                UpdataFivePictureBox_Zoom_And_Mark();//更新图像
            }
        }




        #endregion




        #endregion



        /// <summary>
        /// 结束zlx的部分
        /// </summary>

        #region
        //---------------------------------------------------Start of function from Frm_RealTime_Navigation------------------ --------//

        //Navigation 2, Use scale
        public void NavigationProcess()
        {

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

                

                    //Tex_increaseX.BeginInvoke(new MethodInvoker(delegate { textshow(texdown_guan, tex_guan); }));

                    int indexX = (int)Math.Round(sensor2InVolume.X / voxelHeight) - increasX;

                    int indexY = (int)Math.Round(sensor2InVolume.Y / voxelWidth) + increasY;
                    int indexZ = (int)Math.Round(sensor2InVolume.Z / voxelDepth) + increasZ;



                    Console.WriteLine("x,y,z is {0},{1},{2}", indexX, indexY, indexZ);
                    Console.WriteLine("boxd boxh boxw is {0},{1},{2} ", boxDepth, boxHeight, boxWidth);
                    Console.WriteLine("zishi:{0}  ", Properties.Settings.Default.ScanPosition);
                    if (Properties.Settings.Default.ScanPosition == "Standing Position")
                    {
                        if (indexZ >= 1 && indexZ < boxDepth && indexX >= 1 && indexX <= boxHeight && indexY >= 1 && indexY <= boxWidth)
                        {
                            Console.WriteLine("x,y,z is {0},{1},{2}", indexX, indexY, indexZ);
                            Console.WriteLine("boxd boxh boxw is {0},{1},{2} ", boxDepth, boxHeight, boxWidth);
                            //BeginInvoke((new MethodInvoker(delegate { ThreeD_Navigation_Standing(indexX, indexY, indexZ); })));
                            ThreeD_Navigation_Standing(indexX, indexY, indexZ);

                        }
                    }
                    else if (Properties.Settings.Default.ScanPosition == "Prone Position")
                    {
                        if (indexZ >= 1 && indexZ < boxDepth && indexX >= 1 && indexX <= boxHeight && indexY >= 1 && indexY <= boxWidth)
                        {
                            ThreeD_Navigation_Standing(indexX, indexY, indexZ);
                            //BeginInvoke((new MethodInvoker(delegate { ThreeD_Navigation_Standing(indexX, indexY, indexZ); })));
                        }

                    }


                }
            }
        }

        public void PictureBoxShow3D(object sender, Bitmap bitmapG)
        {
            ((PictureBox)sender).Image = bitmapG;


        }

        public void textshow(object sender, string text)
        {

            ((TextBox)sender).Text = text;

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


        private void Frm_RealTime_Navigation_ZLX_Load(object sender, EventArgs e)
        {
            dSizeFor3DDisplay = new OpenCvSharp.Size(PicProCor.Width, PicProCor.Height);
        }

        private void Frm_RealTime_Navigation_ZLX_SizeChanged(object sender, EventArgs e)
        {
            dSizeFor3DDisplay = new OpenCvSharp.Size(PicProCor.Width, PicProCor.Height);

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
            short[,] saggitalImage = VolumeProjection.SaggitalProjection(volume);
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
            float boxWidthScaleFactor = (float)ppi / (2 * 25.4f);  // Transfer to ppi; (mm/25.4)*96; 1 inch -> 25.4mm; ppi: pixel per inch
            float boxDepthScaleFactor = (float)ppi / (25.4f);  // Transfer to ppi; (mm/25.4)*96; 1 inch -> 25.4mm; ppi: pixel per inch


            visualHeight = (int)Math.Round((float)boxDepthScaleFactor * boxDepth);
            visualWidth = (int)Math.Round((float)boxWidthScaleFactor * boxWidth);



            if (visualHeight > dSizeFor3DDisplay.Height)
            {
                visualHeight = dSizeFor3DDisplay.Height;
                float ppiTemp = (float)visualHeight * 25.4f / boxDepth;
                boxWidthScaleFactor = (float)ppiTemp / (2 * 25.4f);  // Transfer to ppi; (mm/25.4)*96; 1 inch -> 25.4mm; ppi: pixel per inch
                visualWidth = (int)Math.Round((float)boxWidthScaleFactor * boxWidth);
                Console.WriteLine(">> Change the ppi to {0} to fit the volume size", ppiTemp);

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


        }


        // 根据textbox 中的数字将5张图片合理的展示出来//
        //目前已经用UpdataFivePictureBox_Zoom_And_Mark()实现
        //private void Display_zlx_fivepicture()


        // 整体进行拉伸时图片的缩放 //
        private void PictureSizeChanged(object sender, EventArgs e)
        {
            if (volume != null)
            {
                Original_Depth = volume.Length;
                Original_Height = volume[0].GetLength(0);
                Original_Width = volume[0].GetLength(1);

                //// 保持所有图片比例不变化
                PicSag.Width = (int)(PicTra.Width * Original_Height / Original_Width);  // 使得Sag面的宽度保持和Tra面的高度一致
                int poXSag = (int)((panel3.Width - PicSag.Width) / 2);
                int poXProSag = (int)((panel12.Width - PicProSag.Width) / 2);
                int poY = 63;
                PicSag.Location = new System.Drawing.Point(poXSag, poY);
                PicProSag.Width = (int)(PicTra.Width * Original_Height / Original_Width);  // 使得Sag面的宽度保持和Tra面的高度一致
                PicProSag.Location = new System.Drawing.Point(poXProSag, poY);  // 使其居中显示

                PicCor.Width = PicTra.Width;
                PicProCor.Width = PicTra.Width;
                int poXCor = (int)((panel4.Width - PicCor.Width) / 2);
                int poXProCor = (int)((panel11.Width - PicProCor.Width) / 2);
                PicProCor.Location = new System.Drawing.Point(poXProCor, poY);
                PicCor.Location = new System.Drawing.Point(poXCor, poY);
                /// 结束
                /// 

                //Display_zlx_fivepicture();
                UpdataFivePictureBox_Zoom_And_Mark();


            }


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
            //this.real3DPictureBox.Image = bitmap;
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
            //this.real3DPictureBox.Image = bitmap;
        }
        //----------------------- ----------------------------End of funcction from Frm_RealTime_Navigation------------------ --------//
        #endregion

    }







}