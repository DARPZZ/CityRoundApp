using Vamdrup_rundt.Models;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels;

public partial class SignupViewModel : BaseViewModel
{
    [ObservableProperty]
    private string email;
    [ObservableProperty]
    private string password;
    private UserService userService;
    public SignupViewModel()
    {
        userService = new UserService();
       
    }
    [RelayCommand]
    private async Task OnSingUpClicked()
    {
        var user = new UserModel
        {
            Email = Email,
            Password = Password
        };

        bool isCorrectCredentials = await userService.CreateUserAsync(user);
        if (isCorrectCredentials)
        {
            Debug.WriteLine("Correct information");
            await Shell.Current.GoToAsync("///" + nameof(LoginnPage));
        }
    }
    [RelayCommand]
    private async Task OnBackClicked()
    {
        await Shell.Current.GoToAsync("///" + nameof(LoginnPage));
    }
}
