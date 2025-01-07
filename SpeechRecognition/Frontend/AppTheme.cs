using System.Windows;

namespace Interface
{
    public static class AppTheme
    {
        private static bool _currentThemeIsLight;
        private static Uri _lightTheme = new("Themes/Light.xaml", UriKind.Relative);
        private static Uri _darkTheme = new("Themes/Dark.xaml", UriKind.Relative);

        public static void SwitchTheme()
        {
            ResourceDictionary theme;

            if (!_currentThemeIsLight)
            {
                theme = new ResourceDictionary() { Source = _lightTheme };
                _currentThemeIsLight = true;
            }
            else
            {
                theme = new ResourceDictionary() { Source = _darkTheme };
                _currentThemeIsLight = false;
            }

            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(theme);
        }
    }
}
