using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Cashlane.Views;

public partial class FilterPanelView : UserControl
{
    private bool _isOpen = true;

    public FilterPanelView()
    {
        InitializeComponent();
    }

    private void TogglePanel(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isOpen = !_isOpen;
        FilterBody.Visibility = _isOpen ? Visibility.Visible : Visibility.Collapsed;

        var angle = _isOpen ? 180 : 0;
        var animation = new DoubleAnimation(angle, TimeSpan.FromMilliseconds(200));
        ToggleIcon.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
    }
}
