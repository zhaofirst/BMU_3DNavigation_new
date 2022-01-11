using System;
using System.Numerics;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace Ultrasonics3DReconstructor
{
    public class USFileReader
    {
        //b8  
        public int frameCounts;
        public int frameWidth;
        public int frameHeight;

        public float LATERAL_X;
        public float LATERAL_Y;
        public float LATERALXY;

        public byte[][,] frames;
        public byte[][,] changeFrames;

        //gps
        public int gpsEntriesCount;

        public US_GPS_ENTRY[] m_gpsEntries;

        public Vector3 m_gpsCalibrationTop = new Vector3(0, 0, 0);

        public Vector3 m_gpsCalibrationBottom = new Vector3(0, 0, 0);

        public float distanceFromOriginToEnd;

        //rect
        public Rect3[] myRectangles;
        G4GetRects_Post getRect = new G4GetRects_Post();


        // ------------------ Sonix related -----------------//
        public void Readb8(string path)
        {
            //string path = @"D:\Graduate Program\Ultrasonic\Original_Image\us29\US29.b8";

            //读取数据到 binaryReader 
            BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open));


            Console.WriteLine("[+]Reading {0}", path);
            US_FILE_HEADER uS_FILE_HEADER = default(US_FILE_HEADER);
            Console.WriteLine("\tHeader size = {0}", Marshal.SizeOf(uS_FILE_HEADER));
            byte[] arr = binaryReader.ReadBytes(Marshal.SizeOf(uS_FILE_HEADER));

            //将byte数组 arr 转换成结构体US_FILE_HEADER
            uS_FILE_HEADER = (US_FILE_HEADER)ConvertFile.BytesToStruct(arr, typeof(US_FILE_HEADER));
            Console.WriteLine("\tframeCounts is = {0}", uS_FILE_HEADER.frames);
            Console.WriteLine("\tframeSize = {0}x{1} (w*h)", uS_FILE_HEADER.w, uS_FILE_HEADER.h);

            this.frameHeight = uS_FILE_HEADER.h;
            this.frameWidth = uS_FILE_HEADER.w;
            this.frameCounts = uS_FILE_HEADER.frames;
            this.frames = new byte[uS_FILE_HEADER.frames][,];

            //继续读取，每次读取640*480 byte
            int arraySize = this.frameWidth * this.frameHeight;
            for (int i = 0; i < this.frameCounts; i++)
            {
                byte[] array = binaryReader.ReadBytes(arraySize);
                byte[,] array2 = new byte[this.frameHeight, this.frameWidth];
                //将一维数组转换为二维数组
                for (int j = 0; j < arraySize; j++)
                {
                    array2[j / this.frameWidth, j % this.frameWidth] = array[j];
                }

                this.frames[i] = array2;
            }

            //Array.Copy(change_frames, m_frames, m_frames.Length);
            changeFrames = (byte[][,])this.frames.Clone();
            //Mat colormat = Mat.FromImageData(baData, ImreadModes.Color);

            //if (colormat == null)
            //{
            //    Console.WriteLine("null");
            //}
            //Cv2.ImShow("original image", colormat);
            //Mat src = Cv2.ImDecode(this.m_frames[550],ImreadModes.Grayscale); 


            binaryReader.Close();
            Console.WriteLine("-------- Read b8 done --------");
            Console.WriteLine("\n");
        }

        public void ReadGPS(string path)
        {
            //string path = @"D:\Graduate Program\Ultrasonic\Original_Image\us29\US29.gps";
            //读取数据到 binaryReader 
            BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
            Console.WriteLine("[+]Reading {0}", path);
            US_FILE_HEADER uS_FILE_HEADER = default(US_FILE_HEADER);
            Console.WriteLine("\tHeader size = {0}", Marshal.SizeOf(uS_FILE_HEADER));
            byte[] arr = binaryReader.ReadBytes(Marshal.SizeOf(uS_FILE_HEADER));

            //将byte数组 arr 转换成结构体US_FILE_HEADER
            uS_FILE_HEADER = (US_FILE_HEADER)ConvertFile.BytesToStruct(arr, typeof(US_FILE_HEADER));
            Console.WriteLine("\tframeCounts = {0}", uS_FILE_HEADER.frames);

            this.gpsEntriesCount = uS_FILE_HEADER.frames;
            this.m_gpsEntries = new US_GPS_ENTRY[this.gpsEntriesCount];

            //读取GPS数据
            for (int i = 0; i < this.gpsEntriesCount; i++)
            {
                byte[] arr2 = binaryReader.ReadBytes(Marshal.SizeOf(typeof(US_GPS_ENTRY)));

                //For example: there will be 1438 gpsEntries in US29
                this.m_gpsEntries[i] = (US_GPS_ENTRY)ConvertFile.BytesToStruct(arr2, typeof(US_GPS_ENTRY));
            }
            binaryReader.Close();


            //坐标变换
            this.myRectangles = new Rect3[this.gpsEntriesCount];  // For example, there will be 1438 rectangles in US29
            for (int j = 0; j < this.gpsEntriesCount; j++)
            {
                this.myRectangles[j] = new Rect3();                    //每一个数组元素都装着一个rectangle的四个点的坐标信息
                this.myRectangles[j].v = this.m_gpsEntries[j].GetRectangle(LATERAL_X, LATERAL_Y);
                //注：
                //此处之所以还多一个.v是因为涉及到类型转换问题。GetRectangle返回的是 Vector3类型
                //而m_Rectangle[]是Rect3类型。因此使用class Rect3中定义的Vector3数组进行承接
            }


            Vector3 vectorOrigin = new Vector3((float)this.m_gpsEntries[0].x, (float)this.m_gpsEntries[0].y, (float)this.m_gpsEntries[0].z);
            Vector3 other = new Vector3((float)this.m_gpsEntries[this.gpsEntriesCount - 1].x, (float)this.m_gpsEntries[this.gpsEntriesCount - 1].y, (float)this.m_gpsEntries[this.gpsEntriesCount - 1].z);


            distanceFromOriginToEnd = Vector3.Distance(vectorOrigin, other);
            Console.WriteLine("\tDistance from origin to end = {0}mm", distanceFromOriginToEnd);
            Console.WriteLine("-------- Read gps done --------");
            Console.WriteLine("\n");

        }

        public void ReadMetaData(string xmlPath)
        {
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0011: Expected O, but got Unknown
            //IL_0020: Unknown result type (might be due to invalid IL or missing references)
            //IL_0025: Expected O, but got Unknown
            //IL_0031: Unknown result type (might be due to invalid IL or missing references)
            //IL_0036: Expected O, but got Unknown
            //IL_00d1: Unknown result type (might be due to invalid IL or missing references)
            //IL_00e6: Unknown result type (might be due to invalid IL or missing references)
            //IL_00eb: Expected O, but got Unknown
            try
            {
                using (FileStream fileStream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
                {
                    XmlDocument val = new XmlDocument();
                    val.Load((Stream)fileStream);
                    XmlElement val2 = val.SelectSingleNode("/*/parameters/param[@name='@@gpsCalibration@@top']") as XmlElement;
                    XmlElement val3 = val.SelectSingleNode("/*/parameters/param[@name='@@gpsCalibration@@bottom']") as XmlElement;
                    if (val2 != null)
                    {
                        m_gpsCalibrationTop = new Vector3(float.Parse(val2.GetAttribute("x")), float.Parse(val2.GetAttribute("y")), float.Parse(val2.GetAttribute("z")));
                    }
                    if (val3 != null)
                    {
                        m_gpsCalibrationBottom = new Vector3(float.Parse(val3.GetAttribute("x")), float.Parse(val3.GetAttribute("y")), float.Parse(val3.GetAttribute("z")));
                    }
                    foreach (XmlElement item in val.SelectNodes("/*/parameters/param[starts-with(@name,'@@gpsCalibration@@')]"))
                    {
                        XmlElement val4 = item;
                        string attribute = val4.GetAttribute("name");
                        //m_gpsCalibrationCustoms[attribute.Substring("@@gpsCalibration@@".Length)] = new Vector3(float.Parse(val4.GetAttribute("x")), float.Parse(val4.GetAttribute("y")), float.Parse(val4.GetAttribute("z")));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[-]Exception: {0}", ex.ToString());
            }
        }


        // ------------------ G4 related -----------------//
        public void ReadG4(string path)
        {
            BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
            Console.WriteLine(">> Reading G4 gps data...");

            this.gpsEntriesCount = binaryReader.ReadInt32();
            Console.WriteLine("gps Cout is :{0}", this.gpsEntriesCount);

            this.myRectangles = new Rect3[this.gpsEntriesCount];

            // Read G4 data and transform to rect
            for (int i = 0; i < this.gpsEntriesCount; i++)
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

                getRect = new G4GetRects_Post(g4XYZ, g4Quaternion, LATERALXY);
                myRectangles[i] = new Rect3();
                myRectangles[i].v = getRect.GetRectangle();
            }

            binaryReader.Close();

            Console.WriteLine("-------- Read G4 done --------");
            Console.WriteLine("\n");

        }


        // ------------------ Clarius related -----------------//
        public void ReadClarius(string path)
        {
            //string path = @"C:\Users\BMU\Desktop\Test\us57.gps";
            BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
            Console.WriteLine(">> Reading Clarius 2D frames...");

            this.frameCounts = binaryReader.ReadInt32();
            this.frameWidth = binaryReader.ReadInt32();
            this.frameHeight = binaryReader.ReadInt32();
            double laterXY = binaryReader.ReadDouble();
            //this.LATERALXY = binaryReader.ReadDouble();
            this.LATERALXY = (float)laterXY;
            Console.WriteLine("Frame Counts is :{0}, w*h: {1}*{2}", frameCounts, frameWidth, frameHeight);
            this.frames = new byte[frameCounts][,];

            int readBytesSize = this.frameWidth * this.frameHeight;
            for (int i = 0; i < frameCounts; i++)
            {
                byte[] arr = binaryReader.ReadBytes(readBytesSize);
                byte[,] array2 = new byte[this.frameHeight, this.frameWidth];
                //transform 1D array to 2D array
                for (int j = 0; j < readBytesSize; j++)
                {
                    array2[j / this.frameWidth, j % this.frameWidth] = arr[j];
                }
                // Mirror the frame data
                for (int iHeight = 0; iHeight < frameHeight; iHeight++)
                {
                    for (int jWidth = 0; jWidth < frameWidth / 2; jWidth++)
                    {
                        byte temp = array2[iHeight, jWidth];
                        array2[iHeight, jWidth] = array2[iHeight, frameWidth - jWidth - 1];
                        array2[iHeight, frameWidth - jWidth - 1] = temp;
                    }
                }
                this.frames[i] = array2;
            }
            binaryReader.Close();

            Console.WriteLine("-------- Read Clarius done --------");
            Console.WriteLine("\n");
        }


        // ------------------ Image processing related -----------------//

        public void RemoveMuscle()
        {
            Console.WriteLine("[+]Removing muscle layer...Please wait...");

            int imageHeight = this.frames[0].GetLength(0); //hang
            int imageWidth = this.frames[0].GetLength(1);  //lie
            for (int i = 0; i < this.frameCounts; i++)
            {
                for (int j = 0; j < 150; j++)
                {
                    for (int k = 0; k < imageWidth; k++)
                    {
                        this.changeFrames[i][j, k] = 0;
                    }
                }
            }
            Console.WriteLine("-------- Done --------");
            Console.WriteLine("\n");
        }

        public void SamplingFrames()
        {
            Console.WriteLine("[+]Down sampling size to 320*240...Please wait...");

            //使用Bilinear插值方法，把图像降低
            for (int i = 0; i < this.frameCounts; i++)
            {
                //this.change_frames[i] = Bilinear(this.m_frames[i], 0.5f);
                this.changeFrames[i] = Delete(this.frames[i]);
            }
            //byte[,] newImage = Bilinear(this.m_frames[550], 0.5f);
            Console.WriteLine("-------- Done --------");
            Console.WriteLine("\n");
        }

        public static byte[,] Delete(byte[,] inImage)
        {
            int imageHeight = inImage.GetLength(0); //hang
            int imageWidth = inImage.GetLength(1);  //lie
            int outHeight = Convert.ToInt32(Math.Round(imageHeight * 0.5));
            int outWidth = Convert.ToInt32(Math.Round(imageWidth * 0.5));
            //根据放大因子为新图像初始化空间
            byte[,] outImage1 = new byte[outHeight, imageWidth];

            for (int i = 0; i < outHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    outImage1[i, j] = inImage[i * 2, j];
                }
            }

            byte[,] outImage2 = new byte[outHeight, outWidth];


            for (int i = 0; i < outHeight; i++)
            {
                for (int j = 0; j < outWidth; j++)
                {
                    outImage2[i, j] = outImage1[i, j * 2];
                }
            }

            return outImage2;
        }

        /// <summary>
        /// Bilnear image interpolation
        /// </summary>
        /// <param name="inImage"></param>
        /// <param name="zmf"></param>
        /// <returns></returns>
        public byte[,] Bilinear(byte[,] inImage, float zmf)
        {
            int imageHeight = inImage.GetLength(0); //hang
            int imageWidth = inImage.GetLength(1);  //lie
            int outHeight = Convert.ToInt32(Math.Round(imageHeight * zmf));
            int outWidth = Convert.ToInt32(Math.Round(imageWidth * zmf));

            //根据放大因子为新图像初始化空间
            byte[,] outImage = new byte[outHeight, outWidth];

            //扩展图像边缘
            byte[,] IT = new byte[imageHeight + 1, imageWidth + 1];
            for (int i = 0; i < imageHeight; i++)
            {
                for (int j = 0; j < imageWidth; j++)
                {
                    IT[i, j] = inImage[i, j];
                }
            }

            for (int k = 0; k < imageWidth; k++)
            {
                IT[imageHeight, k] = inImage[imageHeight - 1, k];
            }

            for (int l = 0; l < imageHeight; l++)
            {
                IT[l, imageWidth] = inImage[l, imageWidth - 1];
            }

            IT[imageHeight, imageWidth] = inImage[imageHeight - 1, imageWidth - 1];

            //执行bilinear公式
            for (int zj = 1; zj <= outWidth; zj++)
            {
                for (int zi = 1; zi <= outHeight; zi++)
                {
                    float ii = (zi - 1) / zmf;
                    float jj = (zj - 1) / zmf;
                    int i = Convert.ToInt32(Math.Floor(ii));
                    int j = Convert.ToInt32(Math.Floor(jj));

                    float u = ii - i;
                    float v = jj - j;
                    i = i + 1;
                    j = j + 1;
                    outImage[zi - 1, zj - 1] = (byte)((1 - u) * (1 - v) * IT[i - 1, j - 1] + (1 - u) * v * IT[i - 1, j + 1 - 1]
                        + u * (1 - v) * IT[i + 1 - 1, j - 1] + u * v * IT[i + 1 - 1, j + 1 - 1]);
                }
            }


            return outImage;
        }

    }
}
