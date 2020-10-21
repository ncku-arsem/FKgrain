namespace FKgrain {
    partial class ToolBar {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.Inverse_Btn = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Fill_Btn = new System.Windows.Forms.Button();
            this.Opening_Btn = new System.Windows.Forms.Button();
            this.Closing_Btn = new System.Windows.Forms.Button();
            this.Erosion_Btn = new System.Windows.Forms.Button();
            this.Dilation_Btn = new System.Windows.Forms.Button();
            this.Pick = new System.Windows.Forms.Button();
            this.Trace_Btn = new System.Windows.Forms.Button();
            this.Redo_Btn = new System.Windows.Forms.Button();
            this.Undo_Btn = new System.Windows.Forms.Button();
            this.Done_Btn = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Inverse_Btn
            // 
            this.Inverse_Btn.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Inverse_Btn.Location = new System.Drawing.Point(3, 3);
            this.Inverse_Btn.Name = "Inverse_Btn";
            this.Inverse_Btn.Size = new System.Drawing.Size(50, 50);
            this.Inverse_Btn.TabIndex = 0;
            this.Inverse_Btn.Text = "Inverse";
            this.Inverse_Btn.UseVisualStyleBackColor = true;
            this.Inverse_Btn.Click += new System.EventHandler(this.Inverse_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.Inverse_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Fill_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Opening_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Closing_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Erosion_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Dilation_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Pick);
            this.flowLayoutPanel1.Controls.Add(this.Trace_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Redo_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Undo_Btn);
            this.flowLayoutPanel1.Controls.Add(this.Done_Btn);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(620, 60);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // Fill_Btn
            // 
            this.Fill_Btn.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Fill_Btn.Location = new System.Drawing.Point(59, 3);
            this.Fill_Btn.Name = "Fill_Btn";
            this.Fill_Btn.Size = new System.Drawing.Size(50, 50);
            this.Fill_Btn.TabIndex = 1;
            this.Fill_Btn.Text = "Fill";
            this.Fill_Btn.UseVisualStyleBackColor = true;
            this.Fill_Btn.Click += new System.EventHandler(this.Fill_Click);
            // 
            // Opening_Btn
            // 
            this.Opening_Btn.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.Opening_Btn.Location = new System.Drawing.Point(115, 3);
            this.Opening_Btn.Name = "Opening_Btn";
            this.Opening_Btn.Size = new System.Drawing.Size(50, 50);
            this.Opening_Btn.TabIndex = 2;
            this.Opening_Btn.Text = "Opening";
            this.Opening_Btn.UseVisualStyleBackColor = true;
            this.Opening_Btn.Click += new System.EventHandler(this.Opening_Click);
            // 
            // Closing_Btn
            // 
            this.Closing_Btn.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Closing_Btn.Location = new System.Drawing.Point(171, 3);
            this.Closing_Btn.Name = "Closing_Btn";
            this.Closing_Btn.Size = new System.Drawing.Size(50, 50);
            this.Closing_Btn.TabIndex = 5;
            this.Closing_Btn.Text = "Closing";
            this.Closing_Btn.UseVisualStyleBackColor = true;
            this.Closing_Btn.Click += new System.EventHandler(this.Closing_Click);
            // 
            // Erosion_Btn
            // 
            this.Erosion_Btn.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.Erosion_Btn.Location = new System.Drawing.Point(227, 3);
            this.Erosion_Btn.Name = "Erosion_Btn";
            this.Erosion_Btn.Size = new System.Drawing.Size(50, 50);
            this.Erosion_Btn.TabIndex = 7;
            this.Erosion_Btn.Text = "Erosion";
            this.Erosion_Btn.UseVisualStyleBackColor = true;
            this.Erosion_Btn.Click += new System.EventHandler(this.erosion_Click);
            // 
            // Dilation_Btn
            // 
            this.Dilation_Btn.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.Dilation_Btn.Location = new System.Drawing.Point(283, 3);
            this.Dilation_Btn.Name = "Dilation_Btn";
            this.Dilation_Btn.Size = new System.Drawing.Size(50, 50);
            this.Dilation_Btn.TabIndex = 8;
            this.Dilation_Btn.Text = "Dilation";
            this.Dilation_Btn.UseVisualStyleBackColor = true;
            this.Dilation_Btn.Click += new System.EventHandler(this.dilation_Click);
            // 
            // Pick
            // 
            this.Pick.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.Pick.Location = new System.Drawing.Point(339, 3);
            this.Pick.Name = "Pick";
            this.Pick.Size = new System.Drawing.Size(50, 50);
            this.Pick.TabIndex = 9;
            this.Pick.Text = "Pick";
            this.Pick.UseVisualStyleBackColor = true;
            this.Pick.Click += new System.EventHandler(this.Pick_Click);
            // 
            // Trace_Btn
            // 
            this.Trace_Btn.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.Trace_Btn.Location = new System.Drawing.Point(395, 3);
            this.Trace_Btn.Name = "Trace_Btn";
            this.Trace_Btn.Size = new System.Drawing.Size(50, 50);
            this.Trace_Btn.TabIndex = 6;
            this.Trace_Btn.Text = "Trace Boundary";
            this.Trace_Btn.UseVisualStyleBackColor = true;
            this.Trace_Btn.Click += new System.EventHandler(this.Trace_Btn_Click);
            // 
            // Redo_Btn
            // 
            this.Redo_Btn.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Redo_Btn.Location = new System.Drawing.Point(451, 3);
            this.Redo_Btn.Name = "Redo_Btn";
            this.Redo_Btn.Size = new System.Drawing.Size(50, 50);
            this.Redo_Btn.TabIndex = 4;
            this.Redo_Btn.Text = "Redo";
            this.Redo_Btn.UseVisualStyleBackColor = true;
            this.Redo_Btn.Click += new System.EventHandler(this.Redo_Click);
            // 
            // Undo_Btn
            // 
            this.Undo_Btn.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Undo_Btn.Location = new System.Drawing.Point(507, 3);
            this.Undo_Btn.Name = "Undo_Btn";
            this.Undo_Btn.Size = new System.Drawing.Size(50, 50);
            this.Undo_Btn.TabIndex = 3;
            this.Undo_Btn.Text = "Undo";
            this.Undo_Btn.UseVisualStyleBackColor = true;
            this.Undo_Btn.Click += new System.EventHandler(this.Undo_Click);
            // 
            // Done_Btn
            // 
            this.Done_Btn.Font = new System.Drawing.Font("Arial Narrow", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Done_Btn.Location = new System.Drawing.Point(563, 3);
            this.Done_Btn.Name = "Done_Btn";
            this.Done_Btn.Size = new System.Drawing.Size(50, 50);
            this.Done_Btn.TabIndex = 10;
            this.Done_Btn.Text = "Done";
            this.Done_Btn.UseVisualStyleBackColor = true;
            this.Done_Btn.Click += new System.EventHandler(this.Done_Click);
            // 
            // ToolBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 61);
            this.Controls.Add(this.flowLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(634, 100);
            this.MinimumSize = new System.Drawing.Size(634, 100);
            this.Name = "ToolBar";
            this.Text = "ToolBar";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Inverse_Btn;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button Fill_Btn;
        private System.Windows.Forms.Button Opening_Btn;
        private System.Windows.Forms.Button Undo_Btn;
        private System.Windows.Forms.Button Redo_Btn;
        private System.Windows.Forms.Button Closing_Btn;
        private System.Windows.Forms.Button Trace_Btn;
        private System.Windows.Forms.Button Erosion_Btn;
        private System.Windows.Forms.Button Dilation_Btn;
        private System.Windows.Forms.Button Pick;
        private System.Windows.Forms.Button Done_Btn;
    }
}