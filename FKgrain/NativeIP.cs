using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace FKgrain {
    class NativeIP {

        public static Bitmap Combine(Bitmap source1, Bitmap source2,int alpha,string name) {
            var s2a = SetAlpha(source2, source2.Width, source2.Height, name, alpha);
            var target = new Bitmap(source1.Width, source1.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(target)) {
                graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear
                graphics.DrawImage(source1, 0, 0,source1.Width, source1.Height);
                graphics.DrawImage(s2a, 0, 0, s2a.Width, s2a.Height);
            }
            return target;
        }

        public static unsafe Bitmap FastInvertBinary(Bitmap src) {
            Bitmap conv = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
            BitmapData sourceData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData convData = conv.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr source_scan = sourceData.Scan0;
            IntPtr conv_scan = convData.Scan0;
            unsafe
            {
                byte* source_p = (byte*)source_scan.ToPointer();
                byte* conv_p = (byte*)conv_scan.ToPointer();
                for (int h = 0; h < src.Height; h++)
                {
                    byte* source_p2 = source_p;
                    byte* conv_p2 = conv_p;
                    for (int w = 0; w < sourceData.Width; w++)
                    {
                        if ((source_p2[0]) == 0)
                        {
                            conv_p2[0] = (byte)255;
                            conv_p2[1] = (byte)255;
                            conv_p2[2] = (byte)255;
                        }
                        else
                        {
                            conv_p2[0] = (byte)0;
                            conv_p2[1] = (byte)0;
                            conv_p2[2] = (byte)0;
                        }
                        source_p2 += 3;
                        conv_p2 += 3;
                    }
                    source_p += sourceData.Stride;
                    conv_p += convData.Stride;
                }
            }
            src.UnlockBits(sourceData);
            conv.UnlockBits(convData);
            return conv;
            // Lock source and destination in memory for unsafe access
            //var bmbo = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly,
            //                         src.PixelFormat);
            //var bmdn = conv.LockBits(new Rectangle(0, 0, conv.Width, conv.Height), ImageLockMode.ReadWrite,
            //                         conv.PixelFormat);

            //var srcScan0 = bmbo.Scan0;
            //var convScan0 = bmdn.Scan0;

            //var srcStride = bmbo.Stride;
            //var convStride = bmdn.Stride;

            //byte* sourcePixels = (byte*)(void*)srcScan0;
            //byte* destPixels = (byte*)(void*)convScan0;

            //var srcLineIdx = 0;
            //var convLineIdx = 0;
            //var hmax = src.Height-1 ;
            //var wmax = src.Width ;
            //for (int y = 0; y < hmax; y++) {
            //    // find indexes for source/destination lines

            //    // use addition, not multiplication?
            //    srcLineIdx += srcStride;
            //    convLineIdx += convStride;

            //    var srcIdx = srcLineIdx;
            //    for (int x = 0; x < wmax; x++) {
            //        // index for source pixel (32bbp, rgba format)
            //        srcIdx += 1;
            //        //var r = pixel[2];
            //        //var g = pixel[1];
            //        //var b = pixel[0];

            //        // could just check directly?
            //        //if (Color.FromArgb(r,g,b).GetBrightness() > 0.01f)
            //        // destination byte for pixel (1bpp, ie 8pixels per byte)
            //        var idx = convLineIdx + (x >> 3);
            //        // mask out pixel bit in destination byte
            //        destPixels[idx] = (byte)~sourcePixels[idx];
            //    }
            //}
            //src.UnlockBits(bmbo);
            //conv.UnlockBits(bmdn);
        }

        public static unsafe Bitmap FastCombineBinary(Bitmap src, Bitmap conv) {
            if( src.Size != conv.Size)
            {
                throw new Exception("Size not match");
            }
            BitmapData sourceData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData convData = conv.LockBits(new Rectangle(0, 0, conv.Width, conv.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            Bitmap Combine = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
            BitmapData resultData = Combine.LockBits(new Rectangle(0, 0, Combine.Width, Combine.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            IntPtr source_scan = sourceData.Scan0;
            IntPtr conv_scan = convData.Scan0;
            IntPtr res_scan = resultData.Scan0;
            unsafe
            {
                byte* source_p = (byte*)source_scan.ToPointer();
                byte* conv_p = (byte*)conv_scan.ToPointer();
                byte* res_p = (byte*)res_scan.ToPointer();
                for (int h = 0; h < src.Height; h++)
                {
                    byte* source_p2 = source_p;
                    byte* conv_p2 = conv_p;
                    byte* res_p2 = res_p;
                    for (int w = 0; w < sourceData.Width; w++)
                    {
                        if (source_p2[0] != 0 || conv_p2[0] != 0)
                        {
                            res_p2[0] = (byte)255;
                            res_p2[1] = (byte)255;
                            res_p2[2] = (byte)255;
                        }
                        else
                        {
                            res_p2[0] = (byte)0;
                            res_p2[1] = (byte)0;
                            res_p2[2] = (byte)0;
                        }
                        source_p2 += 3;
                        conv_p2 += 3;
                        res_p2 += 3;
                    }
                    source_p += sourceData.Stride;
                    conv_p += convData.Stride;
                    res_p += resultData.Stride;
                }
            }
            src.UnlockBits(sourceData);
            conv.UnlockBits(convData);
            Combine.UnlockBits(resultData);
            return Combine;
        }
  
        public static Bitmap FastBinaryConvert(Bitmap src) {
            Bitmap conv = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
            BitmapData sourceData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData convData = conv.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr source_scan = sourceData.Scan0;
            IntPtr conv_scan = convData.Scan0;
            unsafe
            {
                byte* source_p = (byte*)source_scan.ToPointer();
                byte* conv_p = (byte*)conv_scan.ToPointer();
                for (int h = 0; h < src.Height; h++)
                {
                    byte* source_p2 = source_p;
                    byte* conv_p2 = conv_p;
                    for (int w = 0; w < sourceData.Width; w++)
                    {

                        if ((source_p2[0]) == 255)
                        {
                            conv_p2[0] = (byte)255;
                            conv_p2[1] = (byte)255;
                            conv_p2[2] = (byte)255;
                        }
                        else
                        {
                            conv_p2[0] = (byte)0;
                            conv_p2[1] = (byte)0;
                            conv_p2[2] = (byte)0;
                        }
                        source_p2 += 3;
                        conv_p2 += 3;
                    }
                    source_p += sourceData.Stride;
                    conv_p += convData.Stride;
                }
            }
            src.UnlockBits(sourceData);
            conv.UnlockBits(convData);
            return conv;
        }

        static Dictionary<string,Bitmap> alphaout = new Dictionary<string,Bitmap>();
        public static Bitmap SetAlpha(Bitmap bmpIn,int w,int h,string name, int alpha) {
            if (alphaout.ContainsKey(name)) {
                if (alphaout[name] != null) {
                    alphaout[name].Dispose();
                }
            } else {
                alphaout.Add(name, null);
            }
            alphaout[name] = new Bitmap(w, h);
            float a = alpha / 255f;
            Rectangle r = new Rectangle(0, 0,w, h);
            Console.WriteLine(bmpIn);
            float[][] matrixItems = {
        new float[] {1, 0, 0, 0, 0},
        new float[] {0, 1, 0, 0, 0},
        new float[] {0, 0, 1, 0, 0},
        new float[] {0, 0, 0, a, 0},
        new float[] {0, 0, 0, 0, 1}};

            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

            ImageAttributes imageAtt = new ImageAttributes();
            imageAtt.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            using (Graphics g = Graphics.FromImage(alphaout[name]))
                g.DrawImage(bmpIn, r, r.X, r.Y, r.Width, r.Height, GraphicsUnit.Pixel, imageAtt);

            return alphaout[name];
        }

    }
}
