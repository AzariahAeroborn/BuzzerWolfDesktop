using BuzzerWolf.ViewModels;
using System.Windows;

namespace BuzzerWolf.Views
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public LoginDialog(LoginDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
