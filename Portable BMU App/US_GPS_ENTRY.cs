using System.Numerics;
using System.Runtime.InteropServices;

namespace Ultrasonics3DReconstructor
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct US_GPS_ENTRY
    {
        public double x;

        public double y;

        public double z;

        public unsafe fixed double unkonwn_1[3];    //由于结构体中不能实例化具体大小的数组
                                                    //因此需要使用该方式 进行初始化指针缓冲区大小
        public unsafe fixed double s[9];

        public double time;

        public ushort quality;

        public unsafe fixed byte padding[6];

        public unsafe fixed double unkonwn_2[4];

        public Vector3 TransformWithCalibration(Vector3 vector)
        {
            //Constanr_Matrix of transformation 
            Matrix4x4 matrixC = new Matrix4x4(14.8449f, 0.9477f, -0.0018f, 0, 15.0061f, 0.0016f, 1.000f, 0, 0.1638f, 0.0166f, 0.0052f, 0, 0, 0, 0, 1);
            Vector3 v = new Vector3();
            float[] initial_vector = new float[4] { 1f, vector.Y, vector.X, 0f };
            v.X = matrixC.M11 * initial_vector[0] + matrixC.M12 * initial_vector[1] + matrixC.M13 * initial_vector[2];
            v.Y = matrixC.M21 * initial_vector[0] + matrixC.M22 * initial_vector[1] + matrixC.M23 * initial_vector[2];
            v.Z = matrixC.M31 * initial_vector[0] + matrixC.M32 * initial_vector[1] + matrixC.M33 * initial_vector[2];

            //Rectangle
            return v;
        }

        public unsafe Vector3 TransformWithRotation(Vector3 vector)    //fixed 只能在 unsafe环境下使用
        {
            Matrix4x4 matrixR = new Matrix4x4();
            Vector3 v = new Vector3();
            //Get parameter of transformation in s[]
            fixed (US_GPS_ENTRY* ptr = &this)
            {
                US_GPS_ENTRY* ptr2 = ptr;
                matrixR.M11 = (float)ptr2->s[0]; matrixR.M12 = (float)ptr2->s[1]; matrixR.M13 = (float)ptr2->s[2]; matrixR.M14 = 0f;
                matrixR.M21 = (float)ptr2->s[3]; matrixR.M22 = (float)ptr2->s[4]; matrixR.M23 = (float)ptr2->s[5]; matrixR.M24 = 0f;
                matrixR.M31 = (float)ptr2->s[6]; matrixR.M32 = (float)ptr2->s[7]; matrixR.M33 = (float)ptr2->s[8]; matrixR.M34 = 0f;
            }
            matrixR.M41 = (float)this.x; matrixR.M42 = (float)this.y; matrixR.M43 = (float)this.z; matrixR.M44 = 1f;

            //Console.WriteLine(vector.X); Console.WriteLine(vector.Y); Console.WriteLine(vector.Z);
            //Implement TransformVector
            v.X = matrixR.M11 * vector.X + matrixR.M21 * vector.Y + matrixR.M31 * vector.Z + matrixR.M41;
            v.Y = matrixR.M12 * vector.X + matrixR.M22 * vector.Y + matrixR.M32 * vector.Z + matrixR.M42;
            v.Z = matrixR.M13 * vector.X + matrixR.M23 * vector.Y + matrixR.M33 * vector.Z + matrixR.M43;
            return v;
        }

        /// <summary> 对获取的矩形四个点坐标进行几何变换
        /// 
        /// </summary>
        /// <param name="LATERAL_X"></param>
        /// <param name="LATERAL_Y"></param>
        /// <returns></returns>
        public Vector3[] GetRectangle(float LATERAL_X, float LATERAL_Y)
        {
            Vector3[] rectangle = new Vector3[4];
            int widthInPixels = 640;
            int heightInPixels = 480;

            float resolutionX = (float)widthInPixels * LATERAL_X;
            float resolutionY = (float)heightInPixels * LATERAL_Y;

            // Think a frame is a rectangle. There are four point in a rectangle
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
                rectangle[i] = vector;

                //save the transformation result
            }


            return rectangle;

        }

    }
}
