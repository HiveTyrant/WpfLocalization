using LocalizationService.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocalizationWpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int _index;

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var availableCultures = LocalizationManager.Instance.AvailableCultures;
            _index++;

            if (_index == availableCultures.Count)
                _index = 0;

            LocalizationManager.Instance.CurrentCulture = availableCultures[_index];
       }
    }
}
