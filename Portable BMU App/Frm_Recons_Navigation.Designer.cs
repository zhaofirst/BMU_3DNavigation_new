namespace Portable_BMU_App
{
    partial class Frm_Recons_Navigation
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BrightnessBar = new System.Windows.Forms.TrackBar();
            this.ContrastBar = new System.Windows.Forms.TrackBar();
            this.toolStripButtonGray = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonJet = new System.Windows.Forms.ToolStripButton();
            this.buttonNavigation = new System.Windows.Forms.Button();
            this.real3DPictureBox = new System.Windows.Forms.PictureBox();
            this.toolStripButtonBone = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContrastBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.real3DPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.buttonNavigation);
            this.panel1.Controls.Add(this.real3DPictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1661, 1118);
            this.panel1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.toolStripTextBox1,
            this.toolStripButtonBone,
            this.toolStripButtonGray,
            this.toolStripButtonJet,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1661, 74);
            this.toolStrip1.TabIndex = 17;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 74);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 74);
            this.toolStripTextBox1.Text = "Color:";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 74);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.BrightnessBar);
            this.panel2.Controls.Add(this.ContrastBar);
            this.panel2.Location = new System.Drawing.Point(1256, 514);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(286, 440);
            this.panel2.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(48, 159);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 36);
            this.label2.TabIndex = 13;
            this.label2.Text = "Contrast";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(67, 322);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 89);
            this.button1.TabIndex = 12;
            this.button1.Text = "Save Jpg";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(48, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 36);
            this.label1.TabIndex = 12;
            this.label1.Text = "Brightness";
            // 
            // BrightnessBar
            // 
            this.BrightnessBar.Cursor = System.Windows.Forms.Cursors.Default;
            this.BrightnessBar.Location = new System.Drawing.Point(37, 66);
            this.BrightnessBar.Margin = new System.Windows.Forms.Padding(2);
            this.BrightnessBar.Maximum = 200;
            this.BrightnessBar.Name = "BrightnessBar";
            this.BrightnessBar.Size = new System.Drawing.Size(221, 90);
            this.BrightnessBar.TabIndex = 10;
            this.BrightnessBar.Value = 100;
            this.BrightnessBar.Scroll += new System.EventHandler(this.BrightnessBar_Scroll);
            // 
            // ContrastBar
            // 
            this.ContrastBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.ContrastBar.Location = new System.Drawing.Point(37, 214);
            this.ContrastBar.Margin = new System.Windows.Forms.Padding(2);
            this.ContrastBar.Maximum = 200;
            this.ContrastBar.Name = "ContrastBar";
            this.ContrastBar.Size = new System.Drawing.Size(221, 90);
            this.ContrastBar.TabIndex = 11;
            this.ContrastBar.Value = 100;
            this.ContrastBar.Scroll += new System.EventHandler(this.ContrastBar_Scroll);
            // 
            // toolStripButtonGray
            // 
            this.toolStripButtonGray.AutoSize = false;
            this.toolStripButtonGray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGray.Image = global::Portable_BMU_App.Properties.Resources.bone;
            this.toolStripButtonGray.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonGray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGray.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.toolStripButtonGray.Name = "toolStripButtonGray";
            this.toolStripButtonGray.Size = new System.Drawing.Size(70, 70);
            this.toolStripButtonGray.Text = "toolStripButton1";
            this.toolStripButtonGray.Click += new System.EventHandler(this.toolStripButtonGray_Click);
            // 
            // toolStripButtonJet
            // 
            this.toolStripButtonJet.AutoSize = false;
            this.toolStripButtonJet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonJet.Image = global::Portable_BMU_App.Properties.Resources.jet;
            this.toolStripButtonJet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonJet.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonJet.Name = "toolStripButtonJet";
            this.toolStripButtonJet.Size = new System.Drawing.Size(70, 70);
            this.toolStripButtonJet.Text = "Jet";
            this.toolStripButtonJet.Click += new System.EventHandler(this.toolStripButtonJet_Click);
            // 
            // buttonNavigation
            // 
            this.buttonNavigation.BackgroundImage = global::Portable_BMU_App.Properties.Resources.navigation;
            this.buttonNavigation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonNavigation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNavigation.Location = new System.Drawing.Point(1293, 280);
            this.buttonNavigation.Name = "buttonNavigation";
            this.buttonNavigation.Size = new System.Drawing.Size(177, 99);
            this.buttonNavigation.TabIndex = 15;
            this.buttonNavigation.UseVisualStyleBackColor = true;
            this.buttonNavigation.Click += new System.EventHandler(this.buttonNavigation_Click);
            // 
            // real3DPictureBox
            // 
            this.real3DPictureBox.Location = new System.Drawing.Point(139, 95);
            this.real3DPictureBox.Name = "real3DPictureBox";
            this.real3DPictureBox.Size = new System.Drawing.Size(981, 1001);
            this.real3DPictureBox.TabIndex = 0;
            this.real3DPictureBox.TabStop = false;
            // 
            // toolStripButtonBone
            // 
            this.toolStripButtonBone.AutoSize = false;
            this.toolStripButtonBone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBone.Image = global::Portable_BMU_App.Properties.Resources.bone2;
            this.toolStripButtonBone.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonBone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBone.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.toolStripButtonBone.Name = "toolStripButtonBone";
            this.toolStripButtonBone.Size = new System.Drawing.Size(70, 70);
            this.toolStripButtonBone.Text = "toolStripButton1";
            this.toolStripButtonBone.Click += new System.EventHandler(this.toolStripButtonBone_Click);
            // 
            // Frm_Recons_Navigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1661, 1118);
            this.Controls.Add(this.panel1);
            this.Name = "Frm_Recons_Navigation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frm_Recons_Navigation";
            this.Load += new System.EventHandler(this.Frm_Recons_Navigation_Load);
            this.SizeChanged += new System.EventHandler(this.Frm_Recons_Navigation_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContrastBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.real3DPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox real3DPictureBox;
        private System.Windows.Forms.Button buttonNavigation;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar BrightnessBar;
        private System.Windows.Forms.TrackBar ContrastBar;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripButton toolStripButtonJet;
        private System.Windows.Forms.ToolStripButton toolStripButtonGray;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonBone;
    }
}