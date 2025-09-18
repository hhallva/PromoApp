using System.Windows;

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
                ActivationResultListBox.Items.Add($"Промокод {code} успешно активирован!");
            }
            else
            {
                ActivationResultListBox.Items.Add($"Промокод {code} не найден в списке неактивированных.");
            }

            ActivationPromocodeTextBox.Clear();
        }
    }
}