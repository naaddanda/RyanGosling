using System;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class JournalPage : Page
{
    private readonly JournalRepo _repo = new();

    public JournalPage()
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

            Reload();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "JournalPage.Loaded");
        }
    }

    private void Date_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (Dg == null || DpFrom == null || DpTo == null) return; // Важно!
        Reload();
    }

    private void Reload()
    {
        if (Dg == null || DpFrom == null || DpTo == null) return;
        if (DpFrom.SelectedDate == null || DpTo.SelectedDate == null) return;

        try
        {
            var from = DpFrom.SelectedDate.Value.Date;
            var to = DpTo.SelectedDate.Value.Date;
            if (to < from)
            {
                // мягко “исправляем” диапазон
                var tmp = from;
                from = to;
                to = tmp;
            }

            Dg.ItemsSource = _repo.GetForRange(from, to);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "JournalPage.Reload");
        }
    }
}

