import sys
from bluetooth import *

##"00001101-0000-1000-8000-00805F9B34FB"

service_matches = find_service()
first_match = service_matches[0]
port = first_match[ "port" ]
name = first_match[ "name" ]
host = first_match[ "host" ]
for adr in service_matches :
    if(adr["service-classes"] == "0000FE35-0000-1000-8000-00805F9B34FB") :
        ser = adr["service-classes"]
        port = adr[ "port" ]
        name = adr[ "name" ]
        host = adr[ "host" ]
##    print("Service-Classes: ", adr["service-classes"])
##    print("Service-id: ", adr["service-id"])

print ("connecting to " , host)
sock=BluetoothSocket( RFCOMM )
sock.connect( (host , port) )
sock.send( "PyBluez client says Hello!!" )
data = sock.recv(80)
print ("received: " , data)
sock.close( )
