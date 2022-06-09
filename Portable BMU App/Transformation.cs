using System;
using System.Numerics;

namespace Ultrasonics3DReconstructor
{
    public class Transformation
    {
        public Vector3 gpsCalibrationTop = new Vector3(0f,0f,0f);

        public Vector3 gpsCalibrationBottom = new Vector3(0f, 0f, 0f);

        public Rect3[] rects;

        //Voxel Size
        public float voxelWidth = 0.5f;
        public float voxelHeight = 0.5f;
        public float voxelDepth = 1f;


        /// <summary>
        /// Transform the Rects into a new Coordinate
        /// </summary>
        public void CoordinateTransformation()
        {
            Console.WriteLine("[+]Align the whole model");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorY = new Vector3(0f, 1f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[0 1 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[0] - this.rects[i].v[3];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorY)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Angle{{upVector(0,1,0)}} = {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorY);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

                //对m_gpsCalibrationTop/Bottom进行变换
                this.gpsCalibrationTop = Vector3.Transform(this.gpsCalibrationTop, transMatrix);
                this.gpsCalibrationBottom = Vector3.Transform(this.gpsCalibrationBottom, transMatrix);
            }
            #endregion

            #region 根据向量[0 0 1]进行变换
            Vector3 vectorZ = new Vector3(0f, 0f, 1f);
            Vector3 vectors2 = new Vector3(0f, 0f, 0f);
            for (int i = 1; i < this.rects.Length; i++)
            {
                vectors2 += this.rects[i].v[0] - this.rects[0].v[0];
            }
            vectors2.Y = 0f;                            //令y=0， 也即让该向量投影到XZ平面。
            vectors2 = Vector3.Normalize(vectors2);

            degreeNum = (float)(Math.Acos(Vector3.Dot(vectors2, vectorZ)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Angle{{upVector(0,0,1)}} = {0} degree", degreeNum);
            if (degreeNum > 2f && vectors2.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors2, vectorZ);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[0];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

                //对m_gpsCalibrationTop/Bottom进行变换
                this.gpsCalibrationTop = Vector3.Transform(this.gpsCalibrationTop, transMatrix);
                this.gpsCalibrationBottom = Vector3.Transform(this.gpsCalibrationBottom, transMatrix);
            }

            #endregion

            #region 根据向量[0 0 -1]进行变换
            Vector3 vectorMinusZ = new Vector3(0f, 0f, -1f);

            if (this.gpsCalibrationBottom != this.gpsCalibrationTop)
            {
                Vector3 vectorTB = this.gpsCalibrationTop - this.gpsCalibrationBottom;
                vectorTB.Y = 0;
                vectorTB = Vector3.Normalize(vectorTB);
                Matrix4x4 transMatrix = this.BuildRotateFtomTO(vectorTB, vectorMinusZ);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

                //对m_gpsCalibrationTop/Bottom进行变换
                this.gpsCalibrationTop = Vector3.Transform(this.gpsCalibrationTop, transMatrix);
                this.gpsCalibrationBottom = Vector3.Transform(this.gpsCalibrationBottom, transMatrix);
            }

            #endregion

            this.DoCentroidAllCalibratedRectangles();
            //this.InverseGPS_X();
        }

        /// <summary>
        /// Transform the Rects into a new Coordinate; For G4/Clarius, Width-->x axis, Height-->y axis
        /// </summary>
        public void CoordinateTransformationForPortableUsingMIAS()
        {
            Console.WriteLine(">> Transformation... from transmitter space to observation space");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorY = new Vector3(1f, 0f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[0 1 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[3] - this.rects[i].v[0];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorY)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Align to +X direction, {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorY);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

            }
            #endregion

            #region 根据向量[0 0 1]进行变换
            Vector3 vectorZ = new Vector3(0f, 0f, 1f);
            Vector3 vectors2 = new Vector3(0f, 0f, 0f);
            for (int i = 1; i < this.rects.Length; i++)
            {
                vectors2 += this.rects[i].v[0] - this.rects[0].v[0];
            }
            vectors2.X = 0f;                            //令x=0， 也即让该向量投影到XZ平面。
            vectors2 = Vector3.Normalize(vectors2);

            degreeNum = (float)(Math.Acos(Vector3.Dot(vectors2, vectorZ)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Align to +Z direction, {0} degree", degreeNum);
            if (degreeNum > 2f && vectors2.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors2, vectorZ);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[0];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }
            }

            #endregion


            //this.DoCentroidAllCalibratedRectangles();
            //this.InverseGPS_X();
        }

        /// <summary>
        /// Transform the Rects into a new Coordinate; For G4/Clarius, Width-->x axis, Height-->y axis
        /// </summary>
        public void CoordinateTransformationForPortableUsingMIASWithPlumbLine()
        {
            Console.WriteLine(">> Transformation... from transmitter space to observation space");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorY = new Vector3(1f, 0f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[0 1 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[3] - this.rects[i].v[0];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorY)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Align to +X direction, {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorY);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

            }
            #endregion


            //this.DoCentroidAllCalibratedRectangles();
            //this.InverseGPS_X();
        }

        /// <summary>
        /// Transform the Rects into a new Coordinate; For G4/Clarius, Width-->x axis, Height-->y axis
        /// </summary>
        public void CoordinateTransformationForPortableWyHx()
        {
            Console.WriteLine("[+]Align the whole model");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorX = new Vector3(1f, 0f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[1 0 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[3] - this.rects[i].v[0];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorX)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Angle{{upVector(0,1,0)}} = {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorX);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

            }
            #endregion

            #region 根据向量[0 0 1]进行变换
            Vector3 vectorZ = new Vector3(0f, 0f, 1f);
            Vector3 vectors2 = new Vector3(0f, 0f, 0f);
            for (int i = 1; i < this.rects.Length; i++)
            {
                vectors2 += this.rects[i].v[0] - this.rects[0].v[0];
            }
            vectors2.X = 0f;                            //令y=0， 也即让该向量投影到XZ平面。
            vectors2 = Vector3.Normalize(vectors2);

            degreeNum = (float)(Math.Acos(Vector3.Dot(vectors2, vectorZ)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Angle{{upVector(0,0,1)}} = {0} degree", degreeNum);
            if (degreeNum > 2f && vectors2.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors2, vectorZ);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[0];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

            }

            #endregion   


            this.DoCentroidAllCalibratedRectangles();
            //this.InverseGPS_X();
        }

        /// <summary>
        /// Transform the Rects into a new Coordinate, Please call this function first and then call CalculateBoxSize
        /// Width --> x axis Height --> y axis
        /// </summary>
        public Matrix4x4 GetCalibrationMatrix()
        {
            Matrix4x4 calibrationMatrix = new Matrix4x4();
            calibrationMatrix = Matrix4x4.Identity;

            Console.WriteLine("[+]Align the whole model");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorY = new Vector3(0f, 1f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[1 0 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[3] - this.rects[i].v[0];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorY)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Angle{{upVector(0,1,0)}} = {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorY);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);
                calibrationMatrix = calibrationMatrix * transMatrix;

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

            }
            #endregion

            #region 根据向量[0 0 1]进行变换
            Vector3 vectorZ = new Vector3(0f, 0f, 1f);
            Vector3 vectors2 = new Vector3(0f, 0f, 0f);
            for (int i = 1; i < this.rects.Length; i++)
            {
                vectors2 += this.rects[i].v[0] - this.rects[0].v[0];
            }
            vectors2.Y = 0f;                            //令y=0， 也即让该向量投影到XZ平面。
            vectors2 = Vector3.Normalize(vectors2);

            degreeNum = (float)(Math.Acos(Vector3.Dot(vectors2, vectorZ)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Angle{{upVector(0,0,1)}} = {0} degree", degreeNum);
            if (degreeNum > 2f && vectors2.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors2, vectorZ);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[0];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);
                calibrationMatrix = calibrationMatrix * transMatrix;

                //对每一个矩形的每个顶点进行坐标变换
                for (int j = 0; j < this.rects.Length; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = this.rects[j].v[k];
                        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                    }
                }

            }

            #endregion   
            return calibrationMatrix;
        }

        /// <summary>
        /// Calculate the Box size according all the coordiante of frames
        /// </summary>
        /// <param name="boxOrg"></param>
        /// <param name="boxWidth"></param>
        /// <param name="boxHeight"></param>
        /// <param name="boxDepth"></param>
        public void CalculateBoxSize(int adjustParas, out Vector3 boxOrg, out int boxWidth, out int boxHeight, out int boxDepth)
        {
            Vector3 minVector = this.rects[0].v[0];
            Vector3 maxVector = this.rects[0].v[0];

            //获取所有向量坐标最大的xyz 和 最小的xyz
            for (int i = 0; i < rects.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 tempVector = this.rects[i].v[j];
                    minVector = Vector3.Min(tempVector, minVector);
                    maxVector = Vector3.Max(tempVector, maxVector);
                }
            }
            minVector.X = minVector.X - adjustParas; minVector.Y = minVector.Y - adjustParas; minVector.Z = minVector.Z - adjustParas;
            maxVector.X = maxVector.X + adjustParas; maxVector.Y = maxVector.Y + adjustParas; maxVector.Z = maxVector.Z + adjustParas;

            boxOrg = minVector;
            boxWidth = (int)Math.Round(Math.Abs(minVector.X - maxVector.X) / voxelWidth);
            boxHeight = (int)Math.Round(Math.Abs(minVector.Y - maxVector.Y) / voxelHeight);
            boxDepth = (int)Math.Round(Math.Abs(minVector.Z - maxVector.Z) / voxelDepth);

        }

        /// <summary>
        /// Transform the Rects into a new Coordinate, Please call this function first and then call CalculateBoxSize
        /// Width --> y axis Height --> x axis
        /// </summary>
        public Matrix4x4 GetCalibrationMatrixWyHx()
        {
            Matrix4x4 calibrationMatrix = new Matrix4x4();
            calibrationMatrix = Matrix4x4.Identity;

            Console.WriteLine(">> Transformation... from transmitter space to observation space");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorX = new Vector3(1f, 0f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[1 0 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[3] - this.rects[i].v[0];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorX)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Align to +X direction, {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorX);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);
                calibrationMatrix = calibrationMatrix * transMatrix;

                ////对每一个矩形的每个顶点进行坐标变换
                //for (int j = 0; j < this.rects.Length; j++)
                //{
                //    for (int k = 0; k < 4; k++)
                //    {
                //        Vector3 tempVector = new Vector3();
                //        tempVector = this.rects[j].v[k];
                //        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                //    }
                //}

            }
            #endregion

            #region 根据向量[0 0 1]进行变换
            Vector3 vectorZ = new Vector3(0f, 0f, 1f);
            Vector3 vectors2 = new Vector3(0f, 0f, 0f);
            for (int i = 1; i < this.rects.Length; i++)
            {
                vectors2 += this.rects[i].v[0] - this.rects[0].v[0];
            }
            vectors2.X = 0f;                            //令y=0， 也即让该向量投影到XZ平面。
            vectors2 = Vector3.Normalize(vectors2);

            degreeNum = (float)(Math.Acos(Vector3.Dot(vectors2, vectorZ)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Align to +Z direction, {0} degree", degreeNum);
            if (degreeNum > 2f && vectors2.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors2, vectorZ);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[0];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);
                calibrationMatrix = calibrationMatrix * transMatrix;

                ////对每一个矩形的每个顶点进行坐标变换
                //for (int j = 0; j < this.rects.Length; j++)
                //{
                //    for (int k = 0; k < 4; k++)
                //    {
                //        Vector3 tempVector = new Vector3();
                //        tempVector = this.rects[j].v[k];
                //        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                //    }
                //}

            }

            #endregion   
            return calibrationMatrix;
        }

        // In this function, the vectorZ is not used. So, the transmitter needs to be parallel to the ground
        // So, ther feature of plumb line can be preserved
        public Matrix4x4 GetCalibrationMatrixWyHxForPlumbLine()
        {
            Matrix4x4 calibrationMatrix = new Matrix4x4();
            calibrationMatrix = Matrix4x4.Identity;

            Console.WriteLine(">> Transformation... from transmitter space to observation space");

            //Average the vectors of Y axis [0 1 0]
            Vector3 vectorX = new Vector3(1f, 0f, 0f);
            Vector3 vectors1 = new Vector3(0f, 0f, 0f);


            #region 根据向量[1 0 0]进行变换
            for (int i = 0; i < this.rects.Length; i++)
            {
                vectors1 += this.rects[i].v[3] - this.rects[i].v[0];
            }

            vectors1 = Vector3.Normalize(vectors1);

            //Console.WriteLine(vectors);
            float degreeNum = (float)(Math.Acos(Vector3.Dot(vectors1, vectorX)) * 57.295779513082323);      //求夹角，并转换为角度
            Console.WriteLine("\t Align to +X direction, {0} degree", degreeNum);

            if (degreeNum > 2f && vectors1.Length() > 0f)
            {
                //计算旋转方向的方向向量
                Vector3 rotDir = Vector3.Cross(vectors1, vectorX);
                //获取旋转原点
                Vector3 rotOrg = this.rects[0].v[3];
                //获取变化矩阵
                Matrix4x4 transMatrix = this.MatrixForRotationAroundLine(rotOrg, rotDir, -(int)degreeNum);
                calibrationMatrix = calibrationMatrix * transMatrix;
                

                ////对每一个矩形的每个顶点进行坐标变换
                //for (int j = 0; j < this.rects.Length; j++)
                //{
                //    for (int k = 0; k < 4; k++)
                //    {
                //        Vector3 tempVector = new Vector3();
                //        tempVector = this.rects[j].v[k];
                //        this.rects[j].v[k] = Vector3.Transform(tempVector, transMatrix); //与变换矩阵相乘并更新
                //    }
                //}

            }
            #endregion

            return calibrationMatrix;
        }

        /// <summary>
        /// Calculate the Box size according all the coordiante of frames
        /// </summary>
        /// <param name="boxOrg"></param>
        /// <param name="boxWidth"></param>
        /// <param name="boxHeight"></param>
        /// <param name="boxDepth"></param>
        public void CalculateBoxSizeWyHx(int adjustParas, out Vector3 boxOrg, out int boxWidth, out int boxHeight, out int boxDepth)
        {
            Vector3 minVector = this.rects[0].v[0];
            Vector3 maxVector = this.rects[0].v[0];

            //获取所有向量坐标最大的xyz 和 最小的xyz
            for (int i = 0; i < rects.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 tempVector = this.rects[i].v[j];
                    minVector = Vector3.Min(tempVector, minVector);
                    maxVector = Vector3.Max(tempVector, maxVector);
                }
            }
            minVector.X = minVector.X - adjustParas; minVector.Y = minVector.Y - adjustParas; minVector.Z = minVector.Z - adjustParas;
            maxVector.X = maxVector.X + adjustParas; maxVector.Y = maxVector.Y + adjustParas; maxVector.Z = maxVector.Z + adjustParas;

            boxOrg = minVector;
            boxWidth = (int)Math.Round(Math.Abs(minVector.Y - maxVector.Y) / voxelWidth);
            boxHeight = (int)Math.Round(Math.Abs(minVector.X - maxVector.X) / voxelHeight);
            boxDepth = (int)Math.Round(Math.Abs(minVector.Z - maxVector.Z) / voxelDepth);

        }

        /// <summary>
        /// 获取三维空间绕某条线旋转的矩阵
        /// </summary>
        /// <param name="rotOrg"></param>
        /// <param name="rotDir"></param>
        /// <param name="rotAngleDegree"></param>
        /// <returns></returns>
        public Matrix4x4 MatrixForRotationAroundLine(Vector3 rotOrg, Vector3 rotDir, int rotAngleDegree)
        {
            Matrix4x4 transformMatrix = new Matrix4x4();
            transformMatrix = Matrix4x4.Identity;

            //位移矩阵
            Matrix4x4 translateMatrix = new Matrix4x4();
            translateMatrix = Matrix4x4.Identity;
            translateMatrix.M41 = rotOrg.X * (-1);
            translateMatrix.M42 = rotOrg.Y * (-1);
            translateMatrix.M43 = rotOrg.Z * (-1);

            //Rotate rotation-vector around z-axis so that the rotation-vector lie on xz-plane.
            Matrix4x4 rotToXZ = new Matrix4x4();
            rotToXZ = Matrix4x4.Identity;
            Vector3 rotDirZToZero = new Vector3(rotDir.X, rotDir.Y, 0);
            float rotDirZToZeroLength = rotDirZToZero.Length();
            if (rotDirZToZero.Length() != 0)
            {

                rotToXZ.M11 = rotDir.X / rotDirZToZeroLength;       // x/|(x,y,z)|   cos a
                rotToXZ.M12 = rotDir.Y / rotDirZToZeroLength;       // y/|(x,y,0)|   sin a
                rotToXZ.M21 = -rotDir.Y / rotDirZToZeroLength;      // y/|(x,y,0)|   -sin a
                rotToXZ.M22 = rotDir.X / rotDirZToZeroLength;       // x/|(x,y,0)|   cos a
            }

            //Rotate rotation-vector in XZ-plane so that it is z-axis
            Matrix4x4 rotToZ = new Matrix4x4();
            rotToZ = Matrix4x4.Identity;
            float rotDirLength = rotDir.Length();
            rotToZ.M11 = rotDir.Z / rotDirLength;                   // z/|(x,y,z)|  cos a
            rotToZ.M13 = -rotDirZToZeroLength / rotDirLength;       // |(x,y,0)|/|(x,y,z)| -sin a
            rotToZ.M31 = rotDirZToZeroLength / rotDirLength;        // |(x,y,0)|/|(x,y,z)| sin a
            rotToZ.M33 = rotDir.Z / rotDirLength;                   // z/|(x,y,z)|  cos a

            //Rotate object around z-axis with the specified angle
            Matrix4x4 rotationMatrix = new Matrix4x4();
            rotationMatrix = Matrix4x4.Identity;
            double rotAngleRadius = rotAngleDegree * 0.017453292519943295;
            rotationMatrix.M11 = (float)Math.Cos(rotAngleRadius);
            rotationMatrix.M12 = -(float)Math.Sin(rotAngleRadius);
            rotationMatrix.M21 = (float)Math.Sin(rotAngleRadius);
            rotationMatrix.M22 = (float)Math.Cos(rotAngleRadius);

            //Compose these transformation matrixs together 
            transformMatrix = translateMatrix * transformMatrix;
            transformMatrix = rotToXZ * transformMatrix;
            transformMatrix = rotToZ * transformMatrix;
            transformMatrix = rotationMatrix * transformMatrix;

            //Get matrix inverse and recalculate transformation matrix
            Matrix4x4 rotToZInverse = new Matrix4x4();
            Matrix4x4.Invert(rotToZ, out rotToZInverse);
            transformMatrix = rotToZInverse * transformMatrix;

            Matrix4x4 rotToXZInverse = new Matrix4x4();
            Matrix4x4.Invert(rotToXZ, out rotToXZInverse);
            transformMatrix = rotToXZInverse * transformMatrix;

            Matrix4x4 translateInverse = new Matrix4x4();
            Matrix4x4.Invert(translateMatrix, out translateInverse);
            transformMatrix = translateInverse * transformMatrix;

            return transformMatrix;
        }

        /// <summary>
        /// Builds a matrix that rotates from one vector to another
        /// </summary>
        /// <param name="fromVector"></param>
        /// <param name="toVector"></param>
        /// <returns></returns>
        public Matrix4x4 BuildRotateFtomTO(Vector3 fromVector, Vector3 toVector)
        {
            Matrix4x4 transformMatrix = new Matrix4x4();
            transformMatrix = Matrix4x4.Identity;

            Vector3 fNormalize = Vector3.Normalize(fromVector);
            Vector3 tNormalize = Vector3.Normalize(toVector);
            Vector3 tfVector = Vector3.Cross(tNormalize, fNormalize);
            Vector3 tfVectorNormalize = new Vector3();

            if (tfVector.Length() != 0f)
            {
                tfVectorNormalize = Vector3.Normalize(tfVector);

                float cosValue = Vector3.Dot(fNormalize, tNormalize);  //the value cos Angle
                float degreeAngle = (float)Math.Acos(cosValue);      // Angle
                Vector3 vtVector = tfVectorNormalize * (1 - cosValue);

                transformMatrix.M11 = vtVector.X * tfVectorNormalize.X + cosValue;
                transformMatrix.M22 = vtVector.Y * tfVectorNormalize.Y + cosValue;
                transformMatrix.M33 = vtVector.Z * tfVectorNormalize.Z + cosValue;

                vtVector.X = vtVector.X * tfVectorNormalize.Y;
                vtVector.Z = vtVector.Z * tfVectorNormalize.X;
                vtVector.Y = vtVector.Y * tfVectorNormalize.Z;

                transformMatrix.M12 = vtVector.X - tfVector.Z;
                transformMatrix.M13 = vtVector.Z + tfVector.Y;
                transformMatrix.M14 = 0;

                transformMatrix.M21 = vtVector.X + tfVector.Z;
                transformMatrix.M23 = vtVector.Y - tfVector.X;
                transformMatrix.M24 = 0;

                transformMatrix.M31 = vtVector.Z - tfVector.Y;
                transformMatrix.M32 = vtVector.Y + tfVector.X;
                transformMatrix.M34 = 0;

                transformMatrix.M41 = 0;
                transformMatrix.M42 = 0;
                transformMatrix.M43 = 0;
                transformMatrix.M44 = 1;
            }
            return transformMatrix;

        }

        /// <summary>
        /// 寻找所有的矩阵中心，并求和做平均
        /// 之后让所有的坐标去减这个中心，这样坐标轴(0,0,0)便大致处于物体中心部位
        /// </summary>
        public void DoCentroidAllCalibratedRectangles()
        {
            Vector3 centerVector = new Vector3(0f, 0f, 0f);

            //对每个矩形的四个顶点求平均坐标，获取中心点
            for (int i = 0; i < this.rects.Length; i++)
            {
                centerVector += this.rects[i].GetCenter();
            }
            centerVector /= (float)rects.Length;

            //将所有坐标向量平移到中心点
            for (int j = 0; j < rects.Length; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Vector3 tempVectors = this.rects[j].v[k];
                    tempVectors -= centerVector;
                    this.rects[j].v[k] = tempVectors;
                }
            }

            this.gpsCalibrationBottom -= centerVector;
            this.gpsCalibrationTop -= centerVector;
        }

        public void InverseGPS_X()
        {
            for (int i = 0; i < rects.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.rects[i].v[j].X = 0f - this.rects[i].v[j].X;
                }
            }

            this.gpsCalibrationTop.X = 0f - this.gpsCalibrationTop.X;
            this.gpsCalibrationBottom.X = 0f - this.gpsCalibrationBottom.X;
        }


        public void InverseGPS_Y()
        {
            for (int i = 0; i < rects.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.rects[i].v[j].Y = 0f - this.rects[i].v[j].Y;
                }
            }

        }
    }
}
