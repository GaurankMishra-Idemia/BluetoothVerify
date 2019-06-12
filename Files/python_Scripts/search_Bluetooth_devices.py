
from bluetooth import *

target_name = "My Phone"
target_address = None
nearby_devices = discover_devices( duration=8, flush_cache=True )
for address in nearby_devices :
	print ("NAME: ", lookup_name(address));
	print ("Address: ", address);


