using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class ClientsPage : Page
{
    private readonly ClientRepo _repo = new();
    private List<Client> _all = new();

    public ClientsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _all = _repo.GetAll();
            Dg.ItemsSource = _all;
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ClientsPage.Loaded");
        }
    }

    private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
    {
        if (TxtSearch == null) return;
        if (TxtSearch.Text.Trim() == "Поиск по ФИО/телефону...")
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
            TxtSearch.Text = "Поиск по ФИО/телефону...";
            TxtSearch.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#999999")!;
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Dg == null || TxtSearch == null) return; // Важно!

        var raw = TxtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(raw) || raw == "Поиск по ФИО/телефону...")
        {
            Dg.ItemsSource = _all;
            return;
        }

        try
        {
            Dg.ItemsSource = _repo.Search(_all, raw);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ClientsPage.Search");
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (Dg == null) return;

        try
        {
            var list = (Dg.ItemsSource as IEnumerable<Client>)?.ToList() ?? new List<Client>();
            foreach (var c in list)
            {
                // базовая валидация без “умничания”, чтобы не падать на CHECK (0..100)
                if (c.Скидка.HasValue)
                {
                    if (c.Скидка.Value < 0) c.Скидка = 0;
                    if (c.Скидка.Value > 100) c.Скидка = 100;
                }
                _repo.Update(c);
            }

            _all = _repo.GetAll();
            Dg.ItemsSource = _all;
            MessageBox.Show("Сохранено.", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "ClientsPage.Save");
        }
    }
}

