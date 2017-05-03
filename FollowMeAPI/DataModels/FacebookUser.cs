/*
 * FacebookUserModel class is used for reading and writing user informtion to and from the DB
 * this class contains all the data fields associated with a FacebookUser model
 * it is serializable into JSON format so that it can be passed around the web easily.
 *
 * The purpose of this model was to get the prototype up to date and allow users to login.
 * All that changed from the regular user model was the use of strings for the userID instead of
 * guids
 * 
 * Created By: Eric Garza
 * Created On: May 03, 2017
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
    public class FacebookUser
    {
        // PROPERTIES AND MEMBERS
        [DataMember(Name="FaceBookUserID")]
        [DynamoDBHashKey("FaceBookUserID")]
        public string UserId { get; set; }

        [DataMember(Name = "ActiveTrip")]
        [DynamoDBProperty("ActiveTrip")]
        public string ActiveTrip { get; set; }

        [DataMember(Name = "UserName")]
        [DynamoDBProperty("UserName")]
        public string UserName { get; set; }

        [DataMember(Name = "Name")]
        [DynamoDBProperty("Name")]
        public string Name { get; set; }

        [DataMember(Name = "Bio")]
        [DynamoDBProperty("Bio")]
        public string Bio { get; set; }

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
        public List<string> Friends { get; set; }

        [DataMember(Name = "Followers")]
        [DynamoDBProperty("Followers")]
        public List<string> Followers { get; set; }

        [DataMember(Name = "Following")]
        [DynamoDBProperty("Following")]
        public List<string> Following { get; set; }

        [DataMember(Name="Badges")]
        [DynamoDBProperty("Badges")]
        public List<int> Badges { get; set; }
    }
}