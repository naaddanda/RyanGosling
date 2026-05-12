using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class StocksPage : Page
{
    private readonly SupplyRepo _repo = new();
    private List<Supply> _all = new();

    public StocksPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _all = _repo.GetAllSupplies();
            Dg.ItemsSource = _all;
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "StocksPage.Loaded");
        }
    }

    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TxtSearch == null) return;
        if (TxtSearch.Text.Trim() == "Поиск по расходникам...")
        {
            TxtSearch.Text = "";
            TxtSearch.Foreground = System.Windows.Media.Brushes.Black;
        }
    }

    private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
    {
        if (TxtSearch == null) return;
        if (string.IsNullOrWhiteSpace(TxtSearch.Text))
        {
            TxtSearch.Text = "Поиск по расходникам...";
            TxtSearch.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#999999")!;
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Dg == null || TxtSearch == null) return; // Важно!
        var raw = TxtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(raw) || raw == "Поиск по расходникам...")
        {
            Dg.ItemsSource = _all;
            return;
        }

        var q = raw.ToLowerInvariant();
        Dg.ItemsSource = _all.Where(x => (x.Название ?? "").ToLowerInvariant().Contains(q)).ToList();
    }

    private void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        if (Dg == null) return;
        if (Dg.SelectedItem is not Supply s)
        {
            MessageBox.Show("Выберите расходник в таблице.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var wnd = Window.GetWindow(this) as MainWindow;
        if (wnd == null) return;
        wnd.MainFrame.Navigate(new StoragePage(s.Id, s.Название));
    }
}

