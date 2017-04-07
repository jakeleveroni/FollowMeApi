﻿/*
 * UserModel class is used for reading and writing user informtion to and from the DB
 * this class contains all the data fields associated with a user model
 * it is serializable into JSON format so that it can be passed around the web easily
 * 
 * Created By: Jacob Leveroni
 * Created On: 1/8/2017
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace FollowMeAPI.DataModels
{
    [DataContract]
    [DynamoDBTable("Users")]
    public class UserModel
    {
        // PROPERTIES AND MEMBERS
        [DataMember(Name="Guid")]
        [DynamoDBHashKey("Guid")]
        public Guid UserId { get; set; }

        [DataMember(Name = "ActiveTrip")]
        [DynamoDBProperty("ActiveTrip")]
        public Guid ActiveTrip { get; set; }

        [DataMember(Name = "UserName")]
        [DynamoDBProperty("UserName")]
        public string UserName { get; set; }

        [DataMember(Name = "Name")]
        [DynamoDBProperty("Name")]
        public string Name { get; set; }

        [DataMember(Name = "Email")]
        [DynamoDBProperty("Email")]
        public string Email { get; set; }

        [DataMember(Name = "Password")]
        [DynamoDBProperty("Password")]
        public string Password { get; set; }

        [DataMember(Name = "BirthDate")]
        [DynamoDBProperty("BirthDate")]
        public string BirthDate { get; set; }

        [DataMember(Name = "NumberOfTrips")]
        [DynamoDBProperty("NumberOfTrips")]
        public uint NumberOfTrips { get; set; }

        [DataMember(Name = "TripIds")]
        [DynamoDBProperty("TripIds")]
        public List<string> TripIds { get; set; }

        [DataMember(Name = "TotalMilesTraveled")]
        [DynamoDBProperty("TotalMilesTraveled")]
        public ulong TotalMilesTraveled { get; set; }
        
        [DataMember(Name = "Friends")]
        [DynamoDBProperty("Friends")]
        public List<Guid> Friends { get; set; }

        // METHODS
        public UserModel(){ }

        public UserModel(bool isValid)
        {
            if (!isValid)
            {
                UserId = new Guid();
                UserName = null;
                Name = null;
                Email = null;
                Password = null;
                BirthDate = null;
                TripIds = null;
                Friends = null;
                TotalMilesTraveled = 0;
            }
        }

        public UserModel(Guid id, string userName, string name, string email, string pass, DateTime bd, ulong miles)
        {
            UserId = id;
            UserName = userName;
            Name = name;
            Email = email;
            Password = pass;
            BirthDate = bd.ToString(CultureInfo.InvariantCulture);
            TripIds = new List<string>();
            Friends = new List<Guid>();
            TotalMilesTraveled = miles;
        }

        public UserModel(Guid id, string userName, string name, string email, string pass, DateTime bd, ulong miles, List<Guid> friends = null, List<string> trips = null)
        {
            UserId = id;
            UserName = userName;
            Name = name;
            Email = email;
            Password = pass;
            BirthDate = bd.ToString(CultureInfo.InvariantCulture);
            TotalMilesTraveled = miles;

            if (friends != null)
            {
                Friends = new List<Guid>(friends);
            }
            else
            {
                Friends = new List<Guid>();
            }

            if (trips != null)
            {
                TripIds = new List<string>(trips);
            }
            else
            {
                TripIds = new List<string>();
            }

            NumberOfTrips = (uint)TripIds.Count;
        }

        public UserModel(UserModel other)
        {
            UserId = other.UserId;
            UserName = other.UserName;
            Name = other.Name;
            Email = other.Email;
            Password = other.Password;
            TripIds = new List<string>(other.TripIds);
            Friends = new List<Guid>(other.Friends);
            BirthDate = other.BirthDate.ToString();
            TotalMilesTraveled = other.TotalMilesTraveled;
        }

        public bool AddNewTripId(string trip)
        {
            if (!TripIds.Contains(trip))
            {
                TripIds.Add(trip);
                return true;
            }

            return false;
        }

        public bool RemoveTrip(string trip)
        {
            if (TripIds.Contains(trip))
            {
                TripIds.RemoveAt(TripIds.IndexOf(trip));
                return true;
            }

            return false;
        }

        public bool AddNewFriend(Guid friendId)
        {
            if (!Friends.Contains(friendId))
            {
                Friends.Add(friendId);
                return true;
            }

            return false;
        }

        public bool RemoveFriend(Guid friendId)
        {
            if (Friends.Contains(friendId))
            {
                Friends.Remove(friendId);
                return true;
            }

            return false;
        }

        public Dictionary<string, AttributeValue> FormatUserForDB()
        {
            Dictionary<string, AttributeValue> tripIdAttr = new Dictionary<string, AttributeValue>();
            Dictionary<string, AttributeValue> friendsAttr = new Dictionary<string, AttributeValue>();

            foreach (var trip in TripIds)
            {
                tripIdAttr.Add("TripId", new AttributeValue(trip));
            }

            foreach(var friend in Friends)
            {
                friendsAttr.Add("Friend", new AttributeValue(friend.ToString()));
            }

            return new Dictionary<string, AttributeValue>()
            {
                { "Guid", new AttributeValue { S = UserId.ToString() } },
                { "UserName", new AttributeValue { S = UserName } },
                { "Name", new AttributeValue { S = Name } },
                { "Email", new AttributeValue { S = Email } },
                { "Password", new AttributeValue { S = Password } },
                { "BirthDate", new AttributeValue { S = BirthDate.ToString() } },
                { "TotalMilesTraveled", new AttributeValue { N = TotalMilesTraveled.ToString() } },
                { "NumberOfTrips", new AttributeValue { N = NumberOfTrips.ToString() } },
                { "Trips", new AttributeValue { M = tripIdAttr } },
                { "Friends", new AttributeValue {M =  friendsAttr } }
            };

        }

        public static UserModel DictionaryToUserModel(Dictionary<string, AttributeValue> obj)
        {
            string id  = obj["Guid"].S;
            string userName = obj["UserName"].S;
            string name = obj["Name"].S;
            string email = obj["Email"].S;
            DateTime bday;
            DateTime.TryParse(obj["BirthDate"].S, out bday);
            ulong totalMiles;
            ulong.TryParse(obj["TotalMilesTraveled"].N, out totalMiles);
            string pass = obj["Password"].S;
            List<string> tripIds = new List<string>();

            for (int i = 0; i < obj["TripIds"].L.Count; ++i)
            {
                tripIds.Add(obj["TripIds"].L[i].S);
            }

            List<Guid> friends = new List<Guid>();

            for (int i = 0; i < obj["Friends"].L.Count; ++i)
            {
                friends.Add(new Guid(obj["Friends"].L[i].S));
            }

            return new UserModel(new Guid(id), userName, name, email, pass, bday, totalMiles, friends, tripIds);
        }

        public static bool operator ==(UserModel a, UserModel b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            if (a.UserId == b.UserId && a.UserName == b.UserName &&
                a.Name == b.Name && a.Email == b.Email && 
                a.Password == b.Password && a.BirthDate == b.BirthDate &&
                a.TotalMilesTraveled == b.TotalMilesTraveled && a.NumberOfTrips == b.NumberOfTrips &&
                a.TripIds == b.TripIds && a.Friends == b.Friends)
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(UserModel a, UserModel b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("Name : {0}\nUserName : {1}\nEmail : {2}\nUser Id: {3}\nPassword : {4}\nBirth Date : {5}",
                                 Name, UserName, Email, UserId, Password, BirthDate);
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
