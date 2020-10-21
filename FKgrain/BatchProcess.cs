using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace FKgrain {
    public partial class BatchProcess : Form {



        public BatchProcess() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            string[] files = Directory.GetFiles(InputTextBox.Text);
            string outputPath = OutputTextBox.Text;
            string logPath = LogTextBox.Text;
            string[] logs = File.ReadAllLines(logPath);
            string prefix = "";
            if(outputPath == InputTextBox.Text) {
                prefix = "Result_";
            }
            foreach (string file in files) {
                if (!file.Contains(".bmp")) {
                    continue;
                }
                // Open image
                Bitmap b = new Bitmap(file);
                if(b == null) {
                    continue;
                }
                // convert to binary
                Bitmap bi = NativeIP.FastBinaryConvert(b);

                // Run Logs
                Bitmap result = Log.ApplyLog(bi, logs);
                //Save Image
                string filename = Path.GetFileName(file);
                Console.WriteLine("Save");
                string op = Path.Combine(outputPath, prefix + filename);
                Console.WriteLine(op);
                result.Save(op);

            }
            Close();
        }

        //Inputfolder button click
        private void button2_Click(object sender, EventArgs e) {
            folderBrowserDialog1.Description = "Pick Image Input Folder";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                InputTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        //OutputFolder
        private void button3_Click(object sender, EventArgs e) {

            folderBrowserDialog1.Description = "Pick Output Folder";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                OutputTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            openFileDialog1.Title = "Pick a log file";
            openFileDialog1.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                LogTextBox.Text = openFileDialog1.FileName;
            }
        }
    }
}
