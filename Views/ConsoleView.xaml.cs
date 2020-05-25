using System.Windows.Controls;

namespace PAYMAP_BACKEND.Views
{
    public partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            InitializeComponent();
            WindowManager.SetConsoleView(this);
        }
    }
}