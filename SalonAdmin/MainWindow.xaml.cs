using System;
using System.Windows;
using System.Windows.Controls;
using SalonAdmin.ClassApp; // добавить using

namespace SalonAdmin;

public partial class MainWindow : Window
{
    private User? _currentUser;

    // Конструктор без параметров (на случай прямого запуска)
    public MainWindow()
    {
        InitializeComponent();
        NavigateTo("Dashboard");
    }

    // Конструктор с пользователем (для авторизации)
    public MainWindow(User user) : this()
    {
        _currentUser = user;
        if (_currentUser != null)
            Title = $"SalonAdmin — {_currentUser.Login} ({_currentUser.RoleName})";
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tag)
            NavigateTo(tag);
    }

    private void NavigateTo(string tag)
    {
        var type = Type.GetType($"SalonAdmin.Pages.{tag}Page");
        if (type != null)
            MainFrame.Navigate((Page)Activator.CreateInstance(type)!);
    }
}