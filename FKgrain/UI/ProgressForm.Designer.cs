namespace FKgrain.UI
{
    partial class ProgressForm
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
            this.detrend = new System.Windows.Forms.Label();
            this.VGM = new System.Windows.Forms.Label();
            this.krigging = new System.Windows.Forms.Label();
            this.zerocontour = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // detrend
            // 
            this.detrend.AutoSize = true;
            this.detrend.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detrend.Location = new System.Drawing.Point(12, 10);
            this.detrend.Name = "detrend";
            this.detrend.Size = new System.Drawing.Size(124, 29);
            this.detrend.TabIndex = 0;
            this.detrend.Text = "Detrending";
            // 
            // VGM
            // 
            this.VGM.AutoSize = true;
            this.VGM.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VGM.Location = new System.Drawing.Point(12, 39);
            this.VGM.Name = "VGM";
            this.VGM.Size = new System.Drawing.Size(63, 29);
            this.VGM.TabIndex = 1;
            this.VGM.Text = "VGM";
            // 
            // krigging
            // 
            this.krigging.AutoSize = true;
            this.krigging.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.krigging.Location = new System.Drawing.Point(12, 68);
            this.krigging.Name = "krigging";
            this.krigging.Size = new System.Drawing.Size(91, 29);
            this.krigging.TabIndex = 2;
            this.krigging.Text = "Krigging";
            // 
            // zerocontour
            // 
            this.zerocontour.AutoSize = true;
            this.zerocontour.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zerocontour.Location = new System.Drawing.Point(12, 97);
            this.zerocontour.Name = "zerocontour";
            this.zerocontour.Size = new System.Drawing.Size(201, 29);
            this.zerocontour.TabIndex = 3;
            this.zerocontour.Text = "Zero-Level Contour";
            this.zerocontour.Click += new System.EventHandler(this.zerocontour_Click);
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 142);
            this.Controls.Add(this.zerocontour);
            this.Controls.Add(this.krigging);
            this.Controls.Add(this.VGM);
            this.Controls.Add(this.detrend);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ProgressForm";
            this.Text = "Zero-Level Contour Progress";
            this.Load += new System.EventHandler(this.ProgressForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label detrend;
        public System.Windows.Forms.Label VGM;
        public System.Windows.Forms.Label krigging;
        public System.Windows.Forms.Label zerocontour;
    }
}