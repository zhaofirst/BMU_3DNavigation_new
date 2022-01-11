using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portable_BMU_App
{
    class VolumeProjection
    {
        public static short[,] SaggitalProjection(short[][,] vol)
        {
            int depth = vol.Length;

            int width = vol[0].GetLength(1);
            int height = vol[0].GetLength(0);

            short[,] dMatrix = new short[height, depth];
            for (int k1 = 0; k1 < height; k1++)
            {
                for (int k2 = 0; k2 < depth; k2++)
                {
                    dMatrix[k1, k2] = 0;
                }
            }
            for (int k = 0; k < depth; k++)
            {

                short[,] bb = (short[,])vol[k].Clone();

                short[] c = GetMatrixMax2(bb);

                for (int m = 0; m < height; m++)
                {
                    //dMatrix[m, k] = c[height - 1 - m];
                    dMatrix[m, k] = c[m];

                }

            }
            return dMatrix;

        }

        /// <summary>
        /// 沿列返回二维数组的最大元素
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static short[] GetMatrixMax(short[,] matrix)
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


        public static short[] GetMatrixMax2(short[,] matrix)
        {
            int matrixHeight = matrix.GetLength(1);
            int matrixWidth = matrix.GetLength(0);

            short[] rowVector = new short[matrixWidth];



            for (int i = 0; i < matrixWidth; i++)
            {
                short max = 0;
                for (int j = 0; j < matrixHeight; j++)
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
        /// 将volume投影到冠状面
        /// </summary>
        /// <param name="vol"></param>
        /// <param name="depth"></param>
        /// <param name="width"></param>
        public static short[,] CoronalProjection(short[][,] vol)
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

                short[] c = GetMatrixMax(bb);

                for (int m = 0; m < width; m++)
                {
                    dMatrix[k, m] = c[width - 1 - m];
                    //dMatrix[k, m] = c[m];

                }

            }
            return dMatrix;

        }


        /// <summary>
        /// 将volume投影到冠状面
        /// </summary>
        /// <param name="vol"></param>
        /// <param name="depth"></param>
        /// <param name="width"></param>
        public static short[,] CoronalProjection2(short[][,] vol)
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

                short[] c = GetMatrixMax(bb);

                for (int m = 0; m < width; m++)
                {
                    //dMatrix[k, m] = c[width - 1 - m];
                    dMatrix[k, m] = c[m];

                }

            }
            return dMatrix;

        }
    }
}
