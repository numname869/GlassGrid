using System.ComponentModel;

public class UserSession : INotifyPropertyChanged
{
    private static UserSession _instance = new UserSession();
    public static UserSession Instance => _instance;

    private string _username = "Guest";
    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }
    }

    public bool IsLoggedIn => !string.IsNullOrEmpty(Username) && Username != "Guest";

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
