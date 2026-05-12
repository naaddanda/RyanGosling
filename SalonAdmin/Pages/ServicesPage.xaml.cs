using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class ServicesPage : Page
{
    private readonly ServiceRepo _repo = new();
    private readonly CatalogRepo _catalog = new();

    private List<Service> _all = new();
    private List<string> _categories = new();

    public ServicesPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _categories = _catalog.GetServiceCategories().Select(x => x.Название).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (CmbCategory != null)
            {
                CmbCategory.ItemsSource = new[] { "Все категории" }.Concat(_categories).ToList();
                CmbCategory.SelectedIndex = 0;
            }

            _all = _repo.GetAll();
            Dg.ItemsSource = _all;
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ServicesPage.Loaded");
        }
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (Dg == null || CmbCategory == null) return; // Важно!
        ApplyFilters();
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Dg == null || TxtSearch == null) return; // Важно!
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (Dg == null || CmbCategory == null || TxtSearch == null) return;

        var cat = CmbCategory.SelectedItem as string ?? "Все категории";
        var q = TxtSearch.Text.Trim();
        if (q == "Поиск по названию...") q = "";

        try
        {
            Dg.ItemsSource = _repo.Filter(_all, cat, q);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ServicesPage.Filter");
        }
    }

    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TxtSearch == null) return;
        if (TxtSearch.Text.Trim() == "Поиск по названию...")
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
            TxtSearch.Text = "Поиск по названию...";
            TxtSearch.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#999999")!;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Dg == null) return;
        try
        {
            var list = (Dg.ItemsSource as IEnumerable<Service>)?.ToList() ?? new List<Service>();
            foreach (var s in list)
            {
                if (s.Длительность < 1) s.Длительность = 1;
                if (s.Цена < 0) s.Цена = 0;
                _repo.Update(s);
            }

            _all = _repo.GetAll();
            ApplyFilters();
            MessageBox.Show("Сохранено.", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ServicesPage.Save");
        }
    }
}

