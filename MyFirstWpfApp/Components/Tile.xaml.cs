using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
using Microsoft.Azure.Devices;
using Microsoft.Data.SqlClient;
using MyFirstWpfApp.MVVM.Models;

namespace MyFirstWpfApp.Components
{

    
    public partial class Tile : UserControl
    {
        private readonly RegistryManager _registryManager =
            RegistryManager.CreateFromConnectionString("HostName=KYH-IotHub.azure-devices.net;SharedAccessKeyName=SmartApp;SharedAccessKey=cybbE2SfAmGjfjUbY+6WhyzT31ixSQuFJrlQqNxgC1s=");

        private readonly string _SqlConnection = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\asus\\source\\repos\\NET21\\MyFirstWpfApp\\Device.ConsoleApp\\Data\\deviceDb.mdf;Integrated Security=True;Connect Timeout=30";
        
        public Tile()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register("DeviceName", typeof(string), typeof(Tile));
        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public static readonly DependencyProperty DeviceTypeProperty =
            DependencyProperty.Register("DeviceType", typeof(string), typeof(Tile));
        public string DeviceType
        {
            get { return (string)GetValue(DeviceTypeProperty); }
            set { SetValue(DeviceTypeProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(Tile));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IconActiveProperty =
            DependencyProperty.Register("IconActive", typeof(string), typeof(Tile));
        public string IconActive
        {
            get { return (string)GetValue(IconActiveProperty); }
            set { SetValue(IconActiveProperty, value); }
        }

        public static readonly DependencyProperty IconInActiveProperty =
            DependencyProperty.Register("IconInActive", typeof(string), typeof(Tile));
        public string IconInActive
        {
            get { return (string)GetValue(IconInActiveProperty); }
            set { SetValue(IconInActiveProperty, value); }
        }

        public static readonly DependencyProperty StateActiveProperty =
            DependencyProperty.Register("StateActive", typeof(string), typeof(Tile));
        public string StateActive
        {
            get { return (string)GetValue(StateActiveProperty); }
            set { SetValue(StateActiveProperty, value); }
        }

        public static readonly DependencyProperty StateInActiveProperty =
            DependencyProperty.Register("StateInActive", typeof(string), typeof(Tile));
        public string StateInActive
        {
            get { return (string)GetValue(StateInActiveProperty); }
            set { SetValue(StateInActiveProperty, value); }
        }

       


        private async void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            var deviceItem = (DeviceItems)button!.DataContext;
            RemoveBtn.IsEnabled = false;
            await _registryManager.RemoveDeviceAsync(deviceItem.DeviceId);

            using IDbConnection Connection = new SqlConnection(_SqlConnection);
            await Connection.ExecuteAsync("Delete From Settings Where DeviceId = @DeviceId",
                new { DeviceId = deviceItem.DeviceId });
        }
    }
}
