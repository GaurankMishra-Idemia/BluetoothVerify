// CLIENT program for Linux environment
// socket() >> connect() >> recieve()
//
// We need to create a "SOCKET" and then connect it to a Remote Adress with the "CONNECT" call
// and then finally can Retrieve data with "RECV" call
//--------------------------------------------
#include <stdio.h>
#include <stdlib.h>

#include <sys/types.h>
#include <sys/socket.h>

#include <netinet/in.h>
#include <unistd.h>

int main()
{
	
	//create a socket
	int client_socket;
	client_socket = socket(AF_INET, SOCK_STREAM, 0);  //--------------
	
	//specify the address for the socket
	struct sockaddr_in server_address;
	server_address.sin_family = AF_INET;
	server_address.sin_port = htons(9002);
	server_address.sin_addr.s_addr = INADDR_ANY;
	
	int connection_status = connect(client_socket, (struct sockaddr*) & server_address, sizeof(server_address));  //--------------
	//check for error with connection
	if(connection_status == -1 )
		printf("Error in connection");
	
	//Recieve data from Server
	char buffer[100];
	recv(client_socket, &buffer, sizeof(buffer), 0);  //---------------
	
	//print out the server response
	printf("SERVER:: %s", buffer);
	
	//and close the socket
	close(client_socket);
	
	return 0;
}