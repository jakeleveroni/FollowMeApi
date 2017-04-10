using System;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace FollowMeWebSocketServer
{
    public class FolloMeWebSocketServer
    {
        // defaults to listening on port 80 ... conviently....
        private HttpListener httpListener;


        public FolloMeWebSocketServer()
        {
            Console.WriteLine("Initializing web socket server...");
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("192.168.1.111:443/web-socket");
        }

        public async void Start()
        {
            Console.WriteLine("Starting Web Socket server...");
            httpListener.Start();
            HttpListenerContext listenerContext = await httpListener.GetContextAsync();

            if (listenerContext.Request.IsWebSocketRequest)
            {
                WebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync("PubSub");
                WebSocket webSocket = webSocketContext.WebSocket;


            }
            else
            {
                //Return a 426 - Upgrade Required Status Code
                listenerContext.Response.StatusCode = 426;
                listenerContext.Response.Close();
            }
        }
    }
}