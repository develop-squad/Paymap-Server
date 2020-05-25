using System.Windows.Controls;

namespace PAYMAP_BACKEND.Views
{
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
            WindowManager.SetSettingView(this);
        }
    }
}