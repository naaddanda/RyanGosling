using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class EmployeesPage : Page
{
    private List<Employee> _allEmployees = new();

    public EmployeesPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 Загрузка сотрудников...");
            _allEmployees = EmployeeRepo.GetAll();
            System.Diagnostics.Debug.WriteLine($"✅ Загружено: {_allEmployees.Count} записей");

            Dg.ItemsSource = _allEmployees;
            System.Diagnostics.Debug.WriteLine($"📊 ItemsSource установлен, ActualCount: {Dg.Items.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Ошибка: {ex.Message}");
            ClassDaT.ShowError(ex, "EmployeesPage.Loaded");
        }
    }

    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TxtSearch?.Text.Trim() == "Поиск по ФИО...")
        {
            TxtSearch.Text = "";
            TxtSearch.Foreground = Brushes.Black;
        }
    }

    private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtSearch?.Text))
        {
            TxtSearch.Text = "Поиск по ФИО...";
            TxtSearch.Foreground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Dg == null || TxtSearch == null) return;

        var raw = TxtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(raw) || raw == "Поиск по ФИО...")
        {
            Dg.ItemsSource = _allEmployees;
            return;
        }

        try
        {
            var filtered = _allEmployees.Where(emp =>
                emp.FullName.IndexOf(raw, StringComparison.OrdinalIgnoreCase) >= 0 ||
                emp.Телефон.IndexOf(raw, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();
            Dg.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "EmployeesPage.Search");
        }
    }

    // ✅ Кнопка Edit — получает Employee через CommandParameter
    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Employee emp)
        {
            NavigationService?.Navigate(new EmployeeEditPage(emp));
        }
    }

    // ✅ Кнопка Vacation — получает Employee и показывает заглушку
    private void BtnVacation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Employee emp)
        {
            var isNowVacation = emp.Статус == "Отпуск";
            var action = isNowVacation ? "вернуть с отпуска" : "отправить в отпуск";
            var newStatus = isNowVacation ? "Активен" : "Отпуск";

            var result = MessageBox.Show(
                $"{action} сотрудника {emp.FullName}?\n" +
                $"Статус: {emp.Статус} → {newStatus}",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                EmployeeRepo.ToggleVacation(emp.Id, !isNowVacation);
                Page_Loaded(null, null); // перезагрузка списка
            }
        }
    }
}