using System;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class DashboardPage : Page
{
    private readonly DashboardRepo _repo = new();

    public DashboardPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            TxtTodayCount.Text = _repo.GetTodayBookingsCount().ToString();
            LstFreeMasters.ItemsSource = _repo.GetFreeMastersSummary();
            LstLowStock.ItemsSource = _repo.GetLowStockWarning();
            TxtWeekRevenue.Text = _repo.GetWeekRevenue().ToString("0 ₽");
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "DashboardPage.Loaded");
        }
    }
}

