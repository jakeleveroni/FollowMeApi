/*
 * UserModel class is used for reading and writing user informtion to and from the DB
 * this class contains all the data fields associated with a user model
 * it is serializable into JSON format so that it can be passed around the web easily
 * 
 * Created By: Jacob Leveroni
 * Created On: 1/8/2017
*/

using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;

namespace FollowMeDataBase.Models
{
    [DataContract]
    [DynamoDBTable("Users")]
    public class UserModel
    {
        // PROPERTIES AND MEMBERS
        [DataMember(Name="Guid")]
        [DynamoDBHashKey("Guid")]
        public Guid UserId { get; set; }

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

        // METHODS
        public UserModel()
        {
             
        }

        public UserModel(Guid id, string userName, string name, string email, string pass, DateTime bd, ulong miles)
        {
            UserId = id;
            UserName = userName;
            Name = name;
            Email = email;
            Password = pass;
            BirthDate = bd.ToString();
            TripIds = new List<string>();
            TotalMilesTraveled = miles;
        }

        public UserModel(UserModel other)
        {
            UserId = other.UserId;
            UserName = other.UserName;
            Name = other.Name;
            Email = other.Email;
            Password = other.Password;
            TripIds = new List<string>(other.TripIds);
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

        public Dictionary<string, AttributeValue> FormatUserForDB()
        {
            Dictionary<string, AttributeValue> tripIdAttr = new Dictionary<string, AttributeValue>();
            foreach (var trip in TripIds)
            {
                tripIdAttr.Add("TripId", new AttributeValue(trip));
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
                { "Trips", new AttributeValue { M = tripIdAttr } }
            };

        }

        public static bool operator ==(UserModel a, UserModel b)
        {
            if (a.UserId == b.UserId && a.UserName == b.UserName &&
                a.Name == b.Name && a.Email == b.Email && 
                a.Password == b.Password && a.BirthDate == b.BirthDate &&
                a.TotalMilesTraveled == b.TotalMilesTraveled && a.NumberOfTrips == b.NumberOfTrips &&
                a.TripIds == b.TripIds)
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
