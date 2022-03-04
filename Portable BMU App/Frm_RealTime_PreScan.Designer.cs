namespace Portable_BMU_App
{
    partial class Frm_RealTime_PreScan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RealTime_PreScan));
            this.buttonStart = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.voxelDepth = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.voxelHeight = new System.Windows.Forms.NumericUpDown();
            this.voxelWidth = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDownAdjustParas = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonTop = new System.Windows.Forms.Button();
            this.buttonBottom = new System.Windows.Forms.Button();
            this.labelTop = new System.Windows.Forms.Label();
            this.labelBottom = new System.Windows.Forms.Label();
            this.checkBoxPlumbLine = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.voxelDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxelHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxelWidth)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdjustParas)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStart.Location = new System.Drawing.Point(413, 371);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(205, 116);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "START";
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.voxelDepth);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.voxelHeight);
            this.groupBox3.Controls.Add(this.voxelWidth);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(29, 45);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 213);
            this.groupBox3.TabIndex = 1;
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
            // voxelDepth
            // 
            this.voxelDepth.DecimalPlaces = 3;
            this.voxelDepth.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.voxelDepth.Location = new System.Drawing.Point(136, 157);
            this.voxelDepth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voxelDepth.Name = "voxelDepth";
            this.voxelDepth.Size = new System.Drawing.Size(133, 35);
            this.voxelDepth.TabIndex = 3;
            this.voxelDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.voxelDepth.ValueChanged += new System.EventHandler(this.voxelDepth_ValueChanged);
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
            // voxelHeight
            // 
            this.voxelHeight.DecimalPlaces = 3;
            this.voxelHeight.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.voxelHeight.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.voxelHeight.Location = new System.Drawing.Point(139, 107);
            this.voxelHeight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voxelHeight.Name = "voxelHeight";
            this.voxelHeight.Size = new System.Drawing.Size(131, 35);
            this.voxelHeight.TabIndex = 2;
            this.voxelHeight.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.voxelHeight.ValueChanged += new System.EventHandler(this.voxelHeight_ValueChanged);
            // 
            // voxelWidth
            // 
            this.voxelWidth.DecimalPlaces = 3;
            this.voxelWidth.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.voxelWidth.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.voxelWidth.Location = new System.Drawing.Point(136, 51);
            this.voxelWidth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voxelWidth.Name = "voxelWidth";
            this.voxelWidth.Size = new System.Drawing.Size(131, 35);
            this.voxelWidth.TabIndex = 1;
            this.voxelWidth.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.voxelWidth.ValueChanged += new System.EventHandler(this.voxelWidth_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxPlumbLine);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.numericUpDownAdjustParas);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(577, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(517, 203);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Volume";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(20, 57);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(278, 28);
            this.label7.TabIndex = 37;
            this.label7.Text = "Enlarge the volume:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(449, 61);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 24);
            this.label14.TabIndex = 40;
            this.label14.Text = "mm";
            // 
            // numericUpDownAdjustParas
            // 
            this.numericUpDownAdjustParas.DecimalPlaces = 1;
            this.numericUpDownAdjustParas.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDownAdjustParas.Location = new System.Drawing.Point(309, 59);
            this.numericUpDownAdjustParas.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownAdjustParas.Name = "numericUpDownAdjustParas";
            this.numericUpDownAdjustParas.Size = new System.Drawing.Size(131, 35);
            this.numericUpDownAdjustParas.TabIndex = 4;
            this.numericUpDownAdjustParas.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownAdjustParas.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelBottom);
            this.panel1.Controls.Add(this.labelTop);
            this.panel1.Controls.Add(this.buttonBottom);
            this.panel1.Controls.Add(this.buttonTop);
            this.panel1.Controls.Add(this.buttonStart);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1107, 557);
            this.panel1.TabIndex = 54;
            // 
            // buttonTop
            // 
            this.buttonTop.Location = new System.Drawing.Point(118, 396);
            this.buttonTop.Name = "buttonTop";
            this.buttonTop.Size = new System.Drawing.Size(125, 67);
            this.buttonTop.TabIndex = 3;
            this.buttonTop.Text = "Top";
            this.buttonTop.UseVisualStyleBackColor = true;
            this.buttonTop.Click += new System.EventHandler(this.buttonTop_Click);
            // 
            // buttonBottom
            // 
            this.buttonBottom.Location = new System.Drawing.Point(797, 396);
            this.buttonBottom.Name = "buttonBottom";
            this.buttonBottom.Size = new System.Drawing.Size(125, 67);
            this.buttonBottom.TabIndex = 4;
            this.buttonBottom.Text = "Bottom";
            this.buttonBottom.UseVisualStyleBackColor = true;
            this.buttonBottom.Click += new System.EventHandler(this.buttonBottom_Click);
            // 
            // labelTop
            // 
            this.labelTop.AutoSize = true;
            this.labelTop.Location = new System.Drawing.Point(114, 482);
            this.labelTop.Name = "labelTop";
            this.labelTop.Size = new System.Drawing.Size(82, 24);
            this.labelTop.TabIndex = 5;
            this.labelTop.Text = "label1";
            // 
            // labelBottom
            // 
            this.labelBottom.AutoSize = true;
            this.labelBottom.Location = new System.Drawing.Point(793, 491);
            this.labelBottom.Name = "labelBottom";
            this.labelBottom.Size = new System.Drawing.Size(82, 24);
            this.labelBottom.TabIndex = 6;
            this.labelBottom.Text = "label2";
            // 
            // checkBoxPlumbLine
            // 
            this.checkBoxPlumbLine.AutoSize = true;
            this.checkBoxPlumbLine.Location = new System.Drawing.Point(25, 104);
            this.checkBoxPlumbLine.Name = "checkBoxPlumbLine";
            this.checkBoxPlumbLine.Size = new System.Drawing.Size(194, 32);
            this.checkBoxPlumbLine.TabIndex = 7;
            this.checkBoxPlumbLine.Text = "Plumb Line";
            this.checkBoxPlumbLine.UseVisualStyleBackColor = true;
            // 
            // Frm_RealTime_PreScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1107, 557);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Frm_RealTime_PreScan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pre-scan";
            this.Load += new System.EventHandler(this.Frm_RealTime_PreScan_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Frm_RealTime_PreScan_KeyDown);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.voxelDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxelHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.voxelWidth)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAdjustParas)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown voxelDepth;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown voxelHeight;
        private System.Windows.Forms.NumericUpDown voxelWidth;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericUpDownAdjustParas;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonBottom;
        private System.Windows.Forms.Button buttonTop;
        private System.Windows.Forms.Label labelBottom;
        private System.Windows.Forms.Label labelTop;
        private System.Windows.Forms.CheckBox checkBoxPlumbLine;
    }
}