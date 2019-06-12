using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Sockets;

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        
        BluetoothLEAdvertisementWatcher watcher;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.Received += OnAdvertisementReceived;
            watcher.Start();
        }

        private async void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs advertisementArg)
        {
            //if (advertisementArg.Advertisement.LocalName != "MyIPhone")
               // return;
            //sender.Stop();
            Guid service_guid   = Guid.Parse("d4574b14-b3e9-49a4-b6bf-aba1d8097343");
            Guid character_guid = Guid.Parse("c32e093a-9c7d-42c4-8a8b-2652fb97750c");

            //Debug.WriteLine("uuid is " + guid.ToString() + " address is " + advertisementArg.BluetoothAddress + " name is " + advertisementArg.Advertisement.LocalName);                
            var uuids = advertisementArg.Advertisement.ServiceUuids;
            foreach (Guid uuid in uuids)
            {
                Debug.WriteLine("------------------------------------\nUUID: " + uuid.ToString());
            }
            Debug.WriteLine("\nName is " + advertisementArg.Advertisement.LocalName);            
            Debug.WriteLine("\nULONG BTH ADD: " + advertisementArg.BluetoothAddress);

            var address = advertisementArg.BluetoothAddress;
            var add = string.Format("0x{0:X}", address);  // converted Ulong into string              
            Debug.WriteLine("\nHEX BTH ADD: " + add);

            var manufacturerSections = advertisementArg.Advertisement.ManufacturerData;
            if (manufacturerSections.Count > 0)
            {
                var manufacturerData = manufacturerSections[0];
                var data = new byte[manufacturerData.Data.Length];
                using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                {
                    reader.ReadBytes(data);
                    string utfStringg = Encoding.UTF8.GetString(data, 0, data.Length);
                    //string utfStringg = Encoding.Unicode.GetString(data, 0, data.Length);
                    //Debug.WriteLine("\nMANUFACTURE DATA " + utfStringg + " BYTE Len: " + data.Length + " String Len: " + utfStringg.Length);

                }
            }

            IList<BluetoothLEAdvertisementDataSection> dataSection = advertisementArg.Advertisement.DataSections;

            foreach (BluetoothLEAdvertisementDataSection ad in dataSection)
            {
                if (ad.Data.Length > 0)
                {
                    DataReader dataReader = DataReader.FromBuffer(ad.Data);
                    byte[] bytes = new byte[ad.Data.Length];

                    dataReader.ReadBytes(bytes);

                    if (bytes.Length > 0)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        //Debug.WriteLine("\nDEVICE DATA " + data + "\n");


                        //if(data.Contains("Nitika") && (advertisementArg.BluetoothAddress == 106306580643971))
                        //if(data.Contains("MyIPhone"))    // case sensitive
                        if(data.Contains("Nitika"))    // case sensitive
                        //if (true)    // case sensitive                        
                        {
                            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(advertisementArg.BluetoothAddress);

                            try
                            {
                                var dd = bluetoothLeDevice.DeviceInformation.Pairing;   // commented for other devices, since u cant pair them
                            }
                            catch(Exception e)
                            {
                                Debug.WriteLine(e.ToString());
                            }
                            
                            //await dd.PairAsync();   // not using
                            if (bluetoothLeDevice != null)
                            {
                               // var dev_ser = await bluetoothLeDevice.RequestAccessAsync();     // commented for iphone

                                //Now that you have a BluetoothLEDevice object, the next step is to discover what data the device exposes.                                                                  
                                GattDeviceServicesResult resultDeviceService = await bluetoothLeDevice.GetGattServicesForUuidAsync(service_guid, BluetoothCacheMode.Uncached);
                                //GattDeviceServicesResult resultDeviceService = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                                if (resultDeviceService.Status == GattCommunicationStatus.Success)
                                {
                                    var services = resultDeviceService.Services;
                                    foreach (var service in services)
                                    {
                                        var result = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                                        //var result = await service.GetCharacteristicsForUuidAsync(character_guid, BluetoothCacheMode.Uncached);
                                        if (result.Status == GattCommunicationStatus.Success)
                                        {
                                            //watcher.Stop();
                                            var characteristics = result.Characteristics;

                                            foreach (var characteristic in characteristics)
                                            {
                                                if (characteristic.Uuid == character_guid)        // added for iphone
                                                {
                                                    GattCharacteristicProperties properties = characteristic.CharacteristicProperties;

                                                    if (properties.HasFlag(GattCharacteristicProperties.Read))
                                                    {
                                                        // This characteristic supports reading from it.
                                                        GattReadResult readResult = await characteristic.ReadValueAsync();
                                                        if (readResult.Status == GattCommunicationStatus.Success)
                                                        {
                                                            var reader = DataReader.FromBuffer(readResult.Value);
                                                            byte[] input = new byte[reader.UnconsumedBufferLength];
                                                            reader.ReadBytes(input);
                                                            // Utilize the data as needed
                                                            
                                                            string utfString = Encoding.UTF8.GetString(input, 0, input.Length);
                                                            Debug.WriteLine("\nCHARACTERISTIC DATA " + utfString);
                                                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                                            {
                                                                //UI code here
                                                                var hexStr = BitConverter.ToString(input).Replace("-", "");
                                                                TextObj.FontStyle = Windows.UI.Text.FontStyle.Italic;
                                                                //TextObj.Text = hexStr;
                                                                TextObj.Text = utfString;
                                                                //TextObj.Text = TextObj.Text + utfString + "\n";
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)     // stop Button
        {
            watcher.Stop();
        }


        private void TextObj_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
