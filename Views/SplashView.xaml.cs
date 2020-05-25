using System.Windows.Controls;

namespace PAYMAP_BACKEND.Views
{
    public partial class SplashView : UserControl
    {
        public SplashView()
        {
            InitializeComponent();
            WindowManager.SetSplashView(this);
        }
    }
}