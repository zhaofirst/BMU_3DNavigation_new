namespace Portable_BMU_App
{
    partial class Frm_RealTime_SaveUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RealTime_SaveUI));
            this.textBoxDataFolder = new System.Windows.Forms.TextBox();
            this.labelSaveFolder = new System.Windows.Forms.Label();
            this.labelFilename = new System.Windows.Forms.Label();
            this.labelPatientID = new System.Windows.Forms.Label();
            this.textBoxPatientID = new System.Windows.Forms.TextBox();
            this.textBoxDataName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.checkedListBoxSetDataToExport = new System.Windows.Forms.CheckedListBox();
            this.btnDataFolderSelected = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDataFolder
            // 
            this.textBoxDataFolder.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDataFolder.Location = new System.Drawing.Point(252, 45);
            this.textBoxDataFolder.Name = "textBoxDataFolder";
            this.textBoxDataFolder.Size = new System.Drawing.Size(954, 44);
            this.textBoxDataFolder.TabIndex = 0;
            // 
            // labelSaveFolder
            // 
            this.labelSaveFolder.AutoSize = true;
            this.labelSaveFolder.Font = new System.Drawing.Font("Times New Roman", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSaveFolder.Location = new System.Drawing.Point(21, 43);
            this.labelSaveFolder.Name = "labelSaveFolder";
            this.labelSaveFolder.Size = new System.Drawing.Size(197, 42);
            this.labelSaveFolder.TabIndex = 3;
            this.labelSaveFolder.Text = "Data Folder:";
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.Font = new System.Drawing.Font("Times New Roman", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFilename.Location = new System.Drawing.Point(21, 221);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(193, 42);
            this.labelFilename.TabIndex = 4;
            this.labelFilename.Text = "Data Name:";
            // 
            // labelPatientID
            // 
            this.labelPatientID.AutoSize = true;
            this.labelPatientID.Font = new System.Drawing.Font("Times New Roman", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPatientID.Location = new System.Drawing.Point(23, 133);
            this.labelPatientID.Name = "labelPatientID";
            this.labelPatientID.Size = new System.Drawing.Size(175, 42);
            this.labelPatientID.TabIndex = 5;
            this.labelPatientID.Text = "Patient ID:";
            // 
            // textBoxPatientID
            // 
            this.textBoxPatientID.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPatientID.Location = new System.Drawing.Point(252, 135);
            this.textBoxPatientID.Name = "textBoxPatientID";
            this.textBoxPatientID.Size = new System.Drawing.Size(413, 44);
            this.textBoxPatientID.TabIndex = 6;
            this.textBoxPatientID.TextChanged += new System.EventHandler(this.textBoxPatientID_TextChanged);
            this.textBoxPatientID.Validated += new System.EventHandler(this.textBoxPatientID_Validated);
            // 
            // textBoxDataName
            // 
            this.textBoxDataName.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDataName.Location = new System.Drawing.Point(252, 223);
            this.textBoxDataName.Name = "textBoxDataName";
            this.textBoxDataName.Size = new System.Drawing.Size(413, 44);
            this.textBoxDataName.TabIndex = 7;
            this.textBoxDataName.Validated += new System.EventHandler(this.textBoxDataName_Validated);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1456, 985);
            this.panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(28, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(337, 37);
            this.label1.TabIndex = 11;
            this.label1.Text = "*Export Settings";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.buttonOk);
            this.panel2.Controls.Add(this.checkedListBoxSetDataToExport);
            this.panel2.Controls.Add(this.labelSaveFolder);
            this.panel2.Controls.Add(this.labelPatientID);
            this.panel2.Controls.Add(this.btnDataFolderSelected);
            this.panel2.Controls.Add(this.labelFilename);
            this.panel2.Controls.Add(this.textBoxDataFolder);
            this.panel2.Controls.Add(this.textBoxPatientID);
            this.panel2.Controls.Add(this.textBoxDataName);
            this.panel2.Location = new System.Drawing.Point(35, 91);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1384, 849);
            this.panel2.TabIndex = 10;
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOk.Location = new System.Drawing.Point(1197, 765);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(154, 68);
            this.buttonOk.TabIndex = 10;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // checkedListBoxSetDataToExport
            // 
            this.checkedListBoxSetDataToExport.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBoxSetDataToExport.FormattingEnabled = true;
            this.checkedListBoxSetDataToExport.Location = new System.Drawing.Point(28, 311);
            this.checkedListBoxSetDataToExport.Name = "checkedListBoxSetDataToExport";
            this.checkedListBoxSetDataToExport.Size = new System.Drawing.Size(1323, 433);
            this.checkedListBoxSetDataToExport.TabIndex = 9;
            // 
            // btnDataFolderSelected
            // 
            this.btnDataFolderSelected.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDataFolderSelected.Location = new System.Drawing.Point(1230, 45);
            this.btnDataFolderSelected.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDataFolderSelected.Name = "btnDataFolderSelected";
            this.btnDataFolderSelected.Size = new System.Drawing.Size(123, 47);
            this.btnDataFolderSelected.TabIndex = 8;
            this.btnDataFolderSelected.Text = "...";
            this.btnDataFolderSelected.UseVisualStyleBackColor = true;
            this.btnDataFolderSelected.Click += new System.EventHandler(this.btnDataFolderSelected_Click);
            // 
            // Frm_RealTime_SaveUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1456, 985);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_RealTime_SaveUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Activated += new System.EventHandler(this.Frm_RealTime_SaveUI_Activated);
            this.Load += new System.EventHandler(this.Frm_RealTime_SaveUI_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDataFolder;
        private System.Windows.Forms.Label labelSaveFolder;
        private System.Windows.Forms.Label labelFilename;
        private System.Windows.Forms.Label labelPatientID;
        private System.Windows.Forms.TextBox textBoxPatientID;
        private System.Windows.Forms.TextBox textBoxDataName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDataFolderSelected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckedListBox checkedListBoxSetDataToExport;
        private System.Windows.Forms.Button buttonOk;
    }
}