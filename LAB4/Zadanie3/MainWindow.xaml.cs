using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Zadanie3
{
    public partial class MainWindow : Window
    {
        private const double usd = 0.012261; //все курсы валют взяты на 29.04.2025 23:31 :) 
        private const double eur = 0.010733;
        private const double uah = 0.51;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputRubles.Text == "Введите сумму в рублях")
            {
                outputResult.Text = "Введите сумму!";
                return;
            }

            if (double.TryParse(inputRubles.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rubles))
            {
                double result = 0;
                string currency = "";

                if (radioUSD.IsChecked == true)
                {
                    result = rubles * usd;
                    currency = "$.";
                }
                else if (radioEUR.IsChecked == true)
                {
                    result = rubles * eur;
                    currency = "€.";
                }
                else if (radioUAH.IsChecked == true)
                {
                    result = rubles * uah;
                    currency = "₴.";
                }

                outputResult.Text = $"Это примерно: {result:F2} {currency}";
            }
            else
            {
                outputResult.Text = "Ошибка: введите корректное число!";
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