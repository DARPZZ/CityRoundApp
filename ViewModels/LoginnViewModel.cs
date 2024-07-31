using Vamdrup_rundt.Models;
using Vamdrup_rundt.Services;

namespace Vamdrup_rundt.ViewModels;

public partial class LoginnViewModel : BaseViewModel
{
    [ObservableProperty]
    private string email;
    [ObservableProperty]
    private string password;
    private readonly UserService _userService;
    public LoginnViewModel(UserService userService)
    {
        _userService = userService;
    }


    [RelayCommand]
    private async Task OnSingInClicked()
    {
        var user = new UserModel
        {
            Email = Email,
            Paswword = Password
        };
        Debug.WriteLine(user.Email);
        Debug.WriteLine(user.Paswword);
        bool isCorrectCredentials = await _userService.LogUserInAsync(user);
        if (isCorrectCredentials)
        {
            Debug.WriteLine("Correct info");

        }
        else
        {
            Debug.WriteLine("Not correct information");
        }

    }
}
