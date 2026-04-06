namespace TesteImpressao
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnEntrarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(BluetoothDevicesPage));
        }
    }
}
