namespace FKgrain.UI
{
    partial class TFWGenerator
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.topleftX = new System.Windows.Forms.TextBox();
            this.pixelsize = new System.Windows.Forms.TextBox();
            this.topleftY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(222, 279);
            this.button1.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 56);
            this.button1.TabIndex = 0;
            this.button1.Text = "Generate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Top-Left X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 93);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "Top-Left Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(118, 137);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 29);
            this.label3.TabIndex = 3;
            this.label3.Text = "Pixel Size";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // topleftX
            // 
            this.topleftX.Location = new System.Drawing.Point(312, 35);
            this.topleftX.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.topleftX.Name = "topleftX";
            this.topleftX.Size = new System.Drawing.Size(196, 37);
            this.topleftX.TabIndex = 11;
            this.topleftX.Text = "0";
            // 
            // pixelsize
            // 
            this.pixelsize.Location = new System.Drawing.Point(312, 137);
            this.pixelsize.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pixelsize.Name = "pixelsize";
            this.pixelsize.Size = new System.Drawing.Size(196, 37);
            this.pixelsize.TabIndex = 12;
            this.pixelsize.Text = "1";
            // 
            // topleftY
            // 
            this.topleftY.Location = new System.Drawing.Point(312, 86);
            this.topleftY.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.topleftY.Name = "topleftY";
            this.topleftY.Size = new System.Drawing.Size(196, 37);
            this.topleftY.TabIndex = 13;
            this.topleftY.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(130, 210);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(336, 29);
            this.label4.TabIndex = 14;
            this.label4.Text = "*Top-Left pixel center, not corner";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // TFWGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 368);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.topleftY);
            this.Controls.Add(this.pixelsize);
            this.Controls.Add(this.topleftX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "TFWGenerator";
            this.Text = "TFWGenerator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox topleftX;
        private System.Windows.Forms.TextBox pixelsize;
        private System.Windows.Forms.TextBox topleftY;
        private System.Windows.Forms.Label label4;
    }
}