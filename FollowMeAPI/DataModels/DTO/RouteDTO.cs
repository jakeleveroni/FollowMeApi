using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;


namespace FollowMeAPI.DataModels.DTO
{
    [DataContract(Name = "RouteDTO")]
    public class RouteDTO
    {
        [DataMember(Name = "RoutePoints")]
        public List<RouteDataPoint> RoutePoints { get; set; } = new List<RouteDataPoint>();
    }
}