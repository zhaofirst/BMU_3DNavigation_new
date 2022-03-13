using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portable_BMU_App
{
    class Rubbish
    {


    }
}


// 用来存放以前写过的但是不用的函数，也可能在未来启用


// 根据textbox 中的数字将5张图片合理的展示出来//
//目前已经用UpdataFivePictureBox_Zoom_And_Mark()实现
// 在Frm_RealTime_Navigation_ZLX中实现
//private void Display_zlx_fivepicture()
//{

//Mat newMat1 = new Mat();
//Mat newMat2 = new Mat();
//Mat newMat3 = new Mat();
//Mat newMat4 = new Mat();
//Mat newMat5 = new Mat();

//// 根据textbox 将所有图片合理的展现出来
//int Tran = System.Convert.ToInt32(texdown_heng.Text.Substring(0, texdown_heng.Text.IndexOf("/")));
//int Sag = System.Convert.ToInt32(texdown_shi.Text.Substring(0, texdown_shi.Text.IndexOf("/")));
//int Cor = System.Convert.ToInt32(texdown_guan.Text.Substring(0, texdown_guan.Text.IndexOf("/")));


//short[,] Transverse = volume[Tran];
//short[,] Sagittal = volume_shi[Sag];
//short[,] Coronal = volume_guan[Cor];



//Display_zlx(PicTra, Transverse, 0); // 0横断面 1 冠状面 2 矢状面
//Display_zlx(PicSag, Sagittal, 2);
//Display_zlx(PicCor, Coronal, 1);




//Mat SagPro = new Mat();
//SagPro = SagittalProjectionMat.Clone();
//Mat CorPro = new Mat();
//CorPro = CoronalProjectionMat.Clone();

//OpenCvSharp.Size dsize_shi_Pro = new OpenCvSharp.Size(PicSag.Image.Width, PicSag.Image.Height);
//OpenCvSharp.Size dsize_guan_Pro = new OpenCvSharp.Size(PicCor.Image.Width, PicCor.Image.Height);


//Mat ResizeSagPro = new Mat();
//Mat ResizeCorPro = new Mat();
//Cv2.Resize(SagPro, ResizeSagPro, dsize_shi_Pro);
//Cv2.Resize(CorPro, ResizeCorPro, dsize_guan_Pro);

//Bitmap bitmap_corpro = ConvertFile.MatToBitmap(ResizeCorPro);
//Bitmap bitmap_sagpro = ConvertFile.MatToBitmap(ResizeSagPro);
//PicProCor.Image = bitmap_corpro;
//PicProSag.Image = bitmap_sagpro;

//// 结束展现

//Picguan_clone = ConvertFile.BitmapToMat(new Bitmap(PicCor.Image));
//Picshi_clone = ConvertFile.BitmapToMat(new Bitmap(PicSag.Image));
//Picheng_clone = ConvertFile.BitmapToMat(new Bitmap(PicTra.Image));
//PicSagPro_clone = ConvertFile.BitmapToMat(new Bitmap(PicProSag.Image));
//PicCorPro_clone = ConvertFile.BitmapToMat(new Bitmap(PicProCor.Image));


//updateBrightnessContrast(Picguan_clone, newMat1, BrightnessBar.Value, ContrastBar.Value);
//updateBrightnessContrast(Picshi_clone, newMat2, BrightnessBar.Value, ContrastBar.Value);
//updateBrightnessContrast(Picheng_clone, newMat3, BrightnessBar.Value, ContrastBar.Value);
//updateBrightnessContrast(PicSagPro_clone, newMat4, BrightnessBar.Value, ContrastBar.Value);
//updateBrightnessContrast(PicCorPro_clone, newMat5, BrightnessBar.Value, ContrastBar.Value);




//Bitmap bit_guan = new Bitmap(ConvertFile.MatToBitmap(newMat1));
//Bitmap bit_shi = new Bitmap(ConvertFile.MatToBitmap(newMat2));
//Bitmap bit_heng = new Bitmap(ConvertFile.MatToBitmap(newMat3));
//Bitmap bit_sag = new Bitmap(ConvertFile.MatToBitmap(newMat4));
//Bitmap bit_cor = new Bitmap(ConvertFile.MatToBitmap(newMat5));



//PicCor.Image = bit_guan;
//PicSag.Image = bit_shi;
//PicTra.Image = bit_heng;
//PicProSag.Image = bit_sag;
//PicProCor.Image = bit_cor;



//newMat1 = null;
//newMat2 = null;
//newMat3 = null;
//newMat4 = null;
//newMat5 = null;
//GC.Collect();
//}
