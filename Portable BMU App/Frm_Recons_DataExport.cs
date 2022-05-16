using csmatio.io;
using csmatio.types;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;

namespace Portable_BMU_App
{
    public partial class Frm_Recons_DataExport : Form
    {
        // Voxel Size
        public float voxelWidth;
        public float voxelHeight;
        public float voxelDepth;
        short[,] G = null;
        short[][,] ReconstructedVolume;


        // Data Set
        public USDataCollection2 m_usDataCollection;

        public string selectedMethod = null;   // default selected method is VNN in radioButton Group

        Dlg_waitingRecons dlg_WaitingForm = new Dlg_waitingRecons();

        DateTime beginTime;

        public Frm_Recons_DataExport(USDataCollection2 usDataCollection)
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            m_usDataCollection = usDataCollection;

            m_usDataCollection.voxelWidth = (float)voxel_width.Value;
            m_usDataCollection.voxelHeight = (float)voxel_height.Value;
            m_usDataCollection.voxelDepth = (float)voxel_depth.Value;

            hScrollBarFrames.Maximum = m_usDataCollection.framesCount;    // Set the maximum of hScrollBar.
            this.numericUpDownFrameIndex.Maximum = m_usDataCollection.framesCount;
            this.labelFrameMaximumCounts.Text = m_usDataCollection.framesCount.ToString();
            if (Frm_Recons_DataSelect.isChildDirectry)
            {
                // select method Double Sweep in default in radioButton Group
                radioButtonDS.Checked = true;
                selectedMethod = radioButtonDS.Text;
            }
            else
            {
                // select method FDP in default in radioButton Group
                radioButtonFDP.Checked = true;
                selectedMethod = radioButtonFDP.Text;
            }


            this.checkBoxCloseOnFinish.Visible = false;
            this.btnBack.Visible = false;
        }

        //public Frm_Recons_DataExport()
        //{
        //    InitializeComponent();
        //    voxelWidth = (float)voxel_width.Value;
        //    voxelHeight = (float)voxel_height.Value;
        //    voxelDepth = (float)voxel_depth.Value;
        //    //hScrollBarFrames.Maximum = 1438;
        //}


        // Go back to DataSelect
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
            //Frm_Recons_DataSelect.record_recons_DataSelect.Show();
        }


        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            // determine which method was selected
            RadioButton radioButton = sender as RadioButton;
            selectedMethod = radioButton.Text;
            Console.WriteLine(radioButton.Text);

        }

        private void voxel_width_ValueChanged(object sender, EventArgs e)
        {
            m_usDataCollection.voxelWidth = (float)voxel_width.Value;
        }

        private void voxel_height_ValueChanged(object sender, EventArgs e)
        {
            m_usDataCollection.voxelHeight = (float)voxel_height.Value;
        }

        private void voxel_depth_ValueChanged(object sender, EventArgs e)
        {
            m_usDataCollection.voxelDepth = (float)voxel_depth.Value;
        }

        private void hScrollBarFrames_ValueChanged(object sender, EventArgs e)
        {
            // Display transverse imaging following the value og scrollBar
            //Console.WriteLine(hScrollBarFrames.Value);
            labelFrameCount.Text = hScrollBarFrames.Value.ToString();
            Mat srcG = ConvertFile.ArrayToMat(m_usDataCollection.frames[hScrollBarFrames.Value-1]);
            Mat dstG = new Mat();
            Cv2.ApplyColorMap(srcG, dstG, ColormapTypes.Jet);

            Bitmap bitmapG = ConvertFile.MatToBitmap(dstG);

            this.pictureBoxTransversShow.Image = bitmapG;
        }

        // start to reconstrut when export button was clicked
        private void btnNext_Click(object sender, EventArgs e)
        {
            // write the export dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Mat Files (*.mat)|*.mat|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "mat";
            saveFileDialog.RestoreDirectory = false; // remember the last folder location you opened
            saveFileDialog.InitialDirectory = m_usDataCollection.filePath;
            saveFileDialog.FileName = m_usDataCollection.fileName+".mat";   // set fileName according to the currently dataSet

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;


            // get save path
            string localFilePath = saveFileDialog.FileName.ToString();
            m_usDataCollection.savedPath = localFilePath;

            //dlg_WaitingForm.progressBarWaiting.Style = ProgressBarStyle.Marquee;
            this.timer1.Start();
            beginTime = DateTime.Now;
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
            dlg_WaitingForm.CancelButonClick += new Dlg_waitingRecons.DelegateCancelButtonClick(CancelBackgroudWorker1);
            dlg_WaitingForm.Show();

        }

        public void Reconstruction()
        {
            ReconstructionMethods2 reconstructionMethods;
            //vnnChecked = false;
            if (selectedMethod == "VNN")
            {
                reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.VNN,
                    m_usDataCollection);
                G = reconstructionMethods.projectionMatrix;
                ReconstructedVolume = ( reconstructionMethods.m_box);
            }
            else if (selectedMethod == "PNN")
            {
                reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.PNN,
                    m_usDataCollection);
                G = reconstructionMethods.projectionMatrix;
                ReconstructedVolume = (reconstructionMethods.m_box);
            }
            else if (selectedMethod == "FDP")
            {
                reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.FDP,
                    m_usDataCollection);
                G = reconstructionMethods.projectionMatrix;
                ReconstructedVolume = (reconstructionMethods.m_box);
            }
            else if (selectedMethod == "MPI2")
            {
                reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.MPI2,
                    m_usDataCollection);
                G = reconstructionMethods.projectionMatrix;
                ReconstructedVolume = (reconstructionMethods.m_box);
            }
            else if (selectedMethod == "MPI4")
            {
                reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.MPI4,
                    m_usDataCollection);
                G = reconstructionMethods.projectionMatrix;
                ReconstructedVolume = (reconstructionMethods.m_box);
            }
            else if (selectedMethod == radioButtonDS.Text)
            {
                m_usDataCollection.isHoleFillingUsed = checkBoxisUseHoleFilling.Checked;
                reconstructionMethods = new ReconstructionMethods2(ReconstructionMethods2.Method.Double_Sweep,
                    m_usDataCollection);
                G = reconstructionMethods.projectionMatrix;
                ReconstructedVolume = (reconstructionMethods.m_box);

            }
            else
            {
                MessageBox.Show("Please select a method!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;

            }

            // Transfer these paras to mainform after pre-scan
            // If Width/Deepth is about 1:2
            Frm_RealTime_Navigation_ZLX.boxWidth = reconstructionMethods.mboxWidth;
            Frm_RealTime_Navigation_ZLX.boxHeight = reconstructionMethods.mboxHeight;
            Frm_RealTime_Navigation_ZLX.boxDepth = reconstructionMethods.mboxDepth;
            Frm_RealTime_Navigation_ZLX.boxOrg = reconstructionMethods.mboxOrg;
            Frm_RealTime_Navigation_ZLX.voxelDepth = reconstructionMethods.voxelDepth;
            Frm_RealTime_Navigation_ZLX.voxelHeight = reconstructionMethods.voxelHeight;
            Frm_RealTime_Navigation_ZLX.voxelWidth = reconstructionMethods.voxelWidth;

            //If Width/Deepth is about 1:1
            Frm_RealTime_Navigation_ZLX_Short.boxWidth = reconstructionMethods.mboxWidth;
            Frm_RealTime_Navigation_ZLX_Short.boxHeight = reconstructionMethods.mboxHeight;
            Frm_RealTime_Navigation_ZLX_Short.boxDepth = reconstructionMethods.mboxDepth;
            Frm_RealTime_Navigation_ZLX_Short.boxOrg = reconstructionMethods.mboxOrg;
            Frm_RealTime_Navigation_ZLX_Short.voxelDepth = reconstructionMethods.voxelDepth;
            Frm_RealTime_Navigation_ZLX_Short.voxelHeight = reconstructionMethods.voxelHeight;
            Frm_RealTime_Navigation_ZLX_Short.voxelWidth = reconstructionMethods.voxelWidth;

        }

        // 加入等待对话框的相应操作
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // create a new thread to process recontrction 
            Thread reconsThread = new Thread(Reconstruction);
            reconsThread.Start();
            Console.Write("-------- Reconstruction Thread is running --------");

            // truck in this loop, if no cancel button cliceked or current thread not compelete
            while (worker.CancellationPending != true && reconsThread.IsAlive)
            {

            }

            // if reconsThread is alive, it means cancel button was clicked
            if (reconsThread.IsAlive)
            {
                reconsThread.Interrupt();
                if (!reconsThread.Join(2000))
                {
                    reconsThread.Abort();
                }
                e.Cancel = true;
                MessageBox.Show("Reconstruction has been canceled!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.timer1.Stop();
            }
            // current reconsThread has been finished
            else
            {
                Console.WriteLine("-------- Reconstruction Thread finished! --------");
                Console.WriteLine();
            }
            //dlg_WaitingForm.Close();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                dlg_WaitingForm.Close();
                Console.WriteLine( "Canceled current thread!");
            }
            else if (e.Error != null)
            {
                dlg_WaitingForm.Close();

                Console.WriteLine("Error: " + e.Error.Message);
            }
            else
            {
                dlg_WaitingForm.Close();

                this.timer1.Stop();
                MessageBox.Show("Finished!");

                //------------- Transfer reconstructed volume to Frm_RealTime_Navigation_ZLX------------------//
                //byte[,] byteG = ConvertFile.ShortArrayToByte(G);

                //Mat srcG = ConvertFile.ArrayToMat(byteG);
                //Frm_RealTime_Navigation_ZLX.srcG = srcG;
                //Frm_RealTime_Navigation_ZLX.showMat = srcG;
                //Mat dstG = new Mat();
                //Mat showDstG = new Mat();
                

                int height = ReconstructedVolume[0].GetLength(0);
                int width = ReconstructedVolume[0].GetLength(1);
                int depth = ReconstructedVolume.Length;

                Frm_RealTime_Navigation_ZLX.volume = new short[depth][,];
                for (int i = 0; i < depth; i++)
                {
                    Frm_RealTime_Navigation_ZLX.volume[i] = new short[height, width];
                }

                for (int k = 0; k < ReconstructedVolume.Length; k++)
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            Frm_RealTime_Navigation_ZLX.volume[k][height - 1 - i, j] = ReconstructedVolume[k][i, j];
                            // Console.WriteLine("9999");
                        }
                    }
                }

                //Short 
                Frm_RealTime_Navigation_ZLX_Short.volume = new short[depth][,];
                for (int i = 0; i < depth; i++)
                {
                    Frm_RealTime_Navigation_ZLX_Short.volume[i] = new short[height, width];
                }

                for (int k = 0; k < ReconstructedVolume.Length; k++)
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            Frm_RealTime_Navigation_ZLX_Short.volume[k][height - 1 - i, j] = ReconstructedVolume[k][i, j];
                            // Console.WriteLine("9999");
                        }
                    }
                }

                // ---------------------------- save to mat for MIAS--------------------------//
                SaveVolume2Mat(m_usDataCollection.savedPath, ReconstructedVolume, 
                    m_usDataCollection.fileName, 
                    m_usDataCollection.voxelWidth, 
                    m_usDataCollection.voxelHeight, 
                    m_usDataCollection.voxelDepth,
                    false);

                this.Close();

                short[,] coronalImage;

                //if (depth < (int)(1.3 * width) || depth < (int)(1.3 * height))//判断是否是短图像
                if(false)
                {
                    Frm_RealTime_Navigation_ZLX_Short frm_RealTime_Navigation_ZLX_Short = new Frm_RealTime_Navigation_ZLX_Short();
                    frm_RealTime_Navigation_ZLX_Short.Show();
                    frm_RealTime_Navigation_ZLX_Short.Initialize_five_Picture();
                    
                    // ---------------------------- save to mat for MIAS--------------------------//
                    // Open Remove muscle UI and display result in Mainform
                    //Frm_RealTime_Navigation_ZLX_Short.Frm_RealTime_Navigation_ZLX_Short.panelMain.Visible = true;
                    coronalImage = VolumeProjection.SaggitalProjection(Frm_RealTime_Navigation_ZLX_Short.volume);

                    byte[,] byteG_c = ConvertFile.ShortArrayToByte(coronalImage);
                    Mat srcG_c = ConvertFile.ArrayToMat(byteG_c);
                    Bitmap bitmap = ConvertFile.MatToBitmap(srcG_c);


                    Show_Saggital top_showSag = new Show_Saggital();
                    top_showSag.InitializeComponent();
                    top_showSag.pictureBox1.Image = bitmap;

                    //top_showSag.FormClosed += Frm_RealTime_Navigation_ZLX.frm_RealTime_Navigation_ZLX.top_showSag_closed;
                    top_showSag.FormClosed += frm_RealTime_Navigation_ZLX_Short.top_showSag_closed;
                    //return;

                    top_showSag.Text = "Top";
                    top_showSag.ShowDialog();

                }
                else
                {
                    Frm_RealTime_Navigation_ZLX frm_RealTime_Navigation_ZLX = new Frm_RealTime_Navigation_ZLX();
                    frm_RealTime_Navigation_ZLX.Show();
                    frm_RealTime_Navigation_ZLX.Initialize_five_Picture();
                    
                    // ---------------------------- save to mat for MIAS--------------------------//
                    // Open Remove muscle UI and display result in Mainform
                    //Frm_RealTime_Navigation_ZLX.Frm_RealTime_Navigation_ZLX.panelMain.Visible = true;

                    coronalImage = VolumeProjection.SaggitalProjection(Frm_RealTime_Navigation_ZLX.volume);
                    //Console.WriteLine(frm_RealTime_Navigation_ZLX.vol)


                    byte[,] byteG_c = ConvertFile.ShortArrayToByte(coronalImage);
                    Mat srcG_c = ConvertFile.ArrayToMat(byteG_c);
                    Bitmap bitmap = ConvertFile.MatToBitmap(srcG_c);


                    Show_Saggital top_showSag = new Show_Saggital();
                    top_showSag.InitializeComponent();
                    top_showSag.pictureBox1.Image = bitmap;

                    //top_showSag.FormClosed += Frm_RealTime_Navigation_ZLX.frm_RealTime_Navigation_ZLX.top_showSag_closed;
                    top_showSag.FormClosed += frm_RealTime_Navigation_ZLX.top_showSag_closed;
                    //return;

                    top_showSag.Text = "Top";
                    top_showSag.ShowDialog();

                }



            }
        }


        public static void SaveVolume2Mat(string savedPath, byte[][,] volume, string fileName,float voxelWidth, float voxelHeight, float voxelDepth,bool isRealTime)
        {
            // Save reconstructed volume as MIAS format
            //* Step 1, swap the usData in x axis
            Console.WriteLine("[+] Write to mat File...");
            csmatio.io.MatFileWriterEx writer = new csmatio.io.MatFileWriterEx(savedPath, false);

            //short[] saveVolume = Array.ConvertAll(volume, b => (short[,])b);

            //short[][,] saveVolume = (short[][,]) volume;
            WriteVolumeToMatFile(writer, "usData", volume, isRealTime);

            //* Step 2, save usData_firstFrameV1, usData_firstFrameV1, usDataConcat, usDataVoxelSize, usDataOrg
            writer.WriteArray(new csmatio.types.MLChar("usDataConcat", "usData;"));
            writer.WriteArray(new csmatio.types.MLDouble("usDataVoxelSize", new double[] { voxelWidth, voxelHeight, voxelDepth }, 1));
            writer.WriteArray(new csmatio.types.MLDouble("usDataOrg", new double[] { 0f, 0f, 0f }, 1));

            writer.WriteArray(new csmatio.types.MLDouble("usData_firstFrameV1", new double[] { 0f, 0f, 0f }, 1));
            writer.WriteArray(new csmatio.types.MLDouble("usData_firstFrameV2", new double[] { 0f, 0f, 0f }, 1));

            //* Step 3, save metaData
            //·· Create usData_metaData entry
            List<object> metaData = new List<object>();
            metaData.Add("b8FileName");
            metaData.Add(fileName);
            metaData.Add("transmitterTransform");
            metaData.Add(new float[16]);
            metaData.Add("gpsCalibration_top");
            metaData.Add(new float[3]);
            metaData.Add("gpsCalibration_bottom");
            metaData.Add(new float[3]);

            MemoryStream memStream = new MemoryStream(1024 * 1024);
            new SoapFormatter().Serialize(memStream, metaData.ToArray());
            writer.WriteArray(new MLUInt8("usData_metaData", memStream.ToArray(), 1));

            Console.WriteLine("Done");
            writer.Close();
        }

        public static void SaveVolume2Mat(string savedPath, short[][,] volume, string fileName, float voxelWidth, float voxelHeight, float voxelDepth, bool isRealTime)
        {
            // Save reconstructed volume as MIAS format
            //* Step 1, swap the usData in x axis
            Console.WriteLine("[+] Write to mat File...");
            csmatio.io.MatFileWriterEx writer = new csmatio.io.MatFileWriterEx(savedPath, false);

            //short[] saveVolume = Array.ConvertAll(volume, b => (short[,])b);

            //short[][,] saveVolume = (short[][,]) volume;
            WriteVolumeToMatFile(writer, "usData", volume, isRealTime);

            //* Step 2, save usData_firstFrameV1, usData_firstFrameV1, usDataConcat, usDataVoxelSize, usDataOrg
            writer.WriteArray(new csmatio.types.MLChar("usDataConcat", "usData;"));
            writer.WriteArray(new csmatio.types.MLDouble("usDataVoxelSize", new double[] { voxelWidth, voxelHeight, voxelDepth }, 1));
            writer.WriteArray(new csmatio.types.MLDouble("usDataOrg", new double[] { 0f, 0f, 0f }, 1));

            writer.WriteArray(new csmatio.types.MLDouble("usData_firstFrameV1", new double[] { 0f, 0f, 0f }, 1));
            writer.WriteArray(new csmatio.types.MLDouble("usData_firstFrameV2", new double[] { 0f, 0f, 0f }, 1));

            //* Step 3, save metaData
            //·· Create usData_metaData entry
            List<object> metaData = new List<object>();
            metaData.Add("b8FileName");
            metaData.Add(fileName);
            metaData.Add("transmitterTransform");
            metaData.Add(new float[16]);
            metaData.Add("gpsCalibration_top");
            metaData.Add(new float[3]);
            metaData.Add("gpsCalibration_bottom");
            metaData.Add(new float[3]);

            MemoryStream memStream = new MemoryStream(1024 * 1024);
            new SoapFormatter().Serialize(memStream, metaData.ToArray());
            writer.WriteArray(new MLUInt8("usData_metaData", memStream.ToArray(), 1));

            Console.WriteLine("Done");
            writer.Close();
        }

        static byte[] m_tempBuffer = null;
        /// <summary>
        /// Save usData to mat. Quick. From Duck
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="arrayName"></param>
        /// <param name="vol"></param>
        private static void WriteVolumeToMatFile(MatFileWriterEx writer, String arrayName, byte[][,] vol,bool isRealTime)
        {
            int VOL_DEPTH = vol.Length;
            int VOL_HEIGHT  = vol[0].GetLength(0);
            int VOL_WIDTH = vol[0].GetLength(1);
            writer.WriteNumericMatrix_Header(
                MLArray.mxINT16_CLASS,
                new int[] { VOL_HEIGHT, VOL_WIDTH, VOL_DEPTH },
                arrayName);

            String backspaces = new string('\b', 32);
            unsafe
            {
                if (m_tempBuffer != null)
                    if (m_tempBuffer.Length != VOL_WIDTH * VOL_HEIGHT * sizeof(short))
                        m_tempBuffer = null;
                if (m_tempBuffer == null)
                    m_tempBuffer = new byte[VOL_WIDTH * VOL_HEIGHT * sizeof(short)];

                Console.Write("[+]Write to file...");
                fixed (byte* pBufferBytes = m_tempBuffer)
                {
                    for (int z = 0; z < VOL_DEPTH; z++)
                    {
                        short* pBuffer = (short*)pBufferBytes;
                        for (int x = 0; x < VOL_WIDTH; x++)
                            for (int y = 0; y < VOL_HEIGHT; y++)
                            {
                                if (isRealTime)
                                {
                                    short val = vol[z][y, VOL_WIDTH - x - 1];
                                    *(pBuffer++) = val == short.MaxValue ? (short)0 : (short)val;
                                }
                                else
                                {
                                    short val = vol[z][VOL_HEIGHT - y - 1, VOL_WIDTH - x - 1];
                                    *(pBuffer++) = val == short.MaxValue ? (short)0 : (short)val;
                                }
                            }
                        writer.WriteNumericMatrix_PartialData(m_tempBuffer);
                        Console.Write("{0}[+]Write to file...{1}%", backspaces, z * 100 / VOL_DEPTH);
                    }
                }

            }
            writer.WriteNumericMatrix_End();
            Console.WriteLine();
            return;
        }

        /// <summary>
        /// Save usData to mat. Quick. From Duck
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="arrayName"></param>
        /// <param name="vol"></param>
        private static void WriteVolumeToMatFile(MatFileWriterEx writer, String arrayName, short[][,] vol, bool isRealTime)
        {
            int VOL_DEPTH = vol.Length;
            int VOL_HEIGHT = vol[0].GetLength(0);
            int VOL_WIDTH = vol[0].GetLength(1);
            writer.WriteNumericMatrix_Header(
                MLArray.mxINT16_CLASS,
                new int[] { VOL_HEIGHT, VOL_WIDTH, VOL_DEPTH },
                arrayName);

            String backspaces = new string('\b', 32);
            unsafe
            {
                if (m_tempBuffer != null)
                    if (m_tempBuffer.Length != VOL_WIDTH * VOL_HEIGHT * sizeof(short))
                        m_tempBuffer = null;
                if (m_tempBuffer == null)
                    m_tempBuffer = new byte[VOL_WIDTH * VOL_HEIGHT * sizeof(short)];

                Console.Write("[+]Write to file...");
                fixed (byte* pBufferBytes = m_tempBuffer)
                {
                    for (int z = 0; z < VOL_DEPTH; z++)
                    {
                        short* pBuffer = (short*)pBufferBytes;
                        for (int x = 0; x < VOL_WIDTH; x++)
                            for (int y = 0; y < VOL_HEIGHT; y++)
                            {
                                if (isRealTime)
                                {
                                    short val = vol[z][VOL_HEIGHT - y - 1, VOL_WIDTH - x - 1];
                                    *(pBuffer++) = val == short.MaxValue ? (short)0 : (short)val;
                                }
                                else
                                {
                                    short val = vol[z][VOL_HEIGHT - y - 1, VOL_WIDTH - x - 1];
                                    *(pBuffer++) = val == short.MaxValue ? (short)0 : (short)val;
                                }
                            }
                        writer.WriteNumericMatrix_PartialData(m_tempBuffer);
                        Console.Write("{0}[+]Write to file...{1}%", backspaces, z * 100 / VOL_DEPTH);
                    }
                }

            }
            writer.WriteNumericMatrix_End();
            Console.WriteLine();
            return;
        }

        public void CancelBackgroudWorker1()
        {
            dlg_WaitingForm.progressBarWaiting.Style = ProgressBarStyle.Continuous;
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }

        // update in 1s
        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan timeSpan = DateTime.Now - beginTime;
            dlg_WaitingForm.labelTimeShow.Text = $"{ timeSpan.TotalSeconds:00} s";
              
        }

        private void numericUpDownFrameIndex_ValueChanged(object sender, EventArgs e)
        {
            this.hScrollBarFrames.Value = (int)this.numericUpDownFrameIndex.Value;
        }

        private short[][,] UshortToShort(ushort[][,] u_volume)
        {
            int height = u_volume[0].GetLength(0);
            int width = u_volume[0].GetLength(1);
            int depth = u_volume.Length;


            short[][,] s_volume = new short[depth][,];
            short[][,] return_volume = null;
            for (int i = 0; i < depth; i++)
            {
                s_volume[i] = new short[height, width];
            }

            bool BreakForNullFlag = false;
            for(int i = 0; i < depth; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    for(int k = 0; k < width; k++)
                    {
                        try
                        {
                            s_volume[i][j, k] = Convert.ToInt16(u_volume[i][j, k]);
                        }
                        catch
                        {
                            BreakForNullFlag = true;
                            break;
                        }
                        
                    }

                    if(BreakForNullFlag)
                    {
                        break;
                    }
                }
                if (BreakForNullFlag)
                {
                    return_volume = new short[i][,];
                    for(int ii = 0; ii < i; ii++)
                    {
                        return_volume[ii] = s_volume[ii];
                    }
                    break;
                }
            }


            return return_volume;
        }
    }
}
