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
<<<<<<< HEAD
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
=======
            //DBCallWrappers.DB db = new DBCallWrappers.DB();
>>>>>>> master

            //// create user
            //Guid id = Guid.NewGuid();
            //UserModel um = new UserModel(id, "drew.cortright", "Drew Cortright", "dc@followme.com", "pass1", new DateTime(), 100);

            //// create trip ids
            //Guid tripId1 = Guid.NewGuid();
            //TripModel trip1 = new TripModel(tripId1, "Grand Canyon", 65, "It's a big hole in the ground...");

            //Guid tripId2 = Guid.NewGuid();
            //TripModel trip2 = new TripModel(tripId2, "Scranton Pennsylvania", 257, "Surprisingly prevalent paper company in this town");

            //// add trip ids to user
            //um.AddNewTripId(trip1.TripId.ToString());
            //um.AddNewTripId(trip2.TripId.ToString());

            //// write all contents to database
            //db.AddNewUser(um);
            //db.AddNewTrip(trip1);
            //db.AddNewTrip(trip2);

            //List<UserModel> l1 = new List<UserModel>();
            //List<UserModel> l2 = new List<UserModel>();
            //List<UserModel> l3 = new List<UserModel>();
            //List<UserModel> l4 = new List<UserModel>();

            //l1 = db.QueryUsersByUserNameAndPassword("drew.cortright", "pass1");
            //l2 = db.QueryUsersByName("Eric Garza");
            //l3 = db.QueryUsersByUserName("drew.cortright");
            //l4 = db.QueryUsersByUserName("jacob.leveroni");

            //Console.WriteLine("Press any key to exit...");
            //Console.Read();
        }
    }
}

