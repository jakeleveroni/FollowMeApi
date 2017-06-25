using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace FollowMeAPI.DataModels
{
    [DataContract(Name = "RoutePoints")]
    public class RouteDataPoint
    {
        [DataMember(Name = "Longitude")]
        [DynamoDBProperty("Longitude")]
        public float Longitude { get; set; }

        [DataMember(Name = "Latitude")]
        [DynamoDBProperty("Latitude")]
        public float Latitude { get; set; }

        [IgnoreDataMember]
        [DynamoDBIgnore]
        public Dictionary<string, AttributeValue> RouteAttibute
        {
            get
            {
                return new Dictionary<string, AttributeValue> {
                    { "Longitude", new AttributeValue { S = Longitude.ToString() } },
                    { "Latitude", new AttributeValue { S = Latitude.ToString() } }
                };
            }
        }

        public RouteDataPoint()
        {
            Longitude = Latitude = 0.0f;
        }

        public RouteDataPoint(float lng, float lat)
        {
            Longitude = lng;
            Latitude = lat;
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}