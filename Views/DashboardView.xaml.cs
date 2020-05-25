using System.Windows.Controls;

namespace PAYMAP_BACKEND.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            WindowManager.SetDashboardView(this);
        }
    }
}