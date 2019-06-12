import asyncio
from bleak import discover
from bleak import BleakClient
from bleak import _logger as logger
from bleak.uuids import uuid16_dict


async def print_services(mac_addr: str, loop: asyncio.AbstractEventLoop):
    try:
        async with BleakClient(mac_addr, loop=loop) as client:
            #x = await client.is_connected()       

            for service in client.services:
                # service.obj is instance of 'Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService'
                print("SER UUID: ", service.uuid)
                for char in service.characteristics:
                    # char.obj is instance of 'Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic'
                    print("CHAR UUID: ",char.uuid)
                    
                    if "read" in char.properties:
                        try:
                            value = bytes(await client.read_gatt_char(char.uuid))
                            aa = value.decode('UTF8') 
                            print("------------------------------------------------VALUE: ",aa)
                        except:
                            print("READING ERROR")
    except:
          print("ERROR ")
          
                                                      
      

async def run():
    devices = await discover()
    for d in devices:
        #print(d)
        
        print("NAME: ",d.details.Advertisement.LocalName)
        print("NAME MF: ",d.name)
        try:
            meta_data = d.metadata;
            mf_data = meta_data.manufacturer_data;
            for data in mf_data:
                mfd = data.decode('UTF8') 
                print("MANUFACTURER DATA: ", mfd)
            for uuid in d.metadata.uuids:
                print("ADV MF UUID: ",uuid)
            for dt in d.details.Advertisement.DataSections:
                print("DATA SECTION: ",dt.ToString())
        except:
            print("ERROR ADVERTISEMENT DATA READING ERROR")
        print("ADDRESS: ",d.address)
        #loop1 = asyncio.get_event_loop()
        await print_services(d.address, loop)
        

loop = asyncio.get_event_loop()
loop.run_until_complete(run())

