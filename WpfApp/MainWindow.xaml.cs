using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{

    public partial class MainWindow : Window
    {
        private List<Promocode> _activePromocodes = new();
        private List<Promocode> _inactivePromocodes = new();

        public MainWindow()
        {
            InitializeComponent();

            _inactivePromocodes.Add(new Promocode
            {
                Id = 1,
                Code = "SALE2025",
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(10),
                IsActive = false
            });

            _inactivePromocodes.Add(new Promocode
            {
                Id = 2,
                Code = "DISCOUNT",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                IsActive = false
            });

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
        private void ActivatePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            string code = ActivationPromocodeTextBox.Text.Trim();
            ActivationResultListBox.Items.Clear();

            if (string.IsNullOrEmpty(code))
            {
                ActivationResultListBox.Items.Add("Введите промокод.");
                return;
            }

            var promo = _inactivePromocodes.Find(p => p.Code == code);

            if (promo == null)
            {
                ActivationResultListBox.Items.Add("Промокод не найден.");
                return;
            }

            if (promo.StartDate.HasValue && promo.StartDate > DateTime.Now)
            {
                ActivationResultListBox.Items.Add($"Промокод {code} ещё не доступен. Дата начала: {promo.StartDate:d}");
                return;
            }

            if (promo.EndDate.HasValue && promo.EndDate < DateTime.Now)
            {
                ActivationResultListBox.Items.Add($"Промокод {code} просрочен. Дата окончания: {promo.EndDate:d}");
                return;
            }

            _inactivePromocodes.Remove(promo);
            promo.IsActive = true;
            _activePromocodes.Add(promo);

            UpdateLists();
            ActivationResultListBox.Items.Add($"Промокод {code} успешно активирован!");
            ActivationPromocodeTextBox.Clear();
        }

        private void DeleteInactivePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Promocode promo)
            {
                _inactivePromocodes.Remove(promo);
                UpdateLists();
            }
        }
        private void AddNewPromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            string newCode = NewPromocodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newCode))
            {
                MessageBox.Show("Введите промокод!");
                return;
            }

            if (_inactivePromocodes.Exists(p => p.Code == newCode) ||
                _activePromocodes.Exists(p => p.Code == newCode))
            {
                MessageBox.Show("Такой промокод уже существует!");
                return;
            }

            DateTime? start = null;
            DateTime? end = null;

            if (UnlimitedCheckBox.IsChecked == true)
            {
                start = null;
                end = null;
            }
            else if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                start = StartDatePicker.SelectedDate.Value;
                end = EndDatePicker.SelectedDate.Value;

                if (end < start)
                {
                    MessageBox.Show("Дата окончания не может быть раньше даты начала!");
                    return;
                }
            }
            else if (StartDatePicker.SelectedDate.HasValue && !EndDatePicker.SelectedDate.HasValue)
            {
                start = StartDatePicker.SelectedDate.Value;
                end = null;
            }
            else if (!StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                start = DateTime.Now.Date;
                end = EndDatePicker.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Укажите хотя бы одну дату (или выберите 'Бессрочный')");
                return;
            }

            var promo = new Promocode
            {
                Id = _inactivePromocodes.Count + _activePromocodes.Count + 1,
                Code = newCode,
                StartDate = start,
                EndDate = end,
                IsActive = false
            };

            _inactivePromocodes.Add(promo);
            UpdateLists();

            NewPromocodeTextBox.Clear();
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            UnlimitedCheckBox.IsChecked = false;
        }

        private void UnlimitedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            StartDatePicker.IsEnabled = false;
            EndDatePicker.IsEnabled = false;
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
        }

        private void UnlimitedCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            StartDatePicker.IsEnabled = true;
            EndDatePicker.IsEnabled = true;
        }

        private void DeleteActivePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Promocode promo)
            {
                _activePromocodes.Remove(promo);
                UpdateLists();
            }
        }


    }
}