// Bluetooth_Verify.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

//#include "pch.h"
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <string>

#include <winsock2.h>
#include <ws2bth.h>
#include <BluetoothAPIs.h>


int main(int argc, TCHAR* argv[]) {

	printf("STARTED");

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
	qs->dwSize = sizeof(WSAQUERYSET);
	qs->dwNameSpace = NS_BTH;
	DWORD flags = LUP_CONTAINERS;
	flags |= LUP_FLUSHCACHE | LUP_RETURN_NAME | LUP_RETURN_ADDR;
	HANDLE h;

	// start the device inquiry
	if (SOCKET_ERROR == WSALookupServiceBegin(qs, flags, &h)) {
		ExitProcess(2);
	}

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
			//printf("found: %s - %s\n", buf, qs->lpszServiceInstanceName);  // can use wprintf()
			std::wcout<<"found: "<<buf <<" "<< qs->lpszServiceInstanceName <<std::endl;
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
	WSALookupServiceEnd(h);
	free(qs);
	WSACleanup();

	printf("\nProgram Executed");
	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
