using Vamdrup_rundt.Services;

namespace Vamdrup_rundt;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<MainViewModel>();

		builder.Services.AddSingleton<MainPage>();

		builder.Services.AddSingleton<SignupViewModel>();

		builder.Services.AddSingleton<SignupPage>();

		builder.Services.AddSingleton<LoginnViewModel>();

		builder.Services.AddSingleton<LoginnPage>();

		builder.Services.AddSingleton<ProgressViewModel>();

		builder.Services.AddSingleton<ProgressPage>();

		builder.Services.AddSingleton<Blank3ViewModel>();

		builder.Services.AddSingleton<Blank3Page>();
        builder.Services.AddSingleton<UserService>();

        return builder.Build();
	}
}
