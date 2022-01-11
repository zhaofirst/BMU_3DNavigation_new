namespace Portable_BMU_App
{
    partial class Dlg_waitingRecons
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dlg_waitingRecons));
            this.progressBarWaiting = new System.Windows.Forms.ProgressBar();
            this.labelTimeShow = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBarWaiting
            // 
            this.progressBarWaiting.Location = new System.Drawing.Point(149, 137);
            this.progressBarWaiting.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBarWaiting.MarqueeAnimationSpeed = 30;
            this.progressBarWaiting.Name = "progressBarWaiting";
            this.progressBarWaiting.Size = new System.Drawing.Size(835, 41);
            this.progressBarWaiting.TabIndex = 0;
            // 
            // labelTimeShow
            // 
            this.labelTimeShow.AutoSize = true;
            this.labelTimeShow.Font = new System.Drawing.Font("Times New Roman", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimeShow.Location = new System.Drawing.Point(546, 56);
            this.labelTimeShow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTimeShow.Name = "labelTimeShow";
            this.labelTimeShow.Size = new System.Drawing.Size(45, 42);
            this.labelTimeShow.TabIndex = 1;
            this.labelTimeShow.Text = "...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(140, 56);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(266, 42);
            this.label2.TabIndex = 3;
            this.label2.Text = "Reconstructing...";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(789, 220);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(195, 59);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // Dlg_waitingRecons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 313);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTimeShow);
            this.Controls.Add(this.progressBarWaiting);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Dlg_waitingRecons";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please Waiting...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ProgressBar progressBarWaiting;
        public System.Windows.Forms.Label labelTimeShow;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
    }
}