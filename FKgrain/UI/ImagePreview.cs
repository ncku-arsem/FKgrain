using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FKgrain.UI
{
    public partial class ImagePreview : Form
    {
        public ImagePreview()
        {
            InitializeComponent();
            PictureBox1 = new PanAndZoom();
            PictureBox1.Bounds = new Rectangle(10, 10, 50, 50);
            groupBox1.Controls.Add(PictureBox1);
            PictureBox1.MouseUp += PictureBox1_MouseUp;
            PictureBox1.Bounds = this.Bounds;
            Resize += (ee, s) => { RefreshPictureBoxSize(); };
        }
        public PanAndZoom PictureBox1;

        private void ImagePreview_Load(object sender, EventArgs e)
        {
            RefreshPictureBoxSize();
        }
        private void RefreshPictureBoxSize()
        {
            groupBox1.Size = new Size(Size.Width, Size.Height - 30);
            PictureBox1.SetBounds(groupBox1.Location.X, groupBox1.Location.Y, groupBox1.Size.Width-20 , groupBox1.Size.Height - 20);
        }
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu cm = new ContextMenu();
                cm.MenuItems.Add("Save", new EventHandler(SaveImage));
                this.ContextMenu = cm;
            }
        }
        private void SaveImage(object sender, EventArgs e)
        {
            var saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = saveFileDialog1.FileName;

                Bitmap clone = new Bitmap(PictureBox1.Image.Width, PictureBox1.Image.Height, PixelFormat.Format24bppRgb);

                using (Graphics gr = Graphics.FromImage(clone))
                {
                    gr.DrawImage(PictureBox1.Image, new Rectangle(0, 0, clone.Width, clone.Height));
                }

                clone.Save(filePath, ImageFormat.Bmp);
            }
        }


    }
}
