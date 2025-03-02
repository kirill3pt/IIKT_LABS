using System;
using System.Windows.Forms;

namespace Laboratornaya1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {  
            string login = textBox1.Text;
            string password = textBox2.Text;
            Form1 frm1 = new Form1();
            if (login == "KirillB" && password == "1234567890")
            {
                Hide();
                frm1.ShowDialog();
                Close();
            }
            else
            {
                MessageBox.Show("Wrong login or password!", "!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = true;
            ToolTip helpLogin = new ToolTip();
            helpLogin.SetToolTip(textBox1, "Enter your login here");
            helpLogin.InitialDelay = 200;
            ToolTip helpPass = new ToolTip();
            helpPass.SetToolTip(textBox2, "Enter your password here");
            helpPass.InitialDelay = 200;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) 
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
