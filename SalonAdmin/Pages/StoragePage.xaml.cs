using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using SalonAdmin.ClassApp;

namespace SalonAdmin.Pages;

public partial class StoragePage : Page
{
    private readonly SupplyRepo _repo = new();
    private readonly int _supplyId;
    private List<Purchase> _all = new();

    public StoragePage(int supplyId, string title)
    {
        InitializeComponent();
        _supplyId = supplyId;
        if (TxtTitle != null) TxtTitle.Text = title;
    }

    public StoragePage() : this(0, "")
    {
        // для дизайнера / навигации без параметров
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            if (CmbMode != null && CmbMode.Items.Count == 0)
            {
                CmbMode.ItemsSource = new[] { "Все партии", "Только неистекшие", "Только истекшие" };
                CmbMode.SelectedIndex = 0;
            }

            if (_supplyId == 0)
            {
                Dg.ItemsSource = Array.Empty<Purchase>();
                return;
            }

            _all = _repo.GetPurchasesForSupply(_supplyId);
            ApplyMode();
        }
        catch (Exception ex)
        {
            ClassDaT.ShowError(ex, "StoragePage.Loaded");
        }
    }

    private void CmbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Dg == null || CmbMode == null) return; // Важно!
        ApplyMode();
    }

    private void ApplyMode()
    {
        if (Dg == null || CmbMode == null) return;
        var mode = CmbMode.SelectedItem as string ?? "Все партии";
        var today = DateTime.Today;

        IEnumerable<Purchase> q = _all;
        if (mode == "Только неистекшие")
            q = q.Where(x => x.Срок_годности_партии.Date >= today);
        else if (mode == "Только истекшие")
            q = q.Where(x => x.Срок_годности_партии.Date < today);

        Dg.ItemsSource = q.ToList();
    }
}

