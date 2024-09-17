using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vamdrup_rundt.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            
        }
        [RelayCommand]
        private async Task OnSignOutClicked()
        {
            SecureStorage.Default.Remove("password");
            SecureStorage.Default.Remove("email");
            await Shell.Current.GoToAsync("///" + nameof(LoginnPage));
        }
    }
}
