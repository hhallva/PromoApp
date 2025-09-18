using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _activePromocodes = new List<string>();
        private List<string> _inactivePromocodes = new List<string>();

        public MainWindow()
        {
            _activePromocodes.Add("PROMO223");
            _inactivePromocodes.Add("PROMO123");
            InitializeComponent();
            UpdateLists();
        }

        private void UpdateLists()
        {
            ActivePromocodeListBox.ItemsSource = null;
            ActivePromocodeListBox.ItemsSource = _activePromocodes;
            ActivePromocodeCountLabel.Text = _activePromocodes.Count.ToString();

            InactivePromocodeListBox.ItemsSource = null;
            InactivePromocodeListBox.ItemsSource = _inactivePromocodes;
            InactivePromocodeCountLabel.Text = _inactivePromocodes.Count.ToString();
        }
        private void DeleteActivePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is string code)
            {
                if (MessageBox.Show($"Удалить активированный промокод {code}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _activePromocodes.Remove(code);
                    UpdateLists();
                }
            }
        }

        private void DeleteInactivePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is string code)
            {
                if (MessageBox.Show($"Удалить неактивированный промокод {code}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _inactivePromocodes.Remove(code);
                    UpdateLists();
                }
            }
        }

        private void ActivatePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            string code = ActivationPromocodeTextBox.Text.Trim();
            ActivationResultListBox.Items.Clear();

            if (string.IsNullOrEmpty(code))
            {
                ActivationResultListBox.Items.Add("Введите промокод для активации.");
                return;
            }

            if (_activePromocodes.Contains(code))
            {
                ActivationResultListBox.Items.Add($"Промокод {code} уже активирован.");
            }
            else if (_inactivePromocodes.Contains(code))
            {
                _inactivePromocodes.Remove(code);
                _activePromocodes.Add(code);
                UpdateLists();
                ActivationResultListBox.Items.Add($"Промокод {code} успешно активирован!");
            }
            else
            {
                ActivationResultListBox.Items.Add($"Промокод {code} не найден в списке неактивированных.");
            }

            ActivationPromocodeTextBox.Clear();
        }

        private void AddNewPromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            string newCode = NewPromocodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newCode))
            {
                MessageBox.Show("Введите промокод!");
                return;
            }

            if (_inactivePromocodes.Contains(newCode) || _activePromocodes.Contains(newCode))
            {
                MessageBox.Show("Промокод уже существует!");
                return;
            }

            _inactivePromocodes.Add(newCode);
            UpdateLists();
            NewPromocodeTextBox.Clear();
        }
    }
}