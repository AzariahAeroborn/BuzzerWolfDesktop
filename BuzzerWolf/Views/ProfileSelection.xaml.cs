using BuzzerWolf.ViewModels;
using System.Windows.Controls;

namespace BuzzerWolf.Views
{
    /// <summary>
    /// Interaction logic for BBAPI.xaml
    /// </summary>
    public partial class ProfileSelection : UserControl
    {
        public ProfileSelection()
        {
            InitializeComponent();
        }

        public ProfileSelection(ProfileSelectionViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
