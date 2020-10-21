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
namespace FKgrain {
    [Obsolete]
    public partial class Logs : Form {
        public Logs() {
            InitializeComponent();
            logs = new Stack<Step>();
            redo = new Stack<Step>();
        }
        Stack<Step> logs;
        Stack<Step> redo;
        public ImageForm image;
        public void AddLog(Step Log) {
            logs.Push(Log);
            RefreshLog();
        }
        public void RefreshLog() {
           
            richTextBox1.Text = Logs2String();
        }
        public string Logs2String() {
            StringBuilder b = new StringBuilder();
            Step[] ll = logs.ToArray();
            for (int i = ll.Length - 1; i >= 0; i--) {
                b.Append(ll[i].function);
                foreach (string s in ll[i].parameters) {
                    b.Append(",");
                    b.Append(s);
                }
                b.Append(Environment.NewLine);
            }
            return b.ToString();
        }
        public Step RemoveLog() {
            Step Log = logs.Pop();
            string s = richTextBox1.Text;
            s.Substring(0,s.LastIndexOf(Environment.NewLine));
            return Log;
        }

        public Step[] GetLogs() {
            return logs.ToArray();
        }

        public void Undo() {
            if (logs.Count == 0) {
                return;
            }
            redo.Push(logs.Pop());
            RefreshLog();
        }
        public void Redo() {
            if (redo.Count == 0) {
                return;
            }
            logs.Push(redo.Pop());
            RefreshLog();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {

        }

        private void saveLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            saveFileDialog1.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                var filePath = saveFileDialog1.FileName;
                File.WriteAllText(filePath, Logs2String());
            }
        }

        private void applyLogsToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                var filePath = openFileDialog1.FileName;
                string[] logs = File.ReadAllLines(filePath);
                MWArray a = (MWArray)image.CurrentImageArray.Clone();
                int h = image.CurrentImage.Height;
                int w = image.CurrentImage.Width;

                foreach (string s in logs) {
                    Step st = new Step(s);
                    a = Step.Execute(st, image.CurrentImage, a, w, h);
                    image.SetImage(a);
                    image.Refresh();
                    AddLog(st);
                    Console.WriteLine(s);
                }
            }
        }

        public static Bitmap ApplyLog(Bitmap img, string[] logs) {
            MWArray a = PImage.Bitmap2array(img);
            Bitmap current = img;
            int h = img.Height;
            int w = img.Width;
        
            foreach (string s in logs) {
                Step st = new Step(s);
                Console.WriteLine(st.function);
                var tmp = Step.Execute(st, current, a, w, h);
                if(tmp != null) {
                    a = tmp;
                    current = PImage.Array2bitmap(a, w, h);
                } else {
                    Console.WriteLine("is null");
                    break;
                }
              
            }
            return current;
        }

    }
}
