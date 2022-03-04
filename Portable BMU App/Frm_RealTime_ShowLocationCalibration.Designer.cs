namespace Portable_BMU_App
{
    partial class Frm_RealTime_ShowLocationCalibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_RealTime_ShowLocationCalibration));
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelxDirection = new System.Windows.Forms.Label();
            this.labelOrigin = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonDepthDirection = new System.Windows.Forms.Button();
            this.labelyDirection = new System.Windows.Forms.Label();
            this.buttonCalibration = new System.Windows.Forms.Button();
            this.buttonOrigin = new System.Windows.Forms.Button();
            this.buttonWidthDirection = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelDepth = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.labelWidth = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelxDirection);
            this.panel1.Controls.Add(this.labelOrigin);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(708, 546);
            this.panel1.TabIndex = 22;
            // 
            // labelxDirection
            // 
            this.labelxDirection.AutoSize = true;
            this.labelxDirection.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelxDirection.ForeColor = System.Drawing.Color.Red;
            this.labelxDirection.Location = new System.Drawing.Point(38, 46);
            this.labelxDirection.Name = "labelxDirection";
            this.labelxDirection.Size = new System.Drawing.Size(55, 26);
            this.labelxDirection.TabIndex = 29;
            this.labelxDirection.Text = "0.0.0";
            // 
            // labelOrigin
            // 
            this.labelOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOrigin.AutoSize = true;
            this.labelOrigin.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrigin.ForeColor = System.Drawing.Color.Red;
            this.labelOrigin.Location = new System.Drawing.Point(38, 451);
            this.labelOrigin.Name = "labelOrigin";
            this.labelOrigin.Size = new System.Drawing.Size(55, 26);
            this.labelOrigin.TabIndex = 27;
            this.labelOrigin.Text = "0.0.0";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.buttonDepthDirection);
            this.panel3.Controls.Add(this.labelyDirection);
            this.panel3.Controls.Add(this.buttonCalibration);
            this.panel3.Controls.Add(this.buttonOrigin);
            this.panel3.Controls.Add(this.buttonWidthDirection);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Location = new System.Drawing.Point(25, 28);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(662, 482);
            this.panel3.TabIndex = 38;
            // 
            // buttonDepthDirection
            // 
            this.buttonDepthDirection.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDepthDirection.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDepthDirection.Location = new System.Drawing.Point(17, 44);
            this.buttonDepthDirection.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDepthDirection.Name = "buttonDepthDirection";
            this.buttonDepthDirection.Size = new System.Drawing.Size(226, 68);
            this.buttonDepthDirection.TabIndex = 23;
            this.buttonDepthDirection.Text = "Depth_Direction";
            this.buttonDepthDirection.UseVisualStyleBackColor = true;
            this.buttonDepthDirection.Click += new System.EventHandler(this.buttonXDirection_Click);
            // 
            // labelyDirection
            // 
            this.labelyDirection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelyDirection.AutoSize = true;
            this.labelyDirection.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelyDirection.ForeColor = System.Drawing.Color.Red;
            this.labelyDirection.Location = new System.Drawing.Point(426, 414);
            this.labelyDirection.Name = "labelyDirection";
            this.labelyDirection.Size = new System.Drawing.Size(55, 26);
            this.labelyDirection.TabIndex = 28;
            this.labelyDirection.Text = "0.0.0";
            // 
            // buttonCalibration
            // 
            this.buttonCalibration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCalibration.Font = new System.Drawing.Font("Times New Roman", 13.875F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCalibration.Location = new System.Drawing.Point(441, 44);
            this.buttonCalibration.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCalibration.Name = "buttonCalibration";
            this.buttonCalibration.Size = new System.Drawing.Size(209, 88);
            this.buttonCalibration.TabIndex = 30;
            this.buttonCalibration.Text = "GetBoxSize";
            this.buttonCalibration.UseVisualStyleBackColor = true;
            this.buttonCalibration.Click += new System.EventHandler(this.buttonCalibration_Click);
            // 
            // buttonOrigin
            // 
            this.buttonOrigin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOrigin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonOrigin.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOrigin.Location = new System.Drawing.Point(17, 344);
            this.buttonOrigin.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOrigin.Name = "buttonOrigin";
            this.buttonOrigin.Size = new System.Drawing.Size(183, 68);
            this.buttonOrigin.TabIndex = 22;
            this.buttonOrigin.Text = "Origin";
            this.buttonOrigin.UseVisualStyleBackColor = true;
            this.buttonOrigin.Click += new System.EventHandler(this.buttonOrigin_Click);
            // 
            // buttonWidthDirection
            // 
            this.buttonWidthDirection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWidthDirection.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWidthDirection.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonWidthDirection.Location = new System.Drawing.Point(431, 344);
            this.buttonWidthDirection.Margin = new System.Windows.Forms.Padding(2);
            this.buttonWidthDirection.Name = "buttonWidthDirection";
            this.buttonWidthDirection.Size = new System.Drawing.Size(218, 68);
            this.buttonWidthDirection.TabIndex = 24;
            this.buttonWidthDirection.Text = "Width_Direction";
            this.buttonWidthDirection.UseVisualStyleBackColor = true;
            this.buttonWidthDirection.Click += new System.EventHandler(this.buttonYDirection_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.labelDepth);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.labelHeight);
            this.groupBox1.Controls.Add(this.label);
            this.groupBox1.Controls.Add(this.labelWidth);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(220, 136);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 188);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BoxSize";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 10.875F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 25);
            this.label4.TabIndex = 33;
            this.label4.Text = "boxHeight:";
            // 
            // labelDepth
            // 
            this.labelDepth.AutoSize = true;
            this.labelDepth.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDepth.ForeColor = System.Drawing.Color.Red;
            this.labelDepth.Location = new System.Drawing.Point(112, 151);
            this.labelDepth.Name = "labelDepth";
            this.labelDepth.Size = new System.Drawing.Size(55, 26);
            this.labelDepth.TabIndex = 36;
            this.labelDepth.Text = "0.0.0";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 10.875F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 25);
            this.label5.TabIndex = 34;
            this.label5.Text = "boxDepth:";
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHeight.ForeColor = System.Drawing.Color.Red;
            this.labelHeight.Location = new System.Drawing.Point(112, 54);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(55, 26);
            this.labelHeight.TabIndex = 31;
            this.labelHeight.Text = "0.0.0";
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Times New Roman", 10.875F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(6, 103);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(102, 25);
            this.label.TabIndex = 32;
            this.label.Text = "boxWidth:";
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWidth.ForeColor = System.Drawing.Color.Red;
            this.labelWidth.Location = new System.Drawing.Point(112, 104);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(55, 26);
            this.labelWidth.TabIndex = 35;
            this.labelWidth.Text = "0.0.0";
            // 
            // Frm_RealTime_ShowLocationCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 546);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Frm_RealTime_ShowLocationCalibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_RealTime_ReconsShowLocation";
            this.Load += new System.EventHandler(this.Frm_RealTime_ShowLocationCalibration_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonCalibration;
        private System.Windows.Forms.Label labelxDirection;
        private System.Windows.Forms.Label labelyDirection;
        private System.Windows.Forms.Label labelOrigin;
        private System.Windows.Forms.Button buttonWidthDirection;
        private System.Windows.Forms.Button buttonDepthDirection;
        private System.Windows.Forms.Button buttonOrigin;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelDepth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label labelWidth;
    }
}