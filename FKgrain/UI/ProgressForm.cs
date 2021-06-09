using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FKgrain.UI
{
    public partial class ProgressForm : Form
    {
        bool detrending = false;
        bool calVGM = false;
        bool calFk = false;
        bool calCon = false;
        public ProgressForm()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            detrend.Text = "1. Detrend DEM";
            VGM.Text = "2. Calculate Variogram Model";
            krigging.Text = "3. Calculate Kriging Model";
            zerocontour.Text = "4. Generate Zero-Level Contour";
        }

        string DetrendFileName;
        string FactorialResult;
        string outPath;
        string outPrefix;
        string DetrendingDSMPath;
        string FactorialResultPath;
        string VGMFile;
        string VGMModelFile;
        string TemplateFilePath;
        string ParFilePath;
        string ImageOutPath;
        string DSMPath;

        public void DoAll(string DSMPath, string ImageOutPath)
        {
            Show();
            Update();
            this.ImageOutPath = ImageOutPath.Split('.')[0];
            this.DSMPath = DSMPath;
            DetrendFileName = "DetrendedDEM.txt";
            FactorialResult = "FactorialResult.out";
            outPath = Path.GetDirectoryName(ImageOutPath);
            outPrefix = Path.GetFileName(ImageOutPath).Split('.')[0];
            DetrendingDSMPath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", DetrendFileName);
            FactorialResultPath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", FactorialResult);
            VGMFile = Path.Combine(Directory.GetCurrentDirectory(), "VariogramModel.txt");
            VGMModelFile = Path.Combine(Directory.GetCurrentDirectory(), "variogram_modelling.txt");
            TemplateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", "template.par");
            ParFilePath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", "fk_large.exe.par");
            BackgroundWorker worker = new BackgroundWorker();
            Show();
            ExtraProgram.OpenPreviewForm(ExtraProgram.DrawInputDSM(DSMPath), "Original DEM");
            Focus();
            BackgroundWorker w= null;
            worker.DoWork += new DoWorkEventHandler((ss, eee) =>
            {
                w = Start_Detrending();
                PImage.processor.ho5dtdem(DSMPath, DetrendingDSMPath);

            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, ee) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Detrending_Done(w);
                    ExtraProgram.OpenPreviewForm(ExtraProgram.DrawDetrendDSM(DetrendingDSMPath), "Detrended DEM");
                    RunVGM();
                });
                worker.Dispose();
            });
            worker.RunWorkerAsync();
            Show();
            Focus();
        }

        private void RunVGM()
        {
            BackgroundWorker w = Start_VGM();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((ss, eee) =>
            {
                ExtraProgram.VGM(DetrendingDSMPath, VGMFile, VGMModelFile, null, null);


            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, ee) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    VGM_Done(w);
                    RunKrigging();
                });
                worker.Dispose();
            });
            worker.RunWorkerAsync();
        }
        private void RunKrigging()
        {
            BackgroundWorker w = Start_Krigging();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((ss, eee) =>
            {
                ExtraProgram.SetupHeaderForKriging(DetrendingDSMPath);
                ExtraProgram.CreateParFile(DetrendFileName, FactorialResult, VGMModelFile, TemplateFilePath, ParFilePath, null);
                ExtraProgram.FactorialKrigging(null, FactorialResultPath);

            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, ee) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Krigging_Done(w);
                    Bitmap[] bitmaps = ExtraProgram.DrawKrigingShortAndLongComponent(FactorialResultPath);
                    if (bitmaps == null)
                    {
                        return;
                    }
                    ExtraProgram.OpenPreviewForm(bitmaps[0], "FK short range DEM");
                    ExtraProgram.OpenPreviewForm(bitmaps[1], "FK long range DEM");
                    ExtraProgram.OpenPreviewForm(bitmaps[2], "Ordinary Kriged DEM");
                    GenerateContour();
                });
                worker.Dispose();
            });
            worker.RunWorkerAsync();
        }
        public void SetInOtherThread(Label l, string txt)
        {
            l.Invoke((MethodInvoker)delegate
            {
                l.Text = txt;
            });
        }

        public void GenerateContour()
        {
            BackgroundWorker w = Start_ZeroContour();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((ss, eee) =>
            {
                ExtraProgram.GenerateSHPContour(FactorialResultPath, ImageOutPath + "_Zero_Level_Contour_.tif", null, null);
            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, ee) =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ZeroContour_Done(w);
                    SaveResult();
                });
                worker.Dispose();
            });
            worker.RunWorkerAsync();
        }

        public void SaveResult()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((ss, eee) =>
            {
                if (File.Exists(Path.Combine(outPath, $"{outPrefix}_{DetrendFileName}")))
                {
                    File.Delete(Path.Combine(outPath, $"{outPrefix}_{DetrendFileName}"));
                }
                File.Copy(DetrendingDSMPath, Path.Combine(outPath, $"{outPrefix}_{DetrendFileName}"));
                if (File.Exists(Path.Combine(outPath, $"{outPrefix}_VGM.txt")))
                {
                    File.Delete(Path.Combine(outPath, $"{outPrefix}_VGM.txt"));
                }
                File.Copy(VGMFile, Path.Combine(outPath, $"{outPrefix}_VGM.txt"));
                if (File.Exists(Path.Combine(outPath, $"{outPrefix}_VGM_Model.txt")))
                {
                    File.Delete(Path.Combine(outPath, $"{outPrefix}_VGM_Model.txt"));
                }
                File.Copy(VGMModelFile, Path.Combine(outPath, $"{outPrefix}_VGM_Model.txt"));
                if (File.Exists(Path.Combine(outPath, $"{outPrefix}_{FactorialResult}")))
                {
                    File.Delete(Path.Combine(outPath, $"{outPrefix}_{FactorialResult}"));
                }
                File.Copy(FactorialResultPath, Path.Combine(outPath, $"{outPrefix}_{FactorialResult}"));
                ExtraProgram.SaveResultTiff(DSMPath, Path.Combine(outPath, $"{outPrefix}_{DetrendFileName}"), Path.Combine(outPath, $"{outPrefix}_{FactorialResult}"));
            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, ee) =>
            {
                string filename = Path.GetFileName(Path.ChangeExtension(DSMPath, ".tif"));
                if (File.Exists(Path.Combine(outPath, filename)))
                {
                    File.Delete(Path.Combine(outPath, filename));
                }
                File.Move(Path.ChangeExtension(DSMPath, ".tif"), Path.Combine(outPath, filename));
                if (File.Exists(Path.Combine(outPath, Path.ChangeExtension(filename,".tfw"))))
                {
                    File.Delete(Path.Combine(outPath, Path.ChangeExtension(filename, ".tfw")));
                }
                File.Move(Path.ChangeExtension(DSMPath, ".tfw"), Path.Combine(outPath, Path.ChangeExtension(filename, ".tfw")));
                this.Invoke((MethodInvoker)delegate
                {
                    Bitmap zero;
                    if (string.IsNullOrWhiteSpace(Path.GetExtension(ImageOutPath)))
                    {
                        zero = new Bitmap(ImageOutPath + "_Zero_Level_Contour_.tif");
                    }
                    else
                    {
                        zero = new Bitmap(Path.ChangeExtension(ImageOutPath, "_Zero_Level_Contour_.tif"));
                    }
                    ExtraProgram.OpenPreviewForm(zero, "Zero-Level Contour");
                    Close();
                });
                worker.Dispose();
            });
            worker.RunWorkerAsync();
        }

        public void Detrending_Done(BackgroundWorker w)
        {
            detrending = false;
            SetInOtherThread(detrend, "1. Finish Detrend DEM [Done]");
        }
        public BackgroundWorker Start_Detrending()
        {
            // SetInOtherThread(detrend, "1. Detrending DEM...");
            detrending = true;
            return StartRunning(detrend, "1. Detrending DEM",()=> { return this.detrending; });
        }
        public void VGM_Done(BackgroundWorker w)
        {
            calVGM = false;
            SetInOtherThread(VGM, "2. Finish Calculate Variogram Model [Done]");
        }
        public BackgroundWorker Start_VGM()
        {
            calVGM = true;
            return StartRunning(VGM, "2. Calculating Variogram Model", () => { return this.calVGM; });
            //SetInOtherThread(VGM, "2. Calculating Variagram Model...");
        }
        public void Krigging_Done(BackgroundWorker w)
        {
            calFk = false;
            SetInOtherThread(krigging, "3. Finish Calculate Kriging Model [Done]");
        }
        public BackgroundWorker Start_Krigging()
        {
            calFk = true;
            return StartRunning(krigging, "3. Calculating Kriging Model", () => { return this.calFk; });

            //SetInOtherThread(krigging, "3. Calculating Krigging Model...");
        }
        public void ZeroContour_Done(BackgroundWorker w)
        {
            calCon = false;
            SetInOtherThread(zerocontour, "4. Finish Generate Zero-Level Contour [Done]");
        }
        public BackgroundWorker Start_ZeroContour()
        {
            calCon = true;
            return StartRunning(zerocontour, "4. Generating Zero-Level Contour", () => { return this.calCon; });
            //            SetInOtherThread(zerocontour, "4. Generating Zero-Level Contour...");
        }

        public BackgroundWorker StartRunning(Label l, string pre,Func<bool> running)
        {
            BackgroundWorker progressWorker = new BackgroundWorker();
            progressWorker.WorkerSupportsCancellation = true;
            progressWorker.DoWork += new DoWorkEventHandler((s, e) =>
            {
                BackgroundWorker w = (BackgroundWorker)s;
                System.DateTime t = DateTime.Now;
                while (running() == true)
                {
                    //SetInOtherThread(l, $"{w.CancellationPending},{w.ToString()}");
                    TimeSpan diff = DateTime.Now.Subtract(t);
                    SetInOtherThread(l, pre + $" {diff.Minutes.ToString("D2")}:{diff.Seconds.ToString("D2")}");
                    Thread.Sleep(100);
                }
            });
            progressWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((ss, ee) =>
            {
                progressWorker.Dispose();
            });
            progressWorker.RunWorkerAsync();
            return progressWorker;
        }

        private void zerocontour_Click(object sender, EventArgs e)
        {

        }
    }
}
