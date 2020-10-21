using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using FKgrain.UI;
namespace FKgrain {
    public partial class Form1 : Form {
        public static Form1 instance;
        BackgroundWorker backgroundWorker1;
        public Form1() {
            InitializeComponent();
            instance = this;
            label1.Text = "Starting MR";
        
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler((obj, e) => {
                Bitmap b = new Bitmap("iti.bmp");
                PImage.processor.Erosion(PImage.Bitmap2array(b), "square", 2);
                PImage.init = true;
            });
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler((obj,e) => {
                label1.Text = "MR Done";
                label1.Refresh();
            });
            backgroundWorker1.RunWorkerAsync();
        }
        public SaveFileDialog GetSaveFileDialog() {
            return saveFileDialog1;
        }
        public OpenFileDialog GetOpenFileDialog() {
            return openFileDialog1;
        }

        BackgroundWorker backgroundWorker2 = new BackgroundWorker();
        public void CreateShpFile(string output, string ellipse, string boundaries , string tfw)
        {
            while (backgroundWorker2.IsBusy)
            {
            }
            backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.DoWork += new DoWorkEventHandler((s, ee) => {
                string strCmdText;
                string path = Directory.GetCurrentDirectory();
                string exe_path = Path.Combine(path, "DrawShp","main.exe");
                strCmdText = String.Format("/c {4} --o={0} --eclipse={1} --boundaries={2} --tfw={3}", output, ellipse, boundaries, tfw, exe_path);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.UseShellExecute = true;
                startInfo.Arguments = strCmdText;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            });
            backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler((a, ee) => {
                File.Delete(ellipse);
                File.Delete(boundaries);
            });
            backgroundWorker2.RunWorkerAsync();
        }

        private void generateLocalWorldFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select Image File";
            openFileDialog1.Filter = "bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string openfile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
                if (!openfile.EndsWith(".tif"))
                {
                    Bitmap b = new Bitmap(openfile);
                    string extension = Path.GetExtension(openfile);
                    b.Save(Path.ChangeExtension(openfile,".tif"), System.Drawing.Imaging.ImageFormat.Tiff);
                }
                TFWGenerator tFWGenerator = new TFWGenerator(Path.ChangeExtension(openfile, ".tif"));
                tFWGenerator.StartPosition = FormStartPosition.CenterParent;
                tFWGenerator.ShowDialog(this);
            }
            else
            {
                return;
            }
        }

        private void zerocontourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an DEM file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile, savefile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            saveFileDialog1.Title = "Save Output";
            saveFileDialog1.Filter = "All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savefile = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            UI.ProgressForm f = new ProgressForm();
            f.Show();
            f.DoAll(openfile, savefile);
        }


        private void planerDetrendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an DEM file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile, savefile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            saveFileDialog1.Title = "Save Detrended DEM";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savefile = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            ExtraProgram.PlanarDetrending(openfile, savefile, label1);
        }

        private void vGMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an Detrended DEM file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile, savefile,modelsavefile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            saveFileDialog1.Title = "Save VGM Result";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savefile = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            saveFileDialog1.Title = "Save VGM model Result";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FileName = "";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                modelsavefile = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            ExtraProgram.VGM(openfile, savefile, modelsavefile,label1,null);
        }

        private void factorialKrigingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an Detrended DEM file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile, savefile, VGMmodel;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            openFileDialog1.Title = "Pick an VGM modeling file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                VGMmodel = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            saveFileDialog1.Title = "Save Krigging Result";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savefile = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            //add header , and copy to specific path
            string DetrendFileName = "DetrendedDEM.txt";
            string FactorialResult = "DetrendedDEM.out";
            string TemplateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", "template.par");
            string ParFilePath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", "fk_large.exe.par");
            string DetrendingDSMPath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", DetrendFileName);
            string FactorialResultPath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", FactorialResult);
            ExtraProgram.SetupHeaderForKriging(openfile, DetrendingDSMPath);
            // Create Par file
            ExtraProgram.CreateParFile(DetrendFileName, FactorialResult, VGMmodel, TemplateFilePath, ParFilePath,label1);
            //run program
            ExtraProgram.FactorialKrigging(label1, FactorialResultPath,true,(l)=> {
                Thread.Sleep(100);
                if (File.Exists(savefile))
                {
                    File.Delete(savefile);
                }
                File.Copy(FactorialResultPath, savefile);
            });
        }

        private void zeroContourToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an Krigging file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile, savefile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            saveFileDialog1.Title = "Save Zero Contour Image";
            saveFileDialog1.Filter = "tif files (*.tif)|*.tif|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                savefile = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            
            ExtraProgram.GenerateSHPContour(openfile, savefile,label1,() => {
                Bitmap zero = new Bitmap(savefile);
                ExtraProgram.OpenPreviewForm(zero, "Zero-Level Contour");
            });
      
        }

        private void dSMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an DEM file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            ExtraProgram.OpenPreviewForm(ExtraProgram.DrawInputDSM(openfile),"Original DEM");
        }

        private void detrendDSMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an Detrend DEM file";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string openfile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            ExtraProgram.OpenPreviewForm(ExtraProgram.DrawDetrendDSM(openfile),"Detrended DEM");
        }

        private void kriggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an Factorial Result";
            openFileDialog1.Filter = "out files (*.out)|*.out|All files (*.*)|*.*";
            string openfile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            Bitmap[] s = ExtraProgram.DrawKrigingShortAndLongComponent(openfile);
            if(s != null)
            {
                ExtraProgram.OpenPreviewForm(s[0], "FK short range DEM");
                ExtraProgram.OpenPreviewForm(s[1], "FK long range DEM");;
                ExtraProgram.OpenPreviewForm(s[2], "Ordinary Kriged DEM");
            }
        }

        private void zeroContourToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Pick an zero contour image";
            openFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string openfile;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openfile = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
            ExtraProgram.OpenPreviewForm(new Bitmap(openfile), "Zero-Level Contour");
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var project = new UI.ProjectForm();
            project.Show();
        }
    }
}
