using System;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class ReportsPage : Page
{
    public ReportsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            if (DpFrom != null && DpFrom.SelectedDate == null)
                DpFrom.SelectedDate = DateTime.Today.AddDays(-7);
            if (DpTo != null && DpTo.SelectedDate == null)
                DpTo.SelectedDate = DateTime.Today;

            Recalc();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ReportsPage.Loaded");
        }
    }

    private void Date_Changed(object sender, SelectionChangedEventArgs e)
    { 
        if (DpFrom?.SelectedDate == null || DpTo?.SelectedDate == null) return;
        //Recalc();
    }

    private void BtnCalc_Click(object sender, System.Windows.RoutedEventArgs e) => Recalc();

    private void Recalc()
    {
        if (DpFrom?.SelectedDate == null || DpTo?.SelectedDate == null) return;
        if (TxtRevenue == null || LstTopServices == null || LstTopMasters == null) return;

        try
        {
            var from = DpFrom.SelectedDate.Value.Date;
            var to = DpTo.SelectedDate.Value.Date;
            if (to < from) (from, to) = (to, from); // авто-коррекция если даты перепутаны

            TxtRevenue.Text = ReportsRepo.GetRevenueForRange(from, to).ToString("0 ₽");
            LstTopServices.ItemsSource = ReportsRepo.GetTopServices(from, to, 7);
            LstTopMasters.ItemsSource = ReportsRepo.GetTopMasters(from, to, 7);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ReportsPage.Recalc");
        }
    }
}