using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Text;
using System.Security.Permissions;
using System.Security;


namespace UWP_HELLOWORLD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        //private void InitializeComponent()
        //{
            //throw new NotImplementedException();
        //}

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Guid uuid = Guid.NewGuid();         // to get a new GUID
            //Guid uuid = GattServiceUuids.Battery;
            //Guid uuid = GattServiceUuids.CurrentTime;
            Guid uuid = new Guid("be87f1c1-c0a3-480b-ba30-41dd1f8b415a");

            Guid uuid1 = new Guid("dd2305e1-cde3-48be-b7e5-f3a78ca045d1");
            //Guid uuid1 = Guid.NewGuid();
            //Guid uuid1 = GattCharacteristicUuids.BatteryLevel;
            //Guid uuid1 = GattCharacteristicUuids.CurrentTime;
            Guid uuid2 = Guid.NewGuid();

            GattServiceProvider serviceProvider = null;
            GattLocalCharacteristicParameters ReadParameters = new GattLocalCharacteristicParameters();
            GattLocalCharacteristicParameters WriteParameters = new GattLocalCharacteristicParameters();
            ReadParameters.ReadProtectionLevel = GattProtectionLevel.Plain;
            GattCharacteristic rs;
            GattLocalCharacteristic _readCharacteristic;
            GattLocalCharacteristic _writeCharacteristic;          
            GattLocalCharacteristicResult characteristicResult = null;


            // The GattServiceProvider is used to create and advertise the root primary service definition. 
            GattServiceProviderResult result = await GattServiceProvider.CreateAsync(uuid);

            if (result.Error == BluetoothError.Success)
            {
                serviceProvider = result.ServiceProvider;
                ReadParameters.CharacteristicProperties = GattCharacteristicProperties.Read;

                //ReadParameters.StaticValue = (new byte[] { 0x21 }).AsBuffer();

                //string lines = System.IO.File.ReadAllLines(@"C:\Users\g522891\Documents\rough_code.txt"); 

                string stringToConvert = "Hello Client !! THis is SerVER. Written By Gaurank Mishra.";  //main
                byte[] buffer = Encoding.UTF8.GetBytes(stringToConvert);                                //main 

                //FileAttributes attributes = File.GetAttributes(@"C:\Users\g522891\Desktop\rough_code.txt");
                //string text = System.IO.File.ReadAllText("rough_code.txt");



                //byte[] buffer = System.IO.File.ReadAllBytes("rough_code.txt");
                ReadParameters.StaticValue = (buffer).AsBuffer();                                        //main
                //ReadParameters.StaticValue = (buffer).AsBuffer(stringToConvert);

                ReadParameters.UserDescription = "IDEMIA SERVER";               
            }                    

            characteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(uuid1, ReadParameters);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }

            _readCharacteristic = characteristicResult.Characteristic;
            _readCharacteristic.ReadRequested += ReadCharacteristic_ReadRequested;

            
            characteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(uuid2, WriteParameters);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return; 
            }
            _writeCharacteristic = characteristicResult.Characteristic;
            _writeCharacteristic.WriteRequested += WriteCharacteristic_WriteRequested;        

            //ADVERTISE
            GattServiceProviderAdvertisingParameters advParameters = new GattServiceProviderAdvertisingParameters
            {
                IsDiscoverable = true,
                IsConnectable = true
            };
            serviceProvider.StartAdvertising(advParameters);         
        }

        public async void ReadCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            /*
            var deferral = args.GetDeferral();

            // Our familiar friend - DataWriter.
            var writer = new DataWriter();
            writer.WriteString("GM");   
            // populate writer w/ some data. 
            // ... 

            var request = await args.GetRequestAsync();
            request.RespondWithValue(writer.DetachBuffer());

            deferral.Complete();
            */
            var deferral = args.GetDeferral();
            var writer = new DataWriter();
            writer.WriteByte(0x21);
            var request = await args.GetRequestAsync();
            request.RespondWithValue(writer.DetachBuffer());
            deferral.Complete();           
            
        }

        async void WriteCharacteristic_WriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            var request = await args.GetRequestAsync();
            var reader = DataReader.FromBuffer(request.Value);
            // Parse data as necessary. 

            if (request.Option == GattWriteOption.WriteWithResponse)
            {
                request.Respond();
            }

            deferral.Complete();       
        }   
    }
}
