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
                Console.WriteLine("\nError: failed to create a DynamoDB config; " + ex.Message);
                return;
            }

            try
            {
                client = new AmazonDynamoDBClient(ddbConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: failed to create a DynamoDB client; " + ex.Message);
                return;
            } 

            // get description references to all of our databases
            LoadTables();
        }

        private void LoadTables()
        {
            Console.WriteLine("\n*** Retrieving table information ***");

            try
            {
                m_userTableContext = Table.LoadTable(client, m_userTableName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[LOAD TABLES][ERROR] : Could not load the Users table, " + ex.Message);
                return;
            }
            #region DEBUG_LOAD_TABLE
            // DEEBUG LOGGING
            //Console.WriteLine("Name: {0}", m_userTable.TableName);
            //Console.WriteLine("# of items: {0}", m_userTable.ItemCount);
            //Console.WriteLine("Provision Throughput (reads/sec): {0}", m_userTable.ProvisionedThroughput.ReadCapacityUnits);
            //Console.WriteLine("Provision Throughput (writes/sec): {0}", m_userTable.ProvisionedThroughput.WriteCapacityUnits);

            //Console.WriteLine("Name: {0}", m_tripTable.TableName);
            //Console.WriteLine("# of items: {0}", m_tripTable.ItemCount);
            //Console.WriteLine("Provision Throughput (reads/sec): {0}", m_tripTable.ProvisionedThroughput.ReadCapacityUnits);
            //Console.WriteLine("Provision Throughput (writes/sec): {0}", m_tripTable.ProvisionedThroughput.WriteCapacityUnits);
            #endregion
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
                Console.WriteLine("[ADD NEW USER][ERROR] : Could not add user to database, " + ex.Message);
                return false;
            }
        }

        public bool UpdateExistingUser(UserModel userToUpdate)
        {
            try
            {
                m_userTableContext.UpdateItem(Document.FromJson(userToUpdate.SerializeToJson()), userToUpdate.UserId.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UPDATE EXISTING USER][ERROR] : Issue updating specified user, " + ex.Message);
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
                Console.WriteLine("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
                return false;
            }
        }

        public bool RemoveUser(Guid primaryKey)
        {
            try
            {
                m_userTableContext.DeleteItem(primaryKey.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
                return false;
            }
        }

        public UserModel GetUser(Guid primaryKey)
        {
            try
            {
                Document doc = m_userTableContext.GetItem(primaryKey.ToString());
                var jsonDoc = doc.ToJson();
                return (UserModel)JsonConvert.DeserializeObject(jsonDoc, typeof(UserModel));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return null;
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
                Console.WriteLine("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
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
                Console.WriteLine("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return false;
            }
        }

        public bool UserExists(Guid userId)
        {
            Document doc = null;
            try
            {
                doc = m_userTableContext.GetItem(userId.ToString());
                return !(doc == null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[USER EXISTS][ERROR] : Error occurred while locating user, " + ex.Message);
                return false;
            }
        }
        #endregion
    }
}
