using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace FKgrain.UI
{
    public partial class ProjectForm : Form
    {
        #region Constructor
        public static ProjectForm instance;
        public ProjectForm()
        {
            InitializeComponent();
            instance = this;
            //initialAll_Button();
            sieves = new List<Sieve>();
            AddSieve(0);
        }
        public Sieve AddSieve(int index)
        {
            int Height = 100;
            GroupBox gp = new GroupBox();
            flowbox.Controls.Add(gp);
            gp.Location = new Point(3, 3 + (sieves.Count * Height));
            int space = 3 + ((sieves.Count + 1) * Height);
            label1.Text = $"add Level {sieves.Count + 1}";
            flowbox.Size = new Size(flowbox.Size.Width, space + 50);
            if(flowbox.Size.Height > SieveMaster_box.Size.Height-75)
            {
                vScrollBar1.Maximum = flowbox.Size.Height - (SieveMaster_box.Size.Height-75);
            }
            gp.Size = new Size(flowbox.Width-4, Height-5);
            gp.Text = $"Level #{sieves.Count + 1}";
            gp.Visible = true;
            var si = new Sieve(gp, null, this);
            si.index = index;
            sieves.Add(si);
            return si;
        }
       
        public ProjectForm(string FilePath)
        {
            OriginalImageFilePath = FilePath;
            OriginalImage = new Bitmap(FilePath);
        }
        private void initialAll_Button()
        {
       
        }

        #endregion

        #region Parameters
        private string OriginalImageFilePath;
        public Bitmap OriginalImage;
        private const string PreprocessedImageFilePath = "tmp/preprocess.bmp";
        private Bitmap PreprocessedImage;
        public ImageForm ProcessingImageForm;
        private List<Sieve> sieves;
        #endregion

        #region Original Function

        private void SelectImage_Btn_Click(object sender, EventArgs e)
        {

            if (ProcessingImageForm != null)
            {
               // ShowImageFormOpenAlertPopUpWindow();
                ProcessingImageForm.Focus();
                return;
            }

            openFileDialog1.Title = "Pick an image file";
            openFileDialog1.Filter = "tif files (*.tif)|*.tif|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string openfile = openFileDialog1.FileName;
                if (!openfile.EndsWith(".tif"))
                {
                    Bitmap b = new Bitmap(openfile);
                    string extension = Path.GetExtension(openfile);
                    b.Save(Path.ChangeExtension(openfile, ".tif"), System.Drawing.Imaging.ImageFormat.Tiff);
                }
                if (!File.Exists(Path.ChangeExtension(openfile, ".tfw"))) {
                    TFWGenerator tFWGenerator = new TFWGenerator(Path.ChangeExtension(openfile, ".tif"));
                    tFWGenerator.StartPosition = FormStartPosition.CenterParent;
                    DialogResult res = tFWGenerator.ShowDialog(this);
                    if (res != DialogResult.OK)
                    {
                        return;
                    }
                }
                OriginalImageFilePath = Path.ChangeExtension(openfile, ".tif");
                OriginalImage = new Bitmap(OriginalImageFilePath);
                OpenImageForm(OriginalImage,"Stage 1",(image,form)=> {
                    PreprocessedImage = image;
                    SieveMaster_box.Visible = true;
                    flowbox.Visible = true;
                    sieves[0].groupBox.Visible = true;
                    sieves[0].InitiateSieve(PreprocessedImage, 0);
                    flowbox.Refresh();
                    Focus();
                    TopMost = true;
                    Show();
                    TopMost = false;
                    Select_Ori_Btn.Enabled = false;
                },"Enter Stage 2");
                OriginalImage_Lbl.Text = "Processing :" + Path.GetFileName(OriginalImageFilePath);
            }
        }
   
        public SaveFileDialog SaveFilePrompt(string Title, string Filter)
        {
            saveFileDialog1.Title = Title;
            saveFileDialog1.Filter = Filter;
            return saveFileDialog1;
        }

        #endregion

        #region Preprocessing
     
        private void FormCloseAlert(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Discard Change?",
                  "Discard all change in this session?",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        #endregion

        private void ShowImageFormOpenAlertPopUpWindow()
        {
            //TODO Show PopUP
            MessageBox.Show("Process window has Opened", "Process window had already been opened.", MessageBoxButtons.OK);
        }

        public void OpenImageForm(Bitmap image,string title, Action<Bitmap, ImageForm> OnDone,string DoneButtonText, Action OnCloseCallBack = null)
        {
            if (ProcessingImageForm != null)
            {
                ShowImageFormOpenAlertPopUpWindow();
                return;
            }
            ImageForm form = new ImageForm(image, title, this.Location, (s, ev) =>
            {
                ProcessingImageForm.FormClosing -= FormCloseAlert;
                if (OnDone != null)
                {
                    OnDone(s, ev);
                }
            }, OriginalImage,DoneButtonText);
            ProcessingImageForm = form;
     
            ProcessingImageForm.FormClosing += FormCloseAlert;
            ProcessingImageForm.FormClosed += (s, ev) =>
            {
                ProcessingImageForm = null;
                if (OnCloseCallBack != null)
                {
                    OnCloseCallBack();
                }

            };
            ProcessingImageForm.ShowDialog();
        }


        public void OpenPreviewForm(Bitmap image, string title = "Preview Image")
        {
            ImageForm form = new ImageForm(image,title, this.Location, true,OriginalImage);
            form.Show();
        }

        public void CombineAllSieveResult()
        {
            int width = sieves[0].image.Width;
            int height = sieves[0].image.Height;
            Bitmap baseImage = NativeIP.FastBinaryConvert(sieves[0].image);


            for (int i = 1; i < sieves.Count; i++)
            {
                if (sieves[i].image == null || sieves[i].OriImage == null)
                {
                    break;
                }
                Bitmap NextImage = NativeIP.FastBinaryConvert(sieves[i].image);

                baseImage = NativeIP.FastCombineBinary(NextImage, baseImage);
            }
            OpenPreviewForm(baseImage);
        }

        private bool CheckTFW()
        {
            string tfwpath = Path.ChangeExtension(OriginalImageFilePath, ".tfw");
            if (!File.Exists(tfwpath))
            {
                //TODO generate tfw
                TFWGenerator tFWGenerator =  new TFWGenerator(OriginalImageFilePath);
                tFWGenerator.StartPosition = FormStartPosition.CenterParent;
                var Result = tFWGenerator.ShowDialog(this);
                if (Result != DialogResult.OK)
                {
                    return false;
                }
                else
                {
                    MessageBox.Show("world file created",
              "World File succesfully created.",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
                }
            }
            return true;
        }

        private void finishProject_btn_Click(object sender, EventArgs e)
        {
            if (CheckTFW() == false)
            {
                MessageBox.Show("world file not found!",
                 "Please Create a world file to export result",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
                return; 
            }
            List<string> Ellipse = new List<string>();
            List<int> size = new List<int>();
            int coCount = 0;
            for (int i = 0; i < sieves.Count; i++)
            {
                if (sieves[i] == null)
                {
                    break;
                }
                if (sieves[i].Ellipse == null)
                {
                    break;
                }
                for (int j = 0; j < sieves[i].Ellipse.Length; j++)
                {
                    Ellipse.Add(sieves[i].Ellipse[j]);
                    size.Add(sieves[i].size[j]);
                }
                coCount += sieves[i].coordinates.GetLength(0);
            }
            int[,] coor = new int[coCount, 2];
            int index = 0;
            foreach (Sieve s in sieves)
            {
                if (s == null)
                {
                    break;
                }
                if (s.Ellipse == null)
                {
                    break;
                }
                for (int i = 0; i < s.coordinates.GetLength(0); i++)
                {
                    coor[index, 0] = s.coordinates[i, 0];
                    coor[index, 1] = s.coordinates[i, 1];
                    index++;
                }
            }
            Sieve combine = new Sieve(this);
            combine.OriImage = OriginalImage;
            combine.Ellipse = Ellipse.ToArray();
            combine.size = size.ToArray();
            combine.coordinates = coor;
            string tfwPath = Path.ChangeExtension(OriginalImageFilePath, ".tfw");
            string path = combine.SaveResult(tfwPath);
            if (path != "")
            {
                label1.Text = "Saved\n " + path;
            }
        }
        private int oriflowLocation = 21;
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
           // label1.Text = vScrollBar1.Value.ToString()  + flowbox.Location.ToString();
            flowbox.Location = new Point(flowbox.Location.X, oriflowLocation - vScrollBar1.Value);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
