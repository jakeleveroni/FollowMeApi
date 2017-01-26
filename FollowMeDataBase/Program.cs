using System;
using System.Collections.Generic;
using FollowMeDataBase.Models;
using Utility;

namespace FollowMeDataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize db 
            DBCallWrappers.DB db = new DBCallWrappers.DB();
<<<<<<< HEAD
            Console.WriteLine("Database successfully initialized");

            //Console.WriteLine("Database read/write operations being tested");
            //db.AddNewUser(user);
            //bool x = db.RemoveUser(user);
            //db.FindUser(new Guid("00000000-0000-0000-0000-000000000000"));
            //db.RemoveUser(new Guid("00000000-0000-0000-0000-000000000000"));
            //UserModel model = db.GetUser(user.UserId);
            //db.UserExists(new Guid("00000000-0000-0000-0000-000000000001"));
            //user.AddNewTripId("Second Trip");
            //user.TotalMilesTraveled = 1001;
            //user.NumberOfTrips = 2;
			db.UpdateUser("00000000-0000-0000-0000-000000000000", "Third Trip, Added On The Fly", UserItemEnums.UpdateTrips);

            // STOP : TESTING DB WRAPPERS
=======
>>>>>>> master

            // create user
            Guid id = Guid.NewGuid();
            UserModel um = new Models.UserModel(id, "drew.cortright", "Drew Cortright", "dc@followme.com", "pass", new DateTime(), 100);

            // create trip ids
            Guid tripId1 = Guid.NewGuid();
            TripModel trip1 = new TripModel(tripId1, "Grand Canyon", 65, "It's a big hole in the ground...");

            Guid tripId2 = Guid.NewGuid();
            TripModel trip2 = new TripModel(tripId2, "Scranton Pennsylvania", 257, "Surprisingly prevalent paper company in this town");

            // add trip ids to user
            um.AddNewTripId(tripId1.ToString());
            um.AddNewTripId(tripId2.ToString());

            // write all contents to database
            db.AddNewUser(um);
            db.AddNewTrip(trip1);
            db.AddNewTrip(trip2);

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}

