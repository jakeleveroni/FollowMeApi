using System;
using System.Collections.Generic;
using FollowMeDataBase.Models;

namespace FollowMeDataBase
{
    class Program
            //Console.WriteLine("\nUser.Serialize()\n");
            //Console.WriteLine(user.SerializeToJson());
            // END : TESTING SERIALIZATION OF THE DB MODELS

            // START : TESTING DB WRAPPERS
    {
        static void Main(string[] args)
        {
            // START : TESTING SERIALIZATION OF THE DB MODELS
            UserModel user = new UserModel(new Guid(), "jakeleveroni", "Jacob", "jacobjleveroni@gmail.com", "welcome1", new DateTime(), 0);
            user.AddNewTripId("First Trip");
            //Console.WriteLine("UserModel.ToString()\n");
            //Console.WriteLine(user.ToString());

            Console.WriteLine("Initializing the Database");
            DBCallWrappers.DB db = new DBCallWrappers.DB();
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
            db.UpdateUser(new Guid("00000000-0000-0000-0000-000000000000"), "Third Trip, Added On The Fly", UserItemEnums.UpdateTrips);

            // STOP : TESTING DB WRAPPERS


            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}

