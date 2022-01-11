namespace Portable_BMU_App
{
    partial class Frm_Recons_DataExport
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Recons_DataExport));
            this.panel2 = new System.Windows.Forms.Panel();
            this.numericUpDownFrameIndex = new System.Windows.Forms.NumericUpDown();
            this.labelFrameMaximumCounts = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelFrameCount = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonDS = new System.Windows.Forms.RadioButton();
            this.radioButtonMPI4 = new System.Windows.Forms.RadioButton();
            this.radioButtonFDP = new System.Windows.Forms.RadioButton();
            this.radioButtonPNN = new System.Windows.Forms.RadioButton();
            this.radioButtonVNN = new System.Windows.Forms.RadioButton();
            this.radioButtonMPI2 = new System.Windows.Forms.RadioButton();
            this.btnNext = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.hScrollBarFrames = new System.Windows.Forms.HScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxTransversShow = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.voxel_depth = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.voxel_height = new System.Windows.Forms.NumericUpDown();
            this.voxel_width = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxCloseOnFinish = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBoxisUseHoleFilling = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrameIndex)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTransversShow)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.voxel_depth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxel_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxel_width)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.numericUpDownFrameIndex);
            this.panel2.Controls.Add(this.labelFrameMaximumCounts);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.labelFrameCount);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.groupBox4);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.hScrollBarFrames);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.checkBoxCloseOnFinish);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1673, 975);
            this.panel2.TabIndex = 42;
            // 
            // numericUpDownFrameIndex
            // 
            this.numericUpDownFrameIndex.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.numericUpDownFrameIndex.DecimalPlaces = 1;
            this.numericUpDownFrameIndex.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDownFrameIndex.Location = new System.Drawing.Point(770, 868);
            this.numericUpDownFrameIndex.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownFrameIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrameIndex.Name = "numericUpDownFrameIndex";
            this.numericUpDownFrameIndex.Size = new System.Drawing.Size(103, 35);
            this.numericUpDownFrameIndex.TabIndex = 45;
            this.numericUpDownFrameIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrameIndex.ValueChanged += new System.EventHandler(this.numericUpDownFrameIndex_ValueChanged);
            // 
            // labelFrameMaximumCounts
            // 
            this.labelFrameMaximumCounts.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelFrameMaximumCounts.AutoSize = true;
            this.labelFrameMaximumCounts.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelFrameMaximumCounts.Location = new System.Drawing.Point(907, 867);
            this.labelFrameMaximumCounts.Name = "labelFrameMaximumCounts";
            this.labelFrameMaximumCounts.Size = new System.Drawing.Size(54, 28);
            this.labelFrameMaximumCounts.TabIndex = 60;
            this.labelFrameMaximumCounts.Text = "...";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(880, 872);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 24);
            this.label5.TabIndex = 59;
            this.label5.Text = "/";
            // 
            // labelFrameCount
            // 
            this.labelFrameCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFrameCount.AutoSize = true;
            this.labelFrameCount.Location = new System.Drawing.Point(1589, 847);
            this.labelFrameCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFrameCount.Name = "labelFrameCount";
            this.labelFrameCount.Size = new System.Drawing.Size(46, 24);
            this.labelFrameCount.TabIndex = 56;
            this.labelFrameCount.Text = "...";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBack.Location = new System.Drawing.Point(1181, 898);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(173, 60);
            this.btnBack.TabIndex = 55;
            this.btnBack.Text = "<=Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(422, 170);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(778, 87);
            this.label4.TabIndex = 54;
            this.label4.Text = "Operations:\r\nUse frame scroll bar to view transverse image.\r\nInput index number i" +
    "nto the folowing textbox to view the particular frame.\r\n";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.checkBoxisUseHoleFilling);
            this.groupBox4.Controls.Add(this.radioButtonDS);
            this.groupBox4.Controls.Add(this.radioButtonMPI4);
            this.groupBox4.Controls.Add(this.radioButtonFDP);
            this.groupBox4.Controls.Add(this.radioButtonPNN);
            this.groupBox4.Controls.Add(this.radioButtonVNN);
            this.groupBox4.Controls.Add(this.radioButtonMPI2);
            this.groupBox4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(1207, 41);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(434, 235);
            this.groupBox4.TabIndex = 53;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Methods";
            // 
            // radioButtonDS
            // 
            this.radioButtonDS.AutoSize = true;
            this.radioButtonDS.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButtonDS.Location = new System.Drawing.Point(155, 184);
            this.radioButtonDS.Name = "radioButtonDS";
            this.radioButtonDS.Size = new System.Drawing.Size(71, 32);
            this.radioButtonDS.TabIndex = 56;
            this.radioButtonDS.Text = "DS";
            this.radioButtonDS.UseVisualStyleBackColor = true;
            this.radioButtonDS.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonMPI4
            // 
            this.radioButtonMPI4.AutoSize = true;
            this.radioButtonMPI4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButtonMPI4.Location = new System.Drawing.Point(155, 113);
            this.radioButtonMPI4.Name = "radioButtonMPI4";
            this.radioButtonMPI4.Size = new System.Drawing.Size(99, 32);
            this.radioButtonMPI4.TabIndex = 55;
            this.radioButtonMPI4.Text = "MPI4";
            this.radioButtonMPI4.UseVisualStyleBackColor = true;
            this.radioButtonMPI4.Click += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonFDP
            // 
            this.radioButtonFDP.AutoSize = true;
            this.radioButtonFDP.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButtonFDP.Location = new System.Drawing.Point(155, 55);
            this.radioButtonFDP.Name = "radioButtonFDP";
            this.radioButtonFDP.Size = new System.Drawing.Size(85, 32);
            this.radioButtonFDP.TabIndex = 54;
            this.radioButtonFDP.Text = "FDP";
            this.radioButtonFDP.UseVisualStyleBackColor = true;
            this.radioButtonFDP.Click += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonPNN
            // 
            this.radioButtonPNN.AutoSize = true;
            this.radioButtonPNN.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButtonPNN.Location = new System.Drawing.Point(24, 183);
            this.radioButtonPNN.Name = "radioButtonPNN";
            this.radioButtonPNN.Size = new System.Drawing.Size(85, 32);
            this.radioButtonPNN.TabIndex = 6;
            this.radioButtonPNN.Text = "PNN";
            this.radioButtonPNN.UseVisualStyleBackColor = true;
            this.radioButtonPNN.Click += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonVNN
            // 
            this.radioButtonVNN.AutoSize = true;
            this.radioButtonVNN.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButtonVNN.Location = new System.Drawing.Point(24, 55);
            this.radioButtonVNN.Name = "radioButtonVNN";
            this.radioButtonVNN.Size = new System.Drawing.Size(85, 32);
            this.radioButtonVNN.TabIndex = 4;
            this.radioButtonVNN.Text = "VNN";
            this.radioButtonVNN.UseVisualStyleBackColor = true;
            this.radioButtonVNN.Click += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonMPI2
            // 
            this.radioButtonMPI2.AutoSize = true;
            this.radioButtonMPI2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButtonMPI2.Location = new System.Drawing.Point(24, 115);
            this.radioButtonMPI2.Name = "radioButtonMPI2";
            this.radioButtonMPI2.Size = new System.Drawing.Size(99, 32);
            this.radioButtonMPI2.TabIndex = 5;
            this.radioButtonMPI2.Text = "MPI2";
            this.radioButtonMPI2.UseVisualStyleBackColor = true;
            this.radioButtonMPI2.Click += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(1468, 898);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(173, 60);
            this.btnNext.TabIndex = 43;
            this.btnNext.Text = "Export";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(25, 843);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 27);
            this.label2.TabIndex = 51;
            this.label2.Text = "Frame";
            // 
            // hScrollBarFrames
            // 
            this.hScrollBarFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBarFrames.LargeChange = 1;
            this.hScrollBarFrames.Location = new System.Drawing.Point(108, 843);
            this.hScrollBarFrames.Maximum = 1438;
            this.hScrollBarFrames.Minimum = 1;
            this.hScrollBarFrames.Name = "hScrollBarFrames";
            this.hScrollBarFrames.Size = new System.Drawing.Size(1477, 26);
            this.hScrollBarFrames.TabIndex = 0;
            this.hScrollBarFrames.Value = 1;
            this.hScrollBarFrames.ValueChanged += new System.EventHandler(this.hScrollBarFrames_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.pictureBoxTransversShow);
            this.panel1.Location = new System.Drawing.Point(29, 283);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1611, 555);
            this.panel1.TabIndex = 50;
            // 
            // pictureBoxTransversShow
            // 
            this.pictureBoxTransversShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxTransversShow.Location = new System.Drawing.Point(443, -2);
            this.pictureBoxTransversShow.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxTransversShow.Name = "pictureBoxTransversShow";
            this.pictureBoxTransversShow.Size = new System.Drawing.Size(692, 547);
            this.pictureBoxTransversShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTransversShow.TabIndex = 1;
            this.pictureBoxTransversShow.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.voxel_depth);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.voxel_height);
            this.groupBox3.Controls.Add(this.voxel_width);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(41, 63);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 213);
            this.groupBox3.TabIndex = 49;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Voxel Size";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(277, 160);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 24);
            this.label8.TabIndex = 44;
            this.label8.Text = "mm";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(20, 157);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 28);
            this.label9.TabIndex = 42;
            this.label9.Text = "Depth:";
            // 
            // voxel_depth
            // 
            this.voxel_depth.DecimalPlaces = 3;
            this.voxel_depth.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.voxel_depth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.voxel_depth.Location = new System.Drawing.Point(136, 157);
            this.voxel_depth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voxel_depth.Name = "voxel_depth";
            this.voxel_depth.Size = new System.Drawing.Size(133, 35);
            this.voxel_depth.TabIndex = 23;
            this.voxel_depth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.voxel_depth.ValueChanged += new System.EventHandler(this.voxel_depth_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(20, 51);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 28);
            this.label10.TabIndex = 36;
            this.label10.Text = "Width:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(277, 105);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 24);
            this.label11.TabIndex = 41;
            this.label11.Text = "mm";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(20, 105);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(110, 28);
            this.label12.TabIndex = 37;
            this.label12.Text = "Height:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(275, 53);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(34, 24);
            this.label13.TabIndex = 40;
            this.label13.Text = "mm";
            // 
            // voxel_height
            // 
            this.voxel_height.DecimalPlaces = 3;
            this.voxel_height.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.voxel_height.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.voxel_height.Location = new System.Drawing.Point(139, 107);
            this.voxel_height.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voxel_height.Name = "voxel_height";
            this.voxel_height.Size = new System.Drawing.Size(131, 35);
            this.voxel_height.TabIndex = 22;
            this.voxel_height.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.voxel_height.ValueChanged += new System.EventHandler(this.voxel_height_ValueChanged);
            // 
            // voxel_width
            // 
            this.voxel_width.DecimalPlaces = 3;
            this.voxel_width.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.voxel_width.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.voxel_width.Location = new System.Drawing.Point(136, 51);
            this.voxel_width.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voxel_width.Name = "voxel_width";
            this.voxel_width.Size = new System.Drawing.Size(131, 35);
            this.voxel_width.TabIndex = 21;
            this.voxel_width.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.voxel_width.ValueChanged += new System.EventHandler(this.voxel_width_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(35, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(397, 37);
            this.label1.TabIndex = 7;
            this.label1.Text = "*Step2: Data Export";
            // 
            // checkBoxCloseOnFinish
            // 
            this.checkBoxCloseOnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxCloseOnFinish.AutoSize = true;
            this.checkBoxCloseOnFinish.Checked = true;
            this.checkBoxCloseOnFinish.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCloseOnFinish.Location = new System.Drawing.Point(41, 926);
            this.checkBoxCloseOnFinish.Name = "checkBoxCloseOnFinish";
            this.checkBoxCloseOnFinish.Size = new System.Drawing.Size(342, 28);
            this.checkBoxCloseOnFinish.TabIndex = 7;
            this.checkBoxCloseOnFinish.Text = "Close on finish exporting";
            this.checkBoxCloseOnFinish.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Location = new System.Drawing.Point(0, 975);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1673, 22);
            this.statusStrip1.TabIndex = 41;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 31);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBoxisUseHoleFilling
            // 
            this.checkBoxisUseHoleFilling.AutoSize = true;
            this.checkBoxisUseHoleFilling.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBoxisUseHoleFilling.Location = new System.Drawing.Point(244, 188);
            this.checkBoxisUseHoleFilling.Name = "checkBoxisUseHoleFilling";
            this.checkBoxisUseHoleFilling.Size = new System.Drawing.Size(174, 28);
            this.checkBoxisUseHoleFilling.TabIndex = 57;
            this.checkBoxisUseHoleFilling.Text = "HoleFilling";
            this.checkBoxisUseHoleFilling.UseVisualStyleBackColor = true;
            // 
            // Frm_Recons_DataExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1673, 997);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Frm_Recons_DataExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "3D Reconstruction";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrameIndex)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTransversShow)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.voxel_depth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxel_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxel_width)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxCloseOnFinish;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown voxel_depth;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown voxel_height;
        private System.Windows.Forms.NumericUpDown voxel_width;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.HScrollBar hScrollBarFrames;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.RadioButton radioButtonMPI4;
        public System.Windows.Forms.RadioButton radioButtonFDP;
        public System.Windows.Forms.RadioButton radioButtonPNN;
        public System.Windows.Forms.RadioButton radioButtonVNN;
        public System.Windows.Forms.RadioButton radioButtonMPI2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.PictureBox pictureBoxTransversShow;
        private System.Windows.Forms.Label labelFrameCount;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelFrameMaximumCounts;
        private System.Windows.Forms.NumericUpDown numericUpDownFrameIndex;
        public System.Windows.Forms.RadioButton radioButtonDS;
        private System.Windows.Forms.CheckBox checkBoxisUseHoleFilling;
    }
}