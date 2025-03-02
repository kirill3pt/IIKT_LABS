using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratornaya1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 frm3 = new Form3();
            frm3.ShowDialog();
            if (frm3.DialogResult == DialogResult.OK)
            {
                ListViewItem item = new ListViewItem(frm3.EventDate.ToShortDateString());
                item.SubItems.Add(frm3.EventTime.ToString());
                item.SubItems.Add(frm3.EventText.ToString());
                item.Tag = frm3.EventType;

                if (frm3.EventType == "Memo")
                {
                    item.ImageIndex = 0;
                }
                else if (frm3.EventType == "Meeting")
                {
                    item.ImageIndex = 1;
                }
                else if (frm3.EventType == "Task")
                {
                    item.ImageIndex = 2;
                }
                elementsOfList.Add(item);
                CheckEvent();
            }
        }
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listView1.SelectedItems.Count > 0)
            {
                ListViewItem select = listView1.SelectedItems[0];

                elementsOfList.Remove(select); 
                listView1.Items.Remove(select);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem select = listView1.SelectedItems[0];
                Form3 editfrm3 = new Form3();
                editfrm3.EventDate = DateTime.Parse(select.Text);
                editfrm3.EventTime = TimeSpan.Parse(select.SubItems[1].Text);
                editfrm3.EventText = select.SubItems[2].Text;

                if (editfrm3.ShowDialog() == DialogResult.OK)
                {
                    select.Text = editfrm3.EventDate.ToShortDateString();
                    select.SubItems[1].Text = editfrm3.EventTime.ToString();
                    select.SubItems[2].Text = editfrm3.EventText;
                    select.Tag = editfrm3.EventType;
                }
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem item = listView1.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    item.Selected = true;
                    contextMenuStrip1.Show(listView1, e.Location);
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem select = listView1.SelectedItems[0];
                string message = $"Are you sure you want to delete the record <{select.Text} / {select.SubItems[1].Text} / {select.SubItems[2].Text}>? ";
                DialogResult result = MessageBox.Show($"{message}", "Удаление записи", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    elementsOfList.Remove(select);
                    listView1.Items.Remove(select);
                }

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int minW = 434;
            int minH = 529;

            if (this.Width < minW)
            {
                this.Width = minW;
            }
            if (this.Height < minH)
            {
                this.Height = minH;
            }
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();

                string[] lines = File.ReadAllLines(openFile.FileName);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        ListViewItem item = new ListViewItem(parts[0]);
                        item.SubItems.Add(parts[1]);
                        item.SubItems.Add(parts[2]);
                        item.Tag = parts[3];
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void SaveFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                List<string> lines = new List<string>();

                foreach (ListViewItem item in listView1.Items)
                {
                    string line = item.Text + "|" + item.SubItems[1].Text + "|" + item.SubItems[2].Text + "|" + (item.Tag != null ? item.Tag.ToString() : "Нет категории");
                    lines.Add(line);
                }

                File.WriteAllLines(saveFile.FileName, lines);
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                OpenFile();
            }

            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveFile();
            }
        }
        private List<ListViewItem> elementsOfList = new List<ListViewItem>();
        private void CheckEvent()
        {
            listView1.Items.Clear();

            foreach (ListViewItem item in elementsOfList)
            {
                if (radioButton2.Checked || item.Tag?.ToString() == comboBox1.SelectedItem?.ToString())
                {
                    listView1.Items.Add(item);
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            CheckEvent();
            if (radioButton1.Checked)
            {
                comboBox1.Enabled = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            CheckEvent();
            if (radioButton2.Checked)
            {
                comboBox1.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEvent();
        }
        private void Sort()
        {
            List <ListViewItem> items = new List <ListViewItem>();

            foreach (ListViewItem item in listView1.Items)
            {
                items.Add(item);
            }

            for (int i = 0; i < items.Count - 1; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    TimeSpan time1 = TimeSpan.Parse(items[i].SubItems[1].Text);
                    TimeSpan time2 = TimeSpan.Parse(items[j].SubItems[1].Text);

                    if (time1 > time2)
                    {
                        var temp = items[i];
                        items[i] = items[j];
                        items[j] = temp;
                    }
                }
            }
            listView1.Items.Clear();
            foreach (var item in items)
            {
                listView1.Items.Add(item);
            }
        }
        private void Search(string text)
        {
            listView1.Items.Clear();

            foreach (ListViewItem item in elementsOfList)
            {
                if (item.SubItems[2].Text.ToLower().Contains(text.ToLower()))
                {
                    listView1.Items.Add(item);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sort();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form4 frm4 = new Form4();
            if (frm4.ShowDialog() == DialogResult.OK)
            {
                Search(frm4.search);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Memo");
            comboBox1.Items.Add("Meeting");
            comboBox1.Items.Add("Task");
        }
    }
}
