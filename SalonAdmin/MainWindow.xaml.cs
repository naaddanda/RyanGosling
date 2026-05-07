using System;
using System.Windows;
using System.Windows.Controls;

namespace SalonAdmin
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var type = Type.GetType($"SalonAdmin.Pages.{tag}Page");
                if (type != null) MainFrame.Navigate((Page)Activator.CreateInstance(type));
            }
        }
    }
}