using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Portable_BMU_App
{
    /// <summary>
    /// P/Invoke methods of G4 and Clarius
    /// </summary>
    public static class UnitInterFace
    {
        // ----------------------------- G4 Part -------------------------//

        const string G4DLLFilename = "G4_GPS_DllCPlusPlus.dll";    // Load G4 Dll from Dependences
        //const string G4DLLFilename = @"D:\Users\Chenhb\Programe\G4\G4_GPS_DllCPlusPlus\x64\Debug\G4_GPS_DllCPlusPlus.dll";

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct G4_SENSORDATA
        {

            /// UINT32->unsigned int
            public uint nSnsID;

            /// float[3]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R4)]
            public float[] pos;

            /// float[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R4)]
            public float[] ori;
        }

        public delegate void G4NewInfoCallback(uint val1, uint val2, ref G4_SENSORDATA g4Info);

        [DllImport(G4DLLFilename, EntryPoint = "Initialize")]
        public static extern bool Initialize();

        [DllImport(G4DLLFilename, EntryPoint = "Connect")]
        public static extern bool Connect(string path);

        [DllImport(G4DLLFilename, EntryPoint = "SetupDevice")]
        public static extern bool SetupDevice();

        [DllImport(G4DLLFilename, EntryPoint = "DisConnect")]
        public static extern bool DisConnect();

        [DllImport(G4DLLFilename, EntryPoint = "myTestStartCont")]
        public static extern void myTestStartCont();

        [DllImport(G4DLLFilename, EntryPoint = "add")]
        public static extern int add(int a, int b, string xx);

        [DllImport(G4DLLFilename, EntryPoint = "G4Listener_GetSinglePno")]
        public static extern int G4Listener_GetSinglePno(G4NewInfoCallback g4NewInfo);



        // ----------------------------- Clarius Part -------------------------//
        /// raw image information supplied with each frame
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ClariusRawImageInfo
        {
            /// int
            public int lines;

            /// int
            public int samples;

            /// int
            public int bitsPerSample;

            /// double
            public double axialSize;

            /// double
            public double lateralSize;

            /// int
            public Int64 tm;

            /// int
            public int jpeg;
        }


        /// processedd image information supplied with each frame
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ClariusProcessedImageInfo
        {

            /// int
            public int width;                    //< width of the image in pixels

            /// int
            public int height;                   //< height of the image in pixels

            /// int
            public int bitsPerPixel;             //< bits per pixel of the image

            /// double
            public double micronsPerPixel;       //< microns per pixel (always 1:1 aspect ratio axially/laterally)

            /// int
            public Int64 tm;                     //< timestamp of imagesed
        }

        /// positional data information structure
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct ClariusPosInfo
        {

            /// int64
            public Int64 tm;           ///< timestamp in nanoseconds

            public double gx;          ///< gyroscope x; angular velocity is given in radians per second (rps)
            public double gy;          ///< gyroscope y
            public double gz;          ///< gyroscope z
            public double ax;          ///< accelerometer x; acceleration is normalized to gravity [~9.81m/s^2] (m/s^2)/(m/s^2)
            public double ay;          ///< accelerometer y
            public double az;          ///< accelerometer z
            public double mx;          ///< magnetometer x; magnetic flux density is normalized to the earth's field [~50 mT] (T/T)
            public double my;          ///< magnetometer y
            public double mz;          ///< magnetometer z
            public double qw;          ///< w component (real) of the orientation quaternion
            public double qx;          ///< x component (imaginary) of the orientation quaternion
            public double qy;          ///< y component (imaginary) of the orientation quaternion
            public double qz;          ///< z component (imaginary) of the orientation quaternion
        }


        public delegate void ClariusButtonCallback(int btn, int clicks);


        public delegate void ClariusFreezeCallback(int val);
        public delegate void ClariusErrorCallback(string err);
        public delegate void ClariusProgressCallback(int progress);
        public delegate void ClariusNewProcessedImageFnCallBack(IntPtr buff, ref ClariusProcessedImageInfo nfo, int npos, ref ClariusPosInfo pos);
        public delegate void ClariusNewRawImageCallback(IntPtr buff, ref ClariusRawImageInfo nfo, int npos, ref ClariusPosInfo pos);

        public delegate void ClariusReturnCallback(int retCode);

        [DllImport("listen.dll", EntryPoint = "clariusInitListener")]
        public static extern int clariusInitListener(int argc, string[] argv, string dir, ClariusNewProcessedImageFnCallBack newImage, ClariusNewRawImageCallback newRawImage,
                                  ClariusFreezeCallback freeze, ClariusButtonCallback button,ClariusProgressCallback progress, ClariusErrorCallback err, ClariusReturnCallback fn, int width, int height);

        [DllImport("listen.dll", EntryPoint = "clariusConnect")]
        public static extern int clariusConnect(string ipAddress, uint port, ClariusReturnCallback fn);

        [DllImport("listen.dll", EntryPoint = "clariusDisconnect")]
        public static extern int clariusDisconnect(ClariusReturnCallback fn);

        [DllImport("listen.dll", EntryPoint = "clariusDestroyListener")]
        public static extern int clariusDestroyListener();


        [DllImport("listen.dll", EntryPoint = "clariusUserFunction")]
        public static extern int clariusUserFunction(int userFn, ClariusReturnCallback fn);

        /// user function commands
        /// USER_FN_NONE -> 0
        public const int USER_FN_NONE = 0;

        /// USER_FN_TOGGLE_FREEZE -> 1
        public const int USER_FN_TOGGLE_FREEZE = 1;

        /// USER_FN_CAPTURE_IMAGE -> 2
        public const int USER_FN_CAPTURE_IMAGE = 2;

        /// USER_FN_CAPTURE_CINE -> 3
        public const int USER_FN_CAPTURE_CINE = 3;

        /// USER_FN_DEPTH_DEC -> 4
        public const int USER_FN_DEPTH_DEC = 4;

        /// USER_FN_DEPTH_INC -> 5
        public const int USER_FN_DEPTH_INC = 5;

        /// USER_FN_GAIN_DEC -> 6
        public const int USER_FN_GAIN_DEC = 6;

        /// USER_FN_GAIN_INC -> 7
        public const int USER_FN_GAIN_INC = 7;

        /// USER_FN_TOGGLE_AUTOGAIN -> 8
        public const int USER_FN_TOGGLE_AUTOGAIN = 8;

        /// USER_FN_TOGGLE_ZOOM -> 9
        public const int USER_FN_TOGGLE_ZOOM = 9;

        /// USER_FN_TOGGLE_FLIP -> 10
        public const int USER_FN_TOGGLE_FLIP = 10;

        /// USER_FN_TOGGLE_CINE_PLAY -> 11
        public const int USER_FN_TOGGLE_CINE_PLAY = 11;

        /// USER_FN_MODE_B -> 12
        public const int USER_FN_MODE_B = 12;

        /// USER_FN_MODE_M -> 13
        public const int USER_FN_MODE_M = 13;

        /// USER_FN_MODE_CFI -> 14
        public const int USER_FN_MODE_CFI = 14;

        /// USER_FN_MODE_PDI -> 15
        public const int USER_FN_MODE_PDI = 15;

        /// USER_FN_MODE_PW -> 16
        public const int USER_FN_MODE_PW = 16;

        /// USER_FN_MODE_ELASTOGRAPHY -> 17
        public const int USER_FN_MODE_ELASTOGRAPHY = 17;
    }
}
