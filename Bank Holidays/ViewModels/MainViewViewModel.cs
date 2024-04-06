using System.Text.Json;
using Bank_Holidays.Models;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Bank_Holidays.ViewModels
{
    public class MainViewViewModel : BindableObject
    {
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

        public string NextBankHolidayDateString
        {
            get => nextBankHolidayDateString;
            set
            {
                nextBankHolidayDateString = value;
                OnPropertyChanged(nameof(NextBankHolidayDateString));
            }
        }
        private string nextBankHolidayDateString = "Unknown";

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

        #region Methods

        public async void OnLoad()
        {
            Loading = true;

            API api = await GetAPIData();

            if (api != null)
            {
                UpdateProperties(api);
                Loading = false;

                return;
            }
            QuitWithError("Failed to get API data. Please connect to the internet and try again.");
        }

        private void UpdateProperties(API api)
        {
            DateTime nextDate = GetNextDate(api.englandandwales.events, out int index);
            NextBankHolidayDateString = nextDate.ToString("dd MMMM yyyy");
            NextBankHolidayTitle = api.englandandwales.events[index].title;

            PopulateUpcomingBankHolidays(api.englandandwales.events);
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
                    if (parsedDate > DateTime.Now && parsedDate.Year == DateTime.Now.Year)
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

        private async void QuitWithError(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"{message}", "OK");
            Application.Current.Quit();
        }

        #region API Methods

        private async Task<API> GetAPIData()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(Constants.API_ENDPOINT))
                    {
                        Stream json = await httpResponseMessage.Content.ReadAsStreamAsync();
                        return await JsonSerializer.DeserializeAsync<API>(json);
                    }
                }
            }
            catch { return null; }
        }

        #endregion

        #endregion
    }
}