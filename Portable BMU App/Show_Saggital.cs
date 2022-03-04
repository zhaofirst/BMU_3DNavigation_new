using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Portable_BMU_App
{
    public partial class Show_Saggital : System.Windows.Forms.Form
    {
        public System.Windows.Forms.PictureBox pictureBox1;

        Point LocationXY;
        Point LocationX1Y1;
        public ArrayList points = new ArrayList();

        bool isMouseDown = false;

        public void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlText;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1498, 509);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // Show_Saggital
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1522, 538);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Show_Saggital";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResizeEnd += new System.EventHandler(this.show_Coronal_ResizeEnd);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.show_Coronal_DragOver);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private void show_Coronal_ResizeEnd(object sender, EventArgs e)
        {
            //pictureBox1.Height = Convert.ToInt16((this.Height)*0.92);
            //pictureBox1.Width = Convert.ToInt16((this.Width) * 0.92);
            pictureBox1.Height = Convert.ToInt16((this.Height) - 65);
            pictureBox1.Width = Convert.ToInt16((this.Width) - 60);
        }

        private void show_Coronal_ResizeBegin(object sender, EventArgs e)
        {
            pictureBox1.Height = 10;
            pictureBox1.Width = 10;
        }

        private void show_Coronal_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = true;

            LocationXY = e.Location;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    int depth = Frm_RealTime_Navigation_ZLX.volume.Length;
                    int height = Frm_RealTime_Navigation_ZLX.volume[0].GetLength(0);

                    PointF tPoint = new PointF();
                    tPoint.X = (Convert.ToSingle(((Point)points[i]).X )/ this.pictureBox1.Width) * depth;
                    tPoint.Y = (Convert.ToSingle(((Point)points[i]).Y )/ this.pictureBox1.Height) * height;
                    //Console.WriteLine("x,y: {0},{1}", tPoint.X, tPoint.Y);
                    if (this.Text == "Top")
                    {
                        Frm_RealTime_Navigation_ZLX.top_points.Add(tPoint);
                    }
                    else
                    {
                        Frm_RealTime_Navigation_ZLX.bottom_points.Add(tPoint);
                    }
                }
                this.Close();
            }
        }

        private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown == true)
            {
                LocationX1Y1 = e.Location;

                Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isMouseDown == true)
            {
                LocationX1Y1 = e.Location;
                if(points.Count != 0)
                {
                    if (((Point)points[points.Count - 1]).X < LocationX1Y1.X)
                    {
                        points.Add(LocationX1Y1);
                    }
                }
                else
                {
                    points.Add(LocationX1Y1);
                }
                
                //isMouseDown = false;
            }
        }

        const int RectSize = 16;
        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Red, 3);

            if (points.Count != 0)
            {
                Point currentLocation = (Point)points[points.Count - 1];

                e.Graphics.DrawLine(pen, currentLocation, LocationX1Y1);
                e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(currentLocation.X - RectSize / 2, currentLocation.Y - RectSize / 2, RectSize, RectSize));


            }
            if (points.Count > 1)
            {
                Point[] currentLocation = (Point[])points.ToArray(typeof(Point));

                e.Graphics.DrawLines(pen, currentLocation);

                foreach (var p in currentLocation)
                {
                    e.Graphics.FillEllipse(Brushes.Blue, new Rectangle(p.X - RectSize / 2, p.Y - RectSize / 2, RectSize, RectSize));
                }

            }
        }
    }
}

