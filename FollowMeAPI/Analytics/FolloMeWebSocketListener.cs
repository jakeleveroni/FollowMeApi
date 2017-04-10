using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace FollowMeAPI.Analytics
{
    public class FolloMeWebSocketListener : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs message)
        {
            // all possible message
            //if (message.Data == "Hello")
            //{
            //    Send("World");
            //}
        }
    }
}