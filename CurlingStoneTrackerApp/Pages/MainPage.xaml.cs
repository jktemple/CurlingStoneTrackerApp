using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace CurlingStoneTrackerApp
{
    public partial class MainPage : ContentPage
    {

        private readonly IAdapter _bluetoothAdapter;
        private readonly List<IDevice> _gattDevices = new List<IDevice>();

        public MainPage()
        {
            InitializeComponent();

            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            _bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
            {
                if (foundBleDevice != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                    _gattDevices.Add(foundBleDevice.Device);
            };
        }

        private async Task<bool> PermissionsGrantedAsync()      // Function to make sure that all the appropriate approvals are in place
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanBtn.IsEnabled = false);
            foundBleDevicesListView.ItemsSource = null;

            if (!await PermissionsGrantedAsync()) {
                await DisplayAlert("Permission required", "Application needs location permission", "OK");
                IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanBtn.IsEnabled = true);
                return;
            }

            _gattDevices.Clear();

            if (!_bluetoothAdapter.IsScanning) { 
                await _bluetoothAdapter.StartScanningForDevicesAsync();
            }

            foreach (var device in _bluetoothAdapter.ConnectedDevices) { 
                _gattDevices.Add(device);
            }

            foundBleDevicesListView.ItemsSource = _gattDevices.ToArray();
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanBtn.IsEnabled = true);
        }

        private async void OnFoundBluetoothDevicesListViewItemClicked(object sender, ItemTappedEventArgs e) 
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanBtn.IsEnabled = false);        // Switch on IsBusy indicator
            IDevice selectedItem = e.Item as IDevice;                                                       // The item selected is an IDevice (detected BLE device). Therefore we have to cast the selected item to an IDevice

            if (selectedItem.State == DeviceState.Connected)                                                // Check first if we are already connected to the BLE Device 
            {
                await Navigation.PushAsync(new BLEServicePage(selectedItem));                                   // Navigate to the Services Page to show the services of the selected BLE Device
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await _bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);          // if we are not connected, then try to connect to the BLE Device selected
                    await Navigation.PushAsync(new BLEServicePage(selectedItem));                               // Navigate to the Services Page to show the services of the selected BLE Device
                }
                catch
                {
                    await DisplayAlert("Error connecting", $"Error connecting to BLE device: {selectedItem.Name ?? "N/A"}", "Retry");       // give an error message if it is not possible to connect
                }
            }

            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanBtn.IsEnabled = true);         // switch off the "Isbusy" indicator
        }

    }

}
