using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GlassGrid.ViewModels;

namespace GlassGrid.Views
{
    public partial class LoginWindow : Window
    {
        public event Action<string> UserLoggedIn;

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            UserLoggedIn?.Invoke("Guest");
            this.DialogResult = true;
            this.Close();
        }

        public LoginWindow(IEFUserService userService)
        {
            InitializeComponent();

            var vm = new LoginViewModel(userService);
            DataContext = vm;

         
            PasswordBox.PasswordChanged += (s, e) =>
            {
                vm.Password = PasswordBox.Password;
            };

            
            vm.UserLoggedIn += (username) =>
            {
                UserLoggedIn?.Invoke(username);
                this.DialogResult = true;
                this.Close();
            };
        }
    }





}
