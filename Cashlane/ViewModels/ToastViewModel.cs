namespace Cashlane.ViewModels;

public class ToastViewModel : ViewModelBase
{
    private string _message = "";
    private bool _isVisible;
    private System.Windows.Threading.DispatcherTimer? _timer;

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public void Show(string message)
    {
        _timer?.Stop();

        Message = message;
        IsVisible = true;

        _timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = System.TimeSpan.FromMilliseconds(2500)
        };
        _timer.Tick += (_, _) =>
        {
            IsVisible = false;
            _timer.Stop();
            _timer = null;
        };
        _timer.Start();
    }
}
