namespace Bank_Holidays
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Force dark mode on Android.
            UserAppTheme = AppTheme.Dark;

            MainPage = new AppShell();
        }
    }
}