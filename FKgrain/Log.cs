using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using MathWorks.MATLAB.NET.Arrays;
using System.IO;
namespace FKgrain
{
    public class Log
    {
        public Log()
        {
            logs = new Stack<Step>();
            redo = new Stack<Step>();
        }
        Stack<Step> logs;
        Stack<Step> redo;
        public ImageForm image;
        public event Action<string> OnChanged;
        public void AddLog(Step Log)
        {
            logs.Push(Log);
            Changed();
        }
        public void Changed()
        {
            if(OnChanged != null)
            {
                OnChanged(Logs2String());
            }
        }
        public string Logs2String()
        {
            StringBuilder b = new StringBuilder();
            Step[] ll = logs.ToArray();
            for (int i = ll.Length - 1; i >= 0; i--)
            {
                b.Append(ll[i].function);
                foreach (string s in ll[i].parameters)
                {
                    b.Append(",");
                    b.Append(s);
                }
                b.Append(Environment.NewLine);
            }
            return b.ToString();
        }
        public Step RemoveLog()
        {
            Step Log = logs.Pop();
            Changed();
            return Log;
        }

        public Step[] GetLogs()
        {
            return logs.ToArray();
        }

        public void Undo()
        {
            if (logs.Count == 0)
            {
                return;
            }
            redo.Push(logs.Pop());
            Changed();
        }
        public void Redo()
        {
            if (redo.Count == 0)
            {
                return;
            }
            logs.Push(redo.Pop());
            Changed();
        }

        private void savelog(string savefile)
        {
                File.WriteAllText(savefile, Logs2String());
        }

        private void applylog(string openfile)
        {
            string[] logs = File.ReadAllLines(openfile);
            MWArray a = (MWArray)image.CurrentImageArray.Clone();
            int h = image.CurrentImage.Height;
            int w = image.CurrentImage.Width;
            foreach (string s in logs)
            {
                Step st = new Step(s);
                a = Step.Execute(st, image.CurrentImage, a, w, h);
                image.SetImage(a);
                image.Refresh();
                AddLog(st);
            }
        }

        public static Bitmap ApplyLog(Bitmap img, string[] logs)
        {
            MWArray a = PImage.Bitmap2array(img);
            Bitmap current = img;
            int h = img.Height;
            int w = img.Width;

            foreach (string s in logs)
            {
                Step st = new Step(s);
                Console.WriteLine(st.function);
                var tmp = Step.Execute(st, current, a, w, h);
                if (tmp != null)
                {
                    a = tmp;
                    current = PImage.Array2bitmap(a, w, h);
                }
                else
                {
                    Console.WriteLine("is null");
                    break;
                }

            }
            return current;
        }

    }
}
