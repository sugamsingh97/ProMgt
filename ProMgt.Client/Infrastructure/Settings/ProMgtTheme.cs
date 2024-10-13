using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace ProMgt.Client.Infrastructure.Settings
{
    public class ProMgtTheme
    {
        private static Typography DefaultTypography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new H1()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "6rem",
                FontWeight = 300,
                LineHeight = 1.167,
                LetterSpacing = "-.01562em"
            },
            H2 = new H2()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3.75rem",
                FontWeight = 300,
                LineHeight = 1.2,
                LetterSpacing = "-.00833em"
            },
            H3 = new H3()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H4 = new H4()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "2.125rem",
                FontWeight = 400,
                LineHeight = 1.235,
                LetterSpacing = ".00735em"
            },
            H5 = new H5()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 400,
                LineHeight = 1.334,
                LetterSpacing = "0"
            },
            H6 = new H6()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = 400,
                LineHeight = 1.6,
                LetterSpacing = ".0075em"
            },
            Button = new MudBlazor.Button()
            {
                FontFamily = new[] { "Montserrat", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = ".02857em"
            },
            Body1 = new Body1()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = ".00938em"
            },
            Body2 = new Body2()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            Caption = new Caption()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = ".03333em"
            },
            Subtitle2 = new Subtitle2()
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 600,
                LineHeight = 1.57,
                LetterSpacing = ".00714em"
            }
        };

        private static LayoutProperties DefaultLayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "3px",
            AppbarHeight = "50px"
        };
        public static ZIndex ZIndex = new ZIndex()
        {
            Drawer = 1301,
            Popover = 1303,
            AppBar = 1300,
            Dialog = 1400,
            Snackbar = 1500,
            Tooltip = 1600
        }; 
        public static MudTheme DefaultTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Green.Accent4,
                PrimaryContrastText = "#FFFFFF",

                Secondary = Colors.DeepOrange.Darken1,
                SecondaryContrastText = "#FFFFFF",

                Tertiary = "#ffffff",
                TertiaryContrastText = "#ffffff",


                Info = "#007bc3",
                Success = "#3ea44e",
                Warning = "#ff9800",
                Error = "#d92800",

                Dark = "#404040",
                DarkLighten = "#ebebeb",
                TextDisabled = "#8f8f8f",
                Background = "#ffffff",

                TextPrimary = "#404040",
                TextSecondary = "#666666",

                AppbarBackground = Colors.Gray.Darken3,
                AppbarText = "#ffffff",

                //DrawerBackground = Colors.Gray.Darken3,
                //DrawerText = "#ffffff",

                //Primary = Colors.DeepPurple.Accent3,
                //Secondary = Colors.Green.Default,
                //Tertiary = Colors.Gray.Darken4,
                //AppbarBackground = Colors.Gray.Lighten5,
                //Background = Colors.Gray.Lighten5,                
                //DrawerBackground = Colors.Gray.Lighten5,
                //DrawerText = "rgba(0,0,0, 0.7)",
                //Success = "#007E33",
                //Surface = Colors.Gray.Lighten5,
                //AppbarText = "rgba(255,255,255, 0.70)",
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        };

        public static MudTheme DarkTheme = new MudTheme()
        {
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.Green.Accent4,
                Secondary = Colors.DeepOrange.Darken1,
                Tertiary = "#1E1F21",

                Info = "#007bc3",
                Success = "#3ea44e",
                Warning = "#ff9800",
                Error = "#d92800",


                Surface = "#1E1F21",

                Dark = "#404040",
                DarkLighten = "#ebebeb",
                TextDisabled = "#d0d0d0",

                Background = "#1E1F21",

                TextPrimary = "#ffffff",
                TextSecondary = "#e0e0e0",

                //AppbarBackground = Colors.Gray.Darken3,
                //AppbarText = "#ffffff",

                //DrawerBackground = Colors.Gray.Darken3,
                //DrawerText = "#ffffff",

                //Primary = Colors.DeepPurple.Accent3,
                //Secondary = Colors.Green.Default,               
                //Tertiary = Colors.Gray.Darken4,
                //Success = "#007E33",
                //Black = "#27272f",
                //Background = "#32333d",
                //BackgroundGray = "#27272f",
                //Surface = Colors.Gray.Darken3,
                //DrawerBackground = "#27272f",
                //DrawerText = "rgba(255,255,255, 0.50)",
                //AppbarBackground = Colors.Gray.Darken3,
                //AppbarText = "rgba(255,255,255, 0.70)",
                //TextPrimary = "rgba(255,255,255, 0.70)",
                //TextSecondary = "rgba(255,255,255, 0.50)",
                //ActionDefault = "#adadb1",
                //ActionDisabled = "rgba(255,255,255, 0.26)",
                //ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                //DrawerIcon = "rgba(255,255,255, 0.50)"
            },
            Typography = DefaultTypography,
            LayoutProperties = DefaultLayoutProperties
        };
    }
}
