using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using csmatio.io;
using csmatio.types;

namespace Ultrasonics3DReconstructor
{
    public class ReconstructionMethods2
    {
        //Voxel Size
        public float voxelWidth = 0.5f;
        public float voxelHeight = 0.5f;
        public float voxelDepth = 1f;

        USDataCollection2 m_usDataCollection;    // The usDataCollection used in reconstruction

        public short[][,] m_box;               // 3D volume to storage reconstruction data
        public short[,] projectionMatrix;

        // BoxSize
        public int mboxWidth = 0;
        public int mboxHeight = 0;
        public int mboxDepth = 0;
        public Vector3 mboxOrg = new Vector3(0f, 0f, 0f);

        // Save path
        public string savePath = null;


        // HoleFilling
        bool isHoleFilling = false;
        public enum Method  // enum of type of reconstruction methods
        {
            FDP,
            VNN,
            PNN,
            MPI2,
            MPI4,
            RealTimePNN, // use PNN to do real-time reconstruction
            Double_Sweep
        }


        /// <summary>
        /// constructor of reconstruction
        /// </summary>
        /// <param name="method"></param>
        /// <param name="usDataCollection"></param>
        /// <param name="voxel_width"></param>
        /// <param name="voxel_height"></param>
        /// <param name="voxel_depth"></param>
        public ReconstructionMethods2(Method method, USDataCollection2 usDataCollection)
        {
            m_usDataCollection = usDataCollection;

            voxelWidth = m_usDataCollection.voxelWidth;
            voxelHeight = m_usDataCollection.voxelHeight;
            voxelDepth = m_usDataCollection.voxelDepth;
            savePath = m_usDataCollection.savedPath;
            isHoleFilling = m_usDataCollection.isHoleFillingUsed;
            //voxelWidth = voxel_width;
            //voxelHeight = voxel_height;
            //voxelDepth = voxel_depth;
            //savePath = m_savePath;

            switch (method)
            {
                case Method.VNN:
                    Console.WriteLine("\n[+]Start VNN reconstruction...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    VNN();
                    break;
                case Method.FDP:
                    Console.WriteLine("\n[+]Start FDP reconstruction...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    FDPWyHx_VNN();
                    break;
                case Method.PNN:
                    Console.WriteLine("\n[+]Start PNN reconstruction...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    DoubleSweepWyHxPNN();
                    break;
                case Method.MPI2:
                    Console.WriteLine("\n[+]Start MPI_FDP reconstruction...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    MPI_FDP();
                    break;
                case Method.MPI4:
                    Console.WriteLine("\n[+]Start MPI_FDP reconstruction...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    MPI4_FDP();
                    break;
                case Method.RealTimePNN:
                    Console.WriteLine("\n[+]Start real time reconstruction...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    break;
                case Method.Double_Sweep:
                    Console.WriteLine("\n[+]Start Left and Right reconstruction, using PNN...");
                    Console.WriteLine("Voxel size = {0} x {1} x {2} mm^3", voxelWidth, voxelHeight, voxelDepth);
                    DoubleSweepWyHxPNN();


                    break;
                default:
                    Console.WriteLine(" No method is selected!");
                    break;
            }
        }

        public ReconstructionMethods2()
        {

        }

     

        public void VNN()
        {
            DateTime beforeDT = System.DateTime.Now;


            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            Vector3[] frameNormals = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUX = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUY = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameOrg = new Vector3[m_usDataCollection.framesCount];
            float[] frameUX2Pixel = new float[m_usDataCollection.framesCount];
            float[] frameUY2Pixel = new float[m_usDataCollection.framesCount];

            for (int i = 0; i < m_usDataCollection.framesCount; i++)
            {
                frameUX[i] = m_usDataCollection.calibratedRects[i].v[1] - m_usDataCollection.calibratedRects[i].v[0];     //direction of x vector
                frameUY[i] = m_usDataCollection.calibratedRects[i].v[3] - m_usDataCollection.calibratedRects[i].v[0];     //direction of y vector
                frameNormals[i] = Vector3.Normalize(Vector3.Cross(frameUX[i], frameUY[i]));
                frameUX2Pixel[i] = (float)frameWidth / frameUX[i].Length();
                frameUY2Pixel[i] = (float)frameHeight / frameUY[i].Length();
                frameUX[i] = Vector3.Normalize(frameUX[i]);
                frameUY[i] = Vector3.Normalize(frameUY[i]);
                frameOrg[i] = m_usDataCollection.calibratedRects[i].v[0];
            }

            Console.WriteLine("Build Z table...");
            List<int>[] array2 = new List<int>[boxDepth];
            for (int j = 0; j < array2.Length; j++)
            {
                array2[j] = new List<int>(m_usDataCollection.framesCount / 10);
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                float z = m_usDataCollection.calibratedRects[k].v[0].Z;
                float num4 = z;
                for (int ki = 0; ki < 4; ki++)
                {
                    float tempZ = m_usDataCollection.calibratedRects[k].v[ki].Z;
                    if (z > tempZ)
                    {
                        z = tempZ;
                    }
                    if (num4 < tempZ)
                    {
                        num4 = tempZ;
                    }
                }
                z = z - boxOrg.Z - voxelDepth * 3;       //Get z min
                num4 = num4 - boxOrg.Z + voxelDepth * 3; //Get z max

                int num5 = (int)Math.Round(z / voxelDepth);
                int num6 = (int)Math.Round(num4 / voxelDepth);

                for (int kn = num5; kn <= num6; kn++)
                {
                    if (kn >= 0 && kn < boxDepth)
                    {
                        array2[kn].Add(k);
                    }
                }

            }
            int[][] fromZ2FrameIdxArray = (from v in array2
                                           select v.ToArray()).ToArray();
            Console.WriteLine("Done");

            VNN_util_findMinMaxY(boxOrg, boxWidth, boxHeight, boxDepth, out int[,] runLengthMinY, out int[,] runLengthMaxY);

            Console.WriteLine("A high computation is proccesing...Please waiting...");

            short xxx = 0;
            for (int i4 = 0; i4 < boxDepth; i4++)
            {
                for (int i5 = 0; i5 < boxWidth; i5++)
                {
                    int num18 = runLengthMinY[i4, i5];
                    int num19 = runLengthMaxY[i4, i5];

                    for (int i6 = 0; i6 < num18; i6++)
                    {
                        boxValues[i4][i6, i5] = xxx;
                    }

                    for (int i7 = num18; i7 < num19; i7++)
                    {
                        Vector3 sVector3Df_box = boxOrg;
                        sVector3Df_box.X += i5 * voxelWidth;
                        sVector3Df_box.Y += i7 * voxelHeight;
                        sVector3Df_box.Z += i4 * voxelDepth;

                        float value3 = 3.40282347E+38f;
                        float num22 = 3.40282347E+38f;
                        int num23 = -1;
                        int[] array7 = fromZ2FrameIdxArray[i4];
                        foreach (int i8 in array7)
                        {
                            float num26 = Vector3.Dot(frameNormals[i8], (frameOrg[i8] - sVector3Df_box));
                            float num27 = Math.Abs(num26);
                            if (num22 > num27)
                            {
                                value3 = num26;
                                num22 = num27;
                                num23 = i8;
                            }

                        }

                        if (num23 < 0)
                        {
                            boxValues[i4][i7, i5] = xxx;
                        }
                        else
                        {
                            Vector3 obj2 = frameNormals[num23];
                            Vector3 sVector3Df2_box = sVector3Df_box + obj2 * value3 - frameOrg[num23];
                            Vector3 obj3 = frameUX[num23];
                            Vector3 obj4 = frameUY[num23];
                            float num28 = Vector3.Dot(sVector3Df2_box, obj3) * frameUX2Pixel[num23];
                            float num29 = Vector3.Dot(sVector3Df2_box, obj4) * frameUY2Pixel[num23];
                            int num30 = (int)Math.Round(num28);
                            int num31 = (int)Math.Round(num29);
                            if (num30 < 0 || num30 >= frameWidth || num31 < 0 || num31 >= frameHeight)
                            {
                                boxValues[i4][i7, i5] = xxx;
                            }
                            else
                            {
                                boxValues[i4][i7, i5] = m_usDataCollection.frames[num23][num31, num30];
                            }
                        }
                    }

                    for (int i8 = num19; i8 < boxHeight; i8++)
                    {
                        boxValues[i4][i8, i5] = xxx;
                    }
                }
            }

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);
            Console.WriteLine("CPM-VNN 总共花费时间{0}s", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            //Console.WriteLine("Done");

            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
        }

        /// <summary>
        /// Fast Dot-Projection Method
        /// </summary>
        public short[,] MPI_FDP()
        {
            DateTime beforeDT = System.DateTime.Now;


            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            Vector3[] frameNormals = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUX = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUY = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameOrg = new Vector3[m_usDataCollection.framesCount];
            float[] frameUX2Pixel = new float[m_usDataCollection.framesCount];
            float[] frameUY2Pixel = new float[m_usDataCollection.framesCount];

            for (int i = 0; i < m_usDataCollection.framesCount; i++)
            {
                frameUX[i] = m_usDataCollection.calibratedRects[i].v[1] - m_usDataCollection.calibratedRects[i].v[0];     //direction of x vector
                frameUY[i] = m_usDataCollection.calibratedRects[i].v[3] - m_usDataCollection.calibratedRects[i].v[0];     //direction of y vector
                frameNormals[i] = Vector3.Normalize(Vector3.Cross(frameUX[i], frameUY[i]));
                frameUX2Pixel[i] = (float)frameWidth / frameUX[i].Length();
                frameUY2Pixel[i] = (float)frameHeight / frameUY[i].Length();
                frameUX[i] = Vector3.Normalize(frameUX[i]);
                frameUY[i] = Vector3.Normalize(frameUY[i]);
                frameOrg[i] = m_usDataCollection.calibratedRects[i].v[0];
            }

            Console.WriteLine("Build Z table...");
            List<int>[] array2 = new List<int>[boxDepth];
            for (int j = 0; j < array2.Length; j++)
            {
                array2[j] = new List<int>(m_usDataCollection.framesCount / 10);
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                float z = m_usDataCollection.calibratedRects[k].v[0].Z;
                float num4 = z;
                for (int ki = 0; ki < 4; ki++)
                {
                    float tempZ = m_usDataCollection.calibratedRects[k].v[ki].Z;
                    if (z > tempZ)
                    {
                        z = tempZ;
                    }
                    if (num4 < tempZ)
                    {
                        num4 = tempZ;
                    }
                }
                z = z - boxOrg.Z - voxelDepth * 3;       //Get z min
                num4 = num4 - boxOrg.Z + voxelDepth * 3; //Get z max

                int num5 = (int)Math.Round(z / voxelDepth);
                int num6 = (int)Math.Round(num4 / voxelDepth);

                for (int kn = num5; kn <= num6; kn++)
                {
                    if (kn >= 0 && kn < boxDepth)
                    {
                        array2[kn].Add(k);
                    }
                }

            }
            int[][] fromZ2FrameIdxArray = (from v in array2
                                           select v.ToArray()).ToArray();
            Console.WriteLine("Done");

            VNN_util_findMinMaxY(boxOrg, boxWidth, boxHeight, boxDepth, out int[,] runLengthMinY, out int[,] runLengthMaxY);

            Console.WriteLine("A high computation is proccesing...Please waiting...");
            for (int i4 = 0; i4 < boxDepth; i4++)
            {
                int[] i8 = fromZ2FrameIdxArray[i4];
                Vector3 vector3Box = boxOrg + new Vector3(0f, 0f, (float)i4);

                // 求出 cons 在 i8[]里的所有值
                // 吧a/b 放在两个数组
                float[] cons0 = new float[i8.Length];
                int conCount = 0;
                foreach (int framesIdx in i8)
                {
                    Vector3 tempV0 = vector3Box - frameOrg[framesIdx];
                    cons0[conCount++] = Vector3.Dot(frameNormals[framesIdx], tempV0);
                }

                // 计算一个voxel 向他附近的平面投影
                for (int basic_i5 = 0; basic_i5 < boxWidth; basic_i5++)
                {
                    int basicNum18 = runLengthMinY[i4, basic_i5];
                    int basicNum19 = runLengthMaxY[i4, basic_i5];
                    for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                    {
                        float[] distance = new float[i8.Length];
                        float[] copyDistacne = new float[i8.Length];
                        for (int idx = 0; idx < i8.Length; idx++)
                        {
                            int iidx = i8[idx];
                            float a = frameNormals[iidx].X;
                            float b = frameNormals[iidx].Y;
                            float con = cons0[idx];
                            //耗时最长
                            distance[idx] = (float)Math.Abs(a * basic_i5 * 0.5 + b * basic_i7 * 0.5 + con);
                        }

                        //求出distance 最小值，以及对应的frameIdx
                        //float minimum = 300;
                        //int minimumIdx = 0;
                        //for (int idx = 0; idx < distance.Length; idx++)
                        //{
                        //    if (distance[idx] < minimum)
                        //    {
                        //        minimum = distance[idx];
                        //        minimumIdx = idx;
                        //    }
                        //}

                        Array.Copy(distance, copyDistacne, distance.Length);
                        Array.Sort(copyDistacne);
                        float minimum = copyDistacne[0];
                        int minimumIdx = Array.IndexOf(distance, minimum);

                        float minimum1 = copyDistacne[0];
                        int minimumIdx1 = Array.IndexOf(distance, minimum);

                        short tempBox = 0;
                        int i8Idx0 = i8[minimumIdx];
                        Vector3 sVector3Df_box0 = boxOrg;
                        sVector3Df_box0.X += basic_i5 * voxelWidth;
                        sVector3Df_box0.Y += basic_i7 * voxelHeight;
                        sVector3Df_box0.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box0 = sVector3Df_box0 - frameOrg[i8Idx0];

                        Vector3 obj30 = frameUX[i8Idx0];
                        Vector3 obj40 = frameUY[i8Idx0];
                        float num280 = (float)Vector3.Dot(sVector3Df2_box0, obj30) * frameUX2Pixel[i8Idx0];
                        float num290 = (float)Vector3.Dot(sVector3Df2_box0, obj40) * frameUY2Pixel[i8Idx0];
                        int num300 = (int)Math.Round(num280);
                        int num310 = (int)Math.Round(num290);
                        if ((num300 < 0) || (num300 >= frameWidth) || (num310 < 0) || (num310 >= frameHeight))
                        {
                            tempBox = 0;
                            //boxValues[i4][basic_i7, basic_i5] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            tempBox = m_usDataCollection.frames[i8Idx0][num310, num300];
                            //boxValues[i4][basic_i7, basic_i5] = m_usDataCollection.frames[i8Idx0][num310, num300];
                        }

                        short tempBox1 = 0;
                        int i8Idx = i8[minimumIdx1];
                        Vector3 sVector3Df_box = boxOrg;
                        sVector3Df_box.X += basic_i5 * voxelWidth;
                        sVector3Df_box.Y += basic_i7 * voxelHeight;
                        sVector3Df_box.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box = sVector3Df_box - frameOrg[i8Idx];

                        Vector3 obj3 = frameUX[i8Idx];
                        Vector3 obj4 = frameUY[i8Idx];
                        float num28 = (float)Vector3.Dot(sVector3Df2_box, obj3) * frameUX2Pixel[i8Idx];
                        float num29 = (float)Vector3.Dot(sVector3Df2_box, obj4) * frameUY2Pixel[i8Idx];
                        int num30 = (int)Math.Round(num28);
                        int num31 = (int)Math.Round(num29);
                        if ((num30 < 0) || (num30 >= frameWidth) || (num31 < 0) || (num31 >= frameHeight))
                        {
                            tempBox1 = 0;
                        }
                        else
                        {
                            tempBox1 = m_usDataCollection.frames[i8Idx0][num310, num300];
                        }

                        boxValues[i4][basic_i7, basic_i5] = Math.Max(tempBox, tempBox1);

                    }
                }


            }

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);
            Console.WriteLine("MPI-FDP 总共花费时间{0}s", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");
            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;
            //Console.WriteLine("Done");

            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
            return corolnalMatrix;
        }


        /// <summary>
        /// Fast Dot-Projection Method
        /// </summary>
        public short[,] MPI4_FDP()
        {
            DateTime beforeDT = System.DateTime.Now;


            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            Vector3[] frameNormals = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUX = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUY = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameOrg = new Vector3[m_usDataCollection.framesCount];
            float[] frameUX2Pixel = new float[m_usDataCollection.framesCount];
            float[] frameUY2Pixel = new float[m_usDataCollection.framesCount];

            for (int i = 0; i < m_usDataCollection.framesCount; i++)
            {
                frameUX[i] = m_usDataCollection.calibratedRects[i].v[1] - m_usDataCollection.calibratedRects[i].v[0];     //direction of x vector
                frameUY[i] = m_usDataCollection.calibratedRects[i].v[3] - m_usDataCollection.calibratedRects[i].v[0];     //direction of y vector
                frameNormals[i] = Vector3.Normalize(Vector3.Cross(frameUX[i], frameUY[i]));
                frameUX2Pixel[i] = (float)frameWidth / frameUX[i].Length();
                frameUY2Pixel[i] = (float)frameHeight / frameUY[i].Length();
                frameUX[i] = Vector3.Normalize(frameUX[i]);
                frameUY[i] = Vector3.Normalize(frameUY[i]);
                frameOrg[i] = m_usDataCollection.calibratedRects[i].v[0];
            }

            Console.WriteLine("Build Z table...");
            List<int>[] array2 = new List<int>[boxDepth];
            for (int j = 0; j < array2.Length; j++)
            {
                array2[j] = new List<int>(m_usDataCollection.framesCount / 10);
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                float z = m_usDataCollection.calibratedRects[k].v[0].Z;
                float num4 = z;
                for (int ki = 0; ki < 4; ki++)
                {
                    float tempZ = m_usDataCollection.calibratedRects[k].v[ki].Z;
                    if (z > tempZ)
                    {
                        z = tempZ;
                    }
                    if (num4 < tempZ)
                    {
                        num4 = tempZ;
                    }
                }
                z = z - boxOrg.Z - voxelDepth * 3;       //Get z min
                num4 = num4 - boxOrg.Z + voxelDepth * 3; //Get z max

                int num5 = (int)Math.Round(z / voxelDepth);
                int num6 = (int)Math.Round(num4 / voxelDepth);

                for (int kn = num5; kn <= num6; kn++)
                {
                    if (kn >= 0 && kn < boxDepth)
                    {
                        array2[kn].Add(k);
                    }
                }

            }
            int[][] fromZ2FrameIdxArray = (from v in array2
                                           select v.ToArray()).ToArray();
            Console.WriteLine("Done");

            VNN_util_findMinMaxY(boxOrg, boxWidth, boxHeight, boxDepth, out int[,] runLengthMinY, out int[,] runLengthMaxY);

            Console.WriteLine("A high computation is proccesing...Please waiting...");
            for (int i4 = 0; i4 < boxDepth; i4++)
            {
                int[] i8 = fromZ2FrameIdxArray[i4];
                Vector3 vector3Box = boxOrg + new Vector3(0f, 0f, (float)i4);

                // 求出 cons 在 i8[]里的所有值
                // 吧a/b 放在两个数组
                float[] cons0 = new float[i8.Length];
                int conCount = 0;
                foreach (int framesIdx in i8)
                {
                    Vector3 tempV0 = vector3Box - frameOrg[framesIdx];
                    cons0[conCount++] = Vector3.Dot(frameNormals[framesIdx], tempV0);
                }

                // 计算一个voxel 向他附近的平面投影
                for (int basic_i5 = 0; basic_i5 < boxWidth; basic_i5++)
                {
                    int basicNum18 = runLengthMinY[i4, basic_i5];
                    int basicNum19 = runLengthMaxY[i4, basic_i5];
                    for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                    {
                        float[] distance = new float[boxHeight*boxWidth];
                        float[] copyDistacne = new float[boxHeight * boxWidth];
                        for (int idx = 0; idx < i8.Length; idx++)
                        {
                            int iidx = i8[idx];
                            float a = frameNormals[iidx].X;
                            float b = frameNormals[iidx].Y;
                            float con = cons0[idx];
                            //耗时最长
                            distance[idx] = (float)Math.Abs(a * basic_i5 * 0.5 + b * basic_i7 * 0.5 + con);
                        }

                        //求出distance 最小值，以及对应的frameIdx
                        //float minimum = 300;
                        //int minimumIdx = 0;
                        //for (int idx = 0; idx < distance.Length; idx++)
                        //{
                        //    if (distance[idx] < minimum)
                        //    {
                        //        minimum = distance[idx];
                        //        minimumIdx = idx;
                        //    }
                        //}

                        Array.Copy(distance, copyDistacne, distance.Length);
                        Array.Sort(copyDistacne);

                        float minimum = copyDistacne[0];
                        int minimumIdx = Array.IndexOf(distance, minimum);

                        float minimum1 = copyDistacne[1];
                        int minimumIdx1 = Array.IndexOf(distance, minimum1);

                        float minimum2 = copyDistacne[2];
                        int minimumIdx2 = Array.IndexOf(distance, minimum2);

                        float minimum3 = copyDistacne[3];
                        int minimumIdx3 = Array.IndexOf(distance, minimum3);

                        short tempBox = 0;
                        int i8Idx0 = i8[minimumIdx];
                        Vector3 sVector3Df_box0 = boxOrg;
                        sVector3Df_box0.X += basic_i5 * voxelWidth;
                        sVector3Df_box0.Y += basic_i7 * voxelHeight;
                        sVector3Df_box0.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box0 = sVector3Df_box0 - frameOrg[i8Idx0];

                        Vector3 obj30 = frameUX[i8Idx0];
                        Vector3 obj40 = frameUY[i8Idx0];
                        float num280 = (float)Vector3.Dot(sVector3Df2_box0, obj30) * frameUX2Pixel[i8Idx0];
                        float num290 = (float)Vector3.Dot(sVector3Df2_box0, obj40) * frameUY2Pixel[i8Idx0];
                        int num300 = (int)Math.Round(num280);
                        int num310 = (int)Math.Round(num290);
                        if ((num300 < 0) || (num300 >= frameWidth) || (num310 < 0) || (num310 >= frameHeight))
                        {
                            tempBox = 0;
                            //boxValues[i4][basic_i7, basic_i5] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            tempBox = m_usDataCollection.frames[i8Idx0][num310, num300];
                            //boxValues[i4][basic_i7, basic_i5] = m_usDataCollection.frames[i8Idx0][num310, num300];
                        }

                        short tempBox1 = 0;
                        int i8Idx = i8[minimumIdx1];
                        Vector3 sVector3Df_box = boxOrg;
                        sVector3Df_box.X += basic_i5 * voxelWidth;
                        sVector3Df_box.Y += basic_i7 * voxelHeight;
                        sVector3Df_box.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box = sVector3Df_box - frameOrg[i8Idx];

                        Vector3 obj3 = frameUX[i8Idx];
                        Vector3 obj4 = frameUY[i8Idx];
                        float num28 = (float)Vector3.Dot(sVector3Df2_box, obj3) * frameUX2Pixel[i8Idx];
                        float num29 = (float)Vector3.Dot(sVector3Df2_box, obj4) * frameUY2Pixel[i8Idx];
                        int num30 = (int)Math.Round(num28);
                        int num31 = (int)Math.Round(num29);
                        if ((num30 < 0) || (num30 >= frameWidth) || (num31 < 0) || (num31 >= frameHeight))
                        {
                            tempBox1 = 0;
                        }
                        else
                        {
                            tempBox1 = m_usDataCollection.frames[i8Idx][num31, num30];
                        }

                        short tempBox2 = 0;
                        int i8Idx2 = i8[minimumIdx2];
                        Vector3 sVector3Df_box2 = boxOrg;
                        sVector3Df_box2.X += basic_i5 * voxelWidth;
                        sVector3Df_box2.Y += basic_i7 * voxelHeight;
                        sVector3Df_box2.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box2 = sVector3Df_box2 - frameOrg[i8Idx2];

                        Vector3 obj32 = frameUX[i8Idx2];
                        Vector3 obj42 = frameUY[i8Idx2];
                        float num282 = (float)Vector3.Dot(sVector3Df2_box2, obj32) * frameUX2Pixel[i8Idx2];
                        float num292 = (float)Vector3.Dot(sVector3Df2_box2, obj42) * frameUY2Pixel[i8Idx2];
                        int num302 = (int)Math.Round(num282);
                        int num312 = (int)Math.Round(num292);
                        if ((num302 < 0) || (num302 >= frameWidth) || (num312 < 0) || (num312 >= frameHeight))
                        {
                            tempBox2 = 0;
                            //boxValues[i4][basic_i7, basic_i5] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            tempBox2 = m_usDataCollection.frames[i8Idx2][num312, num302];
                            //boxValues[i4][basic_i7, basic_i5] = m_usDataCollection.frames[i8Idx0][num310, num300];
                        }

                        short tempBox3 = 0;
                        int i8Idx3 = i8[minimumIdx3];
                        Vector3 sVector3Df_box3 = boxOrg;
                        sVector3Df_box3.X += basic_i5 * voxelWidth;
                        sVector3Df_box3.Y += basic_i7 * voxelHeight;
                        sVector3Df_box3.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box3 = sVector3Df_box3 - frameOrg[i8Idx3];

                        Vector3 obj33 = frameUX[i8Idx3];
                        Vector3 obj43 = frameUY[i8Idx3];
                        float num283 = (float)Vector3.Dot(sVector3Df2_box3, obj33) * frameUX2Pixel[i8Idx3];
                        float num293 = (float)Vector3.Dot(sVector3Df2_box3, obj43) * frameUY2Pixel[i8Idx3];
                        int num303 = (int)Math.Round(num283);
                        int num313 = (int)Math.Round(num293);
                        if ((num303 < 0) || (num303 >= frameWidth) || (num313 < 0) || (num313 >= frameHeight))
                        {
                            tempBox3 = 0;
                            //boxValues[i4][basic_i7, basic_i5] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            tempBox3 = m_usDataCollection.frames[i8Idx3][num313, num303];
                            //boxValues[i4][basic_i7, basic_i5] = m_usDataCollection.frames[i8Idx0][num310, num300];
                        }


                        short maxValue = Math.Max(tempBox, tempBox1);
                        maxValue = Math.Max(maxValue, tempBox2);
                        maxValue = Math.Max(maxValue, tempBox3);

                        boxValues[i4][basic_i7, basic_i5] = maxValue;

                    }
                }


            }

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);
            Console.WriteLine("FDP-MPI4 总共花费时间{0}s", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            //Console.WriteLine("Done");

            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
            return corolnalMatrix;
        }

        public void GetPixel(int i5, int i7, int i4, int i8Idx, float frameUX2Pixel)
        {

        }
        /// <summary>
        /// Fast Dot-Projection Method
        /// </summary>
        public short[,] FDP()
        {
            DateTime beforeDT = System.DateTime.Now;


            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            Vector3[] frameNormals = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUX = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUY = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameOrg = new Vector3[m_usDataCollection.framesCount];
            float[] frameUX2Pixel = new float[m_usDataCollection.framesCount];
            float[] frameUY2Pixel = new float[m_usDataCollection.framesCount];

            for (int i = 0; i < m_usDataCollection.framesCount; i++)
            {
                frameUX[i] = m_usDataCollection.calibratedRects[i].v[1] - m_usDataCollection.calibratedRects[i].v[0];     //direction of x vector
                frameUY[i] = m_usDataCollection.calibratedRects[i].v[3] - m_usDataCollection.calibratedRects[i].v[0];     //direction of y vector
                frameNormals[i] = Vector3.Normalize(Vector3.Cross(frameUX[i], frameUY[i]));
                frameUX2Pixel[i] = (float)frameWidth / frameUX[i].Length();
                frameUY2Pixel[i] = (float)frameHeight / frameUY[i].Length();
                frameUX[i] = Vector3.Normalize(frameUX[i]);
                frameUY[i] = Vector3.Normalize(frameUY[i]);
                frameOrg[i] = m_usDataCollection.calibratedRects[i].v[0];
            }

            Console.WriteLine("Build Z table...");
            List<int>[] array2 = new List<int>[boxDepth];
            for (int j = 0; j < array2.Length; j++)
            {
                array2[j] = new List<int>(m_usDataCollection.framesCount / 10);
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                float z = m_usDataCollection.calibratedRects[k].v[0].Z;
                float num4 = z;
                for (int ki = 0; ki < 4; ki++)
                {
                    float tempZ = m_usDataCollection.calibratedRects[k].v[ki].Z;
                    if (z > tempZ)
                    {
                        z = tempZ;
                    }
                    if (num4 < tempZ)
                    {
                        num4 = tempZ;
                    }
                }
                z = z - boxOrg.Z - voxelDepth * 3;       //Get z min
                num4 = num4 - boxOrg.Z + voxelDepth * 3; //Get z max

                int num5 = (int)Math.Round(z / voxelDepth);
                int num6 = (int)Math.Round(num4 / voxelDepth);

                for (int kn = num5; kn <= num6; kn++)
                {
                    if (kn >= 0 && kn < boxDepth)
                    {
                        array2[kn].Add(k);
                    }
                }

            }
            int[][] fromZ2FrameIdxArray = (from v in array2
                                           select v.ToArray()).ToArray();
            Console.WriteLine("Done");

            VNN_util_findMinMaxY(boxOrg, boxWidth, boxHeight, boxDepth, out int[,] runLengthMinY, out int[,] runLengthMaxY);

            Console.WriteLine("A high computation is proccesing...Please waiting...");
            for (int i4 = 0; i4 < boxDepth; i4++)
            {
                int[] i8 = fromZ2FrameIdxArray[i4];
                int frameIdx = i8[0];


                //Start first BasicProject
                float[] disLast = new float[boxHeight * boxWidth];

                Vector3 vector3Box = boxOrg + new Vector3(0f, 0f, (float)i4);
                Vector3 tempV0 = vector3Box - frameOrg[frameIdx];
                float cons0 = Vector3.Dot(frameNormals[frameIdx], tempV0);
                float a0 = frameNormals[frameIdx].X;
                float b0 = frameNormals[frameIdx].Y;

                int lenCompare = 0;
                for (int basic_i5 = 0; basic_i5 < boxWidth; basic_i5++)
                {
                    int basicNum18 = runLengthMinY[i4, basic_i5];
                    int basicNum19 = runLengthMaxY[i4, basic_i5];
                    for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                    {

                        disLast[lenCompare++] = (float)Math.Abs(a0 * basic_i5 * voxelWidth + b0 * basic_i7 * voxelHeight + cons0);
                    }
                }
                //End of first BasicProject

                int[] frameIdxMatrix = new int[boxWidth * boxHeight];
                for (int fi = 0; fi < frameIdxMatrix.Length; fi++)
                {
                    frameIdxMatrix[fi] = frameIdx;
                }

                int i8End = i8.Last();
                for (int ii2 = i8[1]; ii2 <= i8End; ii2++)
                {
                    float[] dis = new float[boxHeight * boxWidth];

                    //Start first BasicProject

                    //Vector3 vector3Box = boxOrg + new Vector3(0f, 0f, (float)i4);
                    Vector3 tempV = vector3Box - frameOrg[ii2];
                    float cons = Vector3.Dot(frameNormals[ii2], tempV);
                    float a = frameNormals[ii2].X;
                    float b = frameNormals[ii2].Y;
                    lenCompare = 0;
                    for (int basic_i5 = 0; basic_i5 < boxWidth; basic_i5++)
                    {
                        int basicNum18 = runLengthMinY[i4, basic_i5];
                        int basicNum19 = runLengthMaxY[i4, basic_i5];
                        for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                        {

                            dis[lenCompare++] = (float)Math.Abs(a * basic_i5 * voxelHeight + b * basic_i7 * voxelWidth + cons);
                        }
                    }
                    //End of first BasicProject


                    //Update new minimum distacne matrix
                    for (int ki = 0; ki < lenCompare; ki++)
                    {
                        if (dis[ki] < disLast[ki])
                        {
                            disLast[ki] = dis[ki];
                            frameIdxMatrix[ki] = ii2;
                        }
                    }

                }

                int cout = 0;
                for (int i5 = 0; i5 < boxWidth; i5++)
                {
                    int num18 = runLengthMinY[i4, i5];
                    int num19 = runLengthMaxY[i4, i5];
                    for (int i7 = num18; i7 < num19; i7++)
                    {
                        int i8Idx = frameIdxMatrix[cout++];
                        Vector3 sVector3Df_box = boxOrg;
                        sVector3Df_box.X += i5 * voxelWidth;
                        sVector3Df_box.Y += i7 * voxelHeight;
                        sVector3Df_box.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box = sVector3Df_box - frameOrg[i8Idx];

                        Vector3 obj3 = frameUX[i8Idx];
                        Vector3 obj4 = frameUY[i8Idx];
                        float num28 = (float)Vector3.Dot(sVector3Df2_box, obj3) * frameUX2Pixel[i8Idx];
                        float num29 = (float)Vector3.Dot(sVector3Df2_box, obj4) * frameUY2Pixel[i8Idx];
                        int num30 = (int)Math.Round(num28);
                        int num31 = (int)Math.Round(num29);
                        if ((num30 < 0) || (num30 >= frameWidth) || (num31 < 0) || (num31 >= frameHeight))
                        {
                            boxValues[i4][i7, i5] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            boxValues[i4][i7, i5] = m_usDataCollection.frames[i8Idx][num31, num30];
                        }
                    }
                }
            }


            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);


            Console.WriteLine("FDP-VNN 总共花费时间{0}s", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            //Console.WriteLine("Done");
            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
            return corolnalMatrix;
        }

        /// <summary>
        /// Fast Dot-Projection Method
        /// </summary>
        public short[,] FDPWyHx()
        {
            DateTime beforeDT = System.DateTime.Now;


            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSizeWyHx(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            Vector3[] frameNormals = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUX = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUY = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameOrg = new Vector3[m_usDataCollection.framesCount];
            float[] frameUX2Pixel = new float[m_usDataCollection.framesCount];
            float[] frameUY2Pixel = new float[m_usDataCollection.framesCount];

            for (int i = 0; i < m_usDataCollection.framesCount; i++)
            {
                frameUY[i] = m_usDataCollection.calibratedRects[i].v[1] - m_usDataCollection.calibratedRects[i].v[0];     //direction of y vector
                frameUX[i] = m_usDataCollection.calibratedRects[i].v[3] - m_usDataCollection.calibratedRects[i].v[0];     //direction of x vector
                frameNormals[i] = Vector3.Normalize(Vector3.Cross(frameUX[i], frameUY[i]));
                frameUX2Pixel[i] = (float)frameHeight / frameUX[i].Length();
                frameUY2Pixel[i] = (float)frameWidth / frameUY[i].Length();
                frameUX[i] = Vector3.Normalize(frameUX[i]);
                frameUY[i] = Vector3.Normalize(frameUY[i]);
                frameOrg[i] = m_usDataCollection.calibratedRects[i].v[0];
            }

            Console.WriteLine("Build Z table...");
            List<int>[] array2 = new List<int>[boxDepth];
            for (int j = 0; j < array2.Length; j++)
            {
                array2[j] = new List<int>(m_usDataCollection.framesCount / 10);
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                float z = m_usDataCollection.calibratedRects[k].v[0].Z;
                float num4 = z;
                for (int ki = 0; ki < 4; ki++)
                {
                    float tempZ = m_usDataCollection.calibratedRects[k].v[ki].Z;
                    if (z > tempZ)
                    {
                        z = tempZ;
                    }
                    if (num4 < tempZ)
                    {
                        num4 = tempZ;
                    }
                }
                z = z - boxOrg.Z - voxelDepth * 3;       //Get z min
                num4 = num4 - boxOrg.Z + voxelDepth * 3; //Get z max

                int num5 = (int)Math.Round(z / voxelDepth);
                int num6 = (int)Math.Round(num4 / voxelDepth);

                for (int kn = num5; kn <= num6; kn++)
                {
                    if (kn >= 0 && kn < boxDepth)
                    {
                        array2[kn].Add(k);
                    }
                }

            }
            int[][] fromZ2FrameIdxArray = (from v in array2
                                           select v.ToArray()).ToArray();
            Console.WriteLine("Done");

            VNN_util_findMinMaxY(boxOrg, boxWidth, boxHeight, boxDepth, out int[,] runLengthMinY, out int[,] runLengthMaxY);

            Console.WriteLine("A high computation is proccesing...Please waiting...");
            for (int i4 = 0; i4 < boxDepth; i4++)
            {
                int[] i8 = fromZ2FrameIdxArray[i4];
                int frameIdx = i8[0];


                //Start first BasicProject
                float[] disLast = new float[boxHeight * boxWidth];

                Vector3 vector3Box = boxOrg + new Vector3(0f, 0f, (float)i4);
                Vector3 tempV0 = vector3Box - frameOrg[frameIdx];
                float cons0 = Vector3.Dot(frameNormals[frameIdx], tempV0);
                float a0 = frameNormals[frameIdx].X;
                float b0 = frameNormals[frameIdx].Y;

                int lenCompare = 0;
                for (int basic_i5 = 0; basic_i5 < boxWidth; basic_i5++)
                {
                    int basicNum18 = runLengthMinY[i4, basic_i5];
                    int basicNum19 = runLengthMaxY[i4, basic_i5];
                    for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                    {

                        disLast[lenCompare++] = (float)Math.Abs(a0 * basic_i5 * voxelHeight + b0 * basic_i7 * voxelWidth + cons0);
                    }
                }
                //End of first BasicProject

                int[] frameIdxMatrix = new int[boxWidth * boxHeight];
                for (int fi = 0; fi < frameIdxMatrix.Length; fi++)
                {
                    frameIdxMatrix[fi] = frameIdx;
                }

                int i8End = i8.Last();
                for (int ii2 = i8[1]; ii2 <= i8End; ii2++)
                {
                    float[] dis = new float[boxHeight * boxWidth];

                    //Start first BasicProject

                    //Vector3 vector3Box = boxOrg + new Vector3(0f, 0f, (float)i4);
                    Vector3 tempV = vector3Box - frameOrg[ii2];
                    float cons = Vector3.Dot(frameNormals[ii2], tempV);
                    float a = frameNormals[ii2].X;
                    float b = frameNormals[ii2].Y;
                    lenCompare = 0;
                    for (int basic_i5 = 0; basic_i5 < boxWidth; basic_i5++)
                    {
                        int basicNum18 = runLengthMinY[i4, basic_i5];
                        int basicNum19 = runLengthMaxY[i4, basic_i5];
                        for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                        {

                            dis[lenCompare++] = (float)Math.Abs(a * basic_i5 * voxelHeight + b * basic_i7 * voxelWidth + cons);
                        }
                    }
                    //End of first BasicProject


                    //Update new minimum distacne matrix
                    for (int ki = 0; ki < lenCompare; ki++)
                    {
                        if (dis[ki] < disLast[ki])
                        {
                            disLast[ki] = dis[ki];
                            frameIdxMatrix[ki] = ii2;
                        }
                    }

                }

                int cout = 0;
                for (int i5 = 0; i5 < boxWidth; i5++)
                {
                    int num18 = runLengthMinY[i4, i5];
                    int num19 = runLengthMaxY[i4, i5];
                    for (int i7 = num18; i7 < num19; i7++)
                    {
                        int i8Idx = frameIdxMatrix[cout++];
                        Vector3 sVector3Df_box = boxOrg;
                        sVector3Df_box.X += i5 * voxelHeight;
                        sVector3Df_box.Y += i7 * voxelWidth;
                        sVector3Df_box.Z += i4 * voxelDepth;
                        Vector3 sVector3Df2_box = sVector3Df_box - frameOrg[i8Idx];

                        Vector3 obj3 = frameUX[i8Idx];
                        Vector3 obj4 = frameUY[i8Idx];
                        float num28 = (float)Vector3.Dot(sVector3Df2_box, obj3) * frameUX2Pixel[i8Idx];
                        float num29 = (float)Vector3.Dot(sVector3Df2_box, obj4) * frameUY2Pixel[i8Idx];
                        int num30 = (int)Math.Round(num28);
                        int num31 = (int)Math.Round(num29);
                        if ((num30 < 0) || (num30 >= frameHeight) || (num31 < 0) || (num31 >= frameWidth))
                        {
                            boxValues[i4][i7, i5] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            boxValues[i4][i7, i5] = m_usDataCollection.frames[i8Idx][num30, num31];
                        }
                    }
                }
            }


            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);


            Console.WriteLine("FDP-VNN 总共花费时间{0}s", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            //Console.WriteLine("Done");
            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
            return corolnalMatrix;
        }

        /// <summary>
        /// Fast Dot-Projection Method
        /// </summary>
        public short[,] FDPWyHx_VNN()
        {
            DateTime beforeDT = System.DateTime.Now;


            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSizeWyHx(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            Vector3[] frameNormals = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUX = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameUY = new Vector3[m_usDataCollection.framesCount];
            Vector3[] frameOrg = new Vector3[m_usDataCollection.framesCount];
            float[] frameUX2Pixel = new float[m_usDataCollection.framesCount];
            float[] frameUY2Pixel = new float[m_usDataCollection.framesCount];

            for (int i = 0; i < m_usDataCollection.framesCount; i++)
            {
                frameUY[i] = m_usDataCollection.calibratedRects[i].v[1] - m_usDataCollection.calibratedRects[i].v[0];     //direction of y vector
                frameUX[i] = m_usDataCollection.calibratedRects[i].v[3] - m_usDataCollection.calibratedRects[i].v[0];     //direction of x vector
                frameNormals[i] = Vector3.Normalize(Vector3.Cross(frameUX[i], frameUY[i]));
                frameUX2Pixel[i] = (float)frameHeight / frameUX[i].Length();
                frameUY2Pixel[i] = (float)frameWidth / frameUY[i].Length();
                frameUX[i] = Vector3.Normalize(frameUX[i]);
                frameUY[i] = Vector3.Normalize(frameUY[i]);
                frameOrg[i] = m_usDataCollection.calibratedRects[i].v[0];
            }

            Console.WriteLine("Build Z table...");
            List<int>[] array2 = new List<int>[boxDepth];
            for (int j = 0; j < array2.Length; j++)
            {
                array2[j] = new List<int>(m_usDataCollection.framesCount / 10);
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                float z = m_usDataCollection.calibratedRects[k].v[0].Z;
                float num4 = z;
                for (int ki = 0; ki < 4; ki++)
                {
                    float tempZ = m_usDataCollection.calibratedRects[k].v[ki].Z;
                    if (z > tempZ)
                    {
                        z = tempZ;
                    }
                    if (num4 < tempZ)
                    {
                        num4 = tempZ;
                    }
                }
                z = z - boxOrg.Z - voxelDepth * 3;       //Get z min
                num4 = num4 - boxOrg.Z + voxelDepth * 3; //Get z max

                int num5 = (int)Math.Round(z / voxelDepth);
                int num6 = (int)Math.Round(num4 / voxelDepth);

                for (int kn = num5; kn <= num6; kn++)
                {
                    if (kn >= 0 && kn < boxDepth)
                    {
                        array2[kn].Add(k);
                    }
                }

            }
            int[][] fromZ2FrameIdxArray = (from v in array2
                                           select v.ToArray()).ToArray();
            Console.WriteLine("Done");

            VNN_util_findMinMaxX(boxOrg, boxWidth, boxHeight, boxDepth, out int[,] runLengthMinX, out int[,] runLengthMaxX);

            Console.WriteLine("A high computation is proccesing...Please waiting...");
            for (int iDepth = 0; iDepth < boxDepth; iDepth++)
            {
                int[] surroundingPlanes = fromZ2FrameIdxArray[iDepth];
                int countsOfSurroundingPlanes = surroundingPlanes.Length;

                //Start first BasicProject
                float[] di = new float[boxHeight * boxWidth];
                for (int i = 0; i < boxHeight * boxWidth; i++)
                {
                    di[i] = 0;
                }
                float[] dMini = new float[boxHeight * boxWidth];
                for (int i = 0; i < boxHeight * boxWidth; i++)
                {
                    dMini[i] = 500;
                }
                int[] frameIdxMatrix = new int[boxWidth * boxHeight];
                for (int fi = 0; fi < boxWidth * boxHeight; fi++)
                {
                    frameIdxMatrix[fi] = 1;
                }

                for (int iframesCountAroundVoxel = 0; iframesCountAroundVoxel < countsOfSurroundingPlanes; iframesCountAroundVoxel++)
                {
                    int iframeIdx = surroundingPlanes[iframesCountAroundVoxel];
                    float d0 = Vector3.Dot(frameNormals[iframeIdx], (frameOrg[iframeIdx]-(boxOrg + new Vector3(0f, 0f, (float)iDepth))));
                    float a0 = frameNormals[iframeIdx].X;
                    float b0 = frameNormals[iframeIdx].Y;
                    int count = 0;
                    for (int iWidth = 0; iWidth < boxWidth; iWidth++)
                    {
                        int numMinX = runLengthMinX[iDepth, iWidth];
                        int numMaxX = runLengthMaxX[iDepth, iWidth];
                        for (int iHeight = numMinX; iHeight < numMaxX; iHeight++)
                        {

                            //di[count++] = (float)Math.Abs(a0 * iWidth * voxelHeight + b0 * iHeight * voxelWidth + d0);
                            di[count++] = (float)(-(a0 * iWidth * voxelHeight + b0 * iHeight * voxelWidth - d0));

                        }
                    }
                    //Update new minimum distacne matrix
                    for (int ki = 0; ki < count; ki++)
                    {
                        if (Math.Abs(di[ki]) < Math.Abs(dMini[ki]))
                        {
                            dMini[ki] = di[ki];
                            frameIdxMatrix[ki] = iframeIdx;
                        }
                    }
                }

                int count2 = 0;
                for (int iWidth = 0; iWidth < boxWidth; iWidth++)
                {
                    int numMinX = runLengthMinX[iDepth, iWidth];
                    int numMaxX = runLengthMaxX[iDepth, iWidth];
                    for (int iHeight = numMinX; iHeight < numMaxX; iHeight++)
                    {

                        int i8Idx = frameIdxMatrix[count2];
                        float minDis = dMini[count2];
                        count2++;
                        Vector3 sVector3Df_box = boxOrg;
                        sVector3Df_box.X += iHeight * voxelHeight;
                        sVector3Df_box.Y += iWidth * voxelWidth;
                        sVector3Df_box.Z += iDepth * voxelDepth;
                        Vector3 sVector3Df2_box = sVector3Df_box + frameNormals[i8Idx] * minDis - frameOrg[i8Idx];

                        Vector3 obj3 = frameUX[i8Idx];
                        Vector3 obj4 = frameUY[i8Idx];
                        float num28 = (float)Vector3.Dot(sVector3Df2_box, obj3) * frameUX2Pixel[i8Idx];
                        float num29 = (float)Vector3.Dot(sVector3Df2_box, obj4) * frameUY2Pixel[i8Idx];
                        int num30 = (int)Math.Round(num28);
                        int num31 = (int)Math.Round(num29);
                        if ((num30 < 0) || (num30 >= frameHeight) || (num31 < 0) || (num31 >= frameWidth))
                        {
                            boxValues[iDepth][iHeight, iWidth] = 0;             //Ps: 此处与matlab相反，因为不需要再进行Rot90
                        }
                        else
                        {
                            boxValues[iDepth][iHeight, iWidth] = m_usDataCollection.frames[i8Idx][num30, num31];
                        }
                    }
                }


            }


            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);


            Console.WriteLine("FDP-VNN 总共花费时间{0}s", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            //Console.WriteLine("Done");
            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
            return corolnalMatrix;
        }

        public void BasicProject(Vector3 vector3Box, Vector3 frameOrg, Vector3 frameNormal, int[,] runLengthMaxY, int[,] runLengthMinY, int zIdx, int height, int width, out float[] dis, out int lenCompare)
        {
            //BasicProject
            dis = new float[height * width];
            Vector3 tempV = vector3Box - frameOrg;
            float cons = Vector3.Dot(frameNormal, tempV);
            float a = frameNormal.X;
            float b = frameNormal.Y;

            lenCompare = 0;
            for (int basic_i5 = 0; basic_i5 < width; basic_i5++)
            {
                int basicNum18 = runLengthMinY[zIdx, basic_i5];
                int basicNum19 = runLengthMaxY[zIdx, basic_i5];
                for (int basic_i7 = basicNum18; basic_i7 < basicNum19; basic_i7++)
                {

                    dis[lenCompare++] = (float)Math.Abs(a * basic_i5 * 0.5 + b * basic_i7 * 0.5 + cons);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ma"> Current Dis in this plane </param>
        /// <param name="mb"> Last Dis in this plane </param>
        /// <param name="mIdx"></param>
        /// <param name="i8Idx"></param>
        /// <param name="md"> FrameIdx </param>
        /// <param name="mc"> Distance </param>
        public void MinMatrix(float[] ma, float[] mb, int[] mIdx, int i8Idx, int len, out float[] mc, out int[] md)
        {
            //int len = ma.Length;
            md = new int[mIdx.Length];
            mc = new float[mb.Length];
            Array.Copy(mIdx, md, mIdx.Length);
            Array.Copy(mb, mc, mb.Length);

            for (int m = 0; m < len; m++)
            {
                if (ma[m] < mb[m])
                {
                    mc[m] = ma[m];
                    md[m] = i8Idx;
                }
            }

        }

        public void PNN()
        {
            DateTime beforeDT = System.DateTime.Now;

            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            Console.WriteLine("A high computation is proccesing...Please waiting...");

            short[][,] boxValues = new short[boxDepth][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }


            for (int frameIdx = 0; frameIdx < m_usDataCollection.framesCount; frameIdx++)
            {
                bool flag2 = true;
                bool flag3 = true;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 a = m_usDataCollection.calibratedRects[frameIdx].v[j] - boxOrg;
                    int num1 = Convert.ToInt32(a.Z / voxelDepth);
                    if (num1 >= zLowerBound)
                    {
                        flag2 = false;
                    }
                    if (num1 <= zUpperBound)
                    {
                        flag3 = false;
                    }
                }


                if (!(flag2 | flag3))
                {
                    Vector3 obj = m_usDataCollection.calibratedRects[frameIdx].v[1] - m_usDataCollection.calibratedRects[frameIdx].v[0];      //x
                    Vector3 obj2 = m_usDataCollection.calibratedRects[frameIdx].v[3] - m_usDataCollection.calibratedRects[frameIdx].v[0];     //y

                    //只计算一个顶点的值，其他的根据公式推出即可
                    Vector3 frameOrg = m_usDataCollection.calibratedRects[frameIdx].v[0];
                    Vector3 fx = obj / frameWidth;
                    Vector3 fy = obj2 / frameHeight;
                    Vector3 cons = fx * 0 + fy * 0 + frameOrg - boxOrg;
                    for (int num12 = 0; num12 < frameWidth; num12++)
                    {

                        for (int num13 = 0; num13 < frameHeight; num13++)
                        {
                            Vector3 a = (fx * num12 + fy * num13 + cons);
                            int num14 = Convert.ToInt32(a.X / voxelWidth);
                            int num15 = Convert.ToInt32(a.Y / voxelHeight);
                            int num16 = Convert.ToInt32(a.Z / voxelDepth);

                            if (num16 >= zLowerBound && num16 < zUpperBound)
                            {
                                byte b = m_usDataCollection.frames[frameIdx][num13, num12];
                                if (b > boxValues[num16][num15, num14])
                                {
                                    boxValues[num16][num15, num14] = b;
                                }

                            }
                        }
                    }
                }
            }

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);


            //将volume投影到冠状面
            this.CoronalProjection(boxValues, boxDepth, boxWidth);

            //保存为.mat文件
            //this.SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            Console.WriteLine("Finished the reconstruction!");
            Console.WriteLine("PNN 总共花费时间{0}ms", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            //Console.WriteLine("Done");
        }

        public void test()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Finished the test!");

        }
        public short[,] DoubleSweepWyHxPNN()
        {
            DateTime beforeDT = System.DateTime.Now;

            int frameHeight = m_usDataCollection.frames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.frames[0].GetLength(1);        //640
            Vector3 boxOrg = new Vector3();
            CalculateBoxSizeWyHx(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);


            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            Console.WriteLine("A high computation is proccesing...Please waiting...");

            short[][,] boxValues = new short[boxDepth][,];
            short[][,] boxCounts = new short[boxDepth][,];

            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxCounts[ik] = new short[boxHeight, boxWidth];
            }

            for (int frameIdx = 0; frameIdx < m_usDataCollection.framesCount; frameIdx++)
            {
                bool flag2 = true;
                bool flag3 = true;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 a = m_usDataCollection.calibratedRects[frameIdx].v[j] - boxOrg;
                    int num1 = Convert.ToInt32(a.Z / voxelDepth);
                    if (num1 >= zLowerBound)
                    {
                        flag2 = false;
                    }
                    if (num1 <= zUpperBound)
                    {
                        flag3 = false;
                    }
                }


                if (!(flag2 | flag3))
                {
                    Vector3 obj = m_usDataCollection.calibratedRects[frameIdx].v[1] - m_usDataCollection.calibratedRects[frameIdx].v[0];      //y
                    Vector3 obj2 = m_usDataCollection.calibratedRects[frameIdx].v[3] - m_usDataCollection.calibratedRects[frameIdx].v[0];     //x

                    //只计算一个顶点的值，其他的根据公式推出即可
                    Vector3 frameOrg = m_usDataCollection.calibratedRects[frameIdx].v[0];
                    Vector3 fy = obj / frameWidth;
                    Vector3 fx = obj2 / frameHeight;
                    Vector3 cons = fx * 0 + fy * 0 + frameOrg - boxOrg;
                    for (int iWidth = 0; iWidth < frameWidth; iWidth++)
                    {

                        for (int iHeight = 0; iHeight < frameHeight; iHeight++)
                        {
                            //Vector3 a = (fx * num12 + fy * num13 + cons);
                            Vector3 a = (fx * iHeight + fy * iWidth + cons);

                            int num14 = Convert.ToInt32(a.X / voxelHeight);
                            int num15 = Convert.ToInt32(a.Y / voxelWidth);
                            int num16 = Convert.ToInt32(a.Z / voxelDepth);

                            if (num16 >= zLowerBound && num16 < zUpperBound)
                            {
                                byte b = m_usDataCollection.frames[frameIdx][iHeight, iWidth];
                                if (b > boxValues[num16][num14, num15])
                                {
                                    boxValues[num16][num14, num15] = (short) (( boxValues[num16][num14, num15] + b)/2);

                                }
                                boxCounts[num16][num14, num15] = -1;


                            }
                        }
                    }
                }
            }

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);


            //将volume投影到冠状面
            this.CoronalProjection(boxValues, boxDepth, boxWidth);

            //保存为.mat文件
            //this.SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            Console.Write("Finished the reconstruction! ");
            Console.WriteLine("Total time is: {0}ms", ts.TotalSeconds);

            //Console.WriteLine("[+] Write to mat File...");

            //SaveVolumeToMat(boxValues, boxWidth, boxHeight, boxDepth);
            m_box = boxValues;

            for (int k = 0; k < boxDepth; k++)
            {
                for (int l = 0; l < boxHeight; l++)
                {
                    for (int m = 0; m < boxWidth; m++)
                    {
                        if (boxCounts[k][l, m] == 0)
                        {
                            m_box[k][l, m] = -1;
                        }
                    }
                }
            }

            if (isHoleFilling)
            {
                Console.WriteLine("Use Hole-Filling stage in PNN, radius: 3...");
                PNN_HoleFilling(3);
            }

            //Console.WriteLine("Done");
            short[,] corolnalMatrix;
            corolnalMatrix = this.CoronalProjection(boxValues, boxDepth, boxWidth);

            projectionMatrix = new short[corolnalMatrix.GetLength(0), corolnalMatrix.GetLength(1)];
            Array.Copy(corolnalMatrix, projectionMatrix, corolnalMatrix.Length);
            return corolnalMatrix;
            //Console.WriteLine("Done");
        }
        public void PNN_HoleFilling(int radius)
        {
            int num = m_box.Length;
            int length = m_box[0].GetLength(0);
            int length2 = m_box[0].GetLength(1);
            short[][,] array = (short[][,])m_box.Clone();
            for (int i = 0; i < num; i++)
            {
                array[i] = (short[,])array[i].Clone();
            }
            for (int j = 0; j < num; j++)
            {
                for (int k = 0; k < length; k++)
                {
                    for (int l = 0; l < length2; l++)
                    {
                        if (m_box[j][k, l] == -1)
                        {
                            int num2 = 0;
                            int num3 = 0;
                            for (int m = -radius; m <= radius; m++)
                            {
                                for (int n = -radius; n <= radius; n++)
                                {
                                    for (int num4 = -radius; num4 <= radius; num4++)
                                    {
                                        int num5 = j + num4;
                                        int num6 = k + m;
                                        int num7 = l + n;
                                        if (num6 >= 0 && num6 < length && num7 >= 0 && num7 < length2 && num5 >= 0 && num5 < num)
                                        {
                                            short num8 = m_box[num5][num6, num7];
                                            if (num8 != -1)
                                            {
                                                num2 += num8;
                                                num3++;
                                            }
                                        }
                                    }
                                }
                            }
                            if (num3 != 0)
                            {
                                short[,] obj = array[j];
                                int num9 = k;
                                int num10 = l;
                                short num11 = (short)(num2 / num3);
                                obj[num9, num10] = num11;
                            }
                        }
                    }
                }
            }
            m_box = array;
        }

        public short[,] RealTimePNN(int frameIdx, int boxWidth, int boxHeight, int boxDepth, Vector3 boxOrg)
        {


            int frameHeight = m_usDataCollection.changeFrames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.changeFrames[0].GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = boxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            short[][,] boxValues = new short[m_usDataCollection.framesCount][,];
            for (int ik = zLowerBound; ik < zUpperBound; ik++)
            {
                boxValues[ik] = new short[boxHeight, boxWidth];
            }

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = m_usDataCollection.calibratedRects[frameIdx].v[j] - boxOrg;
                int num1 = Convert.ToInt32(a.Z / voxelDepth);
                if (num1 >= zLowerBound)
                {
                    flag2 = false;
                }
                if (num1 <= zUpperBound)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = m_usDataCollection.calibratedRects[frameIdx].v[1] - m_usDataCollection.calibratedRects[frameIdx].v[0];      //x
                Vector3 obj2 = m_usDataCollection.calibratedRects[frameIdx].v[3] - m_usDataCollection.calibratedRects[frameIdx].v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = m_usDataCollection.calibratedRects[frameIdx].v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - boxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound)
                        {
                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = m_usDataCollection.changeFrames[frameIdx][num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
            }

            short[,] dMatrix = new short[boxDepth, boxWidth];
            for (int k1 = 0; k1 < boxDepth; k1++)
            {
                for (int k2 = 0; k2 < boxWidth; k2++)
                {
                    dMatrix[k1, k2] = 0;
                }
            }
            for (int k = minNum16; k < maxNum16; k++)
            {

                //short[,] b = new short[480,640];
                short[,] bb = (short[,])boxValues[k].Clone();
                //b.CopyTo(vol[k],0);

                short[] c = this.GetMatrixMax(bb);

                //Console.WriteLine(c.Length);
                for (int m = 0; m < boxWidth; m++)
                {
                    dMatrix[k, m] = (short)c[m];
                }

            }


            return dMatrix;
        }

        // Use boxWidth and boxDepth as coronal plane
        public short[,] RealTimePNN_V1(Rect3 currentRects, byte[,] currentFrame, short[][,] boxValues)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            short[,] dMatrix = new short[mboxDepth, mboxWidth];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelWidth);
                int aY = Convert.ToInt32(a.Y / voxelHeight);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxWidth && aY <= mboxHeight)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //x
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxWidth && num15 >= 0 && num15 < mboxHeight)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxWidth; k2++)
                    {
                        dMatrix[k1, k2] = 0;
                    }
                }
                for (int k = minNum16; k < maxNum16; k++)
                {

                    //short[,] b = new short[480,640];
                    short[,] bb = (short[,])boxValues[k].Clone();
                    //b.CopyTo(vol[k],0);

                    short[] c = this.GetMatrixMax(bb);

                    //Console.WriteLine(c.Length);
                    for (int m = 0; m < mboxWidth; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        dMatrix[k, m] = (short)c[m];
                    }

                }
            }
            //else
            //{
            //    Thread.Sleep(26);
            //}


            return dMatrix;
        }


        public short[,] RealTimePNN_V3(int frameIdx, short[][,] boxValues)
        {


            int frameHeight = m_usDataCollection.changeFrames[0].GetLength(0);       //480
            int frameWidth = m_usDataCollection.changeFrames[0].GetLength(1);        //640
            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            short[,] dMatrix = new short[mboxDepth, mboxHeight];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = false;
            bool flag3 = false;
            //for (int j = 0; j < 4; j++)
            //{
            //    Vector3 a = currentRects.v[j] - mboxOrg;
            //    int aZ = Convert.ToInt32(a.Z / voxelDepth);
            //    int aX = Convert.ToInt32(a.X / voxelWidth);
            //    int aY = Convert.ToInt32(a.Y / voxelHeight);

            //    if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
            //    {
            //        flag2 = false;
            //    }
            //    if (aZ <= zUpperBound && aX <= mboxWidth && aY <= mboxHeight)
            //    {
            //        flag3 = false;
            //    }
            //}


            if (!(flag2 | flag3))
            {
                Vector3 obj = m_usDataCollection.calibratedRects[frameIdx].v[1] - m_usDataCollection.calibratedRects[frameIdx].v[0];      //x
                Vector3 obj2 = m_usDataCollection.calibratedRects[frameIdx].v[3] - m_usDataCollection.calibratedRects[frameIdx].v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = m_usDataCollection.calibratedRects[frameIdx].v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxWidth && num15 >= 0 && num15 < mboxHeight)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {
                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = m_usDataCollection.changeFrames[frameIdx][num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxHeight; k2++)
                    {
                        dMatrix[k1, k2] = 0;
                    }
                }
                for (int k = minNum16; k < maxNum16; k++)
                {

                    //short[,] b = new short[480,640];
                    short[,] bb = (short[,])boxValues[k].Clone();
                    //b.CopyTo(vol[k],0);

                    short[] c = this.GetMatrixMax_Column(bb);

                    //Console.WriteLine(c.Length);
                    for (int m = 0; m < mboxHeight; m++)
                    {
                        dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                    }

                }
            }


            return dMatrix;
        }

        // Use boxWidth and boxDepth as coronal plane
        public short[][,] RealTimePNN_V1(Rect3 currentRects, byte[,] currentFrame, short[][,] boxValues,out short[,] dMatrix)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            dMatrix = new short[mboxDepth, mboxWidth];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelWidth);
                int aY = Convert.ToInt32(a.Y / voxelHeight);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxWidth && aY <= mboxHeight)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //x
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxWidth && num15 >= 0 && num15 < mboxHeight)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxWidth; k2++)
                    {
                        dMatrix[k1, k2] = 0;
                    }
                }
                for (int k = minNum16; k < maxNum16; k++)
                {

                    //short[,] b = new short[480,640];
                    short[,] bb = (short[,])boxValues[k].Clone();
                    //b.CopyTo(vol[k],0);

                    short[] c = this.GetMatrixMax(bb);

                    //Console.WriteLine(c.Length);
                    for (int m = 0; m < mboxWidth; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        dMatrix[k, m] = (short)c[m];
                    }

                }
            }
            //else
            //{
            //    Thread.Sleep(26);
            //}


            return boxValues;
        }

        // Use boxWidth and boxDepth as coronal plane
        public byte[][,] RealTimePNN_V1(Rect3 currentRects, byte[,] currentFrame, byte[][,] boxValues, out byte[,] dMatrix)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            dMatrix = new byte[mboxDepth, mboxWidth];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelWidth);
                int aY = Convert.ToInt32(a.Y / voxelHeight);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxWidth && aY <= mboxHeight)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //x
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxWidth && num15 >= 0 && num15 < mboxHeight)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxWidth; k2++)
                    {
                        dMatrix[k1, k2] = 0;
                    }
                }
                for (int k = minNum16; k < maxNum16; k++)
                {

                    byte[,] bb = (byte[,])boxValues[k].Clone();

                    byte[] c = this.GetMatrixMax(bb);

                    for (int m = 0; m < mboxWidth; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        dMatrix[k, m] = c[m];
                    }

                }
            }


            return boxValues;
        }

        // Use boxWidth and boxDepth as coronal plane
        public byte[][,] RealTimePNN_V1_Sagittal(Rect3 currentRects, byte[,] currentFrame, byte[][,] boxValues, out byte[,] dMatrix)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            dMatrix = new byte[mboxDepth, mboxHeight];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelWidth);
                int aY = Convert.ToInt32(a.Y / voxelHeight);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxWidth && aY <= mboxHeight)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //x
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxWidth && num15 >= 0 && num15 < mboxHeight)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxHeight; k2++)
                    {
                        dMatrix[k1, k2] = 0;
                    }
                }
                for (int k = minNum16; k < maxNum16; k++)
                {

                    byte[,] bb = (byte[,])boxValues[k].Clone();

                    byte[] c = this.GetMatrixMaxRow(bb);

                    for (int m = 0; m < mboxHeight; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        dMatrix[k, m] = c[m];
                    }

                }
            }


            return boxValues;
        }

        // Use boxWidth and boxDepth as coronal and sagittalplane
        public byte[][,] RealTimePNN_Coronal_Sagittal(Rect3 currentRects, byte[,] currentFrame, byte[][,] boxValues, out byte[,] sagittalImage, out byte[,] coronalImage)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            sagittalImage = new byte[mboxDepth, mboxHeight];
            coronalImage = new byte[mboxDepth, mboxWidth];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelWidth);
                int aY = Convert.ToInt32(a.Y / voxelHeight);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxWidth && aY <= mboxHeight)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //x
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //y

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fx = obj / frameWidth;
                Vector3 fy = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fx * num12 + fy * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelWidth);
                        int num15 = Convert.ToInt32(a.Y / voxelHeight);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxWidth && num15 >= 0 && num15 < mboxHeight)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num15, num14])
                            {
                                boxValues[num16][num15, num14] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxHeight; k2++)
                    {
                        sagittalImage[k1, k2] = 0;
                    }
                }

                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxWidth; k2++)
                    {
                        coronalImage[k1, k2] = 0;
                    }
                }

                for (int k = minNum16; k < maxNum16; k++)
                {

                    byte[,] bb = (byte[,])boxValues[k].Clone();

                    byte[] sag = this.GetMatrixMaxRow(bb);

                    byte[] cor = this.GetMatrixMax(bb);


                    for (int m = 0; m < mboxHeight; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        sagittalImage[k, m] = sag[m];
                    }

                    for (int m = 0; m < mboxWidth; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        coronalImage[k, m] = cor[m];
                    }
                }
            }


            return boxValues;
        }

        // Use boxWidth and boxDepth as coronal and sagittalplane
        public byte[][,] RealTimePNN_Coronal_Sagittal_WyHx(Rect3 currentRects, byte[,] currentFrame, byte[][,] boxValues, out byte[,] sagittalImage, out byte[,] coronalImage)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            sagittalImage = new byte[mboxDepth, mboxHeight];
            coronalImage = new byte[mboxDepth, mboxWidth];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelHeight);
                int aY = Convert.ToInt32(a.Y / voxelWidth);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxHeight && aY <=  mboxWidth)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //y
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //x

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fy = obj / frameWidth;
                Vector3 fx = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fy * num12 + fx * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelHeight);
                        int num15 = Convert.ToInt32(a.Y / voxelWidth);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxHeight && num15 >= 0 && num15 < mboxWidth)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            byte b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num14,num15])
                            {
                                boxValues[num16][num14,num15] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxHeight; k2++)
                    {
                        sagittalImage[k1, k2] = 0;
                    }
                }

                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxWidth; k2++)
                    {
                        coronalImage[k1, k2] = 0;
                    }
                }

                for (int k = minNum16; k < maxNum16; k++)
                {

                    byte[,] bb = (byte[,])boxValues[k].Clone();

                    byte[] sag = this.GetMatrixMaxRow(bb);

                    byte[] cor = this.GetMatrixMax(bb);


                    for (int m = 0; m < mboxHeight; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        sagittalImage[k, m] = sag[m];
                    }

                    for (int m = 0; m < mboxWidth; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        coronalImage[k, m] = cor[m];
                    }
                }
            }


            return boxValues;
        }

        public short[][,] RealTimePNN_Coronal_Sagittal_WyHx(Rect3 currentRects, byte[,] currentFrame, short[][,] boxValues, out byte[,] sagittalImage, out byte[,] coronalImage)
        {


            int frameHeight = currentFrame.GetLength(0);       //480
            int frameWidth = currentFrame.GetLength(1);        //640

            //Vector3 boxOrg = new Vector3();
            //CalculateBoxSize(out boxOrg, out int boxWidth, out int boxHeight, out int boxDepth);
            int zLowerBound = 0;                            //Lower Bound of z axis
            int zUpperBound = mboxDepth;                     //Upper Bound of z axis

            //Console.WriteLine("Box Size = {0} x {1} x {2} mm^3", boxWidth, boxHeight, boxDepth);
            //Console.WriteLine("A high computation is proccesing...Please waiting...");

            //short[][,] boxValues = new short[mboxDepth][,];
            //for (int ik = zLowerBound; ik < zUpperBound; ik++)
            //{
            //    boxValues[ik] = new short[mboxHeight, mboxWidth];
            //}
            sagittalImage = new byte[mboxDepth, mboxHeight];
            coronalImage = new byte[mboxDepth, mboxWidth];

            int maxNum16 = -65535;
            int minNum16 = 65535;

            bool flag2 = true;
            bool flag3 = true;
            for (int j = 0; j < 4; j++)
            {
                Vector3 a = currentRects.v[j] - mboxOrg;
                int aZ = Convert.ToInt32(a.Z / voxelDepth);
                int aX = Convert.ToInt32(a.X / voxelHeight);
                int aY = Convert.ToInt32(a.Y / voxelWidth);

                if (aZ >= zLowerBound && aX >= 0 && aY >= 0)
                {
                    flag2 = false;
                }
                if (aZ <= zUpperBound && aX <= mboxHeight && aY <= mboxWidth)
                {
                    flag3 = false;
                }
            }


            if (!(flag2 | flag3))
            {
                Vector3 obj = currentRects.v[1] - currentRects.v[0];      //y
                Vector3 obj2 = currentRects.v[3] - currentRects.v[0];     //x

                //只计算一个顶点的值，其他的根据公式推出即可
                Vector3 frameOrg = currentRects.v[0];
                Vector3 fy = obj / frameWidth;
                Vector3 fx = obj2 / frameHeight;
                Vector3 cons = fx * 0 + fy * 0 + frameOrg - mboxOrg;

                maxNum16 = -65535;
                minNum16 = 65535;
                for (int num12 = 0; num12 < frameWidth; num12++)
                {

                    for (int num13 = 0; num13 < frameHeight; num13++)
                    {
                        Vector3 a = (fy * num12 + fx * num13 + cons);
                        int num14 = Convert.ToInt32(a.X / voxelHeight);
                        int num15 = Convert.ToInt32(a.Y / voxelWidth);
                        int num16 = Convert.ToInt32(a.Z / voxelDepth);

                        if (num16 >= zLowerBound && num16 < zUpperBound && num14 >= 0 && num14 < mboxHeight && num15 >= 0 && num15 < mboxWidth)
                        //if (num16 >= zLowerBound && num16 < zUpperBound)
                        {

                            if (num16 > maxNum16)
                            {
                                maxNum16 = num16;
                            }
                            if (num16 < minNum16)

                            {
                                minNum16 = num16;
                            }
                            short b = currentFrame[num13, num12];
                            if (b > boxValues[num16][num14, num15])
                            {
                                boxValues[num16][num14, num15] = b;
                            }

                        }
                    }
                }
                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxHeight; k2++)
                    {
                        sagittalImage[k1, k2] = 0;
                    }
                }

                for (int k1 = 0; k1 < mboxDepth; k1++)
                {
                    for (int k2 = 0; k2 < mboxWidth; k2++)
                    {
                        coronalImage[k1, k2] = 0;
                    }
                }

                for (int k = minNum16; k < maxNum16; k++)
                {
                    //-------//
                    short[,] tempTransverse = (short[,])boxValues[k].Clone();  // Error cannot transform the short array to byte
                    byte[,] transverseFrame = ConvertFile.ShortArrayToByte(tempTransverse);
                    byte[] sag = this.GetMatrixMaxRow(transverseFrame);

                    byte[] cor = this.GetMatrixMax(transverseFrame);


                    for (int m = 0; m < mboxHeight; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        sagittalImage[k, m] = sag[m];
                    }

                    for (int m = 0; m < mboxWidth; m++)
                    {
                        //dMatrix[mboxDepth - 1 - k, m] = (short)c[m];
                        coronalImage[k, m] = cor[m];
                    }


                }
            }


            return boxValues;
        }

        /// <summary>
        /// Calculate the Box size according all the coordiante of frames
        /// </summary>
        /// <param name="boxOrg"></param>
        /// <param name="boxWidth"></param>
        /// <param name="boxHeight"></param>
        /// <param name="boxDepth"></param>
        public void CalculateBoxSize(out Vector3 boxOrg, out int boxWidth, out int boxHeight, out int boxDepth)
        {
            Vector3 minVector = m_usDataCollection.calibratedRects[0].v[0];
            Vector3 maxVector = m_usDataCollection.calibratedRects[0].v[0];

            //获取所有向量坐标最大的xyz 和 最小的xyz
            for (int i = 0; i < m_usDataCollection.calibratedRects.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 tempVector = m_usDataCollection.calibratedRects[i].v[j];
                    minVector = Vector3.Min(tempVector, minVector);
                    maxVector = Vector3.Max(tempVector, maxVector);
                }
            }

            boxOrg = minVector;
            boxWidth = 2 + (int)Math.Round(Math.Abs(minVector.X - maxVector.X) / voxelWidth);
            boxHeight = 2 + (int)Math.Round(Math.Abs(minVector.Y - maxVector.Y) / voxelHeight);
            boxDepth = 2 + (int)Math.Round(Math.Abs(minVector.Z - maxVector.Z) / voxelDepth);

            mboxDepth = boxDepth;
            mboxHeight = boxHeight;
            mboxWidth = boxWidth;
            mboxOrg = boxOrg;

        }

        /// <summary>
        /// Calculate the Box size according all the coordiante of frames Width--y; Height--x
        /// </summary>
        /// <param name="boxOrg"></param>
        /// <param name="boxWidth"></param>
        /// <param name="boxHeight"></param>
        /// <param name="boxDepth"></param>
        public void CalculateBoxSizeWyHx(out Vector3 boxOrg, out int boxWidth, out int boxHeight, out int boxDepth)
        {
            Vector3 minVector = m_usDataCollection.calibratedRects[0].v[0];
            Vector3 maxVector = m_usDataCollection.calibratedRects[0].v[0];

            //获取所有向量坐标最大的xyz 和 最小的xyz
            for (int i = 0; i < m_usDataCollection.calibratedRects.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 tempVector = m_usDataCollection.calibratedRects[i].v[j];
                    minVector = Vector3.Min(tempVector, minVector);
                    maxVector = Vector3.Max(tempVector, maxVector);
                }
            }

            boxOrg = minVector;
            boxWidth = 2 + (int)Math.Round(Math.Abs(minVector.Y - maxVector.Y) / voxelWidth);
            boxHeight = 2 + (int)Math.Round(Math.Abs(minVector.X - maxVector.X) / voxelHeight);
            boxDepth = 2 + (int)Math.Round(Math.Abs(minVector.Z - maxVector.Z) / voxelDepth);

            mboxDepth = boxDepth;
            mboxHeight = boxHeight;
            mboxWidth = boxWidth;
            mboxOrg = boxOrg;
        }
        /// <summary>
        /// 将C#数组保存到Matlab.mat
        /// </summary>
        /// <param name="boxValues"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        public void SaveVolumeToMat(short[][,] boxValues, int width, int height, int depth)
        {
            //256*300 w*h
            int[] dims = new int[] { height, width, depth };
            MLInt16 volume = new MLInt16("usData", dims);

            //设置矩阵元素
            //[;;0]
            volume.Set(1, 0, 0);

            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        short values = boxValues[i][j, k];

                        volume.Set((short)values, j, k + i * width);  //Set(value,row_ind,col_index+i*n)
                    }
                }
            }

            List<MLArray> mlList = new List<MLArray>();
            mlList.Add(volume);
            //string path = @"D:\us29\new.mat";
            MatFileWriter mfw = new MatFileWriter(this.savePath, mlList, false);
        }

        /// <summary>
        /// 将C#数组保存到Matlab.mat
        /// </summary>
        /// <param name="boxValues"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        public void SaveVolumeToMat(short[][,] boxValues, int width, int height, int depth,string fileName)
        {
            //256*300 w*h
            int[] dims = new int[] { height, width, depth };
            MLInt16 volume = new MLInt16("usData", dims);

            //设置矩阵元素
            //[;;0]
            volume.Set(1, 0, 0);

            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        short values = boxValues[i][j, k];

                        volume.Set((short)values, j, k + i * width);  //Set(value,row_ind,col_index+i*n)
                    }
                }
            }

            List<MLArray> mlList = new List<MLArray>();
            mlList.Add(volume);
            //string path = @"D:\us29\new.mat";
            MatFileWriter mfw = new MatFileWriter(fileName, mlList, false);
        }

        /// <summary>
        /// 将C#数组保存到Matlab.mat
        /// </summary>
        /// <param name="boxValues"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        public void SaveVolumeToMat(byte[][,] boxValues, int width, int height, int depth, string fileName)
        {
            //256*300 w*h
            int[] dims = new int[] { height, width, depth };
            MLInt16 volume = new MLInt16("usData", dims);

            //设置矩阵元素
            //[;;0]
            volume.Set(1, 0, 0);

            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        byte values = boxValues[i][j, k];

                        volume.Set(values, j, k + i * width);  //Set(value,row_ind,col_index+i*n)
                    }
                }
            }

            List<MLArray> mlList = new List<MLArray>();
            mlList.Add(volume);
            //string path = @"D:\us29\new.mat";
            MatFileWriter mfw = new MatFileWriter(fileName, mlList, false);
        }

        public void SaveUshortToMat(short[,] boxValues, string myPath)
        {
            int height = boxValues.GetLength(0);
            int width = boxValues.GetLength(1);

            int[] dims = new int[] { height, width };
            MLInt16 volume = new MLInt16("usData", dims);

            //设置矩阵元素
            //[;;0]
            volume.Set(1, 0, 0);


            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    short values = boxValues[j, k];

                    volume.Set(values, j, k);
                }
            }

            //MLArray mmm = (/*MLArray*/)mwArr;
            List<MLArray> mlList = new List<MLArray>();
            mlList.Add(volume);
            MatFileWriter mfw = new MatFileWriter(myPath, mlList, false);
        }

        /// <summary>
        /// 将volume投影到冠状面
        /// </summary>
        /// <param name="vol"></param>
        /// <param name="depth"></param>
        /// <param name="width"></param>
        public short[,] CoronalProjection(short[][,] vol, int depth, int width)
        {
            short[,] dMatrix = new short[depth, width];
            for (int k1 = 0; k1 < depth; k1++)
            {
                for (int k2 = 0; k2 < width; k2++)
                {
                    dMatrix[k1, k2] = 0;
                }
            }
            for (int k = 0; k < depth; k++)
            {

                short[,] bb = (short[,])vol[k].Clone();

                short[] c = this.GetMatrixMax(bb);

                for (int m = 0; m < width; m++)
                {
                    dMatrix[k, m] = (short)c[width - 1 - m];
                }

            }

            ////调用Matlab imagesc 进行显示
            //RT realTime = new RT();

            //MWNumericArray mwArr = dMatrix;
            //realTime.RealTime(mwArr);

            return dMatrix;


        }

        /// <summary>
        /// 将volume投影到冠状面
        /// </summary>
        /// <param name="vol"></param>
        /// <param name="depth"></param>
        /// <param name="width"></param>
        public short[,] CoronalProjection(short[][,] vol)
        {
            int depth = vol.Length;

            int width = vol[0].GetLength(1);
            short[,] dMatrix = new short[depth, width];
            for (int k1 = 0; k1 < depth; k1++)
            {
                for (int k2 = 0; k2 < width; k2++)
                {
                    dMatrix[k1, k2] = 0;
                }
            }
            for (int k = 0; k < depth; k++)
            {

                short[,] bb = (short[,])vol[k].Clone();

                short[] c = this.GetMatrixMax(bb);

                for (int m = 0; m < width; m++)
                {
                    dMatrix[k, m] = (short)c[width - 1 - m];
                }

            }
            return dMatrix;

        }

        public short[] GetMatrixMax_Column(short[,] matrix)
        {
            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            short[] colVector = new short[matrixHeight];



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
                //找到一hang的最大值，放入行向量
                colVector[i] = max;
            }
            return colVector;
        }

        /// <summary>
        /// 沿列返回二维数组的最大元素
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
        /// 沿列返回二维数组的最大元素
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public byte[] GetMatrixMax(byte[,] matrix)
        {
            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            byte[] rowVector = new byte[matrixWidth];



            for (int i = 0; i < matrixWidth; i++)
            {
                byte max = 0;
                for (int j = 0; j < matrixHeight; j++)
                {
                    byte v = matrix[j, i];
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
        /// 沿行返回二维数组的最大元素
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public byte[] GetMatrixMaxRow(byte[,] matrix)
        {
            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            byte[] rowVector = new byte[matrixHeight];

            for (int i = 0; i < matrixHeight; i++)
            {
                byte max = 0;
                for (int j = 0; j < matrixWidth; j++)
                {
                    byte v = matrix[i, j];
                    if (v > max)
                    {
                        max = v;
                    }
                }
                //找到一行的最大值，放入行向量
                rowVector[i] = max;
            }
            return rowVector;
        }

        /// <summary>
        /// 返回两个矩阵对应元素的最大值
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public short[,] GetMatrixsMax(short[,] matrixA, short[,] matrixB)
        {

            int matrixHeight = matrixA.GetLength(0);
            int matrixWidth = matrixA.GetLength(1);
            short[,] maxMatrix = new short[matrixHeight, matrixWidth];

            for (int i = 0; i < matrixHeight; i++)
            {
                for (int j = 0; j < matrixWidth; j++)
                {
                    maxMatrix[i, j] = (short)Math.Max(matrixA[i, j], matrixB[i, j]);
                }
            }

            return maxMatrix;
        }
        /// <summary>
        /// 返回两个矩阵对应元素的最大值
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        public byte[,] GetMatrixsMax(byte[,] matrixA, byte[,] matrixB)
        {

            int matrixHeight = matrixA.GetLength(0);
            int matrixWidth = matrixA.GetLength(1);
            byte[,] maxMatrix = new byte[matrixHeight, matrixWidth];

            for (int i = 0; i < matrixHeight; i++)
            {
                for (int j = 0; j < matrixWidth; j++)
                {
                    maxMatrix[i, j] = Math.Max(matrixA[i, j], matrixB[i, j]);
                }
            }

            return maxMatrix;
        }


        public void VNN_util_findMinMaxY(Vector3 m_boxOrg, int WIDTH, int HEIGHT, int DEPTH, out int[,] runLengthMinY, out int[,] runLengthMaxY)
        {
            runLengthMinY = new int[DEPTH, WIDTH];
            runLengthMaxY = new int[DEPTH, WIDTH];
            for (int i = 0; i < DEPTH; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    runLengthMinY[i, j] = 2147483647;
                    runLengthMaxY[i, j] = -2147483648;
                }
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                Vector3 sVector3Df = new Vector3(3.40282e+38f, 3.40282e+38f, 3.40282e+38f);
                Vector3 sVector3Df2 = new Vector3(-3.40282e+38f, -3.40282e+38f, -3.40282e+38f);
                for (int n2 = 0; n2 < 4; n2++)
                {
                    sVector3Df = Vector3.Min(sVector3Df, m_usDataCollection.calibratedRects[k].v[n2]);    //get the minimum
                    sVector3Df2 = Vector3.Max(sVector3Df2, m_usDataCollection.calibratedRects[k].v[n2]);  //get the maximum
                }

                sVector3Df -= m_boxOrg;
                sVector3Df2 -= m_boxOrg;

                sVector3Df = new Vector3(sVector3Df.X / voxelWidth, sVector3Df.Y / voxelHeight, sVector3Df.Z / voxelDepth);
                sVector3Df2 = new Vector3(sVector3Df2.X / voxelWidth, sVector3Df2.Y / voxelHeight, sVector3Df2.Z / voxelDepth);

                Vector3 sVector3DfRound = new Vector3((float)Math.Round(sVector3Df.X), (float)Math.Round(sVector3Df.Y), (float)Math.Round(sVector3Df.Z));
                Vector3 sVector3Df2Round = new Vector3((float)Math.Round(sVector3Df2.X), (float)Math.Round(sVector3Df2.Y), (float)Math.Round(sVector3Df2.Z));

                Vector3 sVector3Di = Vector3.Max(new Vector3(0f, 0f, 0f), sVector3DfRound);
                Vector3 sVector3Di2 = Vector3.Min(new Vector3((float)WIDTH, (float)HEIGHT, (float)DEPTH), sVector3Df2Round);


                for (int n3 = (int)sVector3Di.Z; n3 < (int)sVector3Di2.Z; n3++)
                {
                    for (int n4 = (int)sVector3Di.X; n4 < (int)sVector3Di2.X; n4++)
                    {
                        if (runLengthMinY[n3, n4] > sVector3Di.Y)
                        {
                            //int[,] obj = runLengthMinY;
                            //int num = l;
                            //int num2 = m;
                            //int y = (int)sVector3Di.Y;
                            //obj[num, num2] = y;
                            runLengthMinY[n3, n4] = (int)sVector3Di.Y;
                        }
                        if (runLengthMaxY[n3, n4] < sVector3Di2.Y)
                        {
                            //int[,] obj2 = runLengthMaxY;
                            //int num3 = l;
                            //int num4 = m;
                            //int y2 = (int)sVector3Di2.Y;
                            //obj2[num3, num4] = y2;
                            runLengthMaxY[n3, n4] = (int)sVector3Di2.Y;
                        }
                    }
                }
            }
            int num5 = 8;
            int[,] array = (int[,])runLengthMinY.Clone();
            int[,] array2 = (int[,])runLengthMaxY.Clone();
            for (int n = 0; n < DEPTH; n++)
            {
                for (int num6 = 0; num6 < WIDTH; num6++)
                {
                    if (runLengthMinY[n, num6] == 2147483647)
                    {
                        int num7 = 2147483647;
                        int num8 = -2147483648;
                        for (int num9 = -num5; num9 <= num5; num9++)
                        {
                            for (int num10 = -num5; num10 <= num5; num10++)
                            {
                                int num11 = num6 + num9;
                                int num12 = n + num10;
                                if (num11 >= 0 && num11 < WIDTH && num12 >= 0 && num12 < DEPTH)
                                {
                                    int num13 = array[num12, num11];
                                    int num14 = array2[num12, num11];
                                    if (num7 > num13)
                                    {
                                        num7 = num13;
                                    }
                                    if (num8 < num14)
                                    {
                                        num8 = num14;
                                    }
                                }
                            }
                        }
                        if (num7 == 2147483647)
                        {
                            runLengthMinY[n, num6] = 0;
                            runLengthMaxY[n, num6] = 0;
                        }
                        else
                        {
                            for (int num15 = -num5; num15 <= num5; num15++)
                            {
                                for (int num16 = -num5; num16 <= num5; num16++)
                                {
                                    int num17 = num6 + num15;
                                    int num18 = n + num16;
                                    if (num17 >= 0 && num17 < WIDTH && num18 >= 0 && num18 < DEPTH && runLengthMinY[num18, num17] == 2147483647)
                                    {
                                        runLengthMinY[num18, num17] = num7;
                                        runLengthMaxY[num18, num17] = num8;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void VNN_util_findMinMaxX(Vector3 m_boxOrg, int WIDTH, int HEIGHT, int DEPTH, out int[,] runLengthMinX, out int[,] runLengthMaxX)
        {
            runLengthMinX = new int[DEPTH, WIDTH];
            runLengthMaxX = new int[DEPTH, WIDTH];
            for (int i = 0; i < DEPTH; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    runLengthMinX[i, j] = 2147483647;
                    runLengthMaxX[i, j] = -2147483648;
                }
            }

            for (int k = 0; k < m_usDataCollection.framesCount; k++)
            {
                Vector3 sVector3Df = new Vector3(3.40282e+38f, 3.40282e+38f, 3.40282e+38f);
                Vector3 sVector3Df2 = new Vector3(-3.40282e+38f, -3.40282e+38f, -3.40282e+38f);
                for (int n2 = 0; n2 < 4; n2++)
                {
                    sVector3Df = Vector3.Min(sVector3Df, m_usDataCollection.calibratedRects[k].v[n2]);    //get the minimum
                    sVector3Df2 = Vector3.Max(sVector3Df2, m_usDataCollection.calibratedRects[k].v[n2]);  //get the maximum
                }

                sVector3Df -= m_boxOrg;
                sVector3Df2 -= m_boxOrg;

                sVector3Df = new Vector3(sVector3Df.X / voxelHeight, sVector3Df.Y / voxelWidth, sVector3Df.Z / voxelDepth);
                sVector3Df2 = new Vector3(sVector3Df2.X / voxelHeight, sVector3Df2.Y / voxelWidth, sVector3Df2.Z / voxelDepth);

                Vector3 sVector3DfRound = new Vector3((float)Math.Round(sVector3Df.X), (float)Math.Round(sVector3Df.Y), (float)Math.Round(sVector3Df.Z));
                Vector3 sVector3Df2Round = new Vector3((float)Math.Round(sVector3Df2.X), (float)Math.Round(sVector3Df2.Y), (float)Math.Round(sVector3Df2.Z));

                Vector3 sVector3Di = Vector3.Max(new Vector3(0f, 0f, 0f), sVector3DfRound);
                Vector3 sVector3Di2 = Vector3.Min(new Vector3((float)HEIGHT, (float)WIDTH, (float)DEPTH), sVector3Df2Round);


                for (int n3 = (int)sVector3Di.Z; n3 < (int)sVector3Di2.Z; n3++)
                {
                    for (int n4 = (int)sVector3Di.Y; n4 < (int)sVector3Di2.Y; n4++)
                    {
                        if (runLengthMinX[n3, n4] > sVector3Di.X)
                        {
                            //int[,] obj = runLengthMinY;
                            //int num = l;
                            //int num2 = m;
                            //int y = (int)sVector3Di.Y;
                            //obj[num, num2] = y;
                            runLengthMinX[n3, n4] = (int)sVector3Di.X;
                        }
                        if (runLengthMaxX[n3, n4] < sVector3Di2.X)
                        {
                            //int[,] obj2 = runLengthMaxY;
                            //int num3 = l;
                            //int num4 = m;
                            //int y2 = (int)sVector3Di2.Y;
                            //obj2[num3, num4] = y2;
                            runLengthMaxX[n3, n4] = (int)sVector3Di2.X;
                        }
                    }
                }
            }
            int num5 = 8;
            int[,] array = (int[,])runLengthMinX.Clone();
            int[,] array2 = (int[,])runLengthMaxX.Clone();
            for (int n = 0; n < DEPTH; n++)
            {
                for (int num6 = 0; num6 < WIDTH; num6++)
                {
                    if (runLengthMinX[n, num6] == 2147483647)
                    {
                        int num7 = 2147483647;
                        int num8 = -2147483648;
                        for (int num9 = -num5; num9 <= num5; num9++)
                        {
                            for (int num10 = -num5; num10 <= num5; num10++)
                            {
                                int num11 = num6 + num9;
                                int num12 = n + num10;
                                if (num11 >= 0 && num11 < WIDTH && num12 >= 0 && num12 < DEPTH)
                                {
                                    int num13 = array[num12, num11];
                                    int num14 = array2[num12, num11];
                                    if (num7 > num13)
                                    {
                                        num7 = num13;
                                    }
                                    if (num8 < num14)
                                    {
                                        num8 = num14;
                                    }
                                }
                            }
                        }
                        if (num7 == 2147483647)
                        {
                            runLengthMinX[n, num6] = 0;
                            runLengthMaxX[n, num6] = 0;
                        }
                        else
                        {
                            for (int num15 = -num5; num15 <= num5; num15++)
                            {
                                for (int num16 = -num5; num16 <= num5; num16++)
                                {
                                    int num17 = num6 + num15;
                                    int num18 = n + num16;
                                    if (num17 >= 0 && num17 < WIDTH && num18 >= 0 && num18 < DEPTH && runLengthMinX[num18, num17] == 2147483647)
                                    {
                                        runLengthMinX[num18, num17] = num7;
                                        runLengthMaxX[num18, num17] = num8;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
