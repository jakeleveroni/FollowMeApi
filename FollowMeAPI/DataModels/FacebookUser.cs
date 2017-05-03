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
    }
}