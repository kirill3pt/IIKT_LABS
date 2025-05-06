using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Zadanie2
{
    public partial class MainWindow : Window
    {
        private const double ExchangeRate = 0.012261; // взял курс на 29.04.2025: 1 рубль = 0,012261 доллара

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputRubles.Text == "Введите кол-во рублей")
            {
                outputDollars.Text = "Сначала введите сумму!";
                return;
            }

            if (double.TryParse(inputRubles.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rubles))
            {
                double dollars = rubles * ExchangeRate;
                outputDollars.Text = $"Это примерно: {dollars:F2} $";
            }
            else
            {
                outputDollars.Text = "Ошибка: введите корректную сумму!";
            }
        }

        private void InputRubles_GotFocus(object sender, RoutedEventArgs e)
        {
            if (inputRubles.Text == "Введите сумму в рублях")
            {
                inputRubles.Text = "";
                inputRubles.Foreground = Brushes.Black;
            }
        }

        private void InputRubles_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputRubles.Text))
            {
                inputRubles.Text = "Введите сумму в рублях";
                inputRubles.Foreground = Brushes.Gray;
            }
        }
    }
}
