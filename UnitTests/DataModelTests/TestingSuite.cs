using System;
using System.Collections.Generic;
using FollowMeDataBase.DBCallWrappers;
using FollowMeDataBase.Models;
using Newtonsoft.Json;

namespace UnitTests
{
	public class TestingSuite
	{
		private readonly DB db;

		public TestingSuite()
		{
			db = new DB();
		}

		#region USER_TESTING
		// returns the UserModel created
		public UserModel CreateUserModel()
		{
			UserModel um = new UserModel(Guid.NewGuid(), "Test0", "Test0Name", "test@test.net", "testpass", new DateTime(), 0);
			db.AddNewUser(um);
			return um;
		}

		// returns a list of the created UserModels
		public List<UserModel> CreateUserModels(int numUsers)
		{
			List<UserModel> userList = new List<UserModel>();

			for (int i = 0; i < numUsers; ++i)
			{
				UserModel um = new UserModel(Guid.NewGuid(), "Test" + i, "Test" + i + "Name", "test" + 1 + "@test.net", "testpass" + i, new DateTime(), (ulong)i);
				db.AddNewUser(um);
				userList.Add(um);
			}

			return userList;
		}

		// removes the specified user
		public bool RemoveUser(UserModel user)
		{
			return db.RemoveUser(user.UserId.ToString());
		}

		// removes the specified users
		public bool RemoveUsers(List<UserModel> users)
		{
			foreach (var user in users)
			{
				if (!db.RemoveTrip(user.UserId.ToString()))
				{
					return false;
				}
			}

			return true;
		}

		// check if user exists in db
		public bool QueryForUser(Guid id)
		{
			return db.UserExists(id.ToString());
		}

		// check if multiple users are in the db
		public bool QueryForUsers(List<Guid> ids)
		{
			foreach (var id in ids)
			{
				if (!db.UserExists(id.ToString()))
				{
					return false;
				}
			}

			return true;
		}

		// queries the db and compares the result
		public bool CompareUser(UserModel user)
		{
			UserModel um = JsonConvert.DeserializeObject<UserModel>(db.GetUser(user.UserId.ToString()));
			return user == um;
		}

		// queries the db for he provided users and compares them to the results
		public bool CompareUsers(List<UserModel> users)
		{
			foreach (var user in users)
			{
				UserModel um = JsonConvert.DeserializeObject<UserModel>(db.GetUser(user.UserId.ToString()));

				if (um != user)
				{
					return false;
				}
			}

			return true;
		}
		#endregion

		#region TRIP_TESTING
		// adds a trip to the udatabase and returns its Guid
		public Guid CreateTrip()
		{
			TripModel tm = new TripModel(Guid.NewGuid(), "Test0", 100, "Test trip 0");
			db.AddNewTrip(tm);
			return tm.TripId;
		}


		// adds specified number of trips to the database and returns a list of the Guids
		public List<Guid> CreateTrips(int numTrips)
		{
			List<Guid> guidList = new List<Guid>();

			for (int i = 0; i < numTrips; ++i)
			{
				guidList.Add(Guid.NewGuid());
				TripModel tm = new TripModel(guidList[i], "Test" + i, 100 + (ulong)i, "Test trip " + i);
				db.AddNewTrip(tm);
			}

			return guidList;
		}

		// Removes a trip from the db
		public bool RemoveTrip(Guid id)
		{
			return db.RemoveTrip(id.ToString());
		}

		// removes multiple trips specified by their ids
		public bool RemoveTrips(List<Guid> ids)
		{
			foreach (var id in ids)
			{
				if (!db.RemoveTrip(id.ToString()))
				{
					return false;
				}
			}

			return true;
		}

		// Queries the db for a specific trip id
		public bool QueryForTrip(Guid id)
		{
			return db.TripExists(id.ToString());
		}

		// Queies teh db for the specified trip ids
		public bool QuerieForTrips(List<Guid> ids)
		{
			foreach (var id in ids)
			{
				if (!db.TripExists(id.ToString()))
				{
					return false;
				}
			}

			return true;
		}

		// compares the specified trip with its db entry
		public bool CompareTrip(TripModel trip)
		{
			TripModel tm = JsonConvert.DeserializeObject<TripModel>(db.GetTrip(trip.TripId.ToString()));
			return trip == tm;
		}

		// Compares the specified trips with the their db entries
		public bool CompareTrips(List<TripModel> trips)
		{
			foreach (var trip in trips)
			{
				TripModel tm = JsonConvert.DeserializeObject<TripModel>(db.GetTrip(trip.TripId.ToString()));

				if (trip != tm)
				{
					return false;
				}
			}

			return true;
		}
		#endregion

		#region CONTENT_CREATION
		// creates a new user and a new trip with that users account
		public bool CreateUserAndAddTrip()
		{
			UserModel um = new UserModel(Guid.NewGuid(), "CreateUserAndAddTrip", "CUAAT", "CUAAT@fm.com", "pass", new DateTime(), 1);
			TripModel tm = new TripModel(Guid.NewGuid(), "CUAATTrip", 25, "Created a user and added a trip");

			if (!db.AddNewUser(um))
			{
				Console.WriteLine("[CUAAT][ERROR] : Could not create the user");
				return false;
			}

			if (!db.AddNewTrip(tm))
			{
				Console.WriteLine("[CUAAT][ERROR] : Could not create the trip");
				return false;
			}

			if (!db.UpdateUser(um.UserId.ToString(), tm.TripId.ToString(), Utility.UserItemEnums.UpdateTrips))
			{
				Console.WriteLine("[CUAAT][ERROR] : Could not add trip id to user model");
				return false;
			}

			if (um != JsonConvert.DeserializeObject<UserModel>(db.GetUser(um.UserId.ToString())))
			{
				Console.WriteLine("[CUAAT][ERROR] : User model created does not match the queried one");
				return false;
			}

			if (tm != JsonConvert.DeserializeObject<TripModel>(db.GetTrip(tm.TripId.ToString())))
			{
				Console.WriteLine("[CUAAT][ERROR] : Trip model created does not match the queried one");
				return false;
			}

			return true;
		}

		// creates a new user and a new trip with that users account
		public bool CreateUserAndAddTrip(UserModel um, TripModel tm)
		{
			if (!db.AddNewUser(um))
			{
				Console.WriteLine("[CUAAT][ERROR] : Could not create the user");
				return false;
			}

			if (!db.AddNewTrip(tm))
			{
				Console.WriteLine("[CUAAT][ERROR] : Could not create the trip");
				return false;
			}

			if (!db.UpdateUser(um.UserId.ToString(), tm.TripId.ToString(), Utility.UserItemEnums.UpdateTrips))
			{
				Console.WriteLine("[CUAAT][ERROR] : Could not add trip id to user model");
				return false;
			}

			if (um != JsonConvert.DeserializeObject<UserModel>(db.GetUser(um.UserId.ToString())))
			{
				Console.WriteLine("[CUAAT][ERROR] : User model created does not match the queried one");
				return false;
			}

			if (tm != JsonConvert.DeserializeObject<TripModel>(db.GetTrip(tm.TripId.ToString())))
			{
				Console.WriteLine("[CUAAT][ERROR] : Trip model created does not match the queried one");
				return false;
			}

			return true;
		}

		// Removes a trip from the db and the users associated with it
		public bool RemoveTripFromUser(UserModel um, TripModel tm)
		{
			if (!db.RemoveTrip(tm))
			{
				Console.WriteLine("[RTFU][ERROR] : Could not remove trip for db");
				return false;
			}

			if (!db.UpdateUser(um.UserId.ToString(), "null", Utility.UserItemEnums.UpdateTrips))
			{
				Console.WriteLine("[RTFU][ERROR] : Could not modify user TripId list");
				return false;
			}

			if (db.TripExists(tm.TripId.ToString()))
			{
				Console.WriteLine("[RTFU][ERROR] : Trip was not removed");
				return false;
			}

			return true;
		}
		#endregion
	}
}
