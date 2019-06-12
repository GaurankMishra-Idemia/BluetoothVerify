// Bluetooth_Client.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <stdlib.h>
#include <vector>

#include <initguid.h>
#include <winsock2.h>
#include <ws2bth.h>
#include <BluetoothAPIs.h>

using namespace std;

DEFINE_GUID(SAMPLE_UUID, 0x31b44148, 0x041f, 0x42f5, 0x8e, 0x73, 0x18, 0x6d, 0x5a, 0x47, 0x9f, 0xc9);

int main()
{
	printf("STARTED\n\n");

	// setup windows sockets
	WORD wVersionRequested;
	WSADATA wsaData;
	wVersionRequested = MAKEWORD(2, 0);
	if (WSAStartup(wVersionRequested, &wsaData) != 0) {
		fprintf(stderr, "uh oh... windows sockets barfed\n");
		ExitProcess(2);
	}
	// prepare the inquiry data structure
	DWORD qslen = sizeof(WSAQUERYSET);
	WSAQUERYSET *qs = (WSAQUERYSET*)malloc(qslen);
	ZeroMemory(qs, qslen);
	//--------------------------------------

	qs->dwSize = sizeof(WSAQUERYSET);
	qs->dwNameSpace = NS_BTH;
	DWORD flags = LUP_CONTAINERS;
	flags |= LUP_FLUSHCACHE | LUP_RETURN_NAME | LUP_RETURN_ADDR;
	HANDLE h;

	// start the device inquiry
	if (SOCKET_ERROR == WSALookupServiceBegin(qs, flags, &h)) {
		ExitProcess(2);
	}

	vector<BTH_ADDR> Dev_Add;
	vector<int> Dev_Port;
	int index = 0;

	// iterate through the inquiry results
	bool done = false;
	while (!done)
	{
		if (NO_ERROR == WSALookupServiceNext(h, flags, &qslen, qs))
		{
			WCHAR  buf[256] = L"Some dummy initializer";
			//char buff[40] = { 0 };
			SOCKADDR_BTH *sa = (SOCKADDR_BTH*)qs->lpcsaBuffer->RemoteAddr.lpSockaddr;
			BTH_ADDR result = sa->btAddr;
			//DWORD bufsize = sizeof(buff);
			DWORD bufsize = sizeof(struct sockaddr_storage);
			WSAAddressToString(qs->lpcsaBuffer->RemoteAddr.lpSockaddr, sizeof(SOCKADDR_BTH), NULL, buf, &bufsize);

			Dev_Port.push_back(6);
			Dev_Add.push_back(sa->btAddr);

			//printf("found: %s - %s\n", buf, qs->lpszServiceInstanceName);  // can use wprintf()
			std::wcout << "Found " << ++index << " -- " << buf << " " << qs->lpszServiceInstanceName << std::endl;
		}
		else
		{
			int error = WSAGetLastError();
			if (error == WSAEFAULT) {
				free(qs);
				qs = (WSAQUERYSET*)malloc(qslen);
			}
			else if (error == WSA_E_NO_MORE)
			{
				printf("inquiry complete\n");
				done = true;
			}
			else
			{
				printf("uh oh. error code %d\n", error);
				done = true;
			}
		}
	}
	int choice = 0;
	std::cout << "\n\nPlease Enter the INDEX to connect to: ";
	std::cin >> choice;

	//END THE LOOK UP SERVICES
	WSALookupServiceEnd(h);

	//NOW CONNECT TO THE SELECTED DEVICE
	SOCKET client_Socket;
	SOCKADDR_BTH bluetooth_Socket_Address ;
	
	memset(&bluetooth_Socket_Address, 0, sizeof(SOCKADDR_BTH));
	int sa_len = sizeof(bluetooth_Socket_Address);

	// query it for the right port
	// create the socket
	client_Socket = socket(AF_BTH, SOCK_STREAM, BTHPROTO_RFCOMM);
	if (SOCKET_ERROR == client_Socket) {
		ExitProcess(2);
	}

	//fill in the rest of the SOCKADDR¯BTH struct
	bluetooth_Socket_Address.port = Dev_Port[choice-1];
	bluetooth_Socket_Address.btAddr = Dev_Add[choice-1];
	bluetooth_Socket_Address.addressFamily = AF_BTH;

	if (bluetooth_Socket_Address.port == 0) {
		//	ExitProcess(2);
	}
	if (SOCKET_ERROR == connect(client_Socket, (LPSOCKADDR)&bluetooth_Socket_Address, sa_len)) {
		ExitProcess(2);
	}
	send(client_Socket, "hello from CLient!", 20, 0);
	closesocket(client_Socket);
	//-------------------------------------------------------
	free(qs);
	WSACleanup();

	printf("\nProgram END");
	closesocket(client_Socket);
	return 0;
}