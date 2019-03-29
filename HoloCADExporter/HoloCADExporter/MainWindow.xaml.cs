using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            SetState(State.Unprepared);
        }

        private enum State
        {
            Unprepared,
            Prepared,
            Converting,
            Done
        }

        private State _state;

        private void SetState(State newState)
        {
            _state = newState;
            switch (newState)
            {
                case State.Unprepared:
                    statusBar.IsIndeterminate = false;
                    statusBar.Value = 0;
                    statusBlock.Text = "Не подготовлено";
                    convertButton.IsEnabled = false;
                    break;
                case State.Prepared:
                    statusBar.IsIndeterminate = false;
                    statusBar.Value = 0;
                    statusBlock.Text = "Подготовлено";
                    convertButton.IsEnabled = true;
                    break;
                case State.Converting:
                    statusBar.IsIndeterminate = true;
                    statusBlock.Text = "Конвертация";
                    convertButton.IsEnabled = false;
                    break;
                case State.Done:
                    statusBar.IsIndeterminate = false;
                    statusBar.Value = 100;
                    statusBlock.Text = "Завершено";
                    convertButton.IsEnabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void SelectJsonFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Выберите файл HoloCAD", 
                Filter = "HoloCAD json file|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                jsonFileNameBox.Text = dialog.FileName;
            }
        }

        private void SelectOutputDirectory(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                InitialDirectory = directoryPathBox.Text,
                Title = "Select a Directory",
                Filter = "Directory|*.this.directory",
                FileName = "select"
            };
            if (dialog.ShowDialog() == true) {
                string path = dialog.FileName;
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                directoryPathBox.Text = path;
            }
        }

        private async void ConvertButtonPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_state == State.Unprepared || _state == State.Converting)
                {
                    throw new Exception("Нельзя запустить конвертацию сейчас.");
                }
                SetState(State.Converting);
                
                await KompasConverter.ConvertToKompas(jsonFileNameBox.Text, directoryPathBox.Text);
                SetState(State.Done);
            }
            catch (Exception exception)
            {
                SetState(State.Unprepared);
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void FileOrDirectoryChanged(object sender, TextChangedEventArgs e)
        {
            bool isOk = true;
            if (File.Exists(jsonFileNameBox.Text) && Path.GetExtension(jsonFileNameBox.Text) == ".json")
            {
                jsonFileNameBox.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                jsonFileNameBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                isOk = false;
            }

            if (Directory.Exists(directoryPathBox.Text))
            {
                directoryPathBox.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            else
            {
                directoryPathBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                isOk = false;
            }

            SetState(isOk ? State.Prepared : State.Unprepared);
        }
    }
}