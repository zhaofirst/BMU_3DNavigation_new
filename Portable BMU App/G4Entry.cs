using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Portable_BMU_App
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct G4Entry
    {
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R4)]
        public  float[] parasXYZ;

        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R4)]
        public float[] parasQuaternion;
        //public unsafe fixed float ssCalibrationMatrxi[9];
        //public float LATERAL_X = 0.194f;
        //public float LATERAL_Y = 0.194f;
        //[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R4)]
        //public float[] ssCalibrationMatrxi;
        /// <summary> 
        /// 对获取的矩形四个点坐标进行几何变换
        /// </summary>
        /// <param name="LATERAL_X"></param>
        /// <param name="LATERAL_Y"></param>
        /// <returns></returns>
        public Vector3[] GetRectangle(float LATERAL)
        {
            for (int i = 0; i < parasXYZ.Length; i++)
            {
                parasXYZ[i] = parasXYZ[i] * 10;         // Transform cm to mm             
            }

            Vector3[] rectangle = new Vector3[4];
            int widthInPixels = 640;
            int heightInPixels = 480;

            float resolutionX = (float)widthInPixels * LATERAL * 0.001f;
            float resolutionY = (float)heightInPixels * LATERAL * 0.001f;

            //Console.WriteLine("x:{0},y:{1},z:{2}", parasXYZ[0], parasXYZ[1], parasXYZ[2]);
            //Console.WriteLine("a:{0},b:{1},c:{2},d:{3}", parasQuaternion[0], parasQuaternion[1], parasQuaternion[2], parasQuaternion[3]);

            // Think a frame is a rectangle. There are four vertexes in a rectangle
            Vector3[] initialRect = new Vector3[4]
            {
                new Vector3((0f - resolutionX) / 2.0f, 0f, 0f),
                new Vector3(resolutionX / 2.0f, 0f, 0f),
                new Vector3(resolutionX / 2.0f, resolutionY, 0f),
                new Vector3((0f - resolutionX) / 2.0f, resolutionY, 0f)
            };


            for (int i = 0; i < 4; i++)
            {
                Vector3 vector = initialRect[i];
                vector = this.TransformWithCalibration(vector);
                vector = (this.TransformWithRotation(vector));
                rectangle[i] = vector;                                  //save the transformation result

            }

            return rectangle;

        }

        private Vector3 TransformWithCalibration(Vector3 vector)
        {
            //Constanr_Matrix of transformation 
            float[,] matrixC = new float[3, 3] { { 0, 1, 0 }, { -1, 0, 0 }, { 0, 0, 1 } };
            Vector3 v = new Vector3();
            float[] initial_vector = new float[3] { vector.X, vector.Y, 0f };
            v.X = matrixC[0, 0] * initial_vector[0] + matrixC[0, 1] * initial_vector[1] + matrixC[0, 2] * initial_vector[2];
            v.Y = matrixC[1, 0] * initial_vector[0] + matrixC[1, 1] * initial_vector[1] + matrixC[1, 2] * initial_vector[2];
            v.Z = matrixC[2, 0] * initial_vector[0] + matrixC[2, 1] * initial_vector[1] + matrixC[2, 2] * initial_vector[2];
            return v;
        }


        private Vector3 TransformWithRotation(Vector3 vector)    //fixed 只能在 unsafe环境下使用
        {
            Matrix4x4 matrixR = new Matrix4x4();
            Vector3 v = new Vector3();
            float[] ssCalibrationMatrxi = new float[9];
            ssCalibrationMatrxi =  GetSSMatrix();
            //Get parameter of transformation in s[]        
            matrixR.M11 = ssCalibrationMatrxi[0]; matrixR.M12 = ssCalibrationMatrxi[1]; matrixR.M13 = ssCalibrationMatrxi[2]; matrixR.M14 = 0f;
            matrixR.M21 = ssCalibrationMatrxi[3]; matrixR.M22 = ssCalibrationMatrxi[4]; matrixR.M23 = ssCalibrationMatrxi[5]; matrixR.M24 = 0f;
            matrixR.M31 = ssCalibrationMatrxi[6]; matrixR.M32 = ssCalibrationMatrxi[7]; matrixR.M33 = ssCalibrationMatrxi[8]; matrixR.M34 = 0f;

            matrixR.M41 = parasXYZ[0]; matrixR.M42 = parasXYZ[1]; matrixR.M43 = parasXYZ[2]; matrixR.M44 = 1f;


            //Implement TransformVector
            v.X = matrixR.M11 * vector.X + matrixR.M21 * vector.Y + matrixR.M31 * vector.Z + matrixR.M41;
            v.Y = matrixR.M12 * vector.X + matrixR.M22 * vector.Y + matrixR.M32 * vector.Z + matrixR.M42;
            v.Z = matrixR.M13 * vector.X + matrixR.M23 * vector.Y + matrixR.M33 * vector.Z + matrixR.M43;
            return v;
        }

        private float[] GetSSMatrix()
        {
            float a = (float)(parasQuaternion[0]);
            float b = (float)(parasQuaternion[1]);
            float c = (float)(parasQuaternion[2]);
            float d = (float)(parasQuaternion[3]);

            float[] ssCalibrationMatrxi = new float[9];
            ssCalibrationMatrxi[0] = (float)(1 - 2 * Math.Pow(c, 2) - 2 * Math.Pow(d, 2));
            ssCalibrationMatrxi[1] = (float)(2 * b * c + 2 * a * d);//zaici ganxie litongxue de bianxie (~_~!)
            ssCalibrationMatrxi[2] = (float)(2 * b * d - 2 * a * c);
            ssCalibrationMatrxi[3] = (float)(2 * b * c - 2 * a * d);
            ssCalibrationMatrxi[4] = (float)(1 - 2 * Math.Pow(b, 2) - 2 * Math.Pow(d, 2));
            ssCalibrationMatrxi[5] = (float)(2 * a * b + 2 * c * d);
            ssCalibrationMatrxi[6] = (float)(2 * a * c + 2 * b * d);
            ssCalibrationMatrxi[7] = (float)(2 * c * d - 2 * a * b);
            ssCalibrationMatrxi[8] = (float)(1 - 2 * Math.Pow(b, 2) - 2 * Math.Pow(c, 2));

            return ssCalibrationMatrxi;
        }

    }
}
