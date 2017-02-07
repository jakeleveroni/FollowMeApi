using System;
using Utility;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using LogInManager.Login;
using FollowMeDataBase.Models;
using FollowMeDataBase.DBCallWrappers;
using StorageManager.S3Wrapper;
using Newtonsoft.Json;

namespace FollowMeAPI.Sessions
{
    public class FollowMeSession
    {
        // Login manager used for gettting user temporary AWS credentials
        private CustomLogInManager m_loginManager;

        // SessionIdManager used for generating session ids
        private SessionIDManager m_sessionManager;

        // the session ID, used to be able to access a users session
        public string SessionId;

        // All authentication credentials both for the API as well as temporary AWS credentials
        public AuthInfo AuthenticationCreds { get; }

        // users custom gateway to the db
        public DB DBManager;

        // users custom gateway to the s3 
        public S3 StorageManager;

        // users data model
        public UserModel User;

        // constructor, creates a complete user session 
        public FollowMeSession(string userName, string password)
        {
            AuthenticationCreds = null;
            m_sessionManager = new SessionIDManager();

            try
            {
                m_loginManager = new CustomLogInManager();
                AuthenticationCreds = m_loginManager.AuthenticateUserInApp(userName, password);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[SESSION][ERROR] : Could not create the sessions log in manager or authenticate user, " + ex.Message);
                return;
            }

            try 
            {
                if (AuthenticationCreds != null)
                {
                    User = JsonConvert.DeserializeObject<UserModel>(WebApiApplication.db.GetUser(AuthenticationCreds.Id));
                }
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[SESSION][ERROR] : Could not create the sessions database connection or could not retrieve specified user, " + ex.Message);
                return;
            }

            try
            {
                DBManager = new DB(AuthenticationCreds.AWSCredentials.AccessKey, AuthenticationCreds.AWSCredentials.SecretAccessKey, AuthenticationCreds.AWSCredentials.SessionToken);
                StorageManager = new S3(AuthenticationCreds.AWSCredentials.AccessKey, AuthenticationCreds.AWSCredentials.SecretAccessKey, AuthenticationCreds.AWSCredentials.SessionToken);
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[SESSION][ERROR] : Could not create db or s3 context for user, " + ex.Message);
                return;
            }
            
            SessionId = m_sessionManager.CreateSessionID(HttpContext.Current);
        }
    }
}