using System;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using FollowMeDataBase.DBCallWrappers;
using Newtonsoft.Json;

namespace UnitTests
{
	public class TripModelTest
	{
		private DB db;
		public TripModelTest()
		{
			db = new DB();
		}

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
	}
}
