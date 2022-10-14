using MyFirstWpfApp.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Azure.Devices;
using MyFirstWpfApp.Annotations;

namespace MyFirstWpfApp.MVVM.ViewModels
{
    internal class KitchenViewModel : ObservableObject
    {
        private DispatcherTimer timer;

        //ObserableCollection gör så att man refreshar sidan
        private ObservableCollection<DeviceItems> _deviceItems;
        private readonly RegistryManager _registryManager =
            RegistryManager.CreateFromConnectionString("HostName=KYH-IotHub.azure-devices.net;SharedAccessKeyName=SmartApp;SharedAccessKey=cybbE2SfAmGjfjUbY+6WhyzT31ixSQuFJrlQqNxgC1s=");

        public KitchenViewModel()
        {
            _deviceItems = new ObservableCollection<DeviceItems>();

            PopulateDeviceItemsAsync().ConfigureAwait(false);
            UpdateHumidity().ConfigureAwait(false);
            UpdateTemperature().ConfigureAwait(false);
            
            SetTimer(TimeSpan.FromSeconds(5));
        }
        public string Title { get; set; } = "Kitchen and Dining";

        [CanBeNull] private string _currentTemperature;

        public string CurrentTemperature
        {
            get => _currentTemperature!;
            set
            {
                _currentTemperature = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull] private string _humidity;

        public string Humidity
        {
            get => _humidity!;
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }



        public IEnumerable<DeviceItems> DeviceItems => _deviceItems;


        private static readonly Random random = new Random();

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = random.NextDouble();

            return minValue + (next * (minValue - maxValue));
        }

        private async Task UpdateTemperature()
        {
            var randomNumber = new Random();
            CurrentTemperature = $"{randomNumber.Next(20, 25).ToString()}°C";

        }

        private async Task UpdateHumidity()
        {
            var randomNumber = new Random();
            Humidity = $"{randomNumber.Next(40, 55).ToString()}%";

        }

        private void SetTimer(TimeSpan interval)
        {
            timer = new DispatcherTimer()
            {
                Interval = interval,
            };
            timer.Tick += new EventHandler(timer_tickAsync);
            timer.Start();
        }
        private async void timer_tickAsync(object sender, EventArgs e)
        {
            await PopulateDeviceItemsAsync();
            await DeleteDeviceAsync();
            await UpdateDeviceItemsAsync();
            await UpdateTemperature();
            await UpdateHumidity();


        }

        private async Task UpdateDeviceItemsAsync()
        {
            var removeList = new List<DeviceItems>();
            foreach (var item in _deviceItems)
            {
                var device = await _registryManager.GetDeviceAsync(item.DeviceId);
                if (device == null)
                {
                    removeList.Add(item);
                }
            }

            foreach (var item in removeList)
            {
                _deviceItems.Remove(item);
            }
                
        }

        private async Task DeleteDeviceAsync()
        {
            
        }

        private async Task PopulateDeviceItemsAsync()
        {
            //where properties.reported.location = 'Kitchen'

            var result = _registryManager.CreateQuery("select * from devices where properties.reported.location = 'kitchen'");

            if (result.HasMoreResults)
            {
                foreach (var twin in await result.GetNextAsTwinAsync())
                {
                    var _device = _deviceItems.FirstOrDefault(x => x.DeviceId == twin.DeviceId);

                    if (_device == null)
                    {
                        _device = new DeviceItems
                        {
                            DeviceId = twin.DeviceId
                        };

                        try { _device.DeviceName = twin.Properties.Reported["deviceName"] ?? _device.DeviceId; }
                        catch { _device.DeviceName = _device.DeviceId; }
                        try { _device.DeviceType = twin.Properties.Reported["deviceType"]; }
                        catch { }

                        switch (_device.DeviceType.ToLower())
                        {

                            case "fan":
                                _device.IconActive = "\uf72e";
                                _device.IconInActive = "\uf011";
                                _device.StateActive = "ON";
                                _device.StateInActive = "OFF";
                                break;
                            case "light":
                                _device.IconActive = "\uf0eb";
                                _device.IconInActive = "\uf011";
                                _device.StateActive = "ON";
                                _device.StateInActive = "OFF";
                                break;
                            default:
                                _device.IconActive = "\uf784";
                                _device.IconInActive = "\uf011";
                                _device.StateActive = "ENABLE";
                                _device.StateInActive = "DISABLE";
                                break;
                        }
                        _deviceItems.Add(_device);
                    }
                    else { }
                }
            }
            else
            {
                _deviceItems.Clear();
            }

        }
    }
}
