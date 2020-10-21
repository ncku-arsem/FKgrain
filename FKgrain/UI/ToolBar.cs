using System;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FKgrain {
    [Obsolete]
    public partial class ToolBar : Form {
        public ToolBar() {
            InitializeComponent();
        }
        public ImageForm imageform;
        public Form optionform = null;
        
        public enum Mode
        {
            All = 0,
            Preprocessing = 1,
            Trace = 2,
            Sieve=3,
        }

        public ToolBar(ImageForm image) : this(image,Mode.All) {
        }
        public ToolBar(ImageForm image,Mode m) : this()
        {
            imageform = image;
            switch (m)
            {
                case Mode.Preprocessing:
                    {
                        PreprocessMode();
                    }
                    break;
                case Mode.Sieve:
                    {
                        SieveMode();
                    }
                    break;
                case Mode.Trace:
                    {
                        TraceMode();
                    }
                    break;
            }
           
        }

        private void PreprocessMode()
        {
            Trace_Btn.Hide();
            Pick.Hide();
            
            var newSize = new Size(this.Size.Width - 110, this.Size.Height);
            this.MaximumSize = newSize;
            this.MinimumSize = newSize;
            this.Size = newSize;

        }


        private void SieveMode()
        {
            Inverse_Btn.Hide();
            Opening_Btn.Hide();
            Closing_Btn.Hide();
            Fill_Btn.Hide();
            Erosion_Btn.Hide();
            Dilation_Btn.Hide();
            Trace_Btn.Hide();
            Done_Btn.Hide();
            var newSize = new Size(this.Size.Width - 450, this.Size.Height);
            this.MaximumSize = newSize;
            this.MinimumSize = newSize;
            this.Size = newSize;

        }
        private void TraceMode()
        {
            Trace_Btn.Hide();
            Fill_Btn.Hide();
            Pick.Hide();
            var newSize = new Size(this.Size.Width - 165, this.Size.Height);
            this.MaximumSize = newSize;
            this.MinimumSize = newSize;
            this.Size = newSize;

        }

        private Point OptionWindowPosition() {
            Point p = this.Location;
            p.Y += (this.Height);
            return p;
        }
        private void Form3_Load(object sender, EventArgs e) {
        }

        private void Inverse_Click(object sender, EventArgs e) {
            Bitmap bi = NativeIP.FastInvertBinary(imageform.CurrentImage);
            imageform.SetImage(bi);
            //imageform.logs.AddLog(new Step(Step.Inverse, new string[0]));
        }

        private void Fill_Click(object sender, EventArgs e) {
            if (optionform != null) {
                return;
            }
            FillOption f = new FillOption(imageform);
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();

        }

        private void Opening_Click(object sender, EventArgs e) {
            if (optionform != null) {
                return;
            }
            FilterPicker f = new FilterPicker(Step.Opening,(filter, size) => {
                imageform.SetImage(PImage.processor.Opening(imageform.CurrentImageArray,filter,size));
                imageform.log.AddLog(new Step(Step.Opening, new string[] {"filter:"+filter, "size:"+size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }
        private void Closing_Click(object sender, EventArgs e) {
            if (optionform != null) {
                return;
            }
            FilterPicker f = new FilterPicker(Step.Closing, (filter, size) => {
                imageform.SetImage(PImage.processor.Closing(imageform.CurrentImageArray, filter, size));
              //  imageform.logs.AddLog(new Step(Step.Closing, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }

        private void Trace_Btn_Click(object sender, EventArgs e) {
            if (optionform != null) {
                return;
            }
            TraceBoundaryOption f = new TraceBoundaryOption(imageform);
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }

     


        private void erosion_Click(object sender, EventArgs e) {
            if (optionform != null) {
                return;
            }
            FilterPicker f = new FilterPicker(Step.Erosion,(filter, size) => {
                imageform.SetImage(PImage.processor.Erosion(imageform.CurrentImageArray, filter, size));
              //  imageform.logs.AddLog(new Step(Step.Erosion, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }

        private void dilation_Click(object sender, EventArgs e) {
            if(optionform != null) {
                return;
            }

            FilterPicker f = new FilterPicker(Step.Dilation,(filter, size) => {
                imageform.SetImage(PImage.processor.Dialation(imageform.CurrentImageArray, filter, size));
             //   imageform.logs.AddLog(new Step(Step.Dilation, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();

        }

        private void OptionFormClosed(object sender, FormClosedEventArgs e) {
            optionform = null;
        }
        private void Undo_Click(object sender, EventArgs e) {
            imageform.Undo();
        }

        private void Redo_Click(object sender, EventArgs e) {
            imageform.Redo();
        }

        private void Pick_Click(object sender, EventArgs e)
        {
            //draw eclipse
            Bitmap bm = imageform.CurrentImage;
            /*const string tmppath = "tmp/PickMid.bmp";
            if (!Directory.Exists("tmp"))
                Directory.CreateDirectory("tmp");
            */
            Bitmap clone = new Bitmap(bm.Width, bm.Height, PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(bm, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(-75, -150, 150, 300);
                gr.TranslateTransform(1000, 1000);
                using (Pen thick_pen = new Pen(Color.Black, 5))
                {
                    gr.DrawEllipse(thick_pen, rect);
                }
                gr.RotateTransform(-45.0F);
                using (Pen thick_pen = new Pen(Color.Black, 5))
                {
                    gr.DrawEllipse(thick_pen, rect);
                }
            }
            imageform.SetImage(clone);
        }

        public event Action<Bitmap,ImageForm> OnDoneClick;

        private void Done_Click(object sender, EventArgs e) {
            if(OnDoneClick != null)
            {
                OnDoneClick(imageform.CurrentImage,imageform);
            }
            imageform.Close();

            ///OLD COMBINE Method
            /*OpenFileDialog openFileDialog1 =  imageform.mainForm.GetOpenFileDialog();
            openFileDialog1.Title = "Select image";
            openFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string openfile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                openfile = openFileDialog1.FileName;
            } else {
                return;
            }
            Bitmap img2 = new Bitmap(openfile);
            NativeIP.FastCombineBinary(imageform.CurrentImage, img2);
            imageform.SetImage(img2);*/
            //imageform.logs.AddLog(new Step(Step.Inverse, new string[0]));
        }
    }
}
