namespace StoreManager.Webapp.Client.Services;

public class NavState
{
    private bool _isVisible = false;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                NotifyStateChanged();
            }
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
