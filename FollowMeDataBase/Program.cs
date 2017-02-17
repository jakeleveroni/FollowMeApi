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
            //DBCallWrappers.DB db = new DBCallWrappers.DB();

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

            UserModel um = new UserModel(Guid.NewGuid(), "jleveroni", "jacob", "jake@fm.com", "pass", new DateTime(), 100);
            Console.WriteLine(um.SerializeToJson());

            //Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}

