/*
 * The Trip model is used to store all the information that will be 
 * read and written to the Trip table in the database
 * 
 * Created By: Jacob Leveroni
 * Created On: 1/8/2017
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.DataModel;


namespace FollowMeDataBase.Models
{
    [DataContract]
    public class TripModel
    {
        // PROPERTIES AND MEMBERS
        [DataMember(Name = "TripId")]
        [DynamoDBProperty("Guid")]
        public Guid TripId { get; set; }

        [DataMember(Name = "TripName")]
        [DynamoDBProperty("TripName")]
        public string TripName { get; set; }

        [DataMember(Name = "TripMileage")]
        [DynamoDBProperty("TripMileage")]
        public ulong TripMileage { get; set; }

        // METHODS
        public TripModel()
        {

        }

        public TripModel(Guid id, string name, ulong miles)
        {
            TripId = id;
            TripName = name;
            TripMileage = miles;
        }

        public TripModel(TripModel other)
        {
            TripId = other.TripId;
            TripName = other.TripName;
            TripMileage = other.TripMileage;
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static bool operator ==(TripModel a, TripModel b)
        {
            if (a.TripId == b.TripId && a.TripMileage == b.TripMileage && a.TripName == b.TripName)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(TripModel a, TripModel b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("Trip Name : {0}\nTrip Miles : {1}\nTrip Id : {2}", TripName, TripMileage, TripId);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
