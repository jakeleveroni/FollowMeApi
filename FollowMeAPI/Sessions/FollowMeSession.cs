using System;
using Utility;
using System.Collections.Generic;
using System.Web.SessionState;
using LogInManager.Login;
using FollowMeDataBase.Models;
using FollowMeDataBase.DBCallWrappers;
using StorageManager.S3Wrapper;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FollowMeAPI.Sessions
{
    public class FollowMeSession
    {
        // SessionIdManager used for generating session ids
		private SessionIDManager m_sessionManager;

		// the session ID, used to be able to access a users session
        public string SessionId;

        // All authentication credentials both for the API as well as temporary AWS credentials
        public AuthInfo AuthenticationCreds { get; set; }

        // users custom gateway to the db
        public DB DBWrapper;
        private DB m_tmpDb;

        // users custom gateway to the s3 
        public S3 StorageManager;

        // users data model
        public UserModel User;

        private string m_userName;
		private string m_password;

        // constructor, creates a complete user session 
        public FollowMeSession(string userName, string password, DB db)
        {
            m_userName = userName;
            m_password = password;
            m_tmpDb = db;
        }

        public AuthInfo CreateUserSession()
        {
            AuthenticationCreds = null;

            using (CustomLogInManager loginManager = new CustomLogInManager(m_userName, m_password, m_tmpDb))
            {
                AuthenticationCreds = loginManager.AuthenticateUserInApp();
                m_tmpDb = null;

                 

                if (AuthenticationCreds.StatusCode == FolloMeErrorCodes.AWSAndAPIVerified)
                {
                    SessionId = Guid.NewGuid().ToString();
                    return AuthenticationCreds;
                }
                else
                {
                    Tools.logger.Error("[FOLLOW-ME-SESSION][ERROR] : Could not authenticate the users credentials, FolloMeErrorCode - " + AuthenticationCreds.StatusCode);
                    return AuthenticationCreds;
                }
            }
        }

        public bool IsAuthed()
        {
            return (AuthenticationCreds.StatusCode == FolloMeErrorCodes.AWSAndAPIVerified);
        }
    }
}