using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using System.Drawing;
using System.Drawing.Imaging;

namespace FKgrain {
    public class Step {

        public const string Opening = "Openning"; 
        public const string Closing = "Closing";
        public const string Inverse = "Inverse";
        public const string Fill = "Fill";
        public const string TraceBoundary = "TraceBoundary";
        public const string Erosion = "Erosion";
        public const string Dilation = "Dilation";
        public const string Pick = "Pick";
        public const string PickByBoundary = "PickByBoundary";



        public string function;
        public string[] parameters;
        private Step() {
            function = "";
            parameters = new string[0];
        }
        public Step(string funcName, string[] parameters) {
            this.function = funcName;
            this.parameters = parameters;
        }
        public Step(string step) {
            string[] s = step.Substring(step.IndexOf(',') + 1).Split(',');
            //Console.WriteLine(step.Substring(step.IndexOf(',') + 1));
            string p = "";
            if (step.Contains(',')) {
                p = step.Substring(0, step.IndexOf(','));
            } else {
                p = step;
            }
            this.function = p;
            this.parameters = s;
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();
            b.Append(function);
            foreach (string s in parameters) {
                b.Append(",");
                b.Append(s);
            }
            return b.ToString();
        }

        public static MWArray Execute(Step step,Bitmap bit, MWArray image,int width,int height) {
            Console.WriteLine(step.function);
            MWArray im =null;
            switch (step.function) {
                case Opening:
                im = PImage.processor.Opening(image, step.parameters[0].Split(':')[1], int.Parse(step.parameters[1].Split(':')[1]));
                break;
                case Closing:
                im = PImage.processor.Closing(image, step.parameters[0].Split(':')[1], int.Parse(step.parameters[1].Split(':')[1]));
                break;
                case Inverse:
                Bitmap binary = NativeIP.FastInvertBinary(bit);
                im = PImage.Bitmap2array(binary);
                break;
                case Erosion:
                im = PImage.processor.Erosion(image, step.parameters[0].Split(':')[1], int.Parse(step.parameters[1].Split(':')[1]));
                break;
                case Dilation:
                im = PImage.processor.Dialation(image, step.parameters[0].Split(':')[1], int.Parse(step.parameters[1].Split(':')[1]));
                break;
                case Fill:
                string op = step.parameters[0].Split(':')[1];
                if (op == "holes") {
                    im = PImage.processor.FillHoles(image);
                } else {
                    int x = int.Parse(step.parameters[1].Split(':')[1]);
                    int y = int.Parse(step.parameters[2].Split(':')[1]);
                    im = PImage.processor.Fill(image,x,y);
                }
                break;
                case TraceBoundary:
                    {
                        int conn = int.Parse(step.parameters[0].Split(':')[1]);
                        string option = step.parameters[1].Split(':')[1];
                        var a = PImage.processor.TraceBoundary(image, (MWNumericArray)conn, (MWArray)(option));
                        double[,] bound = (double[,])a.ToArray();
                        Bitmap bi = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                        for (int i = 0; i < bound.GetLength(0); i++)
                        {
                            bi.SetPixel((int)bound[i, 1] - 1, (int)bound[i, 0] - 1, Color.White);
                        }
                        Bitmap aa = NativeIP.FastBinaryConvert(bi);

                        Bitmap ab = NativeIP.FastInvertBinary(aa);
                        im = PImage.Bitmap2array(ab);
                    }
                break;
                case Pick: {
                    int threshold = int.Parse(step.parameters[0].Split(':')[1]);
                    string ope = step.parameters[1].Split(':')[1];
                    string option = step.parameters[2].Split(':')[1];
                    im = MaskOption.Pick(image, threshold,true);
                }
                break;
                case PickByBoundary: {
                    string openfile = step.parameters[0].Split(':')[1]+":"+ step.parameters[0].Split(':')[2];
                    im = MaskOption.PickByBoundary(image, openfile);
                }
                break;
            }
            return im;
        }

    }
}
