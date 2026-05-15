namespace Cashlane.ViewModels;

public class ToastViewModel : ViewModelBase
{
    private string _message = "";
    private bool _isVisible;

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
        Message = message;
        IsVisible = true;
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = System.TimeSpan.FromMilliseconds(2500)
        };
        timer.Tick += (_, _) =>
        {
            IsVisible = false;
            timer.Stop();
        };
        timer.Start();
    }
}
