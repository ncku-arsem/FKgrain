using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathWorks.MATLAB.NET.Arrays;
using System.IO;
namespace FKgrain.UI
{

    public partial class SieveUI : Form
    {
        public Bitmap OriImage;
        public Bitmap Overlay;
        public bool[,] OriginalImageArray;
        Queue<position>[] segments;
        int[] segmentsCount;
        PanAndZoom PictureBox1;
        int maxSize = -1;
        int minSize = int.MaxValue;
        public Sieve sieve;
        public SieveUI(Bitmap overlay = null)
        {
            InitializeComponent();
            this.Resize += Form2_Resize;
            PictureBox1 = new PanAndZoom();
            PictureBox1.Bounds = new Rectangle(10, 10, 50, 50);
            PictureBox1.MouseDown += PictureBox1_MouseDown;
            PictureBox1.MouseUp += PictureBox1_MouseUp;
            groupBox2.Controls.Add(PictureBox1);
            if(overlay == null)
            {
                overlay = OriImage;
            }
            Overlay = overlay;
        }
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                trackBar1_Scroll(null, null);
            }
        }
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox1.Image = Overlay;
            }
        }
        private const System.Drawing.Imaging.PixelFormat pixelFormat = System.Drawing.Imaging.PixelFormat.Format1bppIndexed;
        public SieveUI(Bitmap Ori, int threshold, Bitmap overlay=null) : this(overlay)
        {
            
            if (Ori.PixelFormat != pixelFormat)
            {
                Bitmap tmp = NativeIP.FastBinaryConvert(Ori);

                Ori = tmp;
            }
            OriImage = Ori;
            DateTime start = DateTime.Now;
            OriginalImageArray = PImage.Bitmap2NetArray(OriImage);
            TimeSpan timeItTook = DateTime.Now - start;

           
            Size = new Size(750, 750);
            segments = Pick(OriginalImageArray);
            for(int i =0; i < segments.Length; i++)
            {
                int c = segments[i].Count;
                maxSize = Math.Max(c+1, maxSize);
                minSize = Math.Min(c-1, minSize);
            }
            trackBar1.Maximum = maxSize;
            trackBar1.Minimum = minSize;
            if (threshold < trackBar1.Minimum)
            {
                threshold = trackBar1.Minimum;
            }
            else if (threshold > trackBar1.Maximum)
            {
                threshold = trackBar1.Maximum;
            }
            trackBar1.Value = threshold;
            trackBar1.Refresh();
            numericUpDown1.Maximum = maxSize;
            numericUpDown1.Minimum = minSize;
            numericUpDown1.Value = threshold;
            PictureBox1.Image = OriImage;
            label1.Text = "Number of Segments:" + segments.Length;
            segmentsCount = new int[segments.Length];

            for(int i=0;i<segments.Length; i++)
            {
                segmentsCount[i] = segments[i].Count;
            }
            this.WindowState = FormWindowState.Maximized;
            PictureBox1.SetZoomScale(0.667 * 0.667, new Point(0, 0));
            refreshImage(threshold);
        }
        public SieveUI(Bitmap image, Sieve sieve,Bitmap overlay = null,int threshold =-1 ): this(image,threshold,overlay)
        {
            this.sieve = sieve;
            this.Text = $"Level {sieve.index + 1} Thresholding";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
            refreshImage(trackBar1.Value);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

            trackBar1.Value = (int)numericUpDown1.Value;
            refreshImage(trackBar1.Value);
        }

        private void refreshImage(int threshold)
        {
            Draw(threshold, segments,segmentsCount);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int track_val = trackBar1.Value;
            Bitmap larger = draw(track_val, segments, segmentsCount, true);
            Bitmap smaller = draw(track_val, segments, segmentsCount, false);
            this.Close();
            sieve.StartSieveCallBack(track_val,larger,smaller);
        }

        

        private Queue<position>[] Pick(bool[,] img)
        {
            int w = img.GetLength(0);
            int h = img.GetLength(1);
            bool[,] flag = new bool[w, h];

            Queue<position> queue = new Queue<position>();
            List<Queue<position>> segments = new List<Queue<position>>();
            //true mean white
            int[,] dir = new int[8, 2] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 },
                                            {1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }};
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (flag[i, j] == false && img[i, j] == true)
                    {
                        queue.Enqueue(new position(i, j));
                        flag[i, j] = true;
                    }
                    var newsegement = new Queue<position>();
                    while (queue.Count != 0)
                    {
                        position p = queue.Dequeue();
                        newsegement.Enqueue(p);
                        for (int k = 0; k  < 8; k++)
                        {
                            position np = new position(p.x + dir[k, 0], p.y + dir[k, 1]);
                            if (np.x < 0 || np.y < 0 || np.x >= w || np.y >= h)
                            {
                                continue;
                            }
                            if (flag[np.x, np.y] == false && img[np.x, np.y] == true)
                            {
                                if (img[np.x, np.y] == true)
                                {
                                    queue.Enqueue(np);
                                }
                            }
                            flag[np.x, np.y] = true;
                        }
                    }
                    if (newsegement.Count != 0)
                    {
                        segments.Add(newsegement);
                    }

                }
            }
            return segments.ToArray();
        }

        private void Draw(int threshold, Queue<position>[] segments,int[] Count = null)
        {
            if (Count == null || Count.Length != segments.Length)
            {
                Count = new int[segments.Length];
                for (int i = 0; i < segments.Length; i++)
                {
                    Count[i] = segments[i].Count;
                }
            }
     
            PictureBox1.Image = draw(threshold, segments, Count,true);
            int numberofSegmentsDrawn;
            numberofSegmentsDrawn = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (Count[i] >= threshold)
                {
                    numberofSegmentsDrawn++;
                }
            }
            label1.Text = "Number of Segments:" + numberofSegmentsDrawn;
             
        }

        private Bitmap draw(int threshold, Queue<position>[] segments, int[] Count, bool larger)
        {
            bool[,] largerimg;
            largerimg = (bool[,])OriginalImageArray.Clone();
            //draw image
            if (larger)
            {
                Parallel.For(0, segments.Length, i =>
                {
                    if (Count[i] < threshold)
                    {
                        position[] segment = segments[i].ToArray();
                        Parallel.For(0, segment.Length, j =>
                        {
                            position p = segment[j];
                            largerimg[p.x, p.y] = false;
                        });
                    }
                });
            }
            else
            {
                Parallel.For(0, segments.Length, i =>
                {
                    if (Count[i] >= threshold)
                    {
                        position[] segment = segments[i].ToArray();
                        Parallel.For(0, segment.Length, j =>
                        {
                            position p = segment[j];
                            largerimg[p.x, p.y] = false;
                        });
                    }
                });
            }
            return PImage.NetArray2Bitmap(largerimg, pixelFormat);
        }

     

        struct position
        {
            public position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int x, y;
        }

        private void SieveUI_Load(object sender, EventArgs e)
        {
            RefreshPictureBoxSize();
        }
        private void RefreshPictureBoxSize()
        {
            groupBox2.Size = new Size(Size.Width- 310, Size.Height - 30);
            groupBox2.Location = new Point(310,1);
            groupBox1.Size = new Size(300, Size.Height - 30);
            PictureBox1.SetBounds(0, 0, groupBox2.Size.Width - 20, groupBox2.Size.Height - 20);
        }
        //Maximize and minimize handle
        private void Form2_Resize(object sender, EventArgs e)
        {
            RefreshPictureBoxSize();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
