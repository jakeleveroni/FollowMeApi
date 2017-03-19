using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.SessionState;
using Utility;

namespace FollowMeAPI.Sessions
{
    public static class SessionManager
    {
        public static SessionIDManager m_idManager;
        private static HashSet<string> m_activeSessionTokens = new HashSet<string>(StringComparer.InvariantCulture);
        private static Dictionary<string, FollowMeSession> m_sessions = new Dictionary<string, FollowMeSession>();

        public static void FlushSessions()
        {
            m_sessions.Clear();
            m_activeSessionTokens.Clear();
        }

        public static List<string> GetActiveTokens()
        {
            string[] tmpArr = null;
            m_activeSessionTokens.CopyTo(tmpArr);
            return tmpArr.ToList();
        }

        public static bool AddNewSession(FollowMeSession sesh)
        {
            if (!m_activeSessionTokens.Contains(sesh.AuthenticationCreds.AWSCredentials.SessionToken))
            {
                m_activeSessionTokens.Add(sesh.AuthenticationCreds.AWSCredentials.SessionToken);
                m_sessions.Add(sesh.SessionId, sesh);
                return true;
            }

            return false;
        }

        public static bool RemoveSession(FollowMeSession sesh)
        {
            if (m_activeSessionTokens.Contains(sesh.AuthenticationCreds.AWSCredentials.SessionToken))
            {
                try
                {
                    m_activeSessionTokens.Remove(sesh.AuthenticationCreds.AWSCredentials.SessionToken);
                    m_sessions.Remove(sesh.SessionId);
                    return true;
                }
                catch
                {
                    Tools.logger.Error("[SESSION-MANAGER][ERROR] : Could not remove the session, as it was not found");
                    return false;
                }
            }

            return false;
        }

        public static bool ValidateSession(string sessionToken)
        {
			// TODO: This is not final code the session tokens werent working so were by passing them at the time
			return true;

            //if (sessionToken != null && m_activeSessionTokens.Contains(sessionToken))
            //{
            //    return true;
            //}

            //return false;
        }


    }
}