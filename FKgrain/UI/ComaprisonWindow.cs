using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FKgrain {
    public partial class ComaprisonWindow : Form {
        public ComaprisonWindow() {
            InitializeComponent();
        }

        ImageForm form;
        Image ori;
        Bitmap Current;
        PanAndZoom PictureBox1;
        string rd;
        public ComaprisonWindow(ImageForm form ,Image img) : this() {
            PictureBox1 = new PanAndZoom();
            PictureBox1.Bounds = new Rectangle(10, 10, 50, 50);
            PictureBox1.MouseDown += PictureBox1_MouseDown;
            PictureBox1.MouseUp += PictureBox1_MouseUp;
            this.Controls.Add(PictureBox1);
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            rd = GuidString;
            Size = new Size(750, 750);
         
            ori = img;
            this.form = form;
            UpdateImage(form.CurrentImage);
            form.OnSetImage += UpdateImage;
            this.Resize += Form2_Resize;
            this.FormClosing += Form2_Closing;
            RefreshPictureBoxSize();
            PictureBox1.SetZoomScale(0.667 * 0.667, new Point(1, 1));
        }
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                TrackBar_Scroll(null, null);
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                PictureBox1.Image = ori;
            }
        }
        private void UpdateImage(Bitmap B) {
            Current = B.Clone(new Rectangle(0, 0, B.Width, B.Height), B.PixelFormat);
            TrackBar_Scroll(null, null);
        }
        private void Form2_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            form.OnSetImage -= UpdateImage;
        }
        bool maximise = false;
        private void Form2_Resize(object sender, EventArgs e) {
            if (!maximise && WindowState == FormWindowState.Maximized) {
                maximise = true;
            } else if (maximise && WindowState == FormWindowState.Normal) {
                maximise = false;
            }
            RefreshPictureBoxSize();
        }

        private void RefreshPictureBoxSize() {
            PictureBox1.Size = new Size(Size.Width - 20, Size.Height - 50);
        }
  

        private void TrackBar_Scroll(object sender, EventArgs e) {
            PictureBox1.Image = NativeIP.Combine((Bitmap)ori, Current, trackBar1.Value, rd);
             
        }

    }
}
