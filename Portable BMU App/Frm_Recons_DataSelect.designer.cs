namespace Portable_BMU_App
{
    partial class Frm_Recons_DataSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Recons_DataSelect));
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxPlumbLine = new System.Windows.Forms.CheckBox();
            this.labelNextReminder = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkedListBoxDataSetSelect = new System.Windows.Forms.CheckedListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownLY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownLX = new System.Windows.Forms.NumericUpDown();
            this.btnSelect = new System.Windows.Forms.Button();
            this.textBoxShowSeletedPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLX)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBoxPlumbLine);
            this.panel2.Controls.Add(this.labelNextReminder);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1672, 961);
            this.panel2.TabIndex = 40;
            // 
            // checkBoxPlumbLine
            // 
            this.checkBoxPlumbLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxPlumbLine.AutoSize = true;
            this.checkBoxPlumbLine.Checked = true;
            this.checkBoxPlumbLine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPlumbLine.Location = new System.Drawing.Point(42, 901);
            this.checkBoxPlumbLine.Name = "checkBoxPlumbLine";
            this.checkBoxPlumbLine.Size = new System.Drawing.Size(234, 28);
            this.checkBoxPlumbLine.TabIndex = 62;
            this.checkBoxPlumbLine.Text = "Using Plumb Line";
            this.checkBoxPlumbLine.UseVisualStyleBackColor = true;
            // 
            // labelNextReminder
            // 
            this.labelNextReminder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNextReminder.AutoSize = true;
            this.labelNextReminder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNextReminder.Location = new System.Drawing.Point(1106, 894);
            this.labelNextReminder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelNextReminder.Name = "labelNextReminder";
            this.labelNextReminder.Size = new System.Drawing.Size(354, 29);
            this.labelNextReminder.TabIndex = 9;
            this.labelNextReminder.Text = "Please Select a Valid Path First!";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(34, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(477, 37);
            this.label1.TabIndex = 7;
            this.label1.Text = "*Step1: Data Aquisition";
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(1468, 884);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(172, 61);
            this.btnNext.TabIndex = 8;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.checkedListBoxDataSetSelect);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.btnSelect);
            this.panel1.Controls.Add(this.textBoxShowSeletedPath);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(40, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1600, 782);
            this.panel1.TabIndex = 6;
            // 
            // checkedListBoxDataSetSelect
            // 
            this.checkedListBoxDataSetSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxDataSetSelect.CheckOnClick = true;
            this.checkedListBoxDataSetSelect.FormattingEnabled = true;
            this.checkedListBoxDataSetSelect.Location = new System.Drawing.Point(993, 267);
            this.checkedListBoxDataSetSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkedListBoxDataSetSelect.Name = "checkedListBoxDataSetSelect";
            this.checkedListBoxDataSetSelect.Size = new System.Drawing.Size(433, 364);
            this.checkedListBoxDataSetSelect.TabIndex = 41;
            this.checkedListBoxDataSetSelect.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxDataSetSelect_ItemCheck);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(988, 224);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(195, 29);
            this.label8.TabIndex = 40;
            this.label8.Text = "File in the Folder";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(561, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(479, 29);
            this.label7.TabIndex = 34;
            this.label7.Text = "Pleae select a path where usData is located";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownLY);
            this.groupBox1.Controls.Add(this.numericUpDownLX);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10.5F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(170, 480);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(538, 201);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LATERAL RESOLUTION";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(28, 66);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(159, 33);
            this.label3.TabIndex = 36;
            this.label3.Text = "Lateral X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(399, 147);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 28);
            this.label6.TabIndex = 41;
            this.label6.Text = "mm";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(28, 139);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(159, 33);
            this.label4.TabIndex = 37;
            this.label4.Text = "Lateral Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(399, 74);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 28);
            this.label5.TabIndex = 40;
            this.label5.Text = "mm";
            // 
            // numericUpDownLY
            // 
            this.numericUpDownLY.DecimalPlaces = 3;
            this.numericUpDownLY.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDownLY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numericUpDownLY.Location = new System.Drawing.Point(212, 141);
            this.numericUpDownLY.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownLY.Name = "numericUpDownLY";
            this.numericUpDownLY.Size = new System.Drawing.Size(180, 35);
            this.numericUpDownLY.TabIndex = 4;
            this.numericUpDownLY.Value = new decimal(new int[] {
            243,
            0,
            0,
            196608});
            this.numericUpDownLY.ValueChanged += new System.EventHandler(this.numericUpDownLY_ValueChanged);
            // 
            // numericUpDownLX
            // 
            this.numericUpDownLX.DecimalPlaces = 3;
            this.numericUpDownLX.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDownLX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numericUpDownLX.Location = new System.Drawing.Point(212, 67);
            this.numericUpDownLX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.numericUpDownLX.Name = "numericUpDownLX";
            this.numericUpDownLX.Size = new System.Drawing.Size(180, 35);
            this.numericUpDownLX.TabIndex = 3;
            this.numericUpDownLX.Value = new decimal(new int[] {
            243,
            0,
            0,
            196608});
            this.numericUpDownLX.ValueChanged += new System.EventHandler(this.numericUpDownLX_ValueChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelect.Location = new System.Drawing.Point(1228, 45);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(123, 46);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "...";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // textBoxShowSeletedPath
            // 
            this.textBoxShowSeletedPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxShowSeletedPath.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxShowSeletedPath.Location = new System.Drawing.Point(430, 46);
            this.textBoxShowSeletedPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxShowSeletedPath.Name = "textBoxShowSeletedPath";
            this.textBoxShowSeletedPath.Size = new System.Drawing.Size(792, 39);
            this.textBoxShowSeletedPath.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(242, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 33);
            this.label2.TabIndex = 31;
            this.label2.Text = "Data Folder";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 961);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 14, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1672, 36);
            this.statusStrip1.TabIndex = 39;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 31);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // Frm_Recons_DataSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1672, 997);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Frm_Recons_DataSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "3D Reconstruction";
            this.Load += new System.EventHandler(this.Frm_Recons_DataSelect_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLX)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSelect;
        public System.Windows.Forms.TextBox textBoxShowSeletedPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckedListBox checkedListBoxDataSetSelect;
        private System.Windows.Forms.Label labelNextReminder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownLY;
        private System.Windows.Forms.NumericUpDown numericUpDownLX;
        private System.Windows.Forms.CheckBox checkBoxPlumbLine;
    }
}