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
using GlassGrid.Models;
using GlassGrid.ViewModels;

namespace GlassGrid.Views
{
   
public partial class ChangeUserData : Window
    {

        public ChangeUserDataViewModel ViewModel { get; }
        public ChangeUserData(IEFUserService userService)
        {
            InitializeComponent();

            ViewModel = new ChangeUserDataViewModel(userService);
            DataContext = ViewModel;
        }
    }
}
