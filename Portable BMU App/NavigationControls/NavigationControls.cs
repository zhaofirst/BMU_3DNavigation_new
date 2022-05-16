using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Portable_BMU_App.NavigationControls
{
    public class DrawGraphs
    {

        //private void Form1_Paint_1(PictureBox sender, OpenCvSharp.Point startPt, OpenCvSharp.Point endPt)
        public static Bitmap DrawArrows(Bitmap OrBitMap, PointF startPt, PointF endPt, Brush brushParameter = null, Pen penParameter = null)
        {
            //线的起点
            //PointF startPt = new PointF(100, 300);
            //线的终点
            //PointF endPt = new PointF(200, 200);
            //箭头的宽
            float width = 10;
            //箭头夹角
            double angle = 45.0 / 180 * Math.PI;

            //求BC长度
            double widthBE = width / 2 / (Math.Tan(angle / 2));

            //直线向量
            Vector2 lineVector = new Vector2(endPt.X - startPt.X, endPt.Y - startPt.Y);
            //单位向量
            lineVector = Vector2.Normalize(lineVector);

            //求BE向量
            Vector2 beVector = (float)widthBE * -lineVector;

            //求E点坐标
            PointF ePt = new PointF();
            //ePt - endPt = bcVector
            ePt.X = endPt.X + beVector.X;
            ePt.Y = endPt.Y + beVector.Y;

            //因为CD向量和AB向量垂直,所以CD方向向量为
            Vector2 cdVector = new Vector2(-lineVector.Y, lineVector.X);
            //求单位向量
            cdVector = Vector2.Normalize(cdVector);

            //求CE向量
            Vector2 ceVector = width / 2 * cdVector;
            //求C点坐标,ePt - cPt = ceVector;
            PointF cPt = new PointF();
            cPt.X = ePt.X - ceVector.X;
            cPt.Y = ePt.Y - ceVector.Y;

            //求DE向量
            Vector2 deVector = width / 2 * -cdVector;
            //求D点,ePt-dPt = deVector;
            PointF dPt = new PointF();
            dPt.X = ePt.X - deVector.X;
            dPt.Y = ePt.Y - deVector.Y;


            //Bitmap OrBitMap = new Bitmap(sender.Image); // 使用bitmap作为画布
            Graphics gfx = Graphics.FromImage(OrBitMap);

            if (penParameter == null)
            {

                Pen penImagLine = new Pen(Color.Blue);
                //penImagLine.Color = Color.Blue;
                penImagLine.DashPattern = new float[] { 1.0F, 2.0F, 1.0F, 3.0F }; // 设置虚线长度
                gfx.DrawLine(penImagLine, startPt, endPt);
            }
            else
            {
                penParameter.DashPattern = new float[] { 2.0F, 2.0F, 2.0F, 2.0F };
                gfx.DrawLine(penParameter, startPt, endPt);
            }

            //绘制箭头


            //    gfx.FillPolygon(Brushes.Green,
            //        new PointF[]{
            //cPt,dPt,endPt});
            if (brushParameter == null)
            {
                gfx.FillPolygon(Brushes.Green, new PointF[]
                {
                    cPt,dPt,endPt
                });
            }
            else
            {
                gfx.FillPolygon(brushParameter, new PointF[]{
                cPt,dPt,endPt});
            }

            ////}
            //sender.Image = OrBitMap;
            return OrBitMap;
        }
    }
}
