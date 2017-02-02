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
using Amazon.Runtime;
using Newtonsoft.Json;
using Utility;

namespace FollowMeDataBase.DBCallWrappers
{
    public class DB
    {
        AmazonDynamoDBConfig ddbConfig;
        AmazonDynamoDBClient client;
        private Table m_userTableContext;
        private Table m_tripTableContext;
        private readonly string m_userTableName = "Users";
        private readonly string m_tripTableName = "Trips";

        // sets up the DynamoDB connection 
        public DB()
        {
            Initialize();
        }

        public void Initialize()
        {
            // Initialize the database 
            try
            {
                ddbConfig = new AmazonDynamoDBConfig();
                ddbConfig.ServiceURL = "https://dynamodb.us-west-2.amazonaws.com/";
            }
            catch (Exception ex)
            {
                Logger.logger.Error("\nError: failed to create a DynamoDB config; " + ex.Message);
                return;
            }

            try
            { 
                client = new AmazonDynamoDBClient(ddbConfig);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("\nError: failed to create a DynamoDB client; " + ex.Message);
                return;
            } 

            // get description references to all of our databases
            LoadTables();
        }

        private void LoadTables()
        {
            Logger.logger.Info("\n*** Retrieving table information ***");

            try
            {
                m_userTableContext = Table.LoadTable(client, m_userTableName);
                m_tripTableContext = Table.LoadTable(client, m_tripTableName);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[LOAD TABLES][ERROR] : Could not load either the user or trip table, " + ex.Message);
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
                Logger.logger.Error("[ADD NEW USER][ERROR] : Could not add user to database, " + ex.Message);
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
                    Logger.logger.Error("[UPDATE-USER][ERROR] : Invalid update option provided");
                    return false;
            }

            Logger.logger.Info("[UPDATE-USER][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

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
                Logger.logger.Error("[UPDATE-USER][ERROR] : Could not update the user item, + " + ex.Message);
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
                Logger.logger.Error("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
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
                Logger.logger.Error("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
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
                Logger.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
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
                Logger.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
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
                Logger.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
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
                Logger.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return false;
            }
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
                Logger.logger.Error("[ADD NEW TRIP][ERROR] : Could not add trip to database, " + ex.Message);
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
                    Logger.logger.Error("[UPDATE-TRIP][ERROR] : Invalid update option provided");
                    return false;
            }

            Logger.logger.Info("[UPDATE-USER][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

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
                Logger.logger.Error("[UPDATE-TRIP][ERROR] : Could not update the user item, + " + ex.Message);
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
                Logger.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from database, " + ex.Message);
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
                Logger.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from database, " + ex.Message);
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
                Logger.logger.Error("[GET TRIP][ERROR] : Error occurred while locating trip, " + ex.Message);
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
                Logger.logger.Error("[GET TRIP][ERROR] : Error occurred while locating trip, " + ex.Message);
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
                Logger.logger.Error("[TRIP EXISTS][ERROR] : Error occurred while locating trip, " + ex.Message);
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
                Logger.logger.Error("[TRIP EXISTS][ERROR] : Error occurred while locating trip, " + ex.Message);
                return false;
            }
        }
        #endregion

    }
}
