using System.Text.Json;
using Bank_Holidays.Models;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Bank_Holidays.ViewModels
{
    public class MainViewViewModel : BindableObject
    {
        private const string API_ENDPOINT = "https://www.gov.uk/bank-holidays.json";

        #region Properties

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

        public string NextBankHolidayTitle
        {
            get => nextBankHolidayTitle;
            set
            {
                nextBankHolidayTitle = value;
                OnPropertyChanged(nameof(NextBankHolidayTitle));
            }
        }
        private string nextBankHolidayTitle = "Unknown";

        public ObservableCollection<Date> UpcomingBankHolidays { get; } = new ObservableCollection<Date>();

        #endregion

        public MainViewViewModel() { }

        public async void OnLoad()
        {
            Loading = true;

            API api = await GetAPIData();

            if (api != null)
            {
                DateTime nextDate = GetNextDate(api.englandandwales.events, out int index);
                NextBankHoliday = nextDate.ToString("MM MMMM yyyy");
                NextBankHolidayTitle = api.englandandwales.events[index].title;

                PopulateUpcomingBankHolidays(api.englandandwales.events);
            }
            else QuitWithError("Failed to get API data. Please connect to the internet and try again.");

            Loading = false;
        }

        private DateTime GetNextDate(List<Event> list, out int index)
        {
            CultureInfo cultureInfo = new CultureInfo("en-GB");
            DateTime nextBankHoliday = DateTime.MaxValue;
            int temp = 0;

            foreach (var bankHoliday in list)
            {
                if (DateTime.TryParseExact(bankHoliday.date, "yyyy-MM-dd", cultureInfo, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate > DateTime.Now && parsedDate < nextBankHoliday)
                    {
                        nextBankHoliday = parsedDate;
                        break;
                    }
                }
                temp++;
            }

            index = temp;
            return nextBankHoliday;
        }

        private void PopulateUpcomingBankHolidays(List<Event> list)
        {
            CultureInfo cultureInfo = new CultureInfo("en-GB");

            foreach (var bankHoliday in list)
            {
                if (DateTime.TryParseExact(bankHoliday.date, "yyyy-MM-dd", cultureInfo, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate > DateTime.Now && parsedDate.Year == DateTime.Now.Year)
                    {
                        UpcomingBankHolidays.Add(new Date()
                        {
                            DateString = parsedDate.ToString("dd MMMM"),
                            DayOfWeek = parsedDate.ToString("dddd"),
                            BankHolidayName = bankHoliday.title
                        });
                    }
                }
            }
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
            catch { return null; }
        }

        private async void QuitWithError(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"{message}", "OK");
            Application.Current.Quit();
        }
    }
}