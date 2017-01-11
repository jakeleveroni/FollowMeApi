///*
// * The DB class defines a handful of helpful Database read and write actions
// * sepcific to the FollowMe application (wrapper for AWS.DynoDB native functions)
// * 
// * Created By: Jacob Leveroni
// * Created On: 1/8/2017
//*/

//using System;
//using System.Collections.Generic;
//using FollowMeDataBase.Models;
//using Amazon.DynamoDBv2;
//using Amazon.DynamoDBv2.Model;
//using Amazon.DynamoDBv2.DocumentModel;
//using Amazon.Runtime;

//namespace FollowMeDataBase.DBCallWrappers
//{
//    public class DB
//    {
//        AmazonDynamoDBConfig ddbConfig;
//        AmazonDynamoDBClient client;
//        private Table m_userTableContext;
//        private Table m_tripTableContext;
//        private readonly string m_userTableName = "Users";
//        private readonly string m_tripTableName = "Trips";

//        // sets up the DynamoDB connection 
//        public DB()
//        {
//            Initialize();
//        }

//        public void Initialize()
//        {
//            // Initialize the database 
//            try
//            {
//                ddbConfig = new AmazonDynamoDBConfig();
//                ddbConfig.ServiceURL = "https://dynamodb.us-west-2.amazonaws.com/";
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("\nError: failed to create a DynamoDB config; " + ex.Message);
//                return;
//            }

//            try
//            {
//                client = new AmazonDynamoDBClient(ddbConfig);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("\nError: failed to create a DynamoDB client; " + ex.Message);
//                return;
//            }

//            // get description references to all of our databases
//            LoadTables();
//        }

//        private void LoadTables()
//        {
//            Console.WriteLine("\n*** Retrieving table information ***");

//            try
//            {
//                m_userTableContext = Table.LoadTable(client, m_userTableName);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("[LOAD TABLES][ERROR] : Could not load the Users table");
//                return;
//            }

//            //try
//            //{
//            //    m_tripTable = Table.LoadTable(client, m_tripTableName);
//            //}
//            //catch (Exception ex)
//            //{
//            //    Console.WriteLine("[LOAD TABLES][ERROR] : Could not load the Trips table");
//            //    return;
//            //}

//            // DEEBUG LOGGING
//            //Console.WriteLine("Name: {0}", m_userTable.TableName);
//            //Console.WriteLine("# of items: {0}", m_userTable.ItemCount);
//            //Console.WriteLine("Provision Throughput (reads/sec): {0}", m_userTable.ProvisionedThroughput.ReadCapacityUnits);
//            //Console.WriteLine("Provision Throughput (writes/sec): {0}", m_userTable.ProvisionedThroughput.WriteCapacityUnits);

//            //Console.WriteLine("Name: {0}", m_tripTable.TableName);
//            //Console.WriteLine("# of items: {0}", m_tripTable.ItemCount);
//            //Console.WriteLine("Provision Throughput (reads/sec): {0}", m_tripTable.ProvisionedThroughput.ReadCapacityUnits);
//            //Console.WriteLine("Provision Throughput (writes/sec): {0}", m_tripTable.ProvisionedThroughput.WriteCapacityUnits);
//        }

//        public bool AddNewUser(UserModel newUser)
//        {
//            // DEBUG LOGGING
//            //Console.WriteLine("\n*** Adding new user to table ***");

//            //if (!UserExists(newUser))
//            //{
//            //    var request = new PutItemRequest
//            //    {
//            //        TableName = m_userTableName,
//            //        Item = newUser.FormatUserForDB(),
//            //    };

//            //    try
//            //    {
//            //        var response = client.PutItem(request);
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        Console.WriteLine("[ADD USER REQUEST][ERROR] : Could not add user, " + ex.Message);
//            //        return false;
//            //    }

//            //    return true;
//            //}

//            //return false;

//            try
//            {
//                m_userTableContext.PutItem(Document.FromJson(newUser.SerializeToJson()));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("[ADD NEW USER][ERROR] : Could not add user to database, " + ex.Message);
//                return false;
//            }

//            return true;
//        }

//        public bool RemoveUser(UserModel oldUser)
//        {
//            try
//            {
//                m_userTableContext.DeleteItem(oldUser.UserId.ToString());
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("[REMOVE USER][ERROR] : Could not remove user to database, " + ex.Message);
//                return false;
//            }

//            //var request = new DeleteItemRequest
//            //{
//            //    TableName = m_userTableName,
//            //    Key = new Dictionary<string, AttributeValue>()
//            //    {
//            //        {"Guid", new AttributeValue(oldUser.UserId.ToString()) }
//            //    }
//            //};

//            //try
//            //{
//            //    var response = client.DeleteItem(request);
//            //}
//            //catch (Exception ex)
//            //{
//            //    Console.WriteLine("[REMOVE USER REQUEST][ERROR] : Could not remove user, " + ex.Message);
//            //    return false;
//            //}

//            return true;
//        }

//        public bool RemoveUser(Guid primaryKey)
//        {
//            var request = new DeleteItemRequest
//            {
//                TableName = m_userTableName,
//                Key = new Dictionary<string, AttributeValue>()
//                {
//                    {"Guid", new AttributeValue(primaryKey.ToString()) }
//                }
//            };

//            try
//            {
//                var response = client.DeleteItem(request);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("[REMOVE USER REQUEST][ERROR] : Could not remove user, " + ex.Message);
//                return false;
//            }

//            return true;
//        }

//        public bool UserExists(Guid primaryKey)
//        {
//            var request = new GetItemRequest
//            {
//                TableName = m_userTableName,
//                Key = new Dictionary<string, AttributeValue>()
//                {
//                    {"Guid", new AttributeValue(primaryKey.ToString())}
//                }
//            };

//            try
//            {
//                var response = client.GetItem(request);
//                return response.Item.Count > 0;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("[GET ITEM][ERROR] : Failed to retrieve item," + ex.Message);
//                return false;
//            }
//        }

//        public bool UserExists(UserModel userToFind)
//        {
//            var request = new GetItemRequest
//            {
//                TableName = m_userTableName,
//                Key = new Dictionary<string, AttributeValue>()
//                {
//                    {"Guid", new AttributeValue(userToFind.UserId.ToString())}
//                }
//            };

//            try
//            {
//                var response = client.GetItem(request);
//                return response.Item.Count > 0;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("[GET ITEM][ERROR] : Failed to retrieve item," + ex.Message);
//                return false;
//            }
//        }
//    }
//}
