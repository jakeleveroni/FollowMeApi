using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FollowMeDataBase.DBCallWrappers;
using FollowMeDataBase.Models;
using Newtonsoft.Json;

namespace UnitTests.DataModelTests.UserModelTests
{
    public class UserModelTest
    {
        private DB db;

        UserModelTest()
        {
            db = new DB();
        }

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
                UserModel um = new UserModel(Guid.NewGuid(), "Test" + i, "Test" + i + "Name", "test" + 1+ "@test.net", "testpass" + i, new DateTime(), (ulong)i);
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
    }
}
