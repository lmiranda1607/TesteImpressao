using System.Collections.ObjectModel;

#if ANDROID
using Android.Bluetooth;
#endif

namespace TesteImpressao;

public partial class BluetoothDevicesPage : ContentPage
{
    private readonly ObservableCollection<BluetoothDeviceItem> _devices = new();

    public BluetoothDevicesPage()
    {
        InitializeComponent();
        DevicesPicker.ItemsSource = _devices;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDevicesAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDevicesAsync();
    }

    private async void OnConnectAndPrintClicked(object sender, EventArgs e)
    {
        if (DevicesPicker.SelectedItem is not BluetoothDeviceItem selected)
        {
            await DisplayAlert("Atenção", "Selecione um dispositivo primeiro.", "OK");
            return;
        }

        StatusLabel.Text = $"Conectando em {selected.Display}...";

        // TODO: integrar SDK/protocolo ESC/POS da Bixolon para envio real da etiqueta.
        await Task.Delay(600);

        var etiqueta = "Etiqueta teste: deu certo";
        StatusLabel.Text = $"Conectado em {selected.Display}. Enviando etiqueta...";
        await Task.Delay(600);
        StatusLabel.Text = "Etiqueta de teste enviada com sucesso (simulação).";

        await DisplayAlert(
            "Impressão",
            $"Dispositivo: {selected.Display}\nConteúdo: {etiqueta}",
            "OK");
    }

    private async Task LoadDevicesAsync()
    {
        try
        {
            var discovered = await Task.Run(GetBluetoothDevices);

            _devices.Clear();
            foreach (var item in discovered)
            {
                _devices.Add(item);
            }

            StatusLabel.Text = _devices.Count == 0
                ? "Nenhum dispositivo Bluetooth identificado."
                : $"{_devices.Count} dispositivo(s) identificado(s).";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Erro ao listar dispositivos: {ex.Message}";
        }
    }

    private static List<BluetoothDeviceItem> GetBluetoothDevices()
    {
#if ANDROID
        var result = new List<BluetoothDeviceItem>();
        var adapter = BluetoothAdapter.DefaultAdapter;

        if (adapter is null)
        {
            return result;
        }

        var bondedDevices = adapter.BondedDevices;
        if (bondedDevices is null)
        {
            return result;
        }

        foreach (var device in bondedDevices)
        {
            var name = string.IsNullOrWhiteSpace(device.Name) ? "Sem nome" : device.Name;
            result.Add(new BluetoothDeviceItem(name, device.Address));
        }

        return result.OrderBy(x => x.Name).ToList();
#else
        return
        [
            new BluetoothDeviceItem("Bixolon SPP-L310iK5/BEG", "Simulado")
        ];
#endif
    }

    private sealed class BluetoothDeviceItem(string name, string address)
    {
        public string Name { get; } = name;
        public string Address { get; } = address;
        public string Display => $"{Name} ({Address})";
    }
}
