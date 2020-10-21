using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MathWorks.MATLAB.NET.Arrays;

namespace FKgrain
{   
    public partial class ImageForm : Form
    {
        private struct ImageFrame
        {
            public ImageFrame(Bitmap image, double[,] arr)
            {
                this.image = image;
                this.arr = arr;
            }
            public readonly Bitmap image;
            public readonly double[,] arr;
        }

        public Bitmap OriImage;
        public Bitmap Overlay;
        public Bitmap CurrentImage;
        private MWArray currentImageArray;
        public Log log;
        private bool preview;
        private Stack<ImageFrame> undo;
        private Stack<ImageFrame> redo;
        public bool b_DrawEllipse = true;
        double[,] elarr;
        public PanAndZoom PictureBox1;
        public Form optionform = null;
        public event Action<Bitmap, ImageForm> OnDoneClick;
        public MWArray CurrentImageArray
        {
            get
            {
                if (currentImageArray == null)
                {
                    currentImageArray = PImage.Bitmap2array(CurrentImage);
                }
                return currentImageArray;
            }
            set
            {
                currentImageArray = value;
            }
        }

        #region Constructor
        private ImageForm()
        {
            InitializeComponent();
            log = new Log();
            log.OnChanged += (s) =>
            {
                richTextBox1.Text = s;
            };
        }

        public ImageForm(Bitmap image,string Title, Point p, Action<Bitmap, ImageForm> OnDone, Bitmap overlay = null,string DoneButtonText = "Done") : this(image,Title, p, false,  overlay,true,false,DoneButtonText)
        {
            OnDoneClick += OnDone;
        }
        public ImageForm(Bitmap image, string Title, Point p, bool Preview, Bitmap overlay = null, bool convertbinary = true, bool hideTrack = false,string DoneButtonText = "Done") : this()
        {
            Text = Title;
            preview = Preview;
            PictureBox1 = new PanAndZoom();
            PictureBox1.Bounds = new Rectangle(10, 10, 50, 50);
            PictureBox1.MouseDown += PictureBox1_MouseDown;
            PictureBox1.MouseUp += PictureBox1_MouseUp;
            groupBox2.Controls.Add(PictureBox1);
            PictureBox1.OnMousePositionChanged += (np) =>
            {
                label3.Text = np.ToString();
                label3.Refresh();
            };
            PictureBox1.Bounds = groupBox2.Bounds;
            Bitmap bi = image;
            if (convertbinary)
            {
                bi = NativeIP.FastBinaryConvert(image); ;
            }
            OriImage = bi;
            if (overlay == null)
            {
                overlay = bi;
            }
            Overlay = overlay;
            PictureBox1.Image = OriImage;
            PictureBox1.SetZoomScale(0.667 * 0.667, new Point(0, 0));
            Size = new Size(1000, 1000);
            StartPosition = FormStartPosition.Manual;
            p.Y += 150;
            Location = p;
            undo = new Stack<ImageFrame>();
            redo = new Stack<ImageFrame>();
            if (hideTrack)
            {
                trackBar1.Enabled = false;
                trackBar1.Visible = false;
            }
            this.WindowState = FormWindowState.Maximized;
            Resize += (e, s) => { RefreshPictureBoxSize(); };
            done_Button.Text = DoneButtonText;
        }
        #endregion 

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
           
            if (e.Button == MouseButtons.Left)
            {
                RedrawImage(null, null);
            }
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu cm = new ContextMenu();
                cm.MenuItems.Add("Save", new EventHandler(SaveImage));
                cm.MenuItems.Add("Change Overlay", new EventHandler(ChangeOverlay));
                this.ContextMenu = cm;
            }
        }
        private void ChangeOverlay(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Overlay = new Bitmap(openFileDialog.FileName);
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

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox1.Image = Overlay;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            SetImage(OriImage);
            RefreshPictureBoxSize();
        }

        #region setimage
        public void SetImage(MWArray image)
        {

            if (CurrentImage != null)
            {
                undo.Push(new ImageFrame(CurrentImage, elarr));
            }
            double[,] ellpise = new double[0,2];
            if (image.GetType() ==  typeof(MWLogicalArray))
            {
                ellpise =  FitEllipse((MWLogicalArray)image);
            }
            //clear forward when anything new done
            if (redo.Count != 0)
            {
                redo = new Stack<ImageFrame>();
            }
            setImage(image, ellpise);
        }
        public void SetImage(Bitmap image)
        {
            if (CurrentImage != null)
            {
                undo.Push(new ImageFrame(CurrentImage, elarr));
            }
            double[,] ellpise = new double[0, 2];
            ellpise = FitEllipse(PImage.Bitmap2array(image));
            //clear forward when anything new done
            if (redo.Count != 0)
            {
                redo = new Stack<ImageFrame>();
            }
            setImage(image, ellpise);
        }
        public event Action<Bitmap> OnSetImage;

        private void setImage(Bitmap image, double[,] elarr)
        {
            setImage(image, null, elarr);
        }
        private void setImage(MWArray imagearr,double[,] elarr)
        {
            setImage(PImage.Array2bitmap(imagearr, OriImage.Width, OriImage.Height), imagearr, elarr);
        }

        private void setImage(Bitmap image, MWArray imagearr, double[,] elarr)
        {
            this.elarr = elarr;
            CurrentImageArray = imagearr;
            CurrentImage = image;
            RedrawImage(null, null);
            PictureBox1.Refresh();
            if (OnSetImage != null)
            {
                OnSetImage(CurrentImage);
            }
        }


        #endregion

        public void Undo()
        {

            if (undo.Count == 0)
            {
                return;
            }
            Console.WriteLine(undo.Count);
            redo.Push(new ImageFrame( CurrentImage,elarr));
            var tmp = undo.Pop();
            setImage(tmp.image,tmp.arr);
            log.Undo();
        }
        public void Redo()
        {
            if (redo.Count == 0)
            {
                return;
            }
            Console.WriteLine(redo.Count);
            undo.Push(new ImageFrame(CurrentImage, elarr));
            var tmp = redo.Pop();
            setImage(tmp.image, tmp.arr);
            log.Redo();
        }


        private void RefreshPictureBoxSize()
        {
            PictureBox1.SetBounds(groupBox2.Location.X, groupBox2.Location.Y, groupBox2.Size.Width - 20, groupBox2.Size.Height - 20);
        }

        string rd = "Image";
        private void RedrawImage(object sender, EventArgs e)
        {
            if (CurrentImage == null)
                return;

            Bitmap c = NativeIP.Combine(Overlay, CurrentImage, trackBar1.Value, rd);
            if (b_DrawEllipse)
            {
                c = DrawEllipse(c);
            }
            PictureBox1.Image = c;
        }
        #region function Button
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bi = NativeIP.FastInvertBinary(CurrentImage);
            SetImage(bi);
            log.AddLog(new Step(Step.Inverse, new string[0]));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (optionform != null)
            {
                optionform.Focus();
                return;
            }
            FillOption f = new FillOption(this);
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
            f.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (optionform != null)
            {
                optionform.Focus();
                return;
            }
            FilterPicker f = new FilterPicker(Step.Opening, (filter, size) =>
            {
                this.SetImage(PImage.processor.Opening(this.CurrentImageArray, filter, size));
                this.log.AddLog(new Step(Step.Opening, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (optionform != null)
            {
                optionform.Focus();
                return;
            }
            FilterPicker f = new FilterPicker(Step.Closing, (filter, size) =>
            {
                this.SetImage(PImage.processor.Closing(this.CurrentImageArray, filter, size));
                this.log.AddLog(new Step(Step.Closing, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (optionform != null)
            {
                optionform.Focus();
                return;
            }
            FilterPicker f = new FilterPicker(Step.Erosion, (filter, size) =>
            {
                this.SetImage(PImage.processor.Erosion(this.CurrentImageArray, filter, size));
                this.log.AddLog(new Step(Step.Erosion, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (optionform != null)
            {
                optionform.Focus();
                return;
            }
            FilterPicker f = new FilterPicker(Step.Dilation, (filter, size) =>
            {
                this.SetImage(PImage.processor.Dialation(this.CurrentImageArray, filter, size));
                this.log.AddLog(new Step(Step.Dilation, new string[] { "filter:" + filter, "size:" + size.ToString() }));
            });
            optionform = f;
            f.FormClosed += OptionFormClosed;
            f.StartPosition = FormStartPosition.Manual;
            f.Location = OptionWindowPosition();
            f.Show();
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Undo();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (OnDoneClick != null)
            {
                OnDoneClick(this.CurrentImage, this);
            }

            this.Close();
        }
        private double[,] FitEllipse(MWLogicalArray imagearr)
        {
            try {
                MWNumericArray el = (MWNumericArray)PImage.processor.FitEllipse(imagearr, 8, (MWArray)("noholes"));
                return elarr = (double[,])el.ToArray();
            }
            catch(Exception ex)
            {
                return new double[0,2];
            }
        }
        private Bitmap DrawEllipse(Bitmap image)
        {
            if(elarr == null)
            {
                return image;
            }
            Bitmap clone = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            using (Graphics gr = Graphics.FromImage(clone))
            {
                using (Pen thick_pen = new Pen(Color.Red, 5))
                {
                    for (int i = 0; i < elarr.GetLength(0); i++)
                    {
                        int a = (int)Math.Floor(elarr[i, 1]);
                        int b = (int)Math.Floor(elarr[i, 0]);
                        int dx = (int)Math.Floor(elarr[i, 6]);
                        int dy = (int)Math.Floor(elarr[i, 5]);
                        double theta = elarr[i, 2] * 180 / Math.PI;
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        Rectangle rect = new Rectangle(-a, -b, a * 2, b * 2);
                        gr.TranslateTransform(dx, dy);
                        gr.RotateTransform((float)theta);
                        gr.DrawEllipse(thick_pen, rect);
                        gr.RotateTransform((float)-theta);
                        gr.TranslateTransform(-dx, -dy);
                    }
                }
            }
            return clone;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            b_DrawEllipse = checkBox1.Checked;
            /*if (b_DrawEllipse)
            {
                PictureBox1.Image = DrawEllipse(CurrentImage);
            }*/
            RedrawImage(null,null);
        }


        #region Button SubFunction
        private Point OptionWindowPosition()
        {
            Point p = this.Location;
            return p;
        }
        private void OptionFormClosed(object sender, FormClosedEventArgs e)
        {
            optionform = null;
        }







        #endregion

        #endregion

   

        private double[,] FitEllipse(Bitmap image)
        {
            MWLogicalArray imagearr = PImage.Bitmap2array(image);
            MWNumericArray el = (MWNumericArray)PImage.processor.FitEllipse(imagearr, 8, (MWArray)("noholes"));
            double[,] elarr = (double[,])el.ToArray();
            return elarr;
        }
        private Bitmap DrawEllipse(Bitmap image, double[,] ellipse)
        {
            Bitmap clone = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            List<string> output = new List<string>();
            using (Graphics gr = Graphics.FromImage(clone))
            {
                using (Pen thick_pen = new Pen(Color.Red, 5))
                {
                    for (int i = 0; i < ellipse.GetLength(0); i++)
                    {
                        int a = (int)Math.Floor(ellipse[i, 1]);
                        int b = (int)Math.Floor(ellipse[i, 0]);
                        int dx = (int)Math.Floor(ellipse[i, 6]);
                        int dy = (int)Math.Floor(ellipse[i, 5]);
                        double theta = ellipse[i, 2] * 180 / Math.PI;
                        gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        Rectangle rect = new Rectangle(-a, -b, a * 2, b * 2);
                        gr.TranslateTransform(dx, dy);
                        gr.RotateTransform((float)theta);
                        gr.DrawEllipse(thick_pen, rect);
                        gr.RotateTransform((float)-theta);
                        gr.TranslateTransform(-dx, -dy);
                    }
                }
            }
            return clone;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1.instance.saveFileDialog1.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";
            if (Form1.instance.saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = Form1.instance.saveFileDialog1.FileName;
                File.WriteAllText(filePath, log.Logs2String());
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form1.instance.openFileDialog1.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";
            if (Form1.instance.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = Form1.instance.openFileDialog1.FileName;
                string[] logs = File.ReadAllLines(filePath);
                MWArray a = (MWArray)CurrentImageArray.Clone();
                int h = CurrentImage.Height;
                int w = CurrentImage.Width;

                foreach (string s in logs)
                {
                    Step st = new Step(s);
                    a = Step.Execute(st, CurrentImage, a, w, h);
                    SetImage(a);
                    Refresh();
                    log.AddLog(st);
                    //Console.WriteLine(s);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}