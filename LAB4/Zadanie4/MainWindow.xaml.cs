using System.Windows;

namespace Zadanie4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowTemperatures_Click(object sender, RoutedEventArgs e)
        {
            temperatureList.Items.Clear();

            for (int celsius = 0; celsius <= 100; celsius += 5)
            {
                double fahrenheit = 1.8 * celsius + 32;
                temperatureList.Items.Add($"{celsius} °C = {fahrenheit:F1} °F");
            }
        }
    }
}