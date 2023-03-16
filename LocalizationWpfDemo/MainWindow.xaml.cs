using LocalizationService.Localization;
using System.Windows;

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

            // Setup a event handler that will update bindings for all controls on the MainWindow
            LocalizationManager.Instance.PropertyChanged += (o, args) => BindingsHelper.UpdateControlBindings(this);
        }
    }
}
