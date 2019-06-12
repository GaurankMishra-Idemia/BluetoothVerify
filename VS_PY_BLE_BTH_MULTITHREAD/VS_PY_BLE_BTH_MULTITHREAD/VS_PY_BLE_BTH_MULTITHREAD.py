import asyncio
import threading
from bluetooth import *
from bleak import discover
from bleak import BleakClient
from multiprocessing import Process


lst = []

async def run():
    devices = await discover()
    for d in devices:
        lst.append(d);

def BTH():
    print("STARTED BTH \n")
    port = 6
    backlog = 5
    server_sock=BluetoothSocket( RFCOMM )
    server_sock.bind(( "" ,port))
    server_sock.listen(backlog)
    client_sock,client_info = server_sock.accept( )
    print ("Accepted connection from " , client_info)
    data = client_sock.recv(1024)
    print ("received:" , data)
    client_sock.close( )
    server_sock.close( )

async def print_services(mac_addr: str, loop: asyncio.AbstractEventLoop):
    try:
        async with BleakClient(mac_addr, loop=loop) as client:
            try:
                services = await client.get_services()
                for service in services:
                    print("SERVICE UUID: ",service.uuid)
                    characteristics = service.characteristics;
                    for char in characteristics:
                        # char.obj is instance of 'Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic'
                        print("CHAR UUID: ",char.uuid)
                        #uuid_tmp = "333a9d00-7d08-4f24-a3bb-9e85c7328bd4"
                        uuid_tmp = "c32e093a-9c7d-42c4-8a8b-2652fb97750c"   # NITIKA samsung Chart uuid
                        if uuid_tmp == char.uuid:
                            print("MATCH")
                        obj = char.obj;
                        val = obj.ReadValueAsync();
                        #print("Value: ",val.ToString())

                        if "read" in char.properties:
                            try:
                                value = bytes(await client.read_gatt_char(char.uuid))                                
                                #aa = value.hex();  
                                #bb = " ";
                                aa  = " ";
                                bb = value.decode('utf-8') 
                                print("\n------------------------------------------------VALUE: ",bb, "     ",aa)
                            except:
                                print("DECODING ERROR")

            except:
                print("CANNOT FETCH SERVICES \n");
    except:
        print("CANNOT CONNECT TO CLIENT")



def BLE():
    print("STARTED BLE \n")
    loop = asyncio.get_event_loop()
    loop.run_until_complete(run())
    
    for device in lst:
	    #loop = asyncio.get_event_loop()
        print("NAME: ",device.name);
        loop.run_until_complete(print_services(device.address,loop))


if __name__ == '__main__':
    p1 = Process(target=BLE)
    p1.start()
    p2 = Process(target=BTH)
    p2.start()
    p1.join()
    p2.join()

#loop_BLE = asyncio.get_event_loop()
#loop_BTH = asyncio.get_event_loop()

#loop_BLE.run_until_complete(BLE(loop_BLE))
#loop_BTH.run_until_complete(BTH())


#t1 = threading.Thread(target=BLE)
#t1.start()
##t1.join()
#t2 = threading.Thread(target=BTH)
#t2.start()
#t2.join()

#threading.start_new_thread(BLE, ())
#threading.start_new_thread(BTH, ())