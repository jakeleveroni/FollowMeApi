using System;
using System.Net;
using FollowMeAPI.Analytics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FollowMeAPI.WebSockets
{
    public class FolloMeWebSocketServer : IDisposable
    {
        // defaults to listening on port 80 ... conviently....
        public WebSocketServer wss = new WebSocketServer(80, true);

        public FolloMeWebSocketServer()
        {
            // register services
            wss.AddWebSocketService<FolloMeWebSocketListener>("/FolloMeWebSocketListener");

            // start the wss server
            wss.Start();
        }

        public void Dispose()
        {
            wss.Stop((ushort) 0, "FolloMeWebSocketServer was disposed of");
        }
    }
}