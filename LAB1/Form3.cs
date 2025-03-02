using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratornaya1
{
    public partial class Form3 : Form
    {
        public DateTime EventDate { get; set; }
        public string EventText { get; set; }
        public string EventType { get; set; }
        public TimeSpan EventTime { get; set; }
        public Form3()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (monthCalendar1.SelectionStart < DateTime.Today) 
            {
                MessageBox.Show("Эта дата уже прошла!");
                return;
            }

            EventDate = monthCalendar1.SelectionStart;
            EventText = textBox1.Text;
            EventType = comboBox1.SelectedItem?.ToString();
            EventTime = dateTimePicker1.Value.TimeOfDay;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            int minW = 601;
            int minH = 433;

            if (this.Width < minW)
            {
                this.Width = minW;
            }
            if (this.Height < minH)
            {
                this.Height = minH;
            }
        }
    }
}
