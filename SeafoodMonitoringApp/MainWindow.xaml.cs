using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;

namespace SeafoodMonitoringApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string report;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CheckConditionsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateInputs(out string fishType, out int maxTemp, out int maxTime, out int minTemp, out int minTime,
                    out DateTime startDateTime, out List<int> temperatures);

                
                var checker = new StorageConditionsChecker();
                report = checker.CheckStorageConditions(fishType, maxTemp, maxTime, minTemp, minTime, startDateTime, temperatures);
                
                OutputTextBox.Text = report;
                MessageBox.Show("Проверка завершена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(report))
            {
                MessageBox.Show("Нет данных для сохранения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Сохранить отчет"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, report);
                MessageBox.Show("Отчет сохранен.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Выберите файл данных"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(openFileDialog.FileName);
                if (lines.Length < 2)
                {
                    MessageBox.Show("Некорректный формат файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                StartDateTimeTextBox.Text = lines[0];
                TemperatureReadingsTextBox.Text = lines[1];
                MessageBox.Show("Данные загружены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ValidateInputs(out string fishType, out int maxTemp, out int maxTime, out int minTemp, out int minTime,
            out DateTime startDateTime, out List<int> temperatures)
        {
            fishType = FishTypeTextBox.Text;
            if (string.IsNullOrWhiteSpace(fishType))
                throw new Exception("Введите вид рыбы.");

            if (!int.TryParse(MaxTempTextBox.Text, out maxTemp))
                throw new Exception("Введите корректную максимальную температуру.");

            if (!int.TryParse(MaxTempTimeTextBox.Text, out maxTime))
                throw new Exception("Введите корректное время превышения.");

            if (!int.TryParse(MinTempTextBox.Text, out minTemp))
                throw new Exception("Введите корректную минимальную температуру.");

            if (!int.TryParse(MinTempTimeTextBox.Text, out minTime))
                throw new Exception("Введите корректное время ниже минимума.");

            if (!DateTime.TryParse(StartDateTimeTextBox.Text, out startDateTime))
                throw new Exception("Введите корректную дату/время начала измерений.");

            var tempStrings = TemperatureReadingsTextBox.Text.Split(',');
            temperatures = tempStrings.Select(temp => int.TryParse(temp, out int value) ? value : throw new Exception("Некорректные данные температуры.")).ToList();
        }

    }
}
