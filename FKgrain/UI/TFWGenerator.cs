using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
namespace FKgrain.UI
{
    public partial class TFWGenerator : Form
    {

        public string ImagePath;
        public TFWGenerator()
        {
            InitializeComponent();
        }
        public TFWGenerator(string imagepath) : this()
        {
            ImagePath = imagepath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float x = 0;
            
            float y = 0;
          
            float pSize;
            if (!float.TryParse(pixelsize.Text, out pSize))
            {
                MessageBox.Show("Pixel Size is not a valid number",
                 "Please insert a valid Number in Pixel Size",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Information);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
            Bitmap b = new Bitmap(ImagePath);
            string extension = Path.GetExtension(ImagePath);
            string[] tfw = new string[6];
            tfw[0] = pSize.ToString();
            tfw[1] = "0";
            tfw[2] = "0";
            tfw[3] = (-pSize).ToString();
            tfw[4] = x.ToString();
            tfw[5] = y.ToString();
            File.WriteAllLines(ImagePath.Replace(extension, ".tfw"), tfw);
        }

    }
}
