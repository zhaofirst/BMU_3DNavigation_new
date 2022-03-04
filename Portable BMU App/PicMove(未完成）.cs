       应该插在Frm_RealTime_navigation_ZLX中的缩放功能后（未完成Mark功能）



       /////拖动效果
        bool isMove = false;
        System.Drawing.Point pInitMove = new System.Drawing.Point();
        private void Cor_MouseDown_Zoom(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = true;
                pInitMove = e.Location;
            }
        }
        //鼠标松开功能
        private void Cor_MouseUp_Zoom(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }
        void PictureMouseMove(object sender,MouseEventArgs e)
        {
            int compensation_Y_Value = 0;

            compensation_Y_Value = (PicCor.Height - PictureCor_Original_Zoom.Height) / 2; //记录补偿缺口大小
            if (isMove)
            {
                System.Drawing.Point pNow = e.Location;
                //p.X = PicCor.Width - p.X;
                //p.Y = PicCor.Height - p.Y;
                int deltaX = 0;
                int deltaY = 0;
                if (pNow.X > 0 && pNow.Y > compensation_Y_Value && pNow.Y < PicCor.Height - compensation_Y_Value && pNow.X < PicCor.Width)
                {
                    deltaX = (int)((pNow.X - pInitMove.X)/(Cor_Scale_Zoom-1));
                    deltaY = (int)((pNow.Y - pInitMove.Y) / (Cor_Scale_Zoom - 1));

                    LocationCor_Point_Zoom.X = LocationCor_Point_Zoom.X - deltaX;
                    LocationCor_Point_Zoom.Y = LocationCor_Point_Zoom.Y - deltaY;
                    if(LocationCor_Point_Zoom.X < 0)
                    {
                        LocationCor_Point_Zoom.X = 0;
                    }
                    if (LocationCor_Point_Zoom.X > PicCor.Width)
                    {
                        LocationCor_Point_Zoom.X = PicCor.Width;
                    }
                    if (LocationCor_Point_Zoom.Y < 0)
                    {
                        LocationCor_Point_Zoom.Y = 0;
                    }
                    if (LocationCor_Point_Zoom.Y > PictureCor_Original_Zoom.Height)
                    {
                        LocationCor_Point_Zoom.Y = PictureCor_Original_Zoom.Height;
                    }
                    Mat showMat = new Mat();
                    showMat = ShowZoomPictureCorMat();
                    Bitmap bitmapFinal = ConvertFile.MatToBitmap(showMat);
                    PicCor.Image = bitmapFinal;
                }

            }

        }

