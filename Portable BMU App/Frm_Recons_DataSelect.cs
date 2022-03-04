using OpenCvSharp;
using Portable_BMU_App.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ultrasonics3DReconstructor;

namespace Portable_BMU_App
{
    public partial class Frm_Recons_DataSelect : Form
    {
        // Record current frm
        public static Frm_Recons_DataSelect record_recons_DataSelect;

        // Record file information in selected path
        const int DATASETSIZE = 5;
        public string selectedPath = null;
        List<DataSetInfo> usDataSetInfos = new List<DataSetInfo>();
        public float LATERAL_X;
        public float LATERAL_Y;

        static string imgExtern = ".clarius";
        static string gpsExtern = ".g4gps";
        bool isImgExisted = false; bool isGpsExisted = false; bool isXmlExisted = true;
        public static bool isChildDirectry = false;
        MainForm mainform1;
        public Frm_Recons_DataSelect(MainForm mainForm)
        {
            InitializeComponent();
            record_recons_DataSelect = this;            // Record current frm
            this.checkBoxPlumbLine.Checked = true;     // Default using plumb line
            mainform1 = mainForm;
        }

        // Enter frm_Recons_DataExport when Next button click, and close current frm
        private void btnNext_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            USDataCollection2 usDataCollection;
            usDataCollection = ReadDataSet();

            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            //return;

            //Hide current frm, so frm_DataExport can back here
            this.Hide();
            Settings.Default.Save();

            Frm_Recons_DataExport frm_Recons_DataExport = new Frm_Recons_DataExport(usDataCollection);
            frm_Recons_DataExport.Show();

        }

        // Select the path where the data resides is located
        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Prompts user to select a folder 
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Pleae select a path where usData is located";
            folderBrowserDialog.ShowNewFolderButton = false;                  // Forbide create new folder
            folderBrowserDialog.SelectedPath = this.selectedPath;
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            this.selectedPath = folderBrowserDialog.SelectedPath;
            this.textBoxShowSeletedPath.Text = folderBrowserDialog.SelectedPath;    // this operation will trigger the textBoxChanged event
            Console.WriteLine("Selected Path: {0}", selectedPath);
            // Empty this List After USDataCollection was acquired, Or some error would be happen when Back option was triggered in Export Frm, 



            // update data set information according to selected path 
            UpdateDataSetInfo();

        }

        /// <summary>
        /// Inverse the coordinate of Z axis
        /// </summary>
        /// <returns></returns>
        public Rect3[] InverseGPS_Z(Rect3[] rect3)
        {
            //Rect3[] rectangle = new Rect3[];

            for (int i = 0; i < rect3.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rect3[i].v[j].Z = 0f - rect3[i].v[j].Z;
                }
            }

            return rect3;
        }

        public USDataCollection2 ReadDataSet()
        {
            // Read data from folder            
            // determine which checkBox is checked in checkedBoxList
            //Console.WriteLine(checkedListBoxDataSetSelect.Items.Count);

            USDataCollection2 usDataCollection = null;
            int checkedIndex = -1;

            /// Select multi-dataset

            if (isChildDirectry)
            {
                Console.WriteLine(">> -------- Left data-------- <<");

                // L: Read data from folder            
                USFileReader usFileReaderL = new USFileReader();

                checkedIndex = 0;
                usFileReaderL.ReadClarius(usDataSetInfos[checkedIndex].imgPath);

                numericUpDownLX.Value = new decimal(usFileReaderL.LATERALXY * 0.001);
                numericUpDownLY.Value = new decimal(usFileReaderL.LATERALXY * 0.001);
                Properties.Settings.Default.Lateral = usFileReaderL.LATERALXY;

                usFileReaderL.ReadG4(usDataSetInfos[checkedIndex].gpsPath);

                // R: Read data from folder    
                checkedIndex = 1;
                Console.WriteLine(">> -------- Right data -------- <<");
                USFileReader usFileReaderR = new USFileReader();
                usFileReaderR.ReadClarius(usDataSetInfos[1].imgPath);

                numericUpDownLX.Value = new decimal(usFileReaderR.LATERALXY * 0.001);
                numericUpDownLY.Value = new decimal(usFileReaderR.LATERALXY * 0.001);
                Properties.Settings.Default.Lateral = usFileReaderR.LATERALXY;

                usFileReaderR.ReadG4(usDataSetInfos[checkedIndex].gpsPath);
                int frameCounts = usFileReaderL.frameCounts + usFileReaderR.frameCounts;

                // Combine L and R
                byte[][,] framesLR = new byte[frameCounts][,];
                usFileReaderL.frames.CopyTo(framesLR, 0);
                usFileReaderR.frames.CopyTo(framesLR, usFileReaderL.frameCounts);

                Rect3[] rectsLR = new Rect3[frameCounts];
                usFileReaderL.myRectangles.CopyTo(rectsLR, 0);
                usFileReaderR.myRectangles.CopyTo(rectsLR, usFileReaderL.frameCounts);

                //Mat srcG = ConvertFile.ArrayToMat(framesLR[0]);
                //Mat srcG2 = ConvertFile.ArrayToMat(framesLR[usFileReader.frameCounts]);

                //Cv2.ImShow("a", srcG);
                //Cv2.ImShow("b", srcG2);
                //return null;
                Transformation transformation = new Transformation();
                //transformation.gpsCalibrationBottom = usFileReader.m_gpsCalibrationBottom;
                //transformation.gpsCalibrationTop = usFileReader.m_gpsCalibrationTop;
                transformation.rects = usFileReaderL.myRectangles;

                if (this.checkBoxPlumbLine.Checked)
                {
                    Frm_RealTime_Navigation_ZLX.calibrationMatrix = transformation.GetCalibrationMatrixWyHxForPlumbLine();
                }
                else
                {
                    Frm_RealTime_Navigation_ZLX.calibrationMatrix = transformation.GetCalibrationMatrixWyHx();
                }

                for (int k = 0; k < frameCounts; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = rectsLR[k].v[i];
                        rectsLR[k].v[i] = Vector3.Transform(tempVector, Frm_RealTime_Navigation_ZLX.calibrationMatrix);
                    }
                }

                // Inverse X
                for (int i = 0; i < frameCounts; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        rectsLR[i].v[j].X = 0f - rectsLR[i].v[j].X;
                    }
                }


                Console.WriteLine(">> -------- Transformation LR done -------- <<");
                Console.WriteLine("\n");

                checkedIndex = 0;
                usDataCollection = new USDataCollection2(frameCounts, framesLR,
                    framesLR, rectsLR, usDataSetInfos[checkedIndex].fileName + "_LR", this.selectedPath);
            }

            /// Only one 
            else
            {
                for (int i = 0; i < checkedListBoxDataSetSelect.Items.Count; i++)
                {
                    if (checkedListBoxDataSetSelect.GetItemChecked(i))
                    {
                        //Console.WriteLine(i);
                        checkedIndex = i;

                        break;
                    }
                }

                if (checkedIndex == -1)
                {
                    MessageBox.Show("Please select a data set!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                // Read data from folder            
                USFileReader usFileReader = new USFileReader();
                //usFileReader.LATERAL_X = this.LATERAL_X;
                //usFileReader.LATERAL_Y = this.LATERAL_Y;

                usFileReader.ReadClarius(usDataSetInfos[checkedIndex].imgPath);

                numericUpDownLX.Value = new decimal(usFileReader.LATERALXY * 0.001);
                numericUpDownLY.Value = new decimal(usFileReader.LATERALXY * 0.001);
                Properties.Settings.Default.Lateral = usFileReader.LATERALXY;

                usFileReader.ReadG4(usDataSetInfos[checkedIndex].gpsPath);
                //usFileReader.ReadMetaData(usDataSetInfos[checkedIndex].xmlPath);

                //usFileReader.m_Rectangles = InverseGPS_Z(usFileReader.m_Rectangles);

                Transformation transformation = new Transformation();
                //transformation.gpsCalibrationBottom = usFileReader.m_gpsCalibrationBottom;
                //transformation.gpsCalibrationTop = usFileReader.m_gpsCalibrationTop;
                transformation.rects = usFileReader.myRectangles;

                for (int i = 0; i < 2; i++)
                {
                    if (this.checkBoxPlumbLine.Checked)
                    {
                        //transformation.CoordinateTransformationForPortableUsingMIASWithPlumbLine();
                        Frm_RealTime_Navigation_ZLX.calibrationMatrix = transformation.GetCalibrationMatrixWyHxForPlumbLine();

                    }
                    else
                    {
                        //transformation.CoordinateTransformationForPortableUsingMIAS();
                        Frm_RealTime_Navigation_ZLX.calibrationMatrix = transformation.GetCalibrationMatrixWyHx();

                        //transformation.CoordinateTransformationWithoutTopBottom();
                    }
                }

                for (int k = 0; k < usFileReader.frameCounts; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 tempVector = new Vector3();
                        tempVector = transformation.rects[k].v[i];
                        transformation.rects[k].v[i] = Vector3.Transform(tempVector, Frm_RealTime_Navigation_ZLX.calibrationMatrix);
                    }
                }

                transformation.InverseGPS_X();
                //transformation.InverseGPS_Y();

                Console.WriteLine("-------- Transformation done --------");
                Console.WriteLine("\n");


                usDataCollection = new USDataCollection2(usFileReader.frameCounts, usFileReader.frames,
                    usFileReader.changeFrames, transformation.rects, usDataSetInfos[checkedIndex].fileName, this.selectedPath);
            }



            // Empty this List After USDataCollection was acquired, Or  some error would be happen when Back option was triggered in Export Frm, 
            //usDataSetInfos.Clear();

            return usDataCollection;
        }

        public void UpdateDataSetInfo()
        {
            if (this.selectedPath.Length <= 0)
            {
                MessageBox.Show("Pleae select a path firstly!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxShowSeletedPath.Clear();    // clear current textBox
                this.textBoxShowSeletedPath.Focus();    // let the cursor focus on the textBox   
                return;
            }

            // Disenable NextButton and show the reminder
            labelNextReminder.Visible = true;
            btnNext.Enabled = false;




            // Get all the files' full path in the folder
            FileInfo[] filesInFolder = null;
            try
            {
                //filesInFolder = Directory.GetFiles(this.selectedPath);
                DirectoryInfo dir = new DirectoryInfo(this.selectedPath);
                filesInFolder = dir.GetFiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            List<string> dataFileFullPath2 = new List<string>();

            //GetFile(this.selectedPath, dataFileFullPath2);
            //Console.WriteLine();
            // maybe there exists multi-b8 in a folder
            List<string> dataFileFullPath = new List<string>();
            isImgExisted = false; isGpsExisted = false; isXmlExisted = true; isChildDirectry = false;
            GetFile(this.selectedPath, dataFileFullPath);

            //foreach (FileInfo file in filesInFolder)
            //{
            //    Console.WriteLine(file);
            //    string extension = Path.GetExtension(file.FullName);

            //    // Get the file full path of b8File
            //    if (extension == imgExtern)
            //    {
            //        isImgExisted = true;
            //        dataFileFullPath.Add(file.FullName);
            //    }
            //    else if (extension == gpsExtern)
            //    {
            //        isGpsExisted = true;
            //    }
            //    //else if (extension == ".xml")
            //    //{
            //    //    xmlFlag = true;
            //    //}

            //}

            #region If no usData in current directory, throw a warning window and return

            if (!isImgExisted && !isGpsExisted && !isXmlExisted)
            {
                MessageBox.Show("No usData in current directory!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxShowSeletedPath.Clear();    // clear current textBox
                this.textBoxShowSeletedPath.Focus();    // let the cursor focus on the textBox   

                Settings.Default.DataFolder = null;     // If no find usData in current directory, set Default DataFolder to Null
                Settings.Default.Save();
                return;
            }
            else if (!isImgExisted)
            {
                MessageBox.Show("No " + imgExtern + " File in current directory!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxShowSeletedPath.Clear();    // clear current textBox
                this.textBoxShowSeletedPath.Focus();    // let the cursor focus on the textBox   
                return;
            }
            else if (!isGpsExisted)
            {
                MessageBox.Show("No " + gpsExtern + " File in current directory!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxShowSeletedPath.Clear();    // clear current textBox
                this.textBoxShowSeletedPath.Focus();    // let the cursor focus on the textBox   
                return;
            }
            //else if (!xmlFlag)
            //{
            //    MessageBox.Show("No .xml File in current directory!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    this.textBoxShowSeletedPath.Clear();    // clear current textBox
            //    this.textBoxShowSeletedPath.Focus();    // let the cursor focus on the textBox   
            //    return;
            //}
            #endregion

            this.checkedListBoxDataSetSelect.Items.Clear();

            if (usDataSetInfos.Count > 0)
                usDataSetInfos.Clear();

            // Solve the multi-b8File
            for (int i = 0; i < dataFileFullPath.Count; i++)
            {
                // according the b8 file full path to get all the files' path
                DataSetInfo dataSetInfo = new DataSetInfo();
                //Console.WriteLine(b8FileFullPath[i]);
                dataSetInfo.fileName = Path.GetFileNameWithoutExtension(dataFileFullPath[i]);
                dataSetInfo.fileDirectory = Path.GetDirectoryName(dataFileFullPath[i]);
                //Console.WriteLine(dataSetInfo.fileName);

                //dataSetInfo.imgPath = Path.Combine(this.selectedPath, dataSetInfo.fileName) + imgExtern;
                //dataSetInfo.gpsPath = Path.Combine(this.selectedPath, dataSetInfo.fileName) + gpsExtern;
                //dataSetInfo.xmlPath = Path.Combine(this.selectedPath, dataSetInfo.fileName) + ".xml";
                dataSetInfo.imgPath = Path.Combine(dataSetInfo.fileDirectory, dataSetInfo.fileName) + imgExtern;
                dataSetInfo.gpsPath = Path.Combine(dataSetInfo.fileDirectory, dataSetInfo.fileName) + gpsExtern;

                //Console.WriteLine(dataSetInfo.imgPath);
                usDataSetInfos.Add(dataSetInfo);

                // Show the fileName as checkBox in checkBox List
                this.checkedListBoxDataSetSelect.Items.Add(dataSetInfo.fileName);
            }

            // Set the firt dataSet as checked
            this.checkedListBoxDataSetSelect.SetItemChecked(0, true);

            if (isChildDirectry)
            {
                // If there has the child directory, Set the second dataSet as checked
                this.checkedListBoxDataSetSelect.SetItemChecked(1, true);
            }


            // If Path is valid, Enable NextButton 
            labelNextReminder.Visible = false;
            btnNext.Enabled = true;

            // Be sure only save the correct path
            // In order to remmeber the last dataPath when run this programe， we should record this path to Setting file 
            // If not in Realtime status, save current folder
            if (!MainForm.isAllUnitsConencted)
                Settings.Default.DataFolder = this.selectedPath;
            //Settings.Default.Save();
        }

        // Data set information of files in the folder
        public struct DataSetInfo
        {
            public string fileName;
            public string fileDirectory;
            public string imgPath;
            public string gpsPath;
            public string xmlPath;
        }

        /// <summary>
        /// 获取路径下所有文件以及子文件夹中文件
        /// </summary>
        /// <param name="path">全路径根目录</param>
        /// <param name="dataFileFullPath">存放所有文件的全路径</param>
        /// <returns></returns>
        public List<string> GetFile(string path, List<string> dataFileFullPath)
        {

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] filesInFolder = dir.GetFiles();
            DirectoryInfo[] directoryInFolder = dir.GetDirectories();
            foreach (FileInfo file in filesInFolder)
            {

                Console.WriteLine(file.FullName);
                string extension = Path.GetExtension(file.FullName);

                // Get the file full path of b8File
                if (extension == imgExtern)
                {
                    isImgExisted = true;
                    dataFileFullPath.Add(file.FullName);
                }
                else if (extension == gpsExtern)
                {
                    isGpsExisted = true;
                }


            }
            //获取子文件夹内的文件列表，递归遍历
            foreach (DirectoryInfo d in directoryInFolder)
            {
                isChildDirectry = true;
                GetFile(d.FullName, dataFileFullPath);
            }
            return dataFileFullPath;
        }

        private void checkedListBoxDataSetSelect_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //Console.WriteLine("aa");
            // set checkedListBox to radio
            // only one
            //for (int i = 0; i < checkedListBoxDataSetSelect.CheckedIndices.Count; i++)
            //{
            //    if (checkedListBoxDataSetSelect.CheckedIndices[i] != e.Index)
            //    {
            //        checkedListBoxDataSetSelect.SetItemChecked(checkedListBoxDataSetSelect.CheckedIndices[i], false);
            //    }
            //}
        }

        private void numericUpDownLX_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownLY_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Frm_Recons_DataSelect_Load(object sender, EventArgs e)
        {
            //imgExtern = ".clarius";
            //gpsExtern = ".g4gps";

            btnNext.Enabled = false;
            labelNextReminder.Visible = true;

            this.numericUpDownLX.Value = new decimal(Settings.Default.Lateral * 0.001);
            this.numericUpDownLY.Value = new decimal(Settings.Default.Lateral * 0.001);

            this.textBoxShowSeletedPath.Text = Settings.Default.DataFolder;
            this.selectedPath = this.textBoxShowSeletedPath.Text;
            Console.WriteLine("Default Selected Path: {0}", this.selectedPath);



            if (MainForm.isAllUnitsConencted)
            {
                // Realtime status 


                if (!mainform1.isSavedData)
                {

                    this.selectedPath = (mainform1.savePath);  // Use lastest saved path (maybe L or not)

                    /// Save R                
                    string folderName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "R"; // Use current time as the name of folder R
                    mainform1.SaveAllAcquiredData(Path.Combine(this.selectedPath, folderName), folderName);

                    /// Update
                    UpdateDataSetInfo();
                    Console.WriteLine("Change Selected Path to : {0}", this.selectedPath);  // Use lastest saved path (maybe L or not)
                    this.textBoxShowSeletedPath.Text = this.selectedPath;


                }

            }
            else
            {
                // Out of Real-time status

                UpdateDataSetInfo();
            }


            // Choose g4/clarius or Sonix

        }


    }
}
