namespace Vamdrup_rundt.Views;

public partial class LoginnPage : ContentPage
{
	public LoginnPage(LoginnViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
