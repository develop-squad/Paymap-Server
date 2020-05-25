using PAYMAP_BACKEND.Utils;

namespace PAYMAP_BACKEND.Views
{
    public class ConsoleViewModel : PropertyChangedViewModel
    {
        private readonly PropertyChangedViewModel _mainViewModel;

        public ConsoleViewModel(PropertyChangedViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
    }
}