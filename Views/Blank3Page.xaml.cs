namespace Vamdrup_rundt.Views;

public partial class Blank3Page : ContentPage
{
	public Blank3Page(Blank3ViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
