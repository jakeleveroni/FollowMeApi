using System;
using System.Net;
using FollowMeAPI.Analytics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace FollowMeAPI.WebSockets
{
    public class FolloMeWebSocketServer
    {
        private HttpListener _httpListener;
        private bool _isRunning;
        public FolloMeWebSocketServer()
        {
            _isRunning = false;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://localhost:443/web-socket");
            _httpListener.Start();
        }

        public async void Run()
        {
            _isRunning = true;

            while (_isRunning)
            {
                HttpListenerContext listenerContext = await _httpListener.GetContextAsync();

                if (listenerContext.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(listenerContext);
                }
            }
        }

        private async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                if (context.Request.RemoteEndPoint != null)
                {
                    string ipAddr = context.Request.RemoteEndPoint.Address.ToString();
                    Tools.logger.Info($"Connected IPAddress {ipAddr}");
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
                Tools.logger.Error($"Could not create web socket connection: {e.Message}");
                return;
            }

            WebSocket webSocket = webSocketContext.WebSocket;

            try
            {
                byte[] receiveBuffer = new byte[1024];

                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer),
                        CancellationToken.None);

                    // MESSAGING!
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        var message = Encoding.UTF8.GetString(receiveBuffer);

                        switch (message)
                        {
                            case "Login":
                                break;
                            case "Logout":
                                break;
                            case "CreateTrip":
                                break;
                            default:
                                throw new ArgumentException(message: $"Could not process message: {message}");
                        }

                    }
                    //else if (result.MessageType == WebSocketMessageType.Text)
                    //{

                    //}
                    else
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, $"Invalid message type {result.MessageType}", CancellationToken.None);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Tools.logger.Error($"Web socket connection error, connection closed: {e.Message}");
            }
            finally
            {
                webSocket?.Dispose();
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}