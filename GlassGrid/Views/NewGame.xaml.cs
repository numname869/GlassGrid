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
   
    public partial class NewGame : Window
    {
        public NewGame()
        {
            InitializeComponent();
            var app = (App)Application.Current;
            this.DataContext = app.GamePlayVM;
        }

       
        
    }
}
