using MudBlazor;

namespace ProMgt.Client.Infrastructure.Settings
{
    public class ThemeService
    {
        private bool _isDarkMode = false;
        private MudTheme _currentTheme = ProMgtTheme.DefaultTheme;
        public event Func<Task>? OnThemeChanged;
        public MudTheme CurrentTheme => _currentTheme;

        /// <summary>
        /// This checks if the dark mode is active or not
        /// </summary>
        /// <returns></returns>
        public bool IsDarkMode()
        {
            return _isDarkMode;
        }


        public async void ToggleDarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _currentTheme = _isDarkMode ? ProMgtTheme.DarkTheme : ProMgtTheme.DefaultTheme;
            if (OnThemeChanged != null)
            {
                await OnThemeChanged.Invoke();
            }
        }

        public string GetPrimaryColorHex(MudTheme theme)
        {
            string primaryHex;
            if (_isDarkMode)
            {
                primaryHex = theme.PaletteDark.Primary.ToString();
            }
            else
            {
                primaryHex = theme.PaletteLight.Primary.ToString();

            }
            return primaryHex;
        }
    }
}
