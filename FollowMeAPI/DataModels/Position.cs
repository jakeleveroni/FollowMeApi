using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;

namespace FollowMeAPI.DataModels
{
    [DataContract]
    public class Position
    {
        [DataMember(Name = "Longitude")]
        [DynamoDBProperty("Longitude")]
        public float Longitude { get; set; }

        [DataMember(Name = "Latitude")]
        [DynamoDBProperty("Latitude")]
        public float Latitude { get; set; }

        public Position()
        {
        }

        public Position(float lon, float lat)
        {
            Longitude = lon;
            Latitude = lat;
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            return $"{Longitude}, {Latitude}";
        }
    }
}