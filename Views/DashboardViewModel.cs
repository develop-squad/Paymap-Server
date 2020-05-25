using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND.Views
{
    public class DashboardViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        public DashboardViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}