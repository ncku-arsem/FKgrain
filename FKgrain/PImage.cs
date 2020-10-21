using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using ImageProcess;
using System.Drawing.Imaging;
using System.Threading;

namespace FKgrain
{
    class PImage
    {

        public static Processor processor = new Processor();
        public static bool init = false;
        /*     static void run(string[] args) {
                 Bitmap b = new Bitmap("F:\\PersonalProject\\Stone\\contour.bmp");

                 Processor p = new Processor();
                 MWLogicalArray k0 = (MWLogicalArray)p.ToBinary(Bitmap2array(b), 0);
                 Bitmap o = Array2bitmap(k0, b.Width, b.Height);
                 o.Save("F:\\PersonalProject\\Stone\\out.bmp");
                 MWLogicalArray k1 = (MWLogicalArray)p.Inverse(k0);
                 Bitmap o1 = Array2bitmap(k1, b.Width, b.Height);
                 o1.Save("F:\\PersonalProject\\Stone\\out1.bmp");
             }
             */

        public static bool[,] Bitmap2NetArray(Bitmap orig)
        {
            Bitmap myBitmap;
            if (orig.PixelFormat != PixelFormat.Format24bppRgb)
            {
                myBitmap = new Bitmap(orig.Width, orig.Height,PixelFormat.Format24bppRgb);
                using (Graphics gr = Graphics.FromImage(myBitmap))
                {
                    gr.DrawImage(orig, new Rectangle(0, 0, myBitmap.Width, myBitmap.Height));
                }
            }
            else
            {
                myBitmap = orig;
            }
          
            bool[,] ImgData = new bool[myBitmap.Width, myBitmap.Height];
            BitmapData byteArray = myBitmap.LockBits(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr source_scan = byteArray.Scan0;
            unsafe  //專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。           
            {
                byte* source_p = (byte*)source_scan.ToPointer();
                Parallel.For(0, byteArray.Height, h =>
                {
                    byte* new_p = source_p + (h* (byteArray.Width*3));
                    Parallel.For(0, byteArray.Width, w =>
                    {
                        byte* new_pw = new_p+(w*3);
                        ImgData[w, h] = new_pw[0] >= 100 ? true : false; 
                    });
                });
            }
            myBitmap.UnlockBits(byteArray);
            return ImgData;
        }

        public static Bitmap NetArray2Bitmap(bool[,] ImgData, PixelFormat Format)
        {
            Bitmap source = new Bitmap(ImgData.GetLength(0), ImgData.GetLength(1), PixelFormat.Format24bppRgb);
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            IntPtr source_scan = sourceData.Scan0;
            unsafe
            {
                byte* source_p = (byte*)(sourceData.Scan0);
                for (int i = 0; i < sourceData.Height; i++)
                {
                    for (int j = 0; j < sourceData.Width; j++)
                    {
                        if (ImgData[j,i])
                        {
                            source_p[0] = 255;  //R
                            source_p[0 + 1] = 255;  //G
                            source_p[0 + 2] = 255;   //B
                        }
                        else
                        {
                            source_p[0] = 0;  //R
                            source_p[0 + 1] = 0;  //G
                            source_p[0 + 2] = 0;   //B

                        }
                        source_p += 3;
                    }
                    source_p += sourceData.Stride - sourceData.Width * 3;
                }
                /*
                byte* source_p = (byte*)source_scan.ToPointer();
                for (int h = 0; h < sourceData.Height; h++)
                {
                    for (int w = 0; w < sourceData.Width; w++)
                    {
                        if (ImgData[h, w])
                        {
                            source_p[0] = 255;  //R
                            source_p[0+1] = 255;  //G
                            source_p[0+2] = 255;   //B
                        }
                        else
                        {
                            source_p[0] = 0;  //R
                            source_p[0+1] = 0;  //G
                            source_p[0+2] = 0;   //B
                          
                        }
                        source_p+=3;
                    }
                }*/
            }
            source.UnlockBits(sourceData);
            //Bitmap mono = new Bitmap(ImgData.GetLength(0), ImgData.GetLength(1), PixelFormat.Format1bppIndexed);
            //NativeIP.FastBinaryConvert(source, mono);
            return source;
        }

        public static Bitmap NetArray2Bitmap(Color[,] ImgData)
        {

            Bitmap source = new Bitmap(ImgData.GetLength(0), ImgData.GetLength(1), PixelFormat.Format24bppRgb);
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* source_p = (byte*)(sourceData.Scan0);
                for (int i = 0; i < sourceData.Height; i++)
                {
                    for (int j = 0; j < sourceData.Width; j++)
                    {
                        source_p[0] = ImgData[j,i].B;  // ImgData[w, h].B;  //B
                        source_p[0 + 1] = ImgData[j, i].G;// ImgData[w, h].G;  //G
                        source_p[0 + 2] = ImgData[j, i].R;// ImgData[w, h].R;   //R
                        source_p += 3;
                    }
                    source_p += sourceData.Stride - sourceData.Width * 3;
                }
            }
            source.UnlockBits(sourceData);
            return source;
        }

        //--------------------

        public static MWLogicalArray Bitmap2array(Bitmap src)
        {

            BitmapData sourceData = src.LockBits(new Rectangle(0, 0, src.Width , src.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr source_scan = sourceData.Scan0;
            bool[,] bnew = new bool[src.Height,src.Width];
            unsafe
            {
                byte* source_p = (byte*)source_scan.ToPointer();
             
                for (int h = 0; h < src.Height; h++)
                {
                    byte* source_p2 = source_p;
                    for (int w = 0; w < sourceData.Width; w++)
                    {
                        if ((source_p2[0]) == 0)
                        {
                            bnew[h, w] = false;
                        }
                        else
                        {
                            bnew[h, w] = true;
                        }
                        source_p2 += 3;
                    }
                    source_p += sourceData.Stride;
                }
            }
            src.UnlockBits(sourceData);
            ////Get image dimensions
            //int width = bitmap.Width;
            //int height = bitmap.Height;
            ////Declare the double array of grayscale values to be read from "bitmap"
            //bool[,] bnew = new bool[height, width];

            ////Loop to read the data from the Bitmap image into the double array
            //int i, j;
            //for (i = 0; i < width; i++)
            //{
            //    for (j = 0; j < height; j++)
            //    {
            //        Color pixelColor = bitmap.GetPixel(i, j);
            //        double b = pixelColor.GetBrightness(); //the Brightness component
            //        if (b > 0)
            //        {
            //            bnew[j, i] = true;
            //        }
            //        else
            //        {
            //            bnew[j, i] = false;
            //        }
            //        //Note that rows in C# correspond to columns in MWarray
            //    }
            //}
            MWLogicalArray arr = new MWLogicalArray(bnew);
            return arr;
        }

        public unsafe static Bitmap Array2bitmap(MWArray arr, int width, int height)
        {
            bool[,] bnew = (bool[,])((MWLogicalArray)arr).ToArray();
            Bitmap src = new Bitmap(bnew.GetLength(0), bnew.GetLength(1),PixelFormat.Format24bppRgb);
            BitmapData sourceData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr source_scan = sourceData.Scan0;
            unsafe
            {
                byte* source_p = (byte*)source_scan.ToPointer();

                for (int h = 0; h < src.Height; h++)
                {
                    byte* source_p2 = source_p;
                    for (int w = 0; w < sourceData.Width; w++)
                    {
                        if (bnew[h, w] == false)
                        {
                            source_p2[0] = (byte)0;
                            source_p2[1] = (byte)0;
                            source_p2[2] = (byte)0;
                        }
                        else
                        {
                            source_p2[0] = (byte)255;
                            source_p2[1] = (byte)255;
                            source_p2[2] = (byte)255;
                        }
                        source_p2 += 3;
                    }
                    source_p += sourceData.Stride;
                }
            }
            src.UnlockBits(sourceData);
            return src;
        }
    }
}


