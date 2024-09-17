namespace Vamdrup_rundt.Views
{
    public partial class ProgressPage : ContentPage
    {
        private ProgressViewModel ViewModel => BindingContext as ProgressViewModel;

        public ProgressPage(ProgressViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, "PageAppearing");
        }
    }
}
