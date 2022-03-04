using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Portable_BMU_App
{
    public partial class Show_Coronal : System.Windows.Forms.Form
    {
        public System.Windows.Forms.PictureBox pictureBox1;

        Rectangle rect;
        Point LocationXY;
        Point LocationX1Y1;

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
            this.pictureBox1.Size = new System.Drawing.Size(662, 1165);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // Show_Coronal
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(675, 1178);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Show_Coronal";
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
            if(isMouseDown == true)
            {
                LocationX1Y1 = e.Location;
                isMouseDown = false;
            }
        }

        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (rect != null)
            {
                e.Graphics.DrawRectangle(Pens.Red, GetRect());
            }
        }

        private Rectangle GetRect()
        {
            rect = new Rectangle();

            rect.X = Math.Min(LocationXY.X, LocationX1Y1.X);

            rect.Y = Math.Min(LocationXY.Y, LocationX1Y1.Y);

            rect.Width = Math.Abs(LocationXY.X - LocationX1Y1.X);

            rect.Height = Math.Abs(LocationXY.Y - LocationX1Y1.Y);

            return rect;
        }
    }
}
