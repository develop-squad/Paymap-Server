using System.Windows.Controls;

namespace PAYMAP_BACKEND.Views
{
    public partial class CrawlerView : UserControl
    {
        public CrawlerView()
        {
            InitializeComponent();
            WindowManager.SetCrawlerView(this);
        }
    }
}