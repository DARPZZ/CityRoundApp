using Vamdrup_rundt.Models;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels;

public partial class LoginnViewModel : BaseViewModel
{
    [ObservableProperty]
    private string email;
    [ObservableProperty]
    private string password;
    [ObservableProperty]
    private string isChecked;

    private readonly UserService _userService;
    public LoginnViewModel(UserService userService)
    {
        _userService = userService;
        LogInAutomatic();
    }

    [RelayCommand]
    private async Task OnSignUpClicked()
    {
        await Shell.Current.GoToAsync("///" + nameof(SignupPage));
    }

    [RelayCommand]
    private async Task OnSingInClicked()
    {
        var user = new UserModel
        {
            Email = Email,
            Password = Password
        };
       
        bool isCorrectCredentials = await _userService.LogUserInAsync(user);
        if (isCorrectCredentials)
        {
            if (IsChecked != null)
            {
                await SecureStorage.SetAsync("email", Email);
                await SecureStorage.SetAsync("password", Password);
            }

            try
            {
                await SecureStorage.Default.SetAsync("email", user.Email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
            await GoToMainPage();

        }
        else
        {
            Debug.WriteLine("Not correct information");
        }

    }

    private async Task LogInAutomatic()
    {
        string? email = await SecureStorage.GetAsync("email");
        string? password = await SecureStorage.GetAsync("password");
        if (email != null && password != null)
        {

            var user = new UserModel
            {
                Email = email,
                Password = password
            };

            bool isCorrectCredentials = await _userService.LogUserInAsync(user);
            if (isCorrectCredentials)
            {
                try
                {
                    await SecureStorage.Default.SetAsync("email", user.Email);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                }

                await GoToMainPage();
               
            }
        }
    }
    public async Task GoToMainPage()
    {
        await Shell.Current.GoToAsync("///" + nameof(MainPage));
    }
}
