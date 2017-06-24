/*
 * The Trip model is used to store all the information that will be 
 * read and written to the Trip table in the database
 * 
 * Created By: Jacob Leveroni
 * Created On: 1/8/2017
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace FollowMeAPI.DataModels
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

        [DataMember(Name = "Participants")]
        [DynamoDBProperty("Participants")]
        public List<Guid> Participants { get; set; }

        [DataMember(Name = "Moments")]
        [DynamoDBProperty("Moments")]
        public List<Guid> Moments { get; set; }

        [DataMember(Name = "Route")]
        [DynamoDBProperty("Route")]
        public List<RouteDataPoint> Route { get; set; }

        // METHODS
        public TripModel()
        {
            Participants = new List<Guid>();
			Moments = new List<Guid>();
            Route = new List<RouteDataPoint>();
        }

        public TripModel(Guid id, string name, ulong miles, string desc, List<Guid> participants = null)
        {
            TripId = id;
            TripName = name;
            TripMileage = miles;
            TripDescription = desc;
			Moments = new List<Guid>();
            Route = new List<RouteDataPoint>();

            if (participants != null)
            {
                Participants = new List<Guid>(participants);
            }
            else
            {
                Participants = new List<Guid>();
            }
        }

        public TripModel(TripModel other)
        {
            TripId = other.TripId;
            TripName = other.TripName;
            TripMileage = other.TripMileage;
            TripDescription = other.TripDescription;
            Participants = new List<Guid>(other.Participants);
            Moments = new List<Guid>(other.Moments);
            Route = new List<RouteDataPoint>(other.Route);
        }

        public TripModel(bool isInvalid)
        {
            if (isInvalid)
            {
                TripId = new Guid();
                TripName = null;
                TripMileage = 0;
                TripDescription = null;
                Moments = null;
                Participants = null;
                Participants = new List<Guid>();
                Moments = new List<Guid>();
                Route = new List<RouteDataPoint>();
            }
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
            List<Guid> moments = null;
            List <Guid> participants = null;
            List<RouteDataPoint> route = null;

            if (obj["Participants"].L.Count > 0)
            {
                participants = new List<Guid>();
                for (int i = 0; i < obj["Participants"].L.Count; ++i)
                {
                    participants.Add(new Guid(obj["Participants"].L[i].S));
                } 
            }

            if (obj["Moments"].L.Count > 0)
            {
                moments = new List<Guid>();
                for (int i = 0; i < obj["Moments"].L.Count; ++i)
                {
					moments.Add(new Guid(obj["Moments"].L[i].S));
                } 
            }

            if (obj["Route"].L.Count > 0)
            {
                moments = new List<Guid>();
                for (int i = 0; i < obj["Route"].L.Count; ++i)
                {
                    route.Add(new RouteDataPoint(float.Parse(obj["Route"].L[i].M["Longitude"].S), float.Parse(obj["Route"].L[i].M["Latitude"].S)));
                }
            }

            TripModel trip = new TripModel(new Guid(id), name, miles, desc);
            trip.Participants = participants;
            trip.Moments = moments;
            trip.Route = route;

            return trip;
        }

        public static bool operator ==(TripModel a, TripModel b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            if (a.TripId == b.TripId && a.TripMileage == b.TripMileage && 
                a.TripName == b.TripName && a.TripDescription == b.TripDescription &&
                a.Participants == b.Participants && a.Moments == b.Moments && a.Route == b.Route)
            {
                return true;
            }

            return false;
        }

        public bool AddNewParticipant(Guid userId)
        {
            if (!Participants.Contains(userId))
            {
                Participants.Add(userId);
                return true;
            }

            return false;
        }

        public bool RemoveParticipant(Guid userId)
        {
            if (Participants.Contains(userId))
            {
                Participants.Remove(userId);
                return true;
            }

            return false;
        }

        public bool AddMoment(Guid newMoment)
        {
            if (!Moments.Contains(newMoment))
            {
                Moments.Add(newMoment);
                return true;
            }

            return false;
        }

        public bool RemoveMoment(Guid newMoment)
        {
            if (Moments.Contains(newMoment))
            {
                Moments.Remove(newMoment);
                return true;
            }

            return false;
        }

        public void AddRoutePoint(float longitude, float latitude)
        {

            Route.Add(new RouteDataPoint(longitude, latitude));
        }

        public void RemoveRoutePoint(float longitude, float latitude)
        {
            Route.Remove(new RouteDataPoint(longitude, latitude));
        }

        public static bool operator !=(TripModel a, TripModel b)
        {
            return !(a == b);
        }

        [Obsolete("No longer returns completely stringified trip model")]
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
