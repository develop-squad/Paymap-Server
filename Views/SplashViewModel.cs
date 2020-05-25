using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND.Views
{
    public class SplashViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        public SplashViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}