using SalonAdmin.ClassApp;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SalonAdmin.Pages;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (TxtConn == null) return;
        TxtConn.Text = ClassDaT.ConnectionString;
        if (TxtStatus != null) TxtStatus.Text = "";
    }

    private void BtnTest_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var conn = new SqlConnection(ClassDaT.ConnectionString);
            conn.Open();
            if (TxtStatus != null)
            {
                TxtStatus.Text = "Подключение успешно";
                TxtStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#2E7D32")!;
            }
        }
        catch (Exception ex)
        {
            if (TxtStatus != null)
            {
                TxtStatus.Text = "Ошибка подключения";
                TxtStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#C62828")!;
            }
            ClassDaT.ShowError(ex, "SettingsPage.TestConnection");
        }
    }

    private void BtnCalc_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(TxtNum1.Text, out double num1))
        {
            TxtResult.Text = "❌ Первое число — не число";
            TxtResult.Foreground = System.Windows.Media.Brushes.Red;
            return;
        }

        if (!double.TryParse(TxtNum2.Text, out double num2))
        {
            TxtResult.Text = "❌ Второе число — не число";
            TxtResult.Foreground = System.Windows.Media.Brushes.Red;
            return;
        }

        string op = "+";
        if (CbOp.SelectedItem is ComboBoxItem item && item.Content is string opText)
        {
            op = opText;
        }

        double result = 0;
        bool error = false;

        if (op == "+")
        {
            result = num1 + num2;
        }
        else if (op == "-")
        {
            result = num1 - num2;
        }
        else if (op == "*")
        {
            result = num1 * num2;
        }
        else if (op == "/")
        {
            if (num2 == 0)
            {
                TxtResult.Text = "❌ На ноль делить нельзя";
                TxtResult.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }
            result = num1 / num2;
        }
        else
        {
            error = true;
        }

        if (error)
        {
            TxtResult.Text = "❌ Неизвестная операция";
            TxtResult.Foreground = System.Windows.Media.Brushes.Red;
        }
        else
        {
            if (result == Math.Floor(result))
                TxtResult.Text = $"= {result:F0}";
            else
                TxtResult.Text = $"= {result:F2}";
            TxtResult.Foreground = System.Windows.Media.Brushes.Black;
        }
    }

    private void TxtNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        bool isOk = false;

        if (e.Text.Length == 1)
        {
            char c = e.Text[0];

            if (char.IsDigit(c))
            {
                isOk = true;
            }
            else if (c == '.')
            {
                if (sender is TextBox tb && !tb.Text.Contains("."))
                {
                    isOk = true;
                }
            }
            else if (c == '-')
            {
                if (sender is TextBox tb && tb.Text.Length == 0 && !tb.Text.Contains("-"))
                {
                    isOk = true;
                }
            }
        }

        e.Handled = !isOk;
    }
}