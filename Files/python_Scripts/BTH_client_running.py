from bluetooth import *


print ("STARTED")
lst = []
target_address = None
nearby_devices = discover_devices( duration=8, flush_cache=True )
for address in nearby_devices :
	print ("NAME: ", lookup_name(address));
	print ("Address: ", address, "\n----------------------------");
	lst.append(address);

ch = int(input("Enter your Choice: "))
ch = ch - 1;
server_address = lst[ch]
port = 6
sock = BluetoothSocket ( RFCOMM )
sock.connect((server_address , port))
sock.send("hello!! from Client Gaurank")
sock.close( )