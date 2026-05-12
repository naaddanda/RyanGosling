using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class PromotionsPage : Page
{
    private readonly PromotionRepo _repo = new();
    private readonly CatalogRepo _catalog = new();

    private List<Promotion> _all = new();
    private List<KeyValuePair<int, string>> _services = new();

    public PromotionsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _services = _catalog.GetServicesMap().ToList();
            Reload();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "PromotionsPage.Loaded");
        }
    }

    private void Reload()
    {
        _all = _repo.GetAll();
        foreach (var p in _all)
            p.Services = _services;

        Dg.ItemsSource = _all;
    }

    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TxtSearch == null) return;
        if (TxtSearch.Text.Trim() == "Поиск по названию/услуге...")
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
            TxtSearch.Text = "Поиск по названию/услуге...";
            TxtSearch.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#999999")!;
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Dg == null || TxtSearch == null) return; // Важно!
        var raw = TxtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(raw) || raw == "Поиск по названию/услуге...")
        {
            Dg.ItemsSource = _all;
            return;
        }

        var q = raw.ToLowerInvariant();
        Dg.ItemsSource = _all.Where(x =>
                (x.Название ?? "").ToLowerInvariant().Contains(q) ||
                (x.Услуга ?? "").ToLowerInvariant().Contains(q))
            .ToList();
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Dg == null) return;
        try
        {
            var list = (Dg.ItemsSource as IEnumerable<Promotion>)?.ToList() ?? new List<Promotion>();
            foreach (var p in list)
            {
                if (p.Скидка_процент.HasValue)
                {
                    if (p.Скидка_процент.Value < 0) p.Скидка_процент = 0;
                    if (p.Скидка_процент.Value > 100) p.Скидка_процент = 100;
                }
                _repo.Update(p);
            }

            Reload();
            MessageBox.Show("Сохранено.", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "PromotionsPage.Save");
        }
    }
}

