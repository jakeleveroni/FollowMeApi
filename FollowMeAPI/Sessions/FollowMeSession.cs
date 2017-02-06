using System;
using Utility;
using System.Web;
using System.Web.SessionState;
using LogInManager.Login;
using FollowMeDataBase.Models;
using FollowMeDataBase.DBCallWrappers;

namespace FollowMeAPI.Sessions
{
    public class FollowMeSession
    {
        private CustomLogInManager m_loginManager;
        private SessionIDManager m_sessionManager;
        public string SessionId;
        public AuthInfo AuthenticationCreds { get; }
        private DB m_db;
        public UserModel User;

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
                m_db = new DB();

                if (AuthenticationCreds != null)
                {
                    User = (m_db.QueryUsersByUserNameAndPassword(userName, password))[0];
                }
            }
            catch (Exception ex)
            {
                Logger.logger.Error("[SESSION][ERROR] : Could not create the sessions database connection or could not retrieve specified user, " + ex.Message);
                return;
            }
            
            SessionId = m_sessionManager.GetSessionID(HttpContext.Current);
        }
    }
}