using System;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            Loaded += EmployeesPage_Loaded;
        }

        private void EmployeesPage_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Поиск по ФИО...")
                txtSearch.Text = "";
        }

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                txtSearch.Text = "Поиск по ФИО...";
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: модалка редактирования
        }

        private void Vac_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var emp = EmployeeRepo.GetById(id);
                if (emp != null)
                {
                    EmployeeRepo.ToggleVacation(id, emp.Статус != "Отпуск");
                    Load();
                }
            }
        }

        private void Load()
        {
            try
            {
                dg.ItemsSource = EmployeeRepo.GetAll();
            }
            catch (Exception ex)
            {
                Db.ShowError(ex, "Сотрудники");
            }
        }
    }
}