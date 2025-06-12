using System;
using System.Windows;
using System.Windows.Input;
using GlassGrid.Models;


namespace GlassGrid.ViewModels
{
    public class ChangeUserDataViewModel : BaseViewModel
    {
        private readonly IEFUserService _userService;

        public ChangeUserDataViewModel(IEFUserService userService)
        {
            _userService = userService;

            ChangeLoginCommand = new RelayCommand(ChangeLogin);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
        }

        public string NewUsername { get; set; } = "";
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";

        public ICommand ChangeLoginCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        private void ChangeLogin()
        {
            if (string.IsNullOrWhiteSpace(NewUsername))
            {
                MessageBox.Show("Nowy login nie może być pusty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var oldUsername = UserSession.Instance.Username;

            if (!_userService.UpdateUsername(oldUsername, NewUsername))
            {
                MessageBox.Show("Zmiana loginu nie powiodła się. Użytkownik może już istnieć.", "Błąd");
                return;
            }

            // Aktualizujemy nazwę użytkownika w sesji
            UserSession.Instance.Username = NewUsername;

            MessageBox.Show("Login zmieniony pomyślnie!", "Sukces");
        }

        private void ChangePassword()
        {
            var currentUsername = UserSession.Instance.Username;

            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Hasła nie mogą być puste.", "Błąd");
                return;
            }

            if (!_userService.UpdatePassword(currentUsername, OldPassword, NewPassword))
            {
                MessageBox.Show("Błąd przy zmianie hasła. Stare hasło może być nieprawidłowe.", "Błąd");
                return;
            }

            MessageBox.Show("Hasło zmienione pomyślnie!", "Sukces");
        }
    }
}
