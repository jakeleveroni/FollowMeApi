using System;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using FollowMeDataBase.Models;
using FollowMeDataBase.DBCallWrappers;
using Utility;

namespace CognitoManager
{
    public class CustomLogInManager : IDisposable
    {
        private AmazonCognitoIdentityClient m_cognitoClient;
        private DB m_db;
        private string m_userPool;
        private string m_federationId;

        CustomLogInManager()
        {
            Initialize();
        }

        public bool Initialize()
        {
            try
            {
                Logger.logger.Info("[CUSTOM-LOGIN-MANAGER][NOTE] : Creating Cognito client now...");
                m_cognitoClient = new AmazonCognitoIdentityClient(Amazon.RegionEndpoint.USWest2);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : Could not create Cognito client, " + ex.Message);
                return false;
            }

            try
            {
                Logger.logger.Info("[CUSTOM-LOGIN-MANAGER][NOTE] : Creating DB reference now...");
                m_db = new DB();
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : Could not create DB connection, " + ex.Message);
                return false;
            }

            return true;

        }

        // returns the auth info for the user, the auth info will have 
        // a false Authenticated value if query fails
        private AuthInfo AuthenticateUserInApp(string username, string password)
        {
            UserModel um = null;

            //um = m_db.FindUserByUserNameAndPassword(username, password);

            if (um != null)
            {
                return new AuthInfo(username, password, um.UserId.ToString());
            }
            else
            {
                Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : User does not exist, must create a new account first");
                return new AuthInfo();
            }
        }

        public void Dispose()
        {
            m_cognitoClient.Dispose();
            m_db.Dispose();
        }
    }


}
