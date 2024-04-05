using System.Text.Json;
using Bank_Holidays.Models;

namespace Bank_Holidays.ViewModels
{
    public class MainViewViewModel : BindableObject
    {
        private const string API_ENDPOINT = "https://www.gov.uk/bank-holidays.json";

        public string NextBankHoliday
        {
            get => nextBankHoliday;
            set
            {
                nextBankHoliday = value;
                OnPropertyChanged(nameof(NextBankHoliday));
            }
        }
        private string nextBankHoliday = "Unknown";

        public MainViewViewModel()
        {

        }

        private async Task<API> GetAPIData()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(API_ENDPOINT))
                    {
                        Stream json = await httpResponseMessage.Content.ReadAsStreamAsync();
                        return await JsonSerializer.DeserializeAsync<API>(json);
                    }
                }
            }
            catch (HttpRequestException exception) { QuitWithError("Ensure you have a valid internet connection and try again."); }
            catch (Exception exception) { QuitWithError("An unexpected error has occurred."); }

            return null;
        }

        private async void QuitWithError(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"{message}", "OK");
            Application.Current.Quit();
        }
    }
}