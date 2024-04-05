using System.Text.Json;
using Bank_Holidays.Models;
using System.Globalization;

namespace Bank_Holidays.ViewModels
{
    public class MainViewViewModel : BindableObject
    {
        private const string API_ENDPOINT = "https://www.gov.uk/bank-holidays.json";

        public bool Loading 
        {
            get => loading; 
            set
            {
                loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }
        private bool loading = true;

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

        public async void OnLoad()
        {
            Loading = true;

            API api = await GetAPIData();

            DateTime nextDate = GetNextDate(api.englandandwales.events);
            NextBankHoliday = nextDate.ToString("MM MMMM yyyy");

            Loading = false;
        }

        private DateTime GetNextDate(List<Event> list)
        {
            CultureInfo cultureInfo = new CultureInfo("en-GB");
            DateTime nextBankHoliday = DateTime.MaxValue;

            Parallel.ForEach(list, bankHoliday =>
            {
                if (DateTime.TryParseExact(bankHoliday.date, "yyyy-MM-dd", cultureInfo, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate > DateTime.Now && parsedDate < nextBankHoliday)
                    {
                        nextBankHoliday = parsedDate;
                    }
                }
            });

            return nextBankHoliday;
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
            catch (HttpRequestException) { QuitWithError("Ensure you have a valid internet connection and try again."); }
            catch (Exception) { QuitWithError("An unexpected error has occurred."); }

            return null;
        }

        private async void QuitWithError(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"{message}", "OK");
            Application.Current.Quit();
        }
    }
}