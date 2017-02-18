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
        // SessionIdManager used for generating session ids
        private SessionIDManager m_sessionManager;

        // the session ID, used to be able to access a users session
        public string SessionId;

        // All authentication credentials both for the API as well as temporary AWS credentials
        public AuthInfo AuthenticationCreds { get; set; }

        // users custom gateway to the db
        public DB DBManager;

        // users custom gateway to the s3 
        public S3 StorageManager;

        // users data model
        public UserModel User;

        private string m_userName;
        private string m_password;

        // user aws token 


        // constructor, creates a complete user session 
        public FollowMeSession(string userName, string password)
        {
            m_userName = userName;
            m_password = password;
        }

        public AuthInfo CreateUserSession()
        {
            AuthenticationCreds = null;

            using (CustomLogInManager loginManager = new CustomLogInManager(m_userName, m_password))
            {
                AuthenticationCreds = loginManager.AuthenticateUserInApp();

                if (AuthenticationCreds.StatusCode == FolloMeErrorCodes.AWSAndAPIVerified)
                {
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