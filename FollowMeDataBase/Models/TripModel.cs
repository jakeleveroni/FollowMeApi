﻿/*
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
using Amazon.DynamoDBv2.Model;
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

        [DataMember(Name = "TripDescription")]
        [DynamoDBProperty("TripDescription")]
        public string TripDescription { get; set; }

        // METHODS
        public TripModel()
        {

        }

        public TripModel(Guid id, string name, ulong miles, string desc)
        {
            TripId = id;
            TripName = name;
            TripMileage = miles;
            TripDescription = desc;
        }

        public TripModel(TripModel other)
        {
            TripId = other.TripId;
            TripName = other.TripName;
            TripMileage = other.TripMileage;
            TripDescription = other.TripDescription;
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

		public static TripModel DictionaryToTripModel(Dictionary<string, AttributeValue> obj)
		{
			string id = obj["Guid"].S;
			string desc = obj["TripDescription"].S;
			string mileage = obj["TripMileage"].N;
			string name = obj["TripName"].S;
			ulong miles;
			ulong.TryParse(mileage, out miles);

			return new TripModel(new Guid(id), name, miles, desc);
		}

        public static bool operator ==(TripModel a, TripModel b)
        {
            if (a.TripId == b.TripId && a.TripMileage == b.TripMileage && 
                a.TripName == b.TripName && a.TripDescription == b.TripDescription)
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
            return string.Format("Trip Name : {0}\nTrip Miles : {1}\nTrip Id : {2}\nDescription : {3}", TripName, TripMileage, TripId, TripDescription);
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
