namespace FKgrain.UI
{
    partial class ProjectForm
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.OriginalImage_Lbl = new System.Windows.Forms.Label();
            this.Select_Ori_Btn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.flowbox = new System.Windows.Forms.FlowLayoutPanel();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.SieveMaster_box = new System.Windows.Forms.GroupBox();
            this.finishProject_btn = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.RichTextBox();
            this.groupBox5.SuspendLayout();
            this.SieveMaster_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.OriginalImage_Lbl);
            this.groupBox5.Controls.Add(this.Select_Ori_Btn);
            this.groupBox5.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(336, 84);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Input Image";
            // 
            // OriginalImage_Lbl
            // 
            this.OriginalImage_Lbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.OriginalImage_Lbl.AutoSize = true;
            this.OriginalImage_Lbl.Location = new System.Drawing.Point(112, 39);
            this.OriginalImage_Lbl.Name = "OriginalImage_Lbl";
            this.OriginalImage_Lbl.Size = new System.Drawing.Size(167, 29);
            this.OriginalImage_Lbl.TabIndex = 3;
            this.OriginalImage_Lbl.Text = "Select an Image";
            // 
            // Select_Ori_Btn
            // 
            this.Select_Ori_Btn.Location = new System.Drawing.Point(8, 35);
            this.Select_Ori_Btn.Name = "Select_Ori_Btn";
            this.Select_Ori_Btn.Size = new System.Drawing.Size(98, 43);
            this.Select_Ori_Btn.TabIndex = 0;
            this.Select_Ori_Btn.Text = "Stage 1";
            this.Select_Ori_Btn.UseVisualStyleBackColor = true;
            this.Select_Ori_Btn.Click += new System.EventHandler(this.SelectImage_Btn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // flowbox
            // 
            this.flowbox.Location = new System.Drawing.Point(6, 35);
            this.flowbox.Name = "flowbox";
            this.flowbox.Size = new System.Drawing.Size(303, 360);
            this.flowbox.TabIndex = 5;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar1.LargeChange = 1;
            this.vScrollBar1.Location = new System.Drawing.Point(312, 35);
            this.vScrollBar1.Maximum = 0;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(21, 360);
            this.vScrollBar1.TabIndex = 7;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // SieveMaster_box
            // 
            this.SieveMaster_box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SieveMaster_box.Controls.Add(this.vScrollBar1);
            this.SieveMaster_box.Controls.Add(this.flowbox);
            this.SieveMaster_box.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SieveMaster_box.Location = new System.Drawing.Point(12, 102);
            this.SieveMaster_box.Name = "SieveMaster_box";
            this.SieveMaster_box.Size = new System.Drawing.Size(336, 416);
            this.SieveMaster_box.TabIndex = 3;
            this.SieveMaster_box.TabStop = false;
            this.SieveMaster_box.Text = "Stage 2";
            this.SieveMaster_box.Visible = false;
            // 
            // finishProject_btn
            // 
            this.finishProject_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.finishProject_btn.Enabled = false;
            this.finishProject_btn.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finishProject_btn.Location = new System.Drawing.Point(250, 524);
            this.finishProject_btn.Name = "finishProject_btn";
            this.finishProject_btn.Size = new System.Drawing.Size(98, 45);
            this.finishProject_btn.TabIndex = 5;
            this.finishProject_btn.Text = "Save";
            this.finishProject_btn.UseVisualStyleBackColor = true;
            this.finishProject_btn.Click += new System.EventHandler(this.finishProject_btn_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.label1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 521);
            this.label1.Name = "label1";
            this.label1.ReadOnly = true;
            this.label1.Size = new System.Drawing.Size(226, 45);
            this.label1.TabIndex = 8;
            this.label1.Text = "asdasdasdasdasdasdasdasdasdsadasyifdgdifygdsifugsiuvfdghsiufsdhifudshf";
            // 
            // ProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 578);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.finishProject_btn);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.SieveMaster_box);
            this.Name = "ProjectForm";
            this.Text = "Main";
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.SieveMaster_box.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label OriginalImage_Lbl;
        private System.Windows.Forms.Button Select_Ori_Btn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FlowLayoutPanel flowbox;
        private System.Windows.Forms.GroupBox SieveMaster_box;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.RichTextBox label1;
        public System.Windows.Forms.Button finishProject_btn;
    }
}