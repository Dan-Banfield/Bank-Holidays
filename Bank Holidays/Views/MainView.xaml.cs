using Bank_Holidays.ViewModels;

namespace Bank_Holidays.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewViewModel mainViewViewModel)
                mainViewViewModel.OnLoad();
        }
    }
}