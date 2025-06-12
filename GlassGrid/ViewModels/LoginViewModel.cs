using System;
using System.Windows.Input;
using GlassGrid.Models;
using GlassGrid.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _username;
    private string _password;
    private string _message;
    

    public event Action<string> UserLoggedIn;

    private readonly IEFUserService _userService;

    public LoginViewModel(IEFUserService userService)
    {
        _userService = userService;

       
        LoginCommand = new RelayCommand(Login, CanExecuteAuth);
        RegisterCommand = new RelayCommand(Register, CanExecuteAuth);
    }
    private bool _isLoggedIn = false;
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            _isLoggedIn = value;
            OnPropertyChanged(nameof(IsLoggedIn));
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();

                
                LoginCommand.RaiseCanExecuteChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged();

               
                LoginCommand.RaiseCanExecuteChanged();
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            if (_message != value)
            {
                _message = value;
                OnPropertyChanged();
            }
        }
    }

 
    public RelayCommand LoginCommand { get; }
    public RelayCommand RegisterCommand { get; }


    private bool CanExecuteAuth()
    {
        return !string.IsNullOrWhiteSpace(Username)
               && !string.IsNullOrWhiteSpace(Password);
    }

    private void Login()
    {
        if (_userService.ValidateUser(Username, Password))
        {
            Message = "Zalogowano pomyślnie!";
            UserLoggedIn?.Invoke(Username);

        }
        else
        {
            Message = "Nieprawidłowy login lub hasło.";
        }
    }

    private void Register()
    {
        if (_userService.IsUsernameAvailable(Username))
        {
            _userService.RegisterUser(Username, Password);
            Message = "Zarejestrowano pomyślnie!";
        }
        else
        {
            Message = "Użytkownik o takim loginie już istnieje.";
        }
    }
}
