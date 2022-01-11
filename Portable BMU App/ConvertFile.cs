using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ultrasonics3DReconstructor
{
    public class ConvertFile
    {

        /// <summary>
        /// Short Type Convert to Byte
        /// </summary>
        /// <param name="inShort"></param>
        /// <returns></returns>
        public static byte[,] ShortArrayToByte(short[,] inShort)
        {
            int rows = inShort.GetLength(0);
            int cols = inShort.GetLength(1);
            byte[,] outByte = new byte[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    outByte[i, j] = (byte)inShort[i, j];
                }
            }
            return outByte;
        }


        /// <summary>
        /// OpenCvSharp Convert byteArray to Mat 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe static Mat ArrayToMat(byte[,] array)
        {
            Mat mat = new Mat(array.GetLength(0), array.GetLength(1), MatType.CV_8UC1);
            int cols = mat.Cols;
            int rows = mat.Rows;
            byte* ptr = (byte*)mat.Data.ToPointer();
            long step = mat.Step();
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    *MatElm_Byte(ptr, step, i, j) = array[j, i];
                }
            }
            return mat;
        }

        /// <summary>
        /// OpenCvSharp Convert shortArray to Mat
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public unsafe static Mat ArrayToMat(short[,] array)
        {
            Mat mat = new Mat(array.GetLength(0), array.GetLength(1), MatType.CV_8UC3);
            int cols = mat.Cols;
            int rows = mat.Rows;
            byte* ptr = (byte*)mat.Data.ToPointer();
            long step = mat.Step();
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    *MatElm_Int16(ptr, step, i, j) = array[j, i];
                }
            }
            return mat;
        }

        public unsafe static byte* MatElm_Byte(byte* ptr, long step, int x, int y)
        {
            return ptr + y * step + x;
        }

        public unsafe static short* MatElm_Int16(byte* ptr, long step, int x, int y)
        {
            return (short*)(ptr + y * step + x * 2);
        }

        /// <summary>
        /// OpenCvSharp Convert Mat to Bitmap
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap MatToBitmap(Mat img)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img);
        }

        /// <summary>
        /// OpenCvSharp Convert Bitmap to Mat
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Mat BitmapToMat(Bitmap bitmap)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
        }

        /// <summary>
        /// Convert image to byteArray
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }
        }

        /// <summary>
        /// Convert byteArray to image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream mStream = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(mStream);
            }
        }

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

        //// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
    }
}
