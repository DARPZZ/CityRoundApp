namespace Vamdrup_rundt.Views;

public partial class ProgressPage : ContentPage
{
	public ProgressPage(ProgressViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
