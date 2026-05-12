using Microsoft.Win32;
using SalonAdmin.ClassApp;
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
using System.Windows.Shapes;

namespace SalonAdmin.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeeEditPage.xaml
    /// </summary>
    public partial class EmployeeEditPage : Page
    {
        private Employee _emp = new();
        private bool _isNew;

        public EmployeeEditPage()
        {
            InitializeComponent();
        }

        // Конструктор для редактирования существующего сотрудника
        public EmployeeEditPage(Employee emp) : this()
        {
            _emp = emp;
            _isNew = false;
        }

        // Конструктор для создания нового сотрудника
        public EmployeeEditPage(bool isNew) : this()
        {
            _isNew = isNew;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загружаем справочники
                CbQual.ItemsSource = EmployeeRepo.GetQualifications();
                CbStatus.ItemsSource = EmployeeRepo.GetStatuses();

                if (_isNew)
                {
                    TxtTitle.Text = "➕ Новый сотрудник";
                    TxtStazh.Text = "0";
                    TxtRating.Text = "0";
                    // По умолчанию статус "Активен" (предполагаем Id=1)
                    if (CbStatus.Items.Count > 0)
                        CbStatus.SelectedIndex = 0;
                }
                else if (_emp?.Id > 0)
                {
                    TxtTitle.Text = $"✏️ {_emp.FullName}";
                    FillForm();
                }
            }
            catch (Exception ex)
            {
                ClassDaT.ShowError(ex, "EmployeeEditPage.Loaded");
            }
        }

        private void FillForm()
        {
            TxtFam.Text = _emp.Фамилия;
            TxtName.Text = _emp.Имя;
            TxtOt.Text = _emp.Отчество ?? "";
            TxtTel.Text = _emp.Телефон;
            TxtStazh.Text = _emp.Стаж.ToString();
            TxtRating.Text = _emp.Рейтинг?.ToString("0.0") ?? "0";

            if (_emp.Квалификация?.Id > 0)
                CbQual.SelectedValue = _emp.Квалификация.Id;

            if (_emp.Id_статус > 0)
                CbStatus.SelectedValue = _emp.Id_статус;

            TxtPhoto.Text = _emp.Фото ?? "";
        }

        private Employee GetFromForm()
        {
            return new Employee
            {
                Id = _emp.Id,
                Фамилия = TxtFam.Text.Trim(),
                Имя = TxtName.Text.Trim(),
                Отчество = string.IsNullOrWhiteSpace(TxtOt.Text) ? null : TxtOt.Text.Trim(),
                Телефон = TxtTel.Text.Trim(),
                Стаж = int.TryParse(TxtStazh.Text, out var st) ? st : 0,
                Рейтинг = decimal.TryParse(TxtRating.Text, out var r)
                    ? (decimal?)Math.Min(Math.Max(r, 0), 5)
                    : null,
                Квалификация = CbQual.SelectedValue is int qid
                    ? new Qualification { Id = qid }
                    : null,
                Id_статус = CbStatus.SelectedValue is int sid ? sid : 1,
                Статус = (CbStatus.SelectedItem as KeyValuePair<int, string>?)?.Value ?? "Активен",
                Фото = string.IsNullOrWhiteSpace(TxtPhoto.Text) ? null : TxtPhoto.Text.Trim()
            };
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(TxtFam.Text))
            {
                MessageBox.Show("Введите фамилию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtFam.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Введите имя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtTel.Text))
            {
                MessageBox.Show("Введите телефон", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtTel.Focus();
                return false;
            }
            if (!int.TryParse(TxtStazh.Text, out var st) || st < 0)
            {
                MessageBox.Show("Стаж должен быть неотрицательным числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtStazh.Focus();
                return false;
            }
            return true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            try
            {
                var updated = GetFromForm();

                if (_isNew)
                {
                    MessageBox.Show(
                        "Создание нового сотрудника пока не реализовано.\n" +
                        "Для добавления нужно реализовать метод Insert в EmployeeRepo.",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (EmployeeRepo.Update(updated))
                {
                    MessageBox.Show("Данные сохранены ✓", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    if (NavigationService?.CanGoBack == true)
                        NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить изменения", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                ClassDaT.ShowError(ex, "EmployeeEditPage.Save");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void BtnChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif|Все файлы|*.*",
                Title = "Выберите фото сотрудника"
            };

            if (dlg.ShowDialog() == true)
            {
                TxtPhoto.Text = System.IO.Path.GetFileName(dlg.FileName);
            }
        }

        // Валидация: только цифры
        private void TxtInt_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
            e.Handled = !char.IsDigit(e.Text, 0);

        // Валидация: цифры и одна точка
        private void TxtDecimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != ".")
                e.Handled = true;
            if (e.Text == "." && sender is TextBox tb && tb.Text.Contains("."))
                e.Handled = true;
        }

        // Валидация телефона: цифры, +, -, пробелы, скобки
        private void TxtTel_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
            e.Handled = !char.IsDigit(e.Text, 0) && "+-() ".IndexOf(e.Text) < 0;
    }
}