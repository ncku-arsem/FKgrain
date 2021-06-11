using System;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathWorks.MATLAB.NET.Arrays;
using System.Drawing;
using FKgrain.UI;
using System.Collections.Generic;

namespace FKgrain
{
    static class ExtraProgram
    {
        public static Encoding encoding = new UTF8Encoding(false);
        public static System.Diagnostics.ProcessWindowStyle windowstyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        public static void DoAll(string DSMPath, string ImageOutPath, Label label)
        {
            string DetrendFileName = "DetrendedDEM.txt";
            string FactorialResult = "FactorialResult.out";
            string outPath = Path.GetDirectoryName(ImageOutPath);
            string outPrefix = Path.GetFileName(ImageOutPath).Split('.')[0];
            string DetrendingDSMPath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", DetrendFileName);
            string FactorialResultPath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", FactorialResult);
            //hot5dtdem
            PlanarDetrending(DSMPath, DetrendingDSMPath, label);
            //vgm
            string VGMFile = Path.Combine(Directory.GetCurrentDirectory(), "VariogramModel.txt");
            string VGMModelFile = Path.Combine(Directory.GetCurrentDirectory(), "variogram_modelling.txt");
            string TemplateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", "template.par");
            string ParFilePath = Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", "fk_large.exe.par");

            VGM(DetrendingDSMPath, VGMFile, VGMModelFile, label, (la) =>
             {
                 //Facotorial
                 CreateParFile(DetrendFileName, FactorialResult, VGMModelFile, TemplateFilePath, ParFilePath, la);
                 SetupHeaderForKriging(DetrendingDSMPath);
                 FactorialKrigging(la, FactorialResultPath, true, (l) =>
                     {
                         GenerateSHPContour(FactorialResultPath, ImageOutPath, l, () =>
                         {
                             Bitmap zero = new Bitmap(Path.ChangeExtension(ImageOutPath, ".tif"));
                             ExtraProgram.OpenPreviewForm(zero, "Zero-Level Contour");
                             File.Copy(DetrendingDSMPath, Path.Combine(outPath, $"{outPrefix}_{DetrendFileName}"));
                             File.Copy(VGMFile, Path.Combine(outPath, $"{outPrefix}_VGM.txt"));
                             File.Copy(VGMModelFile, Path.Combine(outPath, $"{outPrefix}_VGM_Model.txt"));
                             File.Copy(FactorialResultPath, Path.Combine(outPath, $"{outPrefix}_{FactorialResult}"));
                         });
                     });
             });
        }

        public static void SetupHeaderForKriging(string filePath, string outpath = "")
        {
            if (string.IsNullOrEmpty(outpath))
            {
                outpath = filePath;
            }
            //setup header
            string currentContent = String.Empty;
            if (File.Exists(filePath))
            {
                currentContent = File.ReadAllText(filePath, encoding);
                if (!currentContent.StartsWith("DetrendedDEM\n3\nx\ny\nz\n"))
                {
                    File.WriteAllText(outpath, "DetrendedDEM\n3\nx\ny\nz\n" + currentContent, encoding);
                }
                else
                {
                    File.WriteAllText(outpath, currentContent, encoding);
                }
            }
        }

        public static void ReformatKriggingResult(string openfile, string savepath)
        {
            string[] krigResult = File.ReadAllLines(openfile, encoding);
            int columenum = int.Parse(krigResult[1]);
            float diff, sx, sy;
            StringBuilder sb = new StringBuilder();
            int xsize, ysize;
            {
                string line = krigResult[2 + columenum].Replace("      ", ",");
                line = line.Replace("     ", ",");
                line = line.Replace("    ", ",");
                string[] colume = line.Split(',');
                sx = float.Parse(colume[1]);
                sy = float.Parse(colume[2]);
                line = krigResult[krigResult.Length - 1].Replace("      ", ",");
                line = line.Replace("     ", ",");
                line = line.Replace("    ", ",");
                colume = line.Split(',');
                float ex = float.Parse(colume[1]);
                float ey = float.Parse(colume[2]);
                line = krigResult[krigResult.Length - 2].Replace("      ", ",");
                line = line.Replace("     ", ",");
                line = line.Replace("    ", ",");
                colume = line.Split(',');
                diff = Math.Abs(ex - float.Parse(colume[1]));
                xsize = (int)((ex - sx) / diff) + 1;
                ysize = (int)((ey - sy) / diff) + 1;

            }
            for (int i = 2 + columenum; i < krigResult.Length; i++)
            {
                string line = krigResult[i].Replace("      ", ",");
                line = line.Replace("     ", ",");
                line = line.Replace("    ", ",");
                string[] colume = line.Split(',');
                float x = float.Parse(colume[1]);
                float y = float.Parse(colume[2]);
                float z = float.Parse(colume[5]);
                int xindex = (int)((x - sx) / diff);
                int yindex = (int)((y - sy) / diff);
                sb.AppendFormat("{0}\t{1}\t{2}\n", x, y, z);
            }
            File.WriteAllText(savepath, sb.ToString(), encoding);
        }

        public static Bitmap GenerateContour(string path)
        {
            //path = @"F:\PersonalProject\Stone\UIForm\From\bin\x64\Debug\factorialkriging\ho1S4RPdtNZdsm-49.out";
            string[] krigResult = File.ReadAllLines(path, encoding);
            int columenum = int.Parse(krigResult[1]);
            float[,] Z;
            float diff, sx, sy;
            int xsize, ysize;
            {
                string line = krigResult[2 + columenum].Replace("      ", ",");
                line = line.Replace("     ", ",");
                string[] colume = line.Split(',');
                sx = float.Parse(colume[1]);
                sy = float.Parse(colume[2]);
                line = krigResult[krigResult.Length - 1].Replace("      ", ",");
                line = line.Replace("     ", ",");
                colume = line.Split(',');
                float ex = float.Parse(colume[1]);
                float ey = float.Parse(colume[2]);
                line = krigResult[krigResult.Length - 2].Replace("      ", ",");
                line = line.Replace("     ", ",");
                colume = line.Split(',');
                diff = Math.Abs(ex - float.Parse(colume[1]));
                xsize = (int)((ex - sx) / diff) + 1;
                ysize = (int)((ey - sy) / diff) + 1;
                Z = new float[xsize, ysize];
            }
            for (int i = 2 + columenum; i < krigResult.Length; i++)
            {
                string line = krigResult[i].Replace("      ", ",");
                line = line.Replace("     ", ",");
                string[] colume = line.Split(',');
                float x = float.Parse(colume[1]);
                float y = float.Parse(colume[2]);
                float z = float.Parse(colume[5]);
                int xindex = (int)((x - sx) / diff);
                int yindex = (int)((y - sy) / diff);
                Z[xindex, yindex] = z;
            }
            MWNumericArray arr = new MWNumericArray(Z);
            MWArray BoundResult = PImage.processor.Contour(arr);
            double[,] bound = (double[,])BoundResult.ToArray();
            int[,] coordinates = new int[bound.GetLength(1), 2];
            for (int i = 0; i < bound.GetLength(1); i++)
            {
                coordinates[i, 0] = (int)Math.Round(bound[0, i]);
                coordinates[i, 1] = (int)Math.Round(bound[1, i]);
            }
            //Bitmap bi = new Bitmap(Z.GetLength(0), Z.GetLength(1), PixelFormat.Format24bppRgb);
            bool[,] bi = new bool[Z.GetLength(0), Z.GetLength(1)];

            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    bi[i, j] = true;
                }
            }
            for (int i = 0; i < bound.GetLength(1); i++)
            {
                int x = coordinates[i, 1];
                int y = coordinates[i, 0];
                if (x < xsize && y < ysize)
                {
                    bi[x, ysize - y - 1] = false;
                }
            }
            return PImage.NetArray2Bitmap(bi, PixelFormat.Format24bppRgb);
        }

        public static void SaveResultTiff(string dem, string detrenddem, string fk)
        {
            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler((s, ee) =>
            {
                string strCmdText;
                string path = Directory.GetCurrentDirectory();
                string exe_path = Path.Combine(path, "DrawTiff", "main.exe");
                strCmdText = String.Format("/c {3} --fk=\"{0}\" --dem=\"{1}\" --detrenddem=\"{2}\"", fk, dem, detrenddem, exe_path);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = windowstyle;
                startInfo.FileName = "cmd.exe";
                startInfo.UseShellExecute = true;
                startInfo.Arguments = strCmdText;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            });
            backgroundWorker1.RunWorkerAsync();
            while (backgroundWorker1.IsBusy)
            {
                Thread.Sleep(100);
            }
        }


        public static void GenerateSHPContour(string openfile, string savefile, Label label, Action OnDone,bool wait = true)
        {
            string intemediatekrggingResult = Path.Combine(Directory.GetCurrentDirectory(), "zerocontour", "kriged_result_img.txt");
            ExtraProgram.ReformatKriggingResult(openfile, intemediatekrggingResult);
            string intemediate = Path.Combine(Directory.GetCurrentDirectory(), "zerocontour", "OUTPUT.shp");

            string[] krigResult = File.ReadAllLines(intemediatekrggingResult, encoding);
            float diff, sx, sy, ex, ey;
            int xsize, ysize;
            {
                string[] colume = krigResult[0].Split('\t');
                sx = float.Parse(colume[0]);
                sy = float.Parse(colume[1]);
                colume = krigResult[krigResult.Length - 1].Split('\t');
                ex = float.Parse(colume[0]);
                ey = float.Parse(colume[1]);
                colume = krigResult[krigResult.Length - 2].Split('\t');
                diff = Math.Abs(ex - float.Parse(colume[0]));
                if (diff == 0)
                {
                    diff = Math.Abs(ey - float.Parse(colume[1]));
                }
                xsize = (int)((ex - sx) / diff) + 1;
                ysize = (int)((ey - sy) / diff) + 1;
            }

            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler((s, ee) =>
            {
                string strCmdText;
                string path = Directory.GetCurrentDirectory();
                string exe_path = Path.Combine(path, "zerocontour/gdal_contour");
                string command = string.Format("-b 1 -a ELEV -i 10.0 {0} {1}", intemediatekrggingResult, intemediate);
                strCmdText = "/c " + exe_path + " " + command;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = windowstyle;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = Path.Combine(path);
                startInfo.Arguments = strCmdText;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

            });
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler((a, ee) =>
            {
                BackgroundWorker backgroundWorker2 = new BackgroundWorker();
                backgroundWorker2.DoWork += new DoWorkEventHandler((s, eee) =>
                {
                    string strCmdText;
                    string path = Directory.GetCurrentDirectory();
                    string exe_path = Path.Combine(path, "zerocontour/gdal_rasterize");
                    string command = string.Format("-burn 255 -burn 255 -burn 255 -ot Byte -ts {0} {1} -l OUTPUT {2} {3}", xsize * 4, ysize * 4, intemediate, Path.ChangeExtension(savefile, ".tif"));
                    strCmdText = "/c " + exe_path + " " + command;
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = windowstyle;
                    startInfo.FileName = "cmd.exe";
                    startInfo.WorkingDirectory = Path.Combine(path);
                    startInfo.Arguments = strCmdText;
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                });
                backgroundWorker2.RunWorkerCompleted += new RunWorkerCompletedEventHandler((aa, eee) =>
                {

                    /*string bmppath = savefile;
                    Bitmap b = new Bitmap(Path.ChangeExtension(bmppath, ".bmp"));
                    b.Save(Path.ChangeExtension(bmppath, ".tif"), ImageFormat.Tiff);
                    b.Dispose();*/
                    string extension = Path.GetExtension(savefile);
                    string[] tfw = new string[6];

                    tfw[0] = (diff / 4f).ToString();
                    tfw[1] = "0";
                    tfw[2] = "0";
                    tfw[3] = (-diff / 4f).ToString();
                    tfw[4] = (sx + (diff / 4f)).ToString();
                    tfw[5] = (sy + ((diff / 4f) * ysize * 4)).ToString();
                    if (!string.IsNullOrEmpty(extension))
                    {
                        File.WriteAllLines(savefile.Replace(extension, ".tfw"), tfw, encoding);
                    }
                    else
                    {
                        File.WriteAllLines(savefile + ".tfw", tfw, encoding);
                    }
                    if (OnDone != null)
                    {
                        OnDone();
                    }
                    File.Delete(intemediatekrggingResult);
                    File.Delete(intemediate.Replace(".shp", ".dbf"));
                    File.Delete(intemediate.Replace(".shp", ".shx"));
                    File.Delete(intemediate);
                });
                backgroundWorker2.RunWorkerAsync();
       
                while (backgroundWorker2.IsBusy)
                {
                    Thread.Sleep(100);
                }
            });
            backgroundWorker1.RunWorkerAsync();
            if (wait)
            {
                while (backgroundWorker1.IsBusy)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void OpenPreviewForm(Bitmap image, string title = "Preview Image")
        {
            UI.ImagePreview form = new UI.ImagePreview();
            form.PictureBox1.Image = image;
            form.Text = title;
            form.Show();
            form.Refresh();
        }

        public static void PlanarDetrending(string openfile, string savefile, Label label)
        {
            OpenPreviewForm(DrawInputDSM(openfile), "Original DEM");
            Thread.Sleep(100);
            PImage.processor.ho5dtdem(openfile, savefile);
            OpenPreviewForm(DrawDetrendDSM(savefile), "Detrended DEM");
        }

        public static string CreateParFile(string DSMFile, string ReusltFile, string VGMFile, string TemplateFile, string ParFile, Label label)
        {
            string[] KrigRaw = File.ReadAllLines(VGMFile, encoding);
            if (KrigRaw.Length != 4)
            {
                return "Invalid File";
            }
            string[] a = KrigRaw[1].Split(' ');
            float nug = 0;
            if (!float.TryParse(a[5], out nug))
            {
                return "Invalid nug";

            }
            string[] b = KrigRaw[2].Split(' ');
            float cc = 0;
            if (!float.TryParse(b[4], out cc))
            {
                return "Invalid cc";

            }
            float a_hmax = 0;
            if (!float.TryParse(b[5], out a_hmax))
            {
                return "Invalid a_hmax";
            }
            string[] c = KrigRaw[3].Split(' ');
            float cc2 = 0;
            if (!float.TryParse(c[4], out cc2))
            {
                return "Invalid cc2";
            }
            float a_hmax2 = 0;
            if (!float.TryParse(c[5], out a_hmax2))
            {
                return "Invalid a_hmax2";

            }
            //read in template 
            string Par = File.ReadAllText(TemplateFile, encoding);
            //replace string
            Par = Par.Replace("$(cc)", cc.ToString());
            Par = Par.Replace("$(a_hmax)", a_hmax.ToString());
            Par = Par.Replace("$(cc2)", cc2.ToString());
            Par = Par.Replace("$(a_hmax2)", a_hmax2.ToString());
            Par = Par.Replace("$(filename)", DSMFile);
            Par = Par.Replace("$(outname)", ReusltFile);
            //read DEM file , get number of x , get number of y
            // get dx , dy
            // get min x , min y
            string[] dsm = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "factorialkriging", DSMFile));
            int headersize = 5;
            float minx = float.MaxValue;
            float maxx = float.MinValue;
            float miny = float.MaxValue;
            float maxy = float.MinValue;
            float dx = float.MaxValue;
            float dy = float.MaxValue;
            for (int j = headersize; j < dsm.Length; j++)
            {
                string[] coor = dsm[j].Split(' ');
                if (coor.Length != 3)
                {
                    continue;
                }
                float x = float.Parse(coor[0]);
                float y = float.Parse(coor[1]);
                float z = float.Parse(coor[2]);
                minx = Math.Min(x, minx);
                miny = Math.Min(y, miny);
                maxx = Math.Max(x, maxx);
                maxy = Math.Max(y, maxy);
                if (j != headersize)
                {
                    string[] lastcoor = dsm[j - 1].Split(' ');
                    if (lastcoor.Length != 3)
                    {
                        continue;
                    }
                    float lx = float.Parse(lastcoor[0]);
                    float ly = float.Parse(lastcoor[1]);
                    float lz = float.Parse(lastcoor[2]);
                    float ddx = Math.Abs(x - lx);
                    float ddy = Math.Abs(y - ly);
                    if (ddx != 0)
                    {
                        dx = (float)Math.Round(Math.Min(dx, ddx), 2);
                    }
                    if (ddy != 0)
                    {
                        dy = (float)Math.Round(Math.Min(dy, ddy), 2);
                    }
                }

            }

            Par = Par.Replace("$(nx)", ((int)((maxx - minx) / dx)).ToString());
            Par = Par.Replace("$(minx)", minx.ToString("0.000000"));
            Par = Par.Replace("$(dx)", dx.ToString("0.000000"));
            Par = Par.Replace("$(ny)", ((int)((maxy - miny) / dy)).ToString());
            Par = Par.Replace("$(miny)", miny.ToString("0.000000"));
            Par = Par.Replace("$(dy)", dy.ToString("0.000000"));
            Par = Par.Replace("$(srx)", (dx * 20).ToString("0.000000"));
            Par = Par.Replace("$(sry)", (dy * 20).ToString("0.000000"));
            Par = Par.Replace("$(srz)", (dx * 20).ToString("0.000000"));
            //save par and return save path
            File.WriteAllText(ParFile, Par, encoding);
            return "";
        }

        public static void FactorialKrigging(Label label, String resultPath, bool visuallize = false, Action<Label> onDone = null, bool wait = true)
        {

            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler((s, ee) =>
            {
                string strCmdText;
                string path = Directory.GetCurrentDirectory();
                string exe_path = Path.Combine(path, "factorialkriging/filter_Auto");
                strCmdText = "/c " + exe_path;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = windowstyle;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = Path.Combine(path, "factorialkriging");
                startInfo.Arguments = strCmdText;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
      
            });

            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler((a, ee) =>
            {
                if (visuallize)
                {
                    Bitmap[] bitmaps = DrawKrigingShortAndLongComponent(resultPath);
                    if (bitmaps == null)
                    {
                        return;
                    }
                    OpenPreviewForm(bitmaps[0], "FK short range DEM");
                    OpenPreviewForm(bitmaps[1], "FK long range DEM");
                    OpenPreviewForm(bitmaps[2], "Ordinary Kriged DEM");
                }

                if (onDone != null)
                {
                    onDone(label);
                }
                backgroundWorker1.Dispose();
            });
            backgroundWorker1.RunWorkerAsync();
            if (wait)
            {
                while (backgroundWorker1.IsBusy)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void VGM(string openfile, string savefile, string modelfile, Label label, Action<Label> onDone = null)
        {
            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler((s, ee) =>
            {
                string strCmdText;
                strCmdText = "/c Rscript --vanilla ./vgm/ho1vgm.r " + openfile + " " + savefile + " " + modelfile;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = windowstyle;
                startInfo.FileName = "cmd.exe";
                string path = Directory.GetCurrentDirectory();
                startInfo.WorkingDirectory = path;
                startInfo.Arguments = strCmdText;
                startInfo.UseShellExecute = true;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

            });
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler((a, ee) =>
            {

                if (onDone != null)
                {
                    onDone(label);
                }
                backgroundWorker1.Dispose();
            });
            backgroundWorker1.RunWorkerAsync();
            while (backgroundWorker1.IsBusy)
            {
                Thread.Sleep(100);
            }
        }

        public static Bitmap DrawInputDSM(string name)
        {
            string[] dsm = File.ReadAllLines(name, encoding);
            float[,] Z;
            float diff, sx, sy;
            int xsize, ysize;
            {
                string[] colume = dsm[0].Split(' ');
                sx = float.Parse(colume[0]);
                sy = float.Parse(colume[1]);
                colume = dsm[dsm.Length - 1].Split(' ');
                float ex = float.Parse(colume[0]);
                float ey = float.Parse(colume[1]);
                colume = dsm[1].Split(' ');
                diff = Math.Abs(sy - float.Parse(colume[1]));
                xsize = (int)((ex - sx) / diff) + 1;
                ysize = (int)((ey - sy) / diff) + 1;
                Z = new float[xsize + 2, ysize + 2];
            }
            float maxz = float.MinValue;
            float minz = float.MaxValue;
            for (int i = 0; i < dsm.Length; i++)
            {
                string[] colume = dsm[i].Split(' ');
                float x = float.Parse(colume[0]);
                float y = float.Parse(colume[1]);
                float z = float.Parse(colume[2]);
                int xindex = (int)((x - sx) / diff);
                int yindex = (int)((y - sy) / diff);
                Z[xindex, yindex] = z;

                maxz = Math.Max(z, maxz);
                minz = Math.Min(z, minz);
            }
            Color[,] bi = new Color[Z.GetLength(0), Z.GetLength(1)];

            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    bi[i, ysize - j - 1] = RampGenerator.HotCold(Z[i, j], maxz, minz);
                }
            }
            return PImage.NetArray2Bitmap(bi);
        }
        public static Bitmap DrawDetrendDSM(string name)
        {
            string[] dsm = File.ReadAllLines(name, encoding);
            if (dsm[0] == "DetrendedDEM")
            {
                int col = int.Parse(dsm[1]);
                string[] copy = dsm;
                dsm = new string[copy.Length - 2 - col];
                for (int i = 2 + col; i < copy.Length; i++)
                {
                    dsm[i - 2 - col] = copy[i];
                }
            }
            float[,] Z;
            float diff, sx, sy;
            int xsize, ysize;
            {
                string[] colume = dsm[0].Split(' ');
                sx = float.Parse(colume[0]);
                sy = float.Parse(colume[1]);
                colume = dsm[dsm.Length - 1].Split(' ');
                float ex = float.Parse(colume[0]);
                float ey = float.Parse(colume[1]);
                colume = dsm[1].Split(' ');
                diff = Math.Abs(sy - float.Parse(colume[1]));
                xsize = (int)((ex - sx) / diff) + 1;
                ysize = (int)((ey - sy) / diff) + 1;
                Z = new float[xsize + 2, ysize + 2];
            }
            float maxz = float.MinValue;
            float minz = float.MaxValue;
            for (int i = 0; i < dsm.Length; i++)
            {
                string[] colume = dsm[i].Split(' ');
                float x = float.Parse(colume[0]);
                float y = float.Parse(colume[1]);
                float z = float.Parse(colume[2]);
                int xindex = (int)((x - sx) / diff);
                int yindex = (int)((y - sy) / diff);
                Z[xindex, yindex] = z;
                maxz = Math.Max(z, maxz);
                minz = Math.Min(z, minz);
            }
            Color[,] bi = new Color[Z.GetLength(0), Z.GetLength(1)];
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < ysize; j++)
                {
                    bi[i, ysize - j - 1] = RampGenerator.HotCold(Z[i, j], maxz, minz);
                }
            }
            return PImage.NetArray2Bitmap(bi);

        }
        public static Bitmap[] DrawKrigingShortAndLongComponent(string name)
        {
            try
            {
                string[] krigResult = File.ReadAllLines(name, encoding);
                int columenum = int.Parse(krigResult[1]);
                float[,] Region;
                float[,] Local;
                float diff, sx, sy;
                int xsize, ysize;
                {
                    string line = krigResult[2 + columenum].Replace("      ", ",");
                    line = line.Replace("     ", ",");
                    line = line.Replace("    ", ",");
                    string[] colume = line.Split(',');
                    sx = float.Parse(colume[1]);
                    sy = float.Parse(colume[2]);
                    line = krigResult[krigResult.Length - 1].Replace("      ", ",");
                    line = line.Replace("     ", ",");
                    line = line.Replace("    ", ",");
                    colume = line.Split(',');
                    float ex = float.Parse(colume[1]);
                    float ey = float.Parse(colume[2]);
                    
                    line = krigResult[krigResult.Length - 2].Replace("      ", ",");
                    line = line.Replace("     ", ",");
                    line = line.Replace("    ", ",");
                    colume = line.Split(',');
                    diff = Math.Abs(ex - float.Parse(colume[1]));
                    if(diff == 0)
                    {
                        diff = Math.Abs(ey - float.Parse(colume[2]));
                    }
                    
                    xsize = (int)((ex - sx) / diff) + 1;
                    ysize = (int)((ey - sy) / diff) + 1;
                    Local = new float[xsize, ysize];
                    Region = new float[xsize, ysize];
                }
                float maxz = float.MinValue;
                float minz = float.MaxValue;
                float maxz2 = float.MinValue;
                float minz2 = float.MaxValue;
                float maxzC = float.MinValue;
                float minzC = float.MaxValue;
                for (int i = 2 + columenum; i < krigResult.Length; i++)
                {
                    string line = krigResult[i].Replace("      ", ",");
                    line = line.Replace("     ", ",");
                    line = line.Replace("    ", ",");
                    string[] colume = line.Split(',');
                    float x = float.Parse(colume[1]);
                    float y = float.Parse(colume[2]);
                    float z = float.Parse(colume[5]);
                    float z2 = float.Parse(colume[6]);
                    int xindex = (int)((x - sx) / diff);
                    int yindex = (int)((y - sy) / diff);
                    Local[xindex, yindex] = z;
                    Region[xindex, yindex] = z2;
                    maxz = Math.Max(z, maxz);
                    minz = Math.Min(z, minz);
                    maxz2 = Math.Max(z2, maxz2);
                    minz2 = Math.Min(z2, minz2);
                    maxzC = Math.Max(z + z2, maxzC);
                    minzC = Math.Min(z + z2, minzC);
                }
                Color[,] Localbi = new Color[Local.GetLength(0), Local.GetLength(1)];
                for (int i = 0; i < xsize; i++)
                {
                    for (int j = 0; j < ysize; j++)
                    {
                        Localbi[i, ysize - j - 1] = RampGenerator.HotCold(Local[i, j], maxz, minz);
                    }
                }
                Color[,] Regionbi = new Color[Region.GetLength(0), Region.GetLength(1)];
                for (int i = 0; i < xsize; i++)
                {
                    for (int j = 0; j < ysize; j++)
                    {
                        Regionbi[i, ysize - j - 1] = RampGenerator.HotCold(Region[i, j], maxz2, minz2);
                    }
                }
                Color[,] Combinebi = new Color[Region.GetLength(0), Region.GetLength(1)];
                for (int i = 0; i < xsize; i++)
                {
                    for (int j = 0; j < ysize; j++)
                    {
                        Combinebi[i, ysize - j - 1] = RampGenerator.HotCold(Region[i, j] + Local[i, j], maxzC, minzC);
                    }
                }
                return new Bitmap[] { PImage.NetArray2Bitmap(Localbi), PImage.NetArray2Bitmap(Regionbi), PImage.NetArray2Bitmap(Combinebi) };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public static void BoundaryShapeToEllipse(string boundaryShapePath, string outpath)
        {
            string tmpFile = Path.Combine(Directory.GetCurrentDirectory(), "tmpboundary.txt");
            string tmpellipseFile = Path.Combine(Directory.GetCurrentDirectory(), "tmpellipse.txt");

            string strCmdText;
            string path = Directory.GetCurrentDirectory();
            string exe_path = Path.Combine(path, "BoundToEllipse", "main.exe");
            string command = string.Format("-m=b2csv -shp=\"{0}\" -out=\"{1}\"", boundaryShapePath, tmpFile);
            strCmdText = "/c " + exe_path + " " + command;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = windowstyle;
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = true;
            startInfo.Arguments = strCmdText;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            Thread.Sleep(100);

            while (PImage.init == false)
            {
                Thread.Sleep(10);
            }
            string[] csv = File.ReadAllLines(tmpFile);
            List<string> ellipseout = new List<string>();
            for (int i = 0; i < csv.Length; i++)
            {
                List<float> x = new List<float>();
                List<float> y = new List<float>();
                int line = int.Parse(csv[i]);
                for (int j = 0; j < line; j++)
                {
                    string[] xy = csv[i + j + 1].Split(',');
                    x.Add(float.Parse(xy[0]));
                    y.Add(float.Parse(xy[1]));
                }

                MWArray xa = new MWNumericArray(x.ToArray());
                MWArray ya = new MWNumericArray(y.ToArray());

                MWArray res = PImage.processor.EllipseDirectFit(xa, ya);
                if (res.IsStructArray)
                {
                    try
                    {
                        var a = ((double[,])(((MWStructArray)res).GetField("a", 0).ToArray()))[0, 0];
                        var b = ((double[,])(((MWStructArray)res).GetField("b", 0).ToArray()))[0, 0];
                        var phi = ((double[,])(((MWStructArray)res).GetField("phi", 0).ToArray()))[0, 0] * 180 / Math.PI;
                        var X0 = ((double[,])(((MWStructArray)res).GetField("X0", 0).ToArray()))[0, 0];
                        var Y0 = ((double[,])(((MWStructArray)res).GetField("Y0", 0).ToArray()))[0, 0];
                        var X0_in = ((double[,])(((MWStructArray)res).GetField("X0_in", 0).ToArray()))[0, 0];
                        var Y0_in = ((double[,])(((MWStructArray)res).GetField("Y0_in", 0).ToArray()))[0, 0];
                        ellipseout.Add(String.Format("{0},{1},{2},{3},{4}", Math.Round(a, 6), Math.Round(b, 6), Math.Round(X0_in, 6), Math.Round(Y0_in, 6), Math.Round(phi, 6)));
                    }
                    catch
                    {

                    }
                }

                i += line;
            }
            //write ellipsout
            File.WriteAllLines(tmpellipseFile, ellipseout);

            string strCmdText2;
            string path2 = Directory.GetCurrentDirectory();
            string exe_path2 = Path.Combine(path2, "BoundToEllipse", "main.exe");
            string command2 = string.Format("-m=e2shp -shp=\"{0}\" -out=\"{1}\"", tmpellipseFile, outpath);
            strCmdText2 = "/c " + exe_path2 + " " + command2;
            System.Diagnostics.Process process2 = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo2 = new System.Diagnostics.ProcessStartInfo();
            startInfo2.WindowStyle = windowstyle;
            startInfo2.FileName = "cmd.exe";
            startInfo2.UseShellExecute = true;
            startInfo2.Arguments = strCmdText2;
            process2.StartInfo = startInfo2;
            process2.Start();
            process2.WaitForExit();

            File.Delete(tmpFile);
            File.Delete(tmpellipseFile);

        }
    }
}