// Bluetooth_Server.cpp : This file contains the 'main' function. Program execution begins and ends there.
// 

#include "pch.h"
#include <iostream>

#include <stdio.h>
#include <winsock2.h>
#include <ws2bth.h>
#include <BluetoothAPIs.h>
#include <initguid.h>

DEFINE_GUID(SAMPLE_UUID, 0x31b44148, 0x041f, 0x42f5, 0x8e, 0x73, 0x18, 0x6d, 0x5a, 0x47, 0x9f, 0xc9);

int main()
{
	SOCKET server;
	SOCKADDR_BTH sa;
	int sa_len = sizeof(sa);

	// initialize windows sockets
	WORD wVersionRequested;
	WSADATA wsaData;
	wVersionRequested = MAKEWORD(2, 0);
	if (WSAStartup(wVersionRequested, &wsaData) != 0) {
		ExitProcess(2);
	}
	// create the server socket
	server = socket(AF_BTH, SOCK_STREAM, BTHPROTO_RFCOMM);
	if (SOCKET_ERROR == server) {
		ExitProcess(2);
	}
	// bind the server socket to an arbitrary RFCOMM port
	sa.addressFamily = AF_BTH;
	sa.btAddr = 0;               // for server it should be zero
	sa.port = BT_PORT_ANY;
	//sa.port = 1;
	if (SOCKET_ERROR == bind(server, (const sockaddr*)&sa, sizeof(SOCKADDR_BTH))) {
		ExitProcess(2);
	}

	listen(server, 6);
	// check which port we’re listening on
	if (SOCKET_ERROR == getsockname(server, (SOCKADDR*)&sa, &sa_len)) {
		ExitProcess(2);
	}
	printf("listening on RFCOMM port: %d\n", sa.port);
	// advertise the service
	CSADDR_INFO sockInfo;
	sockInfo.iProtocol = BTHPROTO_RFCOMM;
	sockInfo.iSocketType = SOCK_STREAM;
	sockInfo.LocalAddr.lpSockaddr = (LPSOCKADDR)&sa;
	sockInfo.LocalAddr.iSockaddrLength = sizeof(sa);
	sockInfo.RemoteAddr.lpSockaddr = (LPSOCKADDR)&sa;
	sockInfo.RemoteAddr.iSockaddrLength = sizeof(sa);

	WSAQUERYSET svcInfo;
	memset(&svcInfo, 0, sizeof(WSAQUERYSET));
	svcInfo.dwSize = sizeof(svcInfo);
	svcInfo.dwNameSpace = NS_BTH;
	wchar_t serviceName[] = L"IDEMIA SERVICE";
	wchar_t serviceDesc[] = L"FOR MDL DEVICE";
	svcInfo.lpszServiceInstanceName = serviceName;
	svcInfo.lpszComment = serviceDesc;
	svcInfo.lpServiceClassId = (LPGUID)&SAMPLE_UUID;
	svcInfo.dwNumberOfCsAddrs = 1;
	svcInfo.lpcsaBuffer = &sockInfo;
	if (SOCKET_ERROR == WSASetService(&svcInfo, RNRSERVICE_REGISTER, 0)) {
		ExitProcess(2);
	}
	SOCKADDR_BTH ca;
	int ca_len = sizeof(ca);
	SOCKET client;
	WCHAR  buf[1024] = { 0 };
	DWORD buf_len = sizeof(buf);
	client = accept(server, (LPSOCKADDR)&ca, &ca_len);
	if (SOCKET_ERROR == client) {
		ExitProcess(2);
	}
	WSAAddressToString((LPSOCKADDR)&ca, (DWORD)ca_len, NULL, buf, &buf_len);
	printf("Accepted connection from %s\n", buf);
	int received = 0;
	char arrayChar[50] = { 0 };
	received = recv(client, arrayChar, sizeof(arrayChar), 0);
	if (received > 0) {
		printf("received: %s\n", buf);
	}
	closesocket(client);
	closesocket(server);
	return 0;
}