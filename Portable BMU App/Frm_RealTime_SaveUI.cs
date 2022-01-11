using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portable_BMU_App
{
    public partial class Frm_RealTime_SaveUI : Form
    {
        public MainForm mainForm1;
        public string selectedPath = string.Empty;
        public string patientID = string.Empty;      // save which folder in selected path
        public string dataName = string.Empty;       // save what name for acquired data
        public string rootPath = string.Empty;       // Initialize the saved file path.
        public Frm_RealTime_SaveUI()
        {
            InitializeComponent();

            string[] selectedType = System.Enum.GetNames(typeof(SavedDataType));
            this.checkedListBoxSetDataToExport.Items.AddRange(selectedType);
            this.checkedListBoxSetDataToExport.CheckOnClick = true;
            this.checkedListBoxSetDataToExport.SetItemChecked(0, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(1, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(2, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(3, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(4, true);

        }

        public Frm_RealTime_SaveUI(MainForm mainForm)
        {
            InitializeComponent();
            mainForm1 = mainForm;


            string[] selectedType = { "Volume .mat", "Transverse image .clarius", "Location data .g4gps", "Coronal image .jpg" , "Navigation data .nagps"};
            this.checkedListBoxSetDataToExport.Items.AddRange(selectedType);
            this.checkedListBoxSetDataToExport.CheckOnClick = true;
            this.checkedListBoxSetDataToExport.SetItemChecked(0, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(1, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(2, true);
            this.checkedListBoxSetDataToExport.SetItemChecked(3, true);
            //this.checkedListBoxSetDataToExport.SetItemChecked(4, true);

        }

        void InitialSavePath()
        {
            if (Properties.Settings.Default.SavedRootPath == string.Empty)
            {
                rootPath = @"D:\Data";  // The path to store the G4 and Clarius data
            }
            else
            {
                rootPath = Properties.Settings.Default.SavedRootPath;
            }
            string folderNameWithTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"); // Use current time as patient name (the folder name to save)
            string selectedPath = Path.Combine(rootPath, folderNameWithTime);

            this.textBoxDataFolder.Text = rootPath;
            this.textBoxPatientID.Text = folderNameWithTime;
            this.textBoxDataName.Text = folderNameWithTime;

            this.selectedPath = rootPath;
            this.patientID = folderNameWithTime;
            this.dataName = folderNameWithTime;
        }

        public void GetSavePath()
        {
            string folderName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"); // Use current time as tye name of folder 


            // Prompts user to select a folder 
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Pleae select a path where data will be saved";
            folderBrowserDialog.ShowNewFolderButton = true;                  // Forbide create new folder
            folderBrowserDialog.SelectedPath = rootPath;
            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            this.textBoxDataFolder.Text = folderBrowserDialog.SelectedPath;    // this operation will trigger the textBoxChanged event
            this.selectedPath = folderBrowserDialog.SelectedPath;

            Properties.Settings.Default.SavedRootPath = this.selectedPath;
        }

        void ButtonOkProcess()
        {
            SavedDataType savedDataType = new SavedDataType();
            for (int i = 0; i < this.checkedListBoxSetDataToExport.Items.Count; i++)
            {
                if (checkedListBoxSetDataToExport.GetItemChecked(i))
                {
                    savedDataType = savedDataType | (SavedDataType)(1 << i);
                }
            }

            // Transfer selected save type
            mainForm1.savedDataType = savedDataType;

            // Transfer save path
            mainForm1.savePath = Path.Combine( this.selectedPath,this.patientID);
            mainForm1.saveDataName = this.dataName;

            Console.WriteLine("savepath: {0}",mainForm1.savePath);
            Console.WriteLine("saveFolderName: {0}",mainForm1.saveDataName);
            Properties.Settings.Default.Save();

            // Close window and dispose resources
            this.Dispose();

        }

        private void Frm_RealTime_SaveUI_Load(object sender, EventArgs e)
        {
            InitialSavePath();

        }

        private void btnDataFolderSelected_Click(object sender, EventArgs e)
        {
            GetSavePath();
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            ButtonOkProcess();
        }

        private void Frm_RealTime_SaveUI_Activated(object sender, EventArgs e)
        {
            // Set default focus on buttonOK
            this.buttonOk.Focus();
        }

        private void textBoxPatientID_Validated(object sender, EventArgs e)
        {
            this.patientID = this.textBoxPatientID.Text;
            this.dataName = this.textBoxPatientID.Text; // dataName should be changed following patientID
        }

        private void textBoxDataName_Validated(object sender, EventArgs e)
        {
            this.dataName = this.textBoxDataName.Text;
        }

        private void textBoxPatientID_TextChanged(object sender, EventArgs e)
        {
            //Console.WriteLine(this.textBoxPatientID.Text);
            this.textBoxDataName.Text = this.textBoxPatientID.Text;  // dataName should be changed following patientID
        }
    }
}
