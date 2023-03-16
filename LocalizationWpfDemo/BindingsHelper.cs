using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using LocalizationService.Localization;

namespace LocalizationWpfDemo;

public static class BindingsHelper
{

    /// <summary>
    /// Recursively updates binding for specified object and its child objects.  This method is meant to be 
    /// called, with a Window/UserControl/ContentControl etc as argument, when the current culture is changed
    /// </summary>
    /// <param name="obj">Some control that has a text- or Content binding expression that needs updating </param>
    public static void UpdateControlBindings(DependencyObject obj)
    {
        // When culture is changed the rendering of listviews, comboboxes and other controls that has a ItemSource needs to 
        // be invalidated/redrawn. It is not enough to react on OnPropertChanged events, as the actual content of the ItemsSource 
        // targets has not been changed - it is only the rendering of the content that needs refreshed.

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);

            // Handle ItemsSource view for controls like ListView, ComboBox, Ribbon etc
            if (child is ItemsControl itemsControl)
            {
                if (itemsControl.ItemsSource != null)
                {
                    var view = CollectionViewSource.GetDefaultView(itemsControl.ItemsSource);
                    view.Refresh();
                }
            }

            // Handle ComboBoxSelectedItem 
            if (child is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    var selectedItem = comboBox.SelectedItem;
                    comboBox.SelectedItem = null;
                    comboBox.SelectedItem = selectedItem;
                }
            }

            // List of control bindings that needs to be refreshed on culture change, handled here are TextBlocks and ContentControls (Labels, Button)
            if (child is TextBlock textBlock) { textBlock.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();}
            if (child is ContentControl ctrl) ctrl.GetBindingExpression(ContentControl.ContentProperty)?.UpdateTarget();
            // Expand as needed - I expect other controls need to refresh on Culture change - eg. maybe Calendar or DatePicker?

            // Recursive call to handle all child controls for current control
            UpdateControlBindings(child);
        }
    }
}