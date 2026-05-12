using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class RecordsPage : Page
{
    private readonly BookingRepo _repo = new();
    private List<Booking> _allBookings = new();
    private List<string> _statuses = new();
    private List<string> _masters = new();
    private bool _suppressInlineUpdate;

    public RecordsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (DpDate != null && DpDate.SelectedDate == null)
                DpDate.SelectedDate = DateTime.Today;

            _statuses = _repo.GetStatuses();
            _masters = _repo.GetMasters();

            if (CmbStatus != null)
            {
                CmbStatus.ItemsSource = new[] { "Все статусы" }.Concat(_statuses).ToList();
                CmbStatus.SelectedIndex = 0;
            }

            if (CmbMaster != null)
            {
                CmbMaster.ItemsSource = new[] { "Все мастера" }.Concat(_masters).ToList();
                CmbMaster.SelectedIndex = 0;
            }

            ReloadBookings();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "RecordsPage.Loaded");
        }
    }

    private void BtnToday_Click(object sender, RoutedEventArgs e)
    {
        if (DpDate == null) return;
        DpDate.SelectedDate = DateTime.Today;
    }

    private void DpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DpDate == null) return; // Важно!
        ReloadBookings();
    }

    private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Dg == null || CmbStatus == null || CmbMaster == null) return; // Важно!
        ApplyFilters();
    }

    private void ReloadBookings()
    {
        if (DpDate == null || Dg == null) return;
        if (DpDate.SelectedDate == null) return;

        try
        {
            var dt = DpDate.SelectedDate.Value.Date;
            _allBookings = _repo.GetForDate(dt);

            // пробрасываем список статусов в каждую строку (для inline ComboBox)
            foreach (var b in _allBookings)
                b.Statuses = _statuses.ToList();

            ApplyFilters();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "RecordsPage.ReloadBookings");
        }
    }

    private void ApplyFilters()
    {
        if (Dg == null || CmbStatus == null || CmbMaster == null) return;

        var status = CmbStatus.SelectedItem as string ?? "Все статусы";
        var master = CmbMaster.SelectedItem as string ?? "Все мастера";

        var q = _allBookings.AsEnumerable();
        if (status != "Все статусы")
            q = q.Where(x => string.Equals(x.Статус, status, StringComparison.OrdinalIgnoreCase));
        if (master != "Все мастера")
            q = q.Where(x => string.Equals(x.Мастер, master, StringComparison.OrdinalIgnoreCase));

        Dg.ItemsSource = q.ToList();
    }

    private void InlineStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressInlineUpdate) return;
        if (sender is not ComboBox cb) return;
        if (cb.DataContext is not Booking booking) return;
        if (cb.SelectedItem is not string newStatus) return;
        if (string.IsNullOrWhiteSpace(newStatus)) return;

        // Если статус не менялся — не делаем UPDATE
        if (string.Equals(booking.Статус, newStatus, StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            _repo.UpdateStatus(booking.Id, newStatus);
            ReloadBookings();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "RecordsPage.UpdateStatus");
            // откат визуального выбора
            try
            {
                _suppressInlineUpdate = true;
                cb.SelectedItem = booking.Статус;
            }
            finally
            {
                _suppressInlineUpdate = false;
            }
        }
    }
}

