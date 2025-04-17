// подключаем нужные библиотеки для работы с формами, картинками и т.п.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Laboratornaya3
{
    public partial class Form1 : Form
    {
        Bitmap panelBitmap; // картинка для отрисовки на панели
        List<string> imagePaths = new List<string>(); // пути ко всем загруженным картинкам
        private Bitmap currentImageWithCopyright; // текущая картинка с копирайтом
        private string copyrightText = "© Your Copyright"; // текст копирайта по умолчанию
        private string saveDirectory = ""; // папка для сохранения

        // создаём класс для хранения инфы про картинку
        public class ImageInfo
        {
            public string FileName { get; set; } // имя файла
            public int Width { get; set; } // ширина
            public int Height { get; set; } // высота
        }

        public Form1()
        { 
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // прячем таблицу и кнопки при старте
            dataGridView1.Visible = false;
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Text", HeaderText = "Text" });
            button1.Visible = button2.Visible = button3.Visible = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // закрываем приложение при закрытии формы
            Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // если выбрали выход из меню — закрываем
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // открываем одну картинку
            showControls(); // показываем кнопки и таблицу
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Image Files|*.jpg;*.png;*.bmp;*.gif" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // загружаем и показываем картинку
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = null;

                    Bitmap img = new Bitmap(ofd.FileName);
                    dataGridView1.Rows.Clear();
                    dataGridView1.Rows.Add(Path.GetFileName(ofd.FileName), img.Width, img.Height);

                    imagePaths.Clear();
                    imagePaths.Add(ofd.FileName);
                    pictureBox1.Image = img;

                    draw(new List<Bitmap> { img });
                }
                catch (Exception ex)
                { 
                    MessageBox.Show("Ошибка при загрузке: " + ex.Message); 
                }
            }
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // открываем целую папку с картинками
            showControls();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                List<Bitmap> bitmaps = new List<Bitmap>();
                imagePaths.Clear();
                dataGridView1.Rows.Clear();

                // перебираем все файлы в папке и добавляем их
                foreach (string file in Directory.EnumerateFiles(fbd.SelectedPath))
                {
                    try
                    {
                        Bitmap img = new Bitmap(file);
                        bitmaps.Add(img);
                        imagePaths.Add(file);
                        dataGridView1.Rows.Add(Path.GetFileName(file), img.Width, img.Height);
                    }
                    catch { } // если ошибка — просто пропускаем файл
                }
                draw(bitmaps); // рисуем картинки в панели
            }
        }

        private void draw(List<Bitmap> images)
        {
            // рисуем миниатюры на панели
            int height = 90 * images.Count + 15;
            panelBitmap = new Bitmap(panel1.Width, height);
            panel1.AutoScrollMinSize = new Size(panel1.Width, height);
            Graphics dc = Graphics.FromImage(panelBitmap);
            for (int i = 0; i < images.Count; i++)
            {
                dc.DrawImage(images[i], 10, 90 * i + 15, 90, 90); // рисуем каждую картинку
            }
            panel1.Invalidate(); // обновляем панель
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // рисуем битмап на панели
            e.Graphics.TranslateTransform(panel1.AutoScrollPosition.X, panel1.AutoScrollPosition.Y);
            if (panelBitmap != null)
            {
                e.Graphics.DrawImage(panelBitmap, 0, 0);
            }
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            // при клике выбираем нужную картинку
            int index = (e.Y - panel1.AutoScrollPosition.Y - 15) / 90;
            if (index >= 0 && index < imagePaths.Count)
            {
                try
                {
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = new Bitmap(imagePaths[index]);
                }
                catch 
                { 
                    MessageBox.Show("Ошибка при загрузке картинки");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            AddCopyrightToImages(); // кнопка добавления копирайта
        }
        private void AddCopyrightToImages()
        {
            // добавляем копирайт на картинки из таблицы
            try
            {
                for (int i = 0; i < imagePaths.Count; i++)
                {
                    string text = dataGridView1.Rows[i].Cells["Text"].Value?.ToString();
                    if (string.IsNullOrEmpty(text)) continue;

                    Bitmap original = new Bitmap(imagePaths[i]);
                    using (Graphics g = Graphics.FromImage(original))
                    {
                        Font font = new Font("Arial", 100, FontStyle.Bold);
                        Brush brush = new SolidBrush(Color.Red);
                        Point pos = new Point(
                            original.Width - (int)g.MeasureString(text, font).Width - 60,
                            original.Height - (int)g.MeasureString(text, font).Height - 60);
                        g.DrawString(text, font, brush, pos);
                    }

                    currentImageWithCopyright = new Bitmap(original);
                    pictureBox1.Image = new Bitmap(currentImageWithCopyright);

                    using (Graphics dc = Graphics.FromImage(panelBitmap))
                    {
                        dc.DrawImage(currentImageWithCopyright, 10, 90 * i + 15, 90, 90);
                        dc.DrawImage(Properties.Resources.copyright_icon, 110, 90 * i + 15, 20, 20);
                    }
                }
                panel1.Invalidate();
                MessageBox.Show("Копирайт добавлен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // сохраняем текущую картинку с копирайтом
            if (currentImageWithCopyright == null)
            {
                MessageBox.Show("Добавьте копирайт");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp", Title = "сохранить как" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentImageWithCopyright.Save(sfd.FileName);
                    MessageBox.Show("Сохранено");
                }
                catch (Exception ex)
                { 
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void copyrightTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ввод текста копирайта через окошко
            string input = Interaction.InputBox("Введите копирайт", "Текст копирайта", copyrightText);
            if (!string.IsNullOrEmpty(input))
            {
                copyrightText = input;
            }
        }

        private void copyrightDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // выбираем папку для сохранения
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            { 
                saveDirectory = fbd.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BatchMode();
        }

        private void BatchMode()
        {
            // добавляем копирайт ко всем картинкам сразу и сохраняем их
            if (string.IsNullOrEmpty(saveDirectory))
            { 
                MessageBox.Show("Укажите папку для сохранения");
                return; 
            }
            if (string.IsNullOrEmpty(copyrightText))
            {
                MessageBox.Show("Укажите текст копирайта"); 
                return; 
            }

            try
            {
                panelBitmap = new Bitmap(panel1.Width, 90 * imagePaths.Count + 15);
                panel1.AutoScrollMinSize = new Size(0, panelBitmap.Height);
                Graphics dc = Graphics.FromImage(panelBitmap);

                for (int i = 0; i < imagePaths.Count; i++)
                {
                    Bitmap img = new Bitmap(imagePaths[i]);
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        Font font = new Font("Arial", 50, FontStyle.Bold);
                        Brush brush = new SolidBrush(Color.Red);
                        Point pos = new Point(
                            img.Width - (int)g.MeasureString(copyrightText, font).Width - 10,
                            img.Height - (int)g.MeasureString(copyrightText, font).Height - 10);
                        g.DrawString(copyrightText, font, brush, pos);
                    }
                    dc.DrawImage(img, 10, 90 * i + 15, 90, 90);
                    dc.DrawImage(Properties.Resources.copyright_icon, 110, 90 * i + 15, 20, 20);
                    img.Save(Path.Combine(saveDirectory, Path.GetFileName(imagePaths[i])));
                    img.Dispose();
                }
                panel1.Invalidate();
                MessageBox.Show("Все картинки обработаны и сохранены");
            }
            catch (Exception ex)
            { 
                MessageBox.Show("ошибка: " + ex.Message); 
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // удаляем картинку, если нажали Delete
            if (e.KeyCode == Keys.Delete)
            { 
                Delete(); 
                panel1.Invalidate(); 
            }
        }

        private void Delete()
        {
            // удаляем выбранную картинку из списка и панели
            Point mousePos = panel1.PointToClient(Cursor.Position);
            int index = (mousePos.Y - panel1.AutoScrollPosition.Y - 15) / 90;
            if (index < 0 || index >= imagePaths.Count)
            {
                MessageBox.Show("Не выбрано изображение");
                return;
            }

            imagePaths.RemoveAt(index);
            dataGridView1.Rows.RemoveAt(index);

            List<Bitmap> updatedBitmaps = new List<Bitmap>();
            foreach (string path in imagePaths)
            { 
                updatedBitmaps.Add(new Bitmap(path));
            }
            draw(updatedBitmaps);
        }

        private void showControls()
        {
            // показываем все элементы управления
            dataGridView1.Visible = button1.Visible = button2.Visible = button3.Visible = true;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("made by Kirill Berezhetskij IVT-2\nversion 0.0.1 :)", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}