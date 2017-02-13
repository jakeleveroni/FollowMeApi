/*
 * The DB class defines a handful of helpful Database read and write actions
 * sepcific to the FollowMe application (wrapper for AWS.DynoDB native functions)
 * 
 * Created By: Jacob Leveroni
 * Created On: 1/8/2017
*/

using System;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Utility;

namespace FollowMeDataBase.DBCallWrappers
{
    public class DB : IDisposable
    {
        AmazonDynamoDBConfig ddbConfig;
        AmazonDynamoDBClient client;
        private Table m_userTableContext;
        private Table m_tripTableContext;
        private readonly string m_userTableName = "Users";
        private readonly string m_tripTableName = "Trips";
        private readonly string m_userNameAndPasswordIndex = "UserNameAndPassword-index";
        private readonly string m_nameIndex = "Name-index";
        private readonly string m_userNameIndex = "UserName-index";

        // sets up the DynamoDB connection 
        public DB()
        {
            // Initialize the database 
            try
            {
                ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = "https://dynamodb.us-west-2.amazonaws.com/";
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("\nError: failed to create a DynamoDB config; " + ex.Message);
                return;
            }

            try
            {
                client = new AmazonDynamoDBClient(ddbConfig);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("\nError: failed to create a DynamoDB client; " + ex.Message);
                return;
            }

            // get description references to all of our databases
            LoadTables();
        }

        public DB(string accessKey, string secretAccessKey, string token)
        {
            // Initialize the database 
            try
            {
                ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = "https://dynamodb.us-west-2.amazonaws.com/";
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("\nError: failed to create a DynamoDB config; " + ex.Message);
                return;
            }

            try
            {
                client = new AmazonDynamoDBClient(accessKey, secretAccessKey, token, ddbConfig);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("\nError: failed to create a DynamoDB client; " + ex.Message);
                return;
            }

            // get description references to all of our databases
            LoadTables();
        }

        private void LoadTables()
        {
            Utility.Tools.logger.Info("\n*** Retrieving table information ***");

            try
            {
                m_userTableContext = Table.LoadTable(client, m_userTableName);
                m_tripTableContext = Table.LoadTable(client, m_tripTableName);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[LOAD TABLES][ERROR] : Could not load either the user or trip table, " + ex.Message);
                return;
            }
        }

        #region USER_TABLE_WRAPPERS
        public bool AddNewUser(UserModel newUser)
        {
            try
            {
                m_userTableContext.PutItem(Document.FromJson(newUser.SerializeToJson()));
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[ADD NEW USER][ERROR] : Could not add user to database, " + ex.Message);
                return false;
            }
        }

        public bool UpdateUser(string userId, string newValue, UserItemEnums updateType)
        {
            Dictionary<string, AttributeValue> updateAttribValues = new Dictionary<string, AttributeValue>();
            Dictionary<string, string> updateAtribNames = new Dictionary<string, string>();
            string updateExpression;

            switch (updateType)
            {
                case UserItemEnums.UpdateBirthDate:
                    updateAttribValues.Add(":newBD", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#BD", "BirthDate");
                    updateExpression = "SET #BD = :newBD";
                    break;
                case UserItemEnums.UpdateEmail:
                    updateAttribValues.Add(":newEmail", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#E", "Email");
                    updateExpression = "SET #E = :newEmail";
                    break;
                case UserItemEnums.UpdateMilesTraveled:
                    updateAttribValues.Add(":newMilesTraveled", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#TMT", "TotalMilesTraveled");
                    updateExpression = "SET #TMT = :newMilesTraveled";
                    break;
                case UserItemEnums.UpdateName:
                    updateAttribValues.Add(":newName", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#N", "Name");
                    updateExpression = "SET #N = :newName";
                    break;
                case UserItemEnums.UpdateNumberOfTrips:
                    updateAttribValues.Add(":numberOfTrips", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#NOT", "NumberOfTrips");
                    updateExpression = "SET #NOT = :numberOfTrips";
                    break;
                case UserItemEnums.UpdatePassword:
                    updateAttribValues.Add(":newPass", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#P", "Password");
                    updateExpression = "SET #P = :newPass";
                    break;
                case UserItemEnums.UpdateTrips:
                    List<AttributeValue> newTrip = new List<AttributeValue>();
                    newTrip.Add(new AttributeValue(newValue));
                    updateAttribValues.Add(":newTrip", new AttributeValue { L = newTrip });
                    updateAtribNames.Add("#TID", "TripIds");
                    updateExpression = "SET #TID = list_append(#TID, :newTrip)";
                    break;
                case UserItemEnums.UpdateUserName:
                    updateAttribValues.Add(":newUserName", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#UN", "UserName");
                    updateExpression = "SET #UN = :newUserName";
                    break;
                default:
                    Utility.Tools.logger.Error("[UPDATE-USER][ERROR] : Invalid update option provided");
                    return false;
            }

            Utility.Tools.logger.Info("[UPDATE-USER][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

            var request = new UpdateItemRequest
            {
                TableName = m_userTableName,
                Key = new Dictionary<string, AttributeValue>() { { "Guid", new AttributeValue { S = userId.ToString() } } },
                ExpressionAttributeNames = updateAtribNames,
                ExpressionAttributeValues = updateAttribValues,
                UpdateExpression = updateExpression
            };

            try
            {
                var response = client.UpdateItem(request);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[UPDATE-USER][ERROR] : Could not update the user item, + " + ex.Message);
                return false;
            }
        }

        public bool RemoveUser(UserModel oldUser)
        {
            try
            {
                m_userTableContext.DeleteItem(oldUser.UserId.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
                return false;
            }
        }

        public bool RemoveUser(string primaryKey)
        {
            try
            {
                m_userTableContext.DeleteItem(primaryKey);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
                return false;
            }
        }

        public string GetUser(string primaryKey)
        {
            try
            {
                Document doc = m_userTableContext.GetItem(primaryKey);
                return doc.ToJson();
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return string.Empty;
            }
        }

        public UserModel GetUser(UserModel user)
        {
            try
            {
                Document doc = m_userTableContext.GetItem(user.UserId.ToString());
                var jsonDoc = doc.ToJson();
                return (UserModel)JsonConvert.DeserializeObject(jsonDoc, typeof(UserModel));
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return null;
            }
        }

        public bool UserExists(UserModel user)
        {
            Document doc = null;
            try
            {
                doc = m_userTableContext.GetItem(user.UserId.ToString());
                return !(doc == null);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return false;
            }
        }

        public bool UserExists(string userId)
        {
            Document doc = null;
            try
            {
                doc = m_userTableContext.GetItem(userId);
                return !(doc == null);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return false;
            }
        }

        // returns a list of usermodels that match the specified username and password
        // if more than one user is returned, you have a probblem, 
        // if no users are returned, then that account does not exist
        public List<UserModel> QueryUsersByUserNameAndPassword(string userName, string password)
        {
            int resultLimit = 100;
            List<UserModel> queryResults = new List<UserModel>();

            QueryRequest request = new QueryRequest
            {
                TableName = m_userTableName,
                IndexName = m_userNameAndPasswordIndex,
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#un", "UserName"},
                    {"#p", "Password" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":user_name", new AttributeValue { S =  userName }},
                    {":password", new AttributeValue { S =  password }}
                },
                KeyConditionExpression = "#un = :user_name and #p = :password",
                ScanIndexForward = true,
            };

            QueryResponse response = null;

            try
            {
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for user with username \'" + userName + "\'");
                response = client.Query(request);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
                return queryResults;
            }

            List<Dictionary<string, AttributeValue>> items = response.Items;

            if (items.Count == 0)
            {
                return queryResults;
            }

            resultLimit = (items.Count > resultLimit) ? resultLimit : items.Count;

            for (int i = 0; i < resultLimit; ++i)
            {
                queryResults.Add(UserModel.DictionaryToUserModel(items[i]));
            }

            return queryResults;
        }

        // returns a list of users with matching names
        public List<UserModel> QueryUsersByName(string name)
        {
            int resultLimit = 100;
            List<UserModel> queryResults = new List<UserModel>();

            QueryRequest request = new QueryRequest
            {
                TableName = m_userTableName,
                IndexName = m_nameIndex,
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#n", "Name"},
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":name", new AttributeValue { S =  name }},
                },
                KeyConditionExpression = "#n = :name",
                ScanIndexForward = true,
            };

            QueryResponse response = null;

            try
            {
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for users with name \'" + name + "\'");
                response = client.Query(request);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
                return queryResults;
            }

            List<Dictionary<string, AttributeValue>> items = response.Items;

            if (items.Count == 0)
            {
                return queryResults;
            }

            resultLimit = (items.Count > resultLimit) ? resultLimit : items.Count;

            for (int i = 0; i < resultLimit; ++i)
            {
                queryResults.Add(UserModel.DictionaryToUserModel(items[i]));
            }

            return queryResults;
        }

        // returns a list of users with matching usernames
        public List<UserModel> QueryUsersByUserName(string userName)
        {
            int resultLimit = 100;
            List<UserModel> queryResults = new List<UserModel>();

            QueryRequest request = new QueryRequest
            {
                TableName = m_userTableName,
                IndexName = m_userNameIndex,
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#un", "UserName"},
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":user_name", new AttributeValue { S =  userName }},
                },
                KeyConditionExpression = "#un = :user_name",
                ScanIndexForward = true,
            };

            QueryResponse response = null;

            try
            {
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for users with UserName \'" + userName + "\'");
                response = client.Query(request);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
                return queryResults;
            }

            List<Dictionary<string, AttributeValue>> items = response.Items;

            if (items.Count == 0)
            {
                return queryResults;
            }

            resultLimit = (items.Count > resultLimit) ? resultLimit : items.Count;

            for (int i = 0; i < resultLimit; ++i)
            {
                queryResults.Add(UserModel.DictionaryToUserModel(items[i]));
            }

            return queryResults;
        }
        #endregion

        #region TRIP_TABLE_WRAPPERS
        public bool AddNewTrip(TripModel newTrip)
        {
            try
            {
                m_tripTableContext.PutItem(Document.FromJson(newTrip.SerializeToJson()));
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[ADD NEW TRIP][ERROR] : Could not add trip to database, " + ex.Message);
                return false;
            }
        }

        public bool UpdateTrip(string tripId, string newValue, TripItemEnums updateType)
        {
            Dictionary<string, AttributeValue> updateAttribValues = new Dictionary<string, AttributeValue>();
            Dictionary<string, string> updateAtribNames = new Dictionary<string, string>();
            string updateExpression;

            switch (updateType)
            {
                case TripItemEnums.UpdateTripName:
                    updateAttribValues.Add(":newName", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#N", "TripName");
                    updateExpression = "SET #N = :newName";
                    break;
                case TripItemEnums.UpdateTripMileage:
                    updateAttribValues.Add(":newMileage", new AttributeValue { S = newValue });
                    updateAtribNames.Add("#TM", "TripMileage");
                    updateExpression = "SET #TM = :newMileage";
                    break;
                default:
                    Utility.Tools.logger.Error("[UPDATE-TRIP][ERROR] : Invalid update option provided");
                    return false;
            }

            Utility.Tools.logger.Info("[UPDATE-USER][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

            var request = new UpdateItemRequest
            {
                TableName = m_tripTableName,
                Key = new Dictionary<string, AttributeValue>() { { "TripId", new AttributeValue { S = tripId.ToString() } } },
                ExpressionAttributeNames = updateAtribNames,
                ExpressionAttributeValues = updateAttribValues,
                UpdateExpression = updateExpression
            };

            try
            {
                var response = client.UpdateItem(request);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[UPDATE-TRIP][ERROR] : Could not update the user item, + " + ex.Message);
                return false;
            }
        }

        public bool RemoveTrip(TripModel oldTrip)
        {
            try
            {
                m_tripTableContext.DeleteItem(oldTrip.TripId.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from database, " + ex.Message);
                return false;
            }
        }

        public bool RemoveTrip(string primaryKey)
        {
            try
            {
                m_tripTableContext.DeleteItem(primaryKey);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from database, " + ex.Message);
                return false;
            }
        }

        public string GetTrip(string primaryKey)
        {
            try
            {
                Document doc = m_tripTableContext.GetItem(primaryKey);
                return doc.ToJson();
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[GET TRIP][ERROR] : Error occurred while locating trip, " + ex.Message);
                return string.Empty;
            }
        }

        public TripModel GetTrip(TripModel trip)
        {
            try
            {
                Document doc = m_userTableContext.GetItem(trip.TripId.ToString());
                var jsonDoc = doc.ToJson();
                return (TripModel)JsonConvert.DeserializeObject(jsonDoc, typeof(TripModel));
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[GET TRIP][ERROR] : Error occurred while locating trip, " + ex.Message);
                return null;
            }
        }

        public bool TripExists(TripModel trip)
        {
            Document doc = null;
            try
            {
                doc = m_userTableContext.GetItem(trip.TripId.ToString());
                return !(doc == null);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[TRIP EXISTS][ERROR] : Error occurred while locating trip, " + ex.Message);
                return false;
            }
        }

        public bool TripExists(string tripId)
        {
            Document doc = null;
            try
            {
                doc = m_tripTableContext.GetItem(tripId);
                return !(doc == null);
            }
            catch (Exception ex)
            {
                Utility.Tools.logger.Error("[TRIP EXISTS][ERROR] : Error occurred while locating trip, " + ex.Message);
                return false;
            }
        }

		// returns a list of tripmodels that match the specified name
		// if no tripmodels are returned there were no matches
		public List<TripModel> QueryTripsByName(string tripName)
		{
			int resultLimit = 100;
			List<TripModel> queryResults = new List<TripModel>();

			QueryRequest request = new QueryRequest
			{
				TableName = m_tripTableName,
				IndexName = m_userNameAndPasswordIndex,
				ExpressionAttributeNames = new Dictionary<string, string>
				{
					{"#tn", "TripName"},
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
					{":trip_name", new AttributeValue { S =  tripName }},
				},
				KeyConditionExpression = "#tn = :trip_name",
				ScanIndexForward = true,
			};

			QueryResponse response = null;

			try
			{
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for trips with name \'" + tripName + "\'");
				response = client.Query(request);
			}
			catch (Exception ex)
			{
                Utility.Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
				return queryResults;
			}

			List<Dictionary<string, AttributeValue>> items = response.Items;

			if (items.Count == 0)
			{
				return queryResults;
			}

			resultLimit = (items.Count > resultLimit) ? resultLimit : items.Count;

			for (int i = 0; i < resultLimit; ++i)
			{
				queryResults.Add(TripModel.DictionaryToTripModel(items[i]));
			}

			return queryResults;
		}

        public void Dispose()
        {
        }
        #endregion

    }
}
