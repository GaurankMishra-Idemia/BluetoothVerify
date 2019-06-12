from bluetooth import *


port = 6
backlog = 5
server_sock=BluetoothSocket( RFCOMM )
server_sock.bind(( "" ,port))
server_sock.listen(backlog)
client_sock,client_info = server_sock.accept ( )
print "Accepted connection from " , client_info
data = client_sock.recv(1024)
print "received:" , data
client_sock.close( )
server_sock.close( )

