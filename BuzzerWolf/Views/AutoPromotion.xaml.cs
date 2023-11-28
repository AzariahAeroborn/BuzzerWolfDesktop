using BuzzerWolf.ViewModels;
using System.Windows.Controls;

namespace BuzzerWolf.Views
{
    /// <summary>
    /// Interaction logic for AutoPromotion.xaml
    /// </summary>
    public partial class AutoPromotion : UserControl
    {
        public AutoPromotion()
        {
            InitializeComponent();
        }

        public AutoPromotion(AutoPromotionViewModel vm)
        {
            DataContext = vm;
        }
    }
}
