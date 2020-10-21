using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FKgrain {
    public partial class FilterPicker : Form {

        static string[] filtertype = new string[] { "square" ,"disk"}; 

        public FilterPicker() {
            InitializeComponent();
            for (int i = 0; i < filtertype.Length; i++) {
                comboBox1.Items.Add(filtertype[i]);
            }
            for (int i = 1; i < 21; i++) {
                comboBox2.Items.Add(i);
            }
            comboBox2.SelectedIndex = 3;
            comboBox1.SelectedIndex = 0;
            button1.Focus();
        }
        public FilterPicker(string name,Action<string, int> OnOk) : this() {
            this.OnOk += OnOk;
            this.Text = name;
            
        }

        public Action<string,int> OnOk;

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void Ok_Click(object sender, EventArgs e) {
            if (OnOk != null) {
                OnOk(filtertype[comboBox1.SelectedIndex], comboBox2.SelectedIndex+1);
            }
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void FilterPicker_Load(object sender, EventArgs e) {

        }
    }
}
