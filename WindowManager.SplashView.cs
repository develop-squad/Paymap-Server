using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using PAYMAP_BACKEND.Views;
using MahApps.Metro.Controls;

namespace PAYMAP_BACKEND
{
    partial class WindowManager
    {
        private static SplashView _splashView;
        
        private static ToggleButton _splashCreditPAYMAP;
        private static ToggleButton _splashCreditMJ;
        private static ToggleButton _splashCreditJY;
        private static ToggleButton _splashCreditSB;
        
        private static void InitializeSplashWindow()
        {
            if (_splashView == null) return;
            _splashCreditPAYMAP = (ToggleButton) _splashView.FindName("CreditPAYMAP");
            _splashCreditMJ = (ToggleButton) _splashView.FindName("CreditMJ");
            _splashCreditJY = (ToggleButton) _splashView.FindName("CreditJY");
            _splashCreditSB = (ToggleButton) _splashView.FindName("CreditSB");
            if (_splashCreditPAYMAP == null || _splashCreditMJ == null || _splashCreditJY == null || _splashCreditSB == null) return;

            _splashCreditPAYMAP.Click += (sender, args) => System.Diagnostics.Process.Start("https://github.com/pay-map");
            _splashCreditMJ.Click += (sender, args) => System.Diagnostics.Process.Start("https://github.com/MoonJuhan");
            _splashCreditJY.Click += (sender, args) => System.Diagnostics.Process.Start("https://github.com/LIMECAKE");
            _splashCreditSB.Click += (sender, args) => System.Diagnostics.Process.Start("https://github.com/seongbuming");
        }

        public static void SetSplashView(SplashView splashView)
        {
            _splashView = splashView;
            InitializeSplashWindow();
        }
    }
    
}