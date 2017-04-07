/*
 * The DB class defines a handful of helpful Database read and write actions
 * sepcific to the FollowMe application (wrapper for AWS.DynoDB native functions)
 * 
 * Created By: Jacob Leveroni
 * Created On: 1/8/2017
*/

using System;
using System.Collections.Generic;
using FollowMeAPI;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using FollowMeAPI.DataModels;
using Newtonsoft.Json;


namespace FollowMeDataBase.DBCallWrappers
{
	public class DB : IDisposable
	{
		private AmazonDynamoDBConfig _config;
		private readonly AmazonDynamoDBClient _client;
		private Table _userTableContext;
		private Table _tripTableContext;
		private Table _momentTableContext;
		private readonly string m_userTableName = "Users";
		private readonly string m_tripTableName = "Trips";
		private readonly string m_MomentTableName = "Moments";

		// secondary indices for users table
		private readonly string m_userNameAndPasswordIndex = "UserNameAndPassword-index";
		private readonly string m_nameIndex = "Name-index";
		private readonly string m_userNameIndex = "UserName-index";

		// sets up the DynamoDB connection 
		public DB()
		{
			// Initialize the database 
			try
			{
				_config = new AmazonDynamoDBConfig();
				_config.ServiceURL = "https://dynamodb.us-west-2.amazonaws.com/";
			}
			catch (Exception ex)
			{
				Tools.logger.Error("\nError: failed to create a DynamoDB config; " + ex.Message);
				return;
			}

			try
			{
				_client = new AmazonDynamoDBClient(_config);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("\nError: failed to create a DynamoDB _client; " + ex.Message);
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
				_config = new AmazonDynamoDBConfig();
				_config.ServiceURL = "https://dynamodb.us-west-2.amazonaws.com/";
			}
			catch (Exception ex)
			{
				Tools.logger.Error("\nError: failed to create a DynamoDB config; " + ex.Message);
				return;
			}

			try
			{
				_client = new AmazonDynamoDBClient(accessKey, secretAccessKey, token, _config);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("\nError: failed to create a DynamoDB _client; " + ex.Message);
				return;
			}

			// get description references to all of our databases
			LoadTables();
		}

		private void LoadTables()
		{
			Tools.logger.Info("\n*** Retrieving table information ***");

			try
			{
				_userTableContext = Table.LoadTable(_client, m_userTableName);
				_tripTableContext = Table.LoadTable(_client, m_tripTableName);
				_momentTableContext = Table.LoadTable(_client, m_MomentTableName);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[LOAD TABLES][ERROR] : Could not load any of the tables, " + ex.Message);
				return;
			}
		}

		#region USER_TABLE_WRAPPERS
		public bool AddNewUser(UserModel newUser)
		{
			try
			{
				_userTableContext.PutItem(Document.FromJson(newUser.SerializeToJson()));
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[ADD NEW USER][ERROR] : Could not add user to database, " + ex.Message);
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
				case UserItemEnums.UpdateFriends:
					List<AttributeValue> newFriend = new List<AttributeValue>();
					newFriend.Add(new AttributeValue(newValue));
					updateAttribValues.Add(":newFriend", new AttributeValue { L = newFriend });
					updateAtribNames.Add("#F", "Friends");
					updateExpression = "SET #F = list_append(#F, :newFriend)";
					break;
				case UserItemEnums.UpdateUserName:
					updateAttribValues.Add(":newUserName", new AttributeValue { S = newValue });
					updateAtribNames.Add("#UN", "UserName");
					updateExpression = "SET #UN = :newUserName";
					break;
				default:
					Tools.logger.Error("[UPDATE-USER][ERROR] : Invalid update option provided");
					return false;
			}

			Tools.logger.Info("[UPDATE-USER][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

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
				var response = _client.UpdateItem(request);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[UPDATE-USER][ERROR] : Could not update the user item, + " + ex.Message);
				return false;
			}
		}

		public bool RemoveUser(UserModel oldUser)
		{
			try
			{
				_userTableContext.DeleteItem(oldUser.UserId.ToString());
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
				return false;
			}
		}

		public bool RemoveUser(string primaryKey)
		{
			try
			{
				_userTableContext.DeleteItem(primaryKey);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
				return false;
			}
		}

		public string GetUser(string primaryKey)
		{
			try
			{
				Document doc = _userTableContext.GetItem(primaryKey);
				return doc.ToJson();
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
				return string.Empty;
			}
		}

		public UserModel GetUser(UserModel user)
		{
			try
			{
				Document doc = _userTableContext.GetItem(user.UserId.ToString());
				var jsonDoc = doc.ToJson();
				return (UserModel)JsonConvert.DeserializeObject(jsonDoc, typeof(UserModel));
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
				return null;
			}
		}

		public bool UserExists(UserModel user)
		{
			Document doc = null;
			try
			{
				doc = _userTableContext.GetItem(user.UserId.ToString());
				return !(doc == null);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
				return false;
			}
		}

		public bool UserExists(string userId)
		{
			Document doc = null;
			try
			{
				doc = _userTableContext.GetItem(userId);
				return !(doc == null);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
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
				Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for user with username \'" + userName + "\'");
				response = _client.Query(request);
			}
			catch (Exception ex)
			{
				Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
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
				Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for users with name \'" + name + "\'");
				response = _client.Query(request);
			}
			catch (Exception ex)
			{
				Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
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
				Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for users with UserName \'" + userName + "\'");
				response = _client.Query(request);
			}
			catch (Exception ex)
			{
				Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
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
				_tripTableContext.PutItem(Document.FromJson(newTrip.SerializeToJson()));
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[ADD NEW TRIP][ERROR] : Could not add trip to database, " + ex.Message);
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
				case TripItemEnums.UpdateParticipants:
					List<AttributeValue> newParticipants = new List<AttributeValue>();
					newParticipants.Add(new AttributeValue(newValue));
					updateAttribValues.Add(":newParts", new AttributeValue { L = newParticipants });
					updateAtribNames.Add("#P", "Participants");
					updateExpression = "SET #P = list_append(#P, :newParts)";
					break;
				case TripItemEnums.UpdateMoments:
					List<AttributeValue> newMoments = new List<AttributeValue>();
					newMoments.Add(new AttributeValue(newValue));
					updateAttribValues.Add(":newMoment", new AttributeValue { L = newMoments });
					updateAtribNames.Add("#M", "Moments");
					updateExpression = "SET #M = list_append(#M, :newMoment)";
					break;
				default:
					Tools.logger.Error("[UPDATE-TRIP][ERROR] : Invalid update option provided");
					return false;
			}

			Tools.logger.Info("[UPDATE-USER][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

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
				var response = _client.UpdateItem(request);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[UPDATE-TRIP][ERROR] : Could not update the user item, + " + ex.Message);
				return false;
			}
		}

		public bool RemoveTrip(TripModel oldTrip)
		{
			try
			{
				_tripTableContext.DeleteItem(oldTrip.TripId.ToString());
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from database, " + ex.Message);
				return false;
			}
		}

		public bool RemoveTrip(string primaryKey)
		{
			try
			{
				_tripTableContext.DeleteItem(primaryKey);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[REMOVE TRIP][ERROR] : Could not remove trip from database, " + ex.Message);
				return false;
			}
		}

		public string GetTrip(string primaryKey)
		{
			try
			{
				Document doc = _tripTableContext.GetItem(primaryKey);
				return doc.ToJson();
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[GET TRIP][ERROR] : Error occurred while locating trip, " + ex.Message);
				return string.Empty;
			}
		}

		public TripModel GetTrip(TripModel trip)
		{
			try
			{
				Document doc = _userTableContext.GetItem(trip.TripId.ToString());
				var jsonDoc = doc.ToJson();
				return (TripModel)JsonConvert.DeserializeObject(jsonDoc, typeof(TripModel));
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[GET TRIP][ERROR] : Error occurred while locating trip, " + ex.Message);
				return null;
			}
		}

		public bool TripExists(TripModel trip)
		{
			Document doc = null;
			try
			{
				doc = _userTableContext.GetItem(trip.TripId.ToString());
				return !(doc == null);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[TRIP EXISTS][ERROR] : Error occurred while locating trip, " + ex.Message);
				return false;
			}
		}

		public bool TripExists(string tripId)
		{
			Document doc = null;
			try
			{
				doc = _tripTableContext.GetItem(tripId);
				return !(doc == null);
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[TRIP EXISTS][ERROR] : Error occurred while locating trip, " + ex.Message);
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
				Tools.logger.Info("[DB-CALL-WRAPPER][NOTE] : Querying for trips with name \'" + tripName + "\'");
				response = _client.Query(request);
			}
			catch (Exception ex)
			{
				Tools.logger.Info("[DB-CALL-WRAPPER][ERROR] : Query failed, " + ex.Message);
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

		#region MOMENTS_TABLE_WRAPPERS

		public string GetMoment(string momentId)
		{
			try
			{
				Document doc = _momentTableContext.GetItem(momentId);
				return doc.ToJson();
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[GET MOMENT][ERROR] : Error occurred while locating moment, " + ex.Message);
				return string.Empty;
			}
		}

		public bool AddNewMoment(MomentModel newMoment, Guid tripId)
		{
			// add the moment to the Db and update the trip to have the location of the moment
			try
			{
				_momentTableContext.PutItem(Document.FromJson(newMoment.SerializeToJson()));
				UpdateTrip(tripId.ToString(), newMoment.Guid.ToString(), TripItemEnums.UpdateMoments);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[ADD NEW MOMENT][ERROR] : Could not add moment to database, " + ex.Message);
				return false;
			}
		}

		public bool DeleteMoment(string momentId)
		{
			try
			{
				_momentTableContext.DeleteItem(momentId);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[REMOVE MOMENT][ERROR] : Could not remove moment from database, " + ex.Message);
				return false;
			}
		}

		public bool UpdateMoment(string momentId, string newValue, MomentItemEnums updateType)
		{
			Dictionary<string, AttributeValue> updateAttribValues = new Dictionary<string, AttributeValue>();
			Dictionary<string, string> updateAtribNames = new Dictionary<string, string>();
			string updateExpression;

			switch (updateType)
			{
				case MomentItemEnums.UpdateTitle:
					updateAttribValues.Add(":newTitle", new AttributeValue { S = newValue });
					updateAtribNames.Add("#T", "Title");
					updateExpression = "SET #T = :newTitle";
					break;
				case MomentItemEnums.UpdateCreator:
					updateAttribValues.Add(":newCreator", new AttributeValue { S = newValue });
					updateAtribNames.Add("#C", "Creator");
					updateExpression = "SET #C = :newCreator";
					break;
				case MomentItemEnums.UpdateLatitude:
					updateAttribValues.Add(":newLong", new AttributeValue { S = newValue });
					updateAtribNames.Add("#LG", "Longitude");
					updateExpression = "SET #LG = :newLong";
					break;
				case MomentItemEnums.UpdateLongitude:
					updateAttribValues.Add(":newLat", new AttributeValue { S = newValue });
					updateAtribNames.Add("#LT", "Latitude");
					updateExpression = "SET #LT = :newLat";
					break;
				default:
					Tools.logger.Error("[UPDATE-MOMENT][ERROR] : Invalid update option provided");
					return false;
			}

			Tools.logger.Info("[UPDATE-MOMENT][NOTE] : Query : " + updateExpression + " NEW-VALUE : " + newValue);

			var request = new UpdateItemRequest
			{
				TableName = m_MomentTableName,
				Key = new Dictionary<string, AttributeValue>() { { "Guid", new AttributeValue { S = momentId } } },
				ExpressionAttributeNames = updateAtribNames,
				ExpressionAttributeValues = updateAttribValues,
				UpdateExpression = updateExpression
			};

			try
			{
				var response = _client.UpdateItem(request);
				return true;
			}
			catch (Exception ex)
			{
				Tools.logger.Error("[UPDATE-USER][ERROR] : Could not update the moment item, + " + ex.Message);
				return false;
			}
		}
		#endregion

	}
}
