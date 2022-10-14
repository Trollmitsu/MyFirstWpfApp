using System.ComponentModel.Design;
using Microsoft.Azure.Devices.Client;
using System.Net.Http.Json;
using Microsoft.Azure.Devices.Shared;
using System.Data.SqlClient;
using Dapper;
using Microsoft.WindowsAzure.Storage.Blob;

DeviceClient deviceClient;
var interval = 1000;
var deviceId = "";
var device_connectionString = "HostName=KYH-IotHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=MxYmHj/BhWJpHWWje5ey4yP6DeYp+REfT77NJhdGnO8=";
var deviceName = "IntelliFan";
var deviceType = "Fan";
var location = "kitchen";

var connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\asus\\source\\repos\\NET21\\MyFirstWpfApp\\Device.ConsoleApp\\Data\\deviceDb.mdf;Integrated Security=True;Connect Timeout=30";
using var conn = new SqlConnection(connectionString);

deviceId = await conn.QueryFirstOrDefaultAsync<string>("SELECT DeviceId FROM Settings");
if (string.IsNullOrEmpty(deviceId))
{
    deviceId = Guid.NewGuid().ToString();
    
    await conn.ExecuteAsync("INSERT INTO Settings VALUES (@DeviceId, @ConnectionString, @DeviceName, @DeviceType, @Location)", new
    {
        DeviceId = deviceId,
        ConnectionString = device_connectionString,
        DeviceName = deviceName,
        DeviceType = deviceType,
        Location = location
    });
}

using var client = new HttpClient();
var result = await client.PostAsJsonAsync("http://localhost:7231/api/devices/connect", new { deviceId });
device_connectionString = await result.Content.ReadAsStringAsync();

deviceClient = DeviceClient.CreateFromConnectionString(device_connectionString, TransportType.Mqtt);
var twin = await deviceClient.GetTwinAsync();


var twinCollection = new TwinCollection();
twinCollection["deviceName"] = deviceName;
twinCollection["deviceType"] = deviceType;
twinCollection["location"] = location;

await deviceClient.UpdateReportedPropertiesAsync(twinCollection);

