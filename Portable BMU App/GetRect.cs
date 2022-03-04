using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Portable_BMU_App
{
    class G4GetRects_Pre
    {
        public float[] parasXYZ = new float[3];
        public float[] parasQuaternion = new float[4];
        public float[] ssCalibrationMatrxi = new float[9];
        public double LATERAL_X = 0.194f;
        public double LATERAL_Y = 0.194f;
        public G4GetRects_Pre(float[] inputXYZ, float[] inputAER, double lateral)
        {
            LATERAL_X = lateral * 0.001f;
            LATERAL_Y = lateral * 0.001f;
            Array.Copy(inputXYZ, parasXYZ, inputXYZ.Length);
            Array.Copy(inputAER, parasQuaternion, inputAER.Length);
            for (int i = 0; i < parasXYZ.Length; i++)
            {
                parasXYZ[i] = parasXYZ[i] * 10;         // Transform cm to mm             
            }
        }


        public G4GetRects_Pre()
        { }
        /// <summary> 
        /// 对获取的矩形四个点坐标进行几何变换
        /// </summary>
        /// <param name="LATERAL_X"></param>
        /// <param name="LATERAL_Y"></param>
        /// <returns></returns>
        public Vector3[] GetRectangle()
        {
            Vector3[] rectangle = new Vector3[4];
            int widthInPixels = 640;
            int heightInPixels = 480;

            float resolutionWidth = widthInPixels * (float)LATERAL_X;
            float resolutionHeight = heightInPixels * (float)LATERAL_Y;

            // Think a frame is a rectangle. There are four vertexes in a rectangle
            Vector3[] initialRect = new Vector3[4]
            {
                //new Vector3((0f - resolutionWidth) / 2.0f, 0f, 0f),
                //new Vector3(resolutionWidth / 2.0f, 0f, 0f),
                //new Vector3(resolutionWidth / 2.0f, resolutionHeight, 0f),
                //new Vector3((0f - resolutionWidth) / 2.0f, resolutionHeight, 0f)
                new Vector3(0f,(0f - resolutionWidth) / 2.0f, 0f),
                new Vector3(0f,resolutionWidth / 2.0f, 0f),
                new Vector3(resolutionHeight,resolutionWidth / 2.0f, 0f),
                new Vector3(resolutionHeight,(0f - resolutionWidth) / 2.0f, 0f)
            };


            for (int i = 0; i < 4; i++)
            {
                Vector3 vector = initialRect[i];

                if (MainForm.ChooseScannerType == UnitInterFaceV733.ChooseScannerType.Clarius)
                {
                    vector = this.TransformWithCalibration(vector);
                    //Console.WriteLine("Clarius");
                }
                else
                {
                    vector = this.TransformWithCalibrationForC3New(vector);
                    //Console.WriteLine("Clarius New");
                }
                //vector = this.TransformWithCalibration(vector);
                vector = (this.TransformWithRotation(vector));

                rectangle[i] = vector;                                  //save the transformation result

            }

            return rectangle;
        }

        private Vector3 TransformWithCalibration(Vector3 vector)
        {
            //Constanr_Matrix of transformation 
            //float[,] matrixC = new float[3, 3] { { 0, 1, 0 }, { 1, 0, 0 }, { 0, 0, 1 } };
            //float[,] matrixC = new float[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };

            Vector3 v = vector;
            //float[] initial_vector = new float[3] {vector.X, vector.Y,0f };
            //v.X = matrixC[0, 0] * initial_vector[0] + matrixC[0, 1] * initial_vector[1] + matrixC[0, 2] * initial_vector[2];
            //v.Y = matrixC[1, 0] * initial_vector[0] + matrixC[1, 1] * initial_vector[1] + matrixC[1, 2] * initial_vector[2];
            //v.Z = matrixC[2, 0] * initial_vector[0] + matrixC[2, 1] * initial_vector[1] + matrixC[2, 2] * initial_vector[2];

            Matrix4x4 matrixR = new Matrix4x4();

            //Get parameter of transformation in s[]        
            //matrixR.M11 = -0.9975f; matrixR.M12 = -0.0625f; matrixR.M13 = 0.0317f; matrixR.M14 = 0f;
            //matrixR.M21 = 0.0618f; matrixR.M22 = -0.9979f; matrixR.M23 = -0.0218f; matrixR.M24 = 0f;
            //matrixR.M31 = 0.0330f; matrixR.M32 = -0.0198f; matrixR.M33 = 0.9993f; matrixR.M34 = 0f;

            //matrixR.M41 = -204.9082f+10f; matrixR.M42 = -6.828f; matrixR.M43 = 44.4672f; matrixR.M44 = 1f;

            matrixR.M11 = 1; matrixR.M12 = 0f; matrixR.M13 = 0; matrixR.M14 = 0f;
            matrixR.M21 = 0; matrixR.M22 = 1; matrixR.M23 = 0; matrixR.M24 = 0f;
            matrixR.M31 = 0; matrixR.M32 = 0; matrixR.M33 = 1; matrixR.M34 = 0f;
            matrixR.M41 = 250f; matrixR.M42 = 10f; matrixR.M43 = 30f; matrixR.M44 = 1f;
            //Implement TransformVector
            Vector3 v2 = new Vector3();
            v2.X = matrixR.M11 * v.X + matrixR.M21 * v.Y + matrixR.M31 * v.Z + matrixR.M41;
            v2.Y = matrixR.M12 * v.X + matrixR.M22 * v.Y + matrixR.M32 * v.Z + matrixR.M42;
            v2.Z = matrixR.M13 * v.X + matrixR.M23 * v.Y + matrixR.M33 * v.Z + matrixR.M43;

            return v2;
        }

        private Vector3 TransformWithCalibrationForC3New(Vector3 vector)
        {
            Vector3 v = vector;

            Matrix4x4 matrixR = new Matrix4x4();

            matrixR.M11 = 1; matrixR.M12 = 0f; matrixR.M13 = 0; matrixR.M14 = 0f;
            matrixR.M21 = 0; matrixR.M22 = 1; matrixR.M23 = 0; matrixR.M24 = 0f;
            matrixR.M31 = 0; matrixR.M32 = 0; matrixR.M33 = 1; matrixR.M34 = 0f;

            matrixR.M41 = 210; matrixR.M42 = -15f; matrixR.M43 = 11; matrixR.M44 = 1f;
            //Implement TransformVector
            Vector3 v2 = new Vector3();
            v2.X = matrixR.M11 * v.X + matrixR.M21 * v.Y + matrixR.M31 * v.Z + matrixR.M41;
            v2.Y = matrixR.M12 * v.X + matrixR.M22 * v.Y + matrixR.M32 * v.Z + matrixR.M42;
            v2.Z = matrixR.M13 * v.X + matrixR.M23 * v.Y + matrixR.M33 * v.Z + matrixR.M43;

            return v2;
        }

        private Vector3 TransformWithRotation(Vector3 vector)    //fixed 只能在 unsafe环境下使用
        {
            Matrix4x4 matrixR = new Matrix4x4();
            Vector3 v = new Vector3();
            GetSSMatrix();
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

        private void GetSSMatrix()
        {
            float a = (float)(parasQuaternion[0]);
            float b = (float)(parasQuaternion[1]);
            float c = (float)(parasQuaternion[2]);
            float d = (float)(parasQuaternion[3]);

            ssCalibrationMatrxi[0] = (float)(1 - 2 * Math.Pow(c, 2) - 2 * Math.Pow(d, 2));
            ssCalibrationMatrxi[1] = (float)(2 * b * c + 2 * a * d);//zaici ganxie litongxue de bianxie (~_~!)
            ssCalibrationMatrxi[2] = (float)(2 * b * d - 2 * a * c);
            ssCalibrationMatrxi[3] = (float)(2 * b * c - 2 * a * d);
            ssCalibrationMatrxi[4] = (float)(1 - 2 * Math.Pow(b, 2) - 2 * Math.Pow(d, 2));
            ssCalibrationMatrxi[5] = (float)(2 * a * b + 2 * c * d);
            ssCalibrationMatrxi[6] = (float)(2 * a * c + 2 * b * d);
            ssCalibrationMatrxi[7] = (float)(2 * c * d - 2 * a * b);
            ssCalibrationMatrxi[8] = (float)(1 - 2 * Math.Pow(b, 2) - 2 * Math.Pow(c, 2));


        }
    }
}
