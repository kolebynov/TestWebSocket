using System;
using Wcf.WsServer;

var service = new RecordEventsService();
service.Start();
Console.ReadLine();