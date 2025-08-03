using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vamdrup_rundt.Models;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels
{
    public partial class ProgressViewModel : BaseViewModel
    {
        
        private ObservableCollection<string> allStreets = new ObservableCollection<string>();
        [ObservableProperty]
        private IEnumerable<string> streets;
        [ObservableProperty]
        private IEnumerable<string> streetsNotCompleted;
        [ObservableProperty]
        private string by;
        [ObservableProperty]
        private string searchWord;
        [ObservableProperty]
        private string procentageResult;
        List<VisitedStreetsModel> completedStreets = new List<VisitedStreetsModel>();
        List<StreetModel2> allStreetsList = new List<StreetModel2>();
        [ObservableProperty]
        private string postnummer;
        private VisitedStreetsDataService _visitedStreetsDataService = new VisitedStreetsDataService();
        private StreetDataService _streetDataServerice =  new StreetDataService();
        private string emailLogin;

        public ProgressViewModel()
        {
            streetsNotCompleted = new ObservableCollection<string>();
            streets = new ObservableCollection<string>();
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
                var streetData = await _visitedStreetsDataService.GetAllUsersActiveStreets(emailLogin);

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
        private async Task ItemSelected(string item)
        {
            completedStreets.Clear();
            var spilttedItem = item.Split(" ");
            string postNummer = spilttedItem[3];
            int intPostnummer = int.Parse(spilttedItem[3]);
            await GetAllStreetsInCity(intPostnummer);
            await GetStreetsInCityCompleted(intPostnummer);
            CalculateProcentage();
            GetMissingStreets();

        }
        private async Task GetStreetsInCityCompleted(int post)
        {
            
            var streetData = await _visitedStreetsDataService.GetVisitedStreetByID(emailLogin,post);

            if (streetData != null)
            {
                foreach (var street in streetData)
                {
                    var ko = new VisitedStreetsModel
                    {
                        StreetName = street.StreetName
                    };
                    completedStreets.Add(ko);
                    
                }
               

            }
        }
        private async Task GetAllStreetsInCity(int post)
        {

            var streetData = await _streetDataServerice.GetAllStreetsInCity(post);

            if (streetData != null)
            {
                foreach (var street in streetData)
                {
                    var ko = new StreetModel2
                    {
                        id = street.id,
                        name = street.name,
                        postNummer = street.postNummer,
                    };
                    allStreetsList.Add(ko);

                }
            }
        }
        private void CalculateProcentage()
        {
            double allStreetCount = allStreetsList.Count();
            double allCompletedStreets = completedStreets.Count();
            double procentageCompleted = (allCompletedStreets / allStreetCount)*100;
            procentageCompleted = Math.Round(procentageCompleted, 2);
            ProcentageResult = $"You have completed {procentageCompleted}% for this city ";
        }
        private void GetMissingStreets()
        {
            var missingStreets = allStreetsList
                .Where(street => !completedStreets.Any(completed => completed.StreetName == street.name))
                .OrderBy(s => s.name)
                .ToList();
            StreetsNotCompleted = missingStreets.Select(street => street.name).ToList();
        }



    }
}
