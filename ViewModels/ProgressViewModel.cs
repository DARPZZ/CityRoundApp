using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels
{
    public partial class ProgressViewModel : BaseViewModel
    {
        private ObservableCollection<string> allStreets = new ObservableCollection<string>();
        [ObservableProperty]
        private IEnumerable<string> streets;
        [ObservableProperty]
        private string by;
        [ObservableProperty]
        private string searchWord;

        [ObservableProperty]
        private string postnummer;
        private VisitedStreetsDataService _streetsDataService = new VisitedStreetsDataService();
        private string emailLogin;

        public ProgressViewModel()
        {
            streets = new ObservableCollection<string>();
            InitializeDataAsync();
            MessagingCenter.Subscribe<ProgressPage>(this, "PageAppearing", async (sender) =>
            {
                await InitializeDataAsync();
            });
            MessagingCenter.Subscribe<ProgressPage>(this, "PageAppearing", async (sender) =>
            {
                await InitializeDataAsync();
            });
        }

        public async Task InitializeDataAsync()
        {
            allStreets.Clear();
            await GetUserEmailFromLogin();

            if (!string.IsNullOrEmpty(emailLogin))
            {
                var streetData = await _streetsDataService.GetAllUsersActiveStreets(emailLogin);

                if (streetData != null)
                {
                    foreach (var street in streetData)
                    {
                        allStreets.Add("By: " + street.by + "\t Postnummer: " + street.postnummer);
                    }
                    Streets = allStreets.Distinct().ToList();
                }
            }
        }

        private async Task GetUserEmailFromLogin()
        {
            emailLogin = await SecureStorage.Default.GetAsync("email");
        }

        partial void OnSearchWordChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Streets = allStreets.Distinct();
            }
            else
            {
                Streets = allStreets.Where(s => s.Contains(value, StringComparison.OrdinalIgnoreCase)).Distinct().ToList();
            }
        }
        [RelayCommand]
        private void ItemSelected(object item)
        {
            Debug.WriteLine(item);
        }



    }
}
