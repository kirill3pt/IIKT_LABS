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
using System.Windows.Forms.DataVisualization.Charting;

namespace Laboratornaya2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) //открыть директорию
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                treeView1.Nodes.Clear();
                DirectoryInfo rootDir = new DirectoryInfo(fbd.SelectedPath);
                TreeNode rootNode = new TreeNode(rootDir.Name) { Tag = rootDir.FullName };
                treeView1.Nodes.Add(rootNode);
                LoadDirectories(rootDir, rootNode);
                chart1.Visible = true;
            }
        }
        private void LoadDirectories(DirectoryInfo dir, TreeNode node) //загрузка директорий
        {
            try
            {
                foreach (var directory in dir.GetDirectories())
                {
                    TreeNode subNode = new TreeNode(directory.Name) { Tag = directory.FullName };
                    node.Nodes.Add(subNode);
                    LoadDirectories(directory, subNode);
                }

                foreach (var file in dir.GetFiles())
                {
                    TreeNode fileNode = new TreeNode(file.Name) { Tag = file.FullName };
                    node.Nodes.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException) { }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) //обновление дерева при смене директории
        {
            listView1.Items.Clear();
            string path = e.Node.Tag.ToString();

            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                int smallFiles = 0, mediumFiles = 0, largeFiles = 0;

                foreach (var file in dir.GetFiles())
                {
                    ListViewItem item = new ListViewItem(file.Name);
                    item.Checked = true;
                    item.SubItems.Add(file.Length.ToString());
                    item.SubItems.Add(file.Extension);
                    ColorOfElements(item, file.Extension);
                    listView1.Items.Add(item);

                    if (file.Length < 100 * 1024) // < 100 KB
                        smallFiles++;
                    else if (file.Length < 1 * 1024 * 1024) // 100 KB - 1 MB
                        mediumFiles++;
                    else // > 1 MB
                        largeFiles++;
                }
                UpdateStatusBar();
                UpdateChart(smallFiles, mediumFiles, largeFiles);
            }
        }
        private void UpdateChart(int small, int medium, int large) //функция для обновления графика
        {
            chart1.Series.Clear();
            Series series = new Series("Количество")
            {
                ChartType = SeriesChartType.Column
            };

            series.Points.AddXY("Малые", small);
            series.Points.AddXY("Средние", medium);
            series.Points.AddXY("Большие", large);

            chart1.Series.Add(series);
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Interval = Math.Max(1, (small + medium + large) / 10);
            chart1.ChartAreas[0].RecalculateAxesScale();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) //выход
        {
            Close();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) //сохранение
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = "C:\\Desktop";
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("Дерево пустое. Сохранение невозможно.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(sfd.FileName))
                {
                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        SaveNode(node, writer, 0);
                    }
                    MessageBox.Show("Файл успешно сохранен!", "Cохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }
        private void SaveNode(TreeNode node, StreamWriter writer, int level) //сохранение дерева
        {
            writer.WriteLine(new string(' ', level * 2) + node.Text);
            foreach (TreeNode element in node.Nodes)
            {
                SaveNode(element, writer, level + 1);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) //помощь
        {
            MessageBox.Show("enginered by Kirill Berezhetskij, group IVT-2\n" +
                "version - 0.0.1", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e) //данные при загрузке формы
        {
            chart1.Visible = false;
            toolStripSIZE.Text = "Общий размер: 0 Байт";
            toolStripCOUNT.Text = "Выбрано файлов: 0";
        }
        private void UpdateCheckBox() //функция для обновления CheckBox'ов
        {
            int smallCount = 0, mediumCount = 0, largeCount = 0;
            
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    long fileSize = long.Parse(item.SubItems[1].Text);
                    if (fileSize < 100 * 1024)
                    {
                        smallCount++;
                    }
                    else if (fileSize < 1024 * 1024)
                    {
                        mediumCount++;
                    }
                    else
                    {
                        largeCount++;
                    }
                }
            }
            chart1.Series[0].Points.Clear();
            chart1.Series[0].Points.AddXY("Малые", smallCount);
            chart1.Series[0].Points.AddXY("Средние", mediumCount);
            chart1.Series[0].Points.AddXY("Большие", largeCount);
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e) //изменение ListView1
        {
            UpdateStatusBar();
            UpdateCheckBox();
        }
        private void ColorOfElements(ListViewItem item, string extension) //функция для присвоения цветов определенным элементам
        {
            extension = extension.ToLower();

            if (new[] { ".png", ".jpg", ".bmp", ".gif" }.Contains(extension))
            {
                item.BackColor = Color.LightBlue;
            }
            else if (new[] { ".docx", ".xlsx", ".pdf", ".txt", ".doc", ".ppt", ".xlsm", ".xls", ".pptx" }.Contains(extension))
            {
                item.BackColor = Color.LightGreen;
            }
            else if (new[] { ".zip", ".rar", ".7z" }.Contains(extension))
            {
                item.BackColor = Color.LightYellow;
            }
            else if (new[] { ".exe", ".dll" }.Contains(extension))
            {
                item.BackColor = Color.PaleVioletRed;
            }
            else
            {
                item.BackColor = Color.White;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e) //функция для изменения шрифта в ListView
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    listView1.Font = fontDialog.Font;
                }
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e) //функция для изменения цвета шрифта в ListView
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    listView1.ForeColor = colorDialog.Color;
                }
            }
        }
        private void UpdateStatusBar() //функция для обновления данных в statusBar
        {
            long totalSizeChecked = 0;
            int checkedCount = 0;

            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    checkedCount++;
                    if (long.TryParse(item.SubItems[1].Text, out long fileSize))
                    {
                        totalSizeChecked += fileSize;
                    }
                }
            }
            toolStripSIZE.Text = $"Общий размер: {totalSizeChecked} Байт";
            toolStripCOUNT.Text = $"Выбрано файлов: {checkedCount}";
        }
    }
}
