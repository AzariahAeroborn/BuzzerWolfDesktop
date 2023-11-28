using BuzzerWolf.ViewModels;
using System.Windows.Controls;

namespace BuzzerWolf.Views
{
    /// <summary>
    /// Interaction logic for BBAPI.xaml
    /// </summary>
    public partial class BBAPILogin : UserControl
    {
        public BBAPILogin()
        {
            InitializeComponent();
        }

        public BBAPILogin(LoginViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
