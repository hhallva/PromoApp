using DataLayer.DTOs;
using DataLayer.Models;
using DataLayer.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{

    public partial class MainWindow : Window
    {
        PromocodeService promocodeService = new PromocodeService();

        private List<Promocode> promocodes = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPromocodes();
        }

        private async Task LoadPromocodes()
        {
            promocodes = await promocodeService.GetAllAsync();
            ActivePromocodeListBox.ItemsSource = promocodes.Where(p => !p.IsActive);
            InactivePromocodeListBox.ItemsSource = promocodes.Where(p => p.IsActive);

            ActivePromocodeCountLabel.Text = promocodes.Count.ToString();
            InactivePromocodeCountLabel.Text = promocodes.Count.ToString();
        }

        
        private async void ActivatePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            string code = ActivationPromocodeTextBox.Text.Trim();
            ActivationResultListBox.Items.Clear();

            if (string.IsNullOrEmpty(code))
            {
                ActivationResultListBox.Items.Add("Введите промокод.");
                return;
            }

            var promo = promocodes.Find(p => p.Code == code);

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
                        
            await promocodeService.ActivatePromocodeAsync(code);

            await LoadPromocodes();
            ActivationResultListBox.Items.Add($"Промокод {code} успешно активирован!");
            ActivationPromocodeTextBox.Clear();
        }

        private async void DeleteInactivePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Promocode promo)
            {
                await promocodeService.DeletePromocodeAsync(promo.Code);
                await LoadPromocodes();
            }
        }
        private async void AddNewPromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            string newCode = NewPromocodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(newCode))
            {
                MessageBox.Show("Введите промокод!");
                return;
            }

            if (promocodes.Exists(p => p.Code == newCode))
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

            var promo = new PostPromocodeDto
            {
                Code = newCode,
                StartDate = start,
                EndDate = end,
            };

            await promocodeService.AddPromocodeAsync(promo);
            await LoadPromocodes();

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

        private async void DeleteActivePromocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Promocode promo)
            {
                await promocodeService.DeletePromocodeAsync(promo.Code);
                await LoadPromocodes();
            }
        }
    }
}