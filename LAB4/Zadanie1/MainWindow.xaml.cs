using System;
using System.Data;
using System.Windows;

namespace Zadanie1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int[,] array = new int[5, 7];
            Random rand = new Random();

            // заполняем массив случайными числами
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 7; j++)
                    array[i, j] = rand.Next(50, 100);

            // создаём таблицу для вывода
            DataTable dt = new DataTable();
            for (int j = 0; j < 7; j++)
                dt.Columns.Add($"Столбец {j + 1}");

            for (int i = 0; i < 5; i++)
            {
                DataRow row = dt.NewRow();
                for (int j = 0; j < 7; j++)
                    row[j] = array[i, j];
                dt.Rows.Add(row);
            }

            dataGrid.ItemsSource = dt.DefaultView;
        }
    }
}