using SalonAdmin.ClassApp;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SalonAdmin.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                tbToday.Text = DashboardRepo.GetTodayBookingsCount().ToString();
                tbSlots.Text = DashboardRepo.GetFreeMasters();
                tbStock.Text = DashboardRepo.GetLowStock();
                tbRev.Text = $"{DashboardRepo.GetWeekRevenue():N0} ₽";
            }
            catch (Exception ex)
            {
                Db.ShowError(ex, "Дашборд");
            }
        }
    }
}