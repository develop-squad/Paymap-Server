using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND.Views
{
    public class CrawlerViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        public CrawlerViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}