using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

/*
 * This will be the controller we add events to for user badges as well as analytics 
 * the various routes we define here will fire off events to the EventReceiver which will 
 * handle behave appropriately based on the event being fired
 */

namespace FollowMeAPI.Controllers
{
    public class EventController : ApiController
    {
    }
}
