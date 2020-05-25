using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND.Views
{
    public class SettingViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        public SettingViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}