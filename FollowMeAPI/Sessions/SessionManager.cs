using System;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Linq;
using System.Web;
using Utility;

namespace FollowMeAPI.Sessions
{
    public static class SessionManager
    {
        public static SessionIDManager m_idManager;
        private static HashSet<string> m_activeSessionIds = new HashSet<string>(StringComparer.InvariantCulture);
        private static Dictionary<string, FollowMeSession> m_sessions = new Dictionary<string, FollowMeSession>();

        public static bool AddNewSession(FollowMeSession sesh)
        {
            if (!m_activeSessionIds.Contains(sesh.SessionId))
            {
                m_activeSessionIds.Add(sesh.SessionId);
                m_sessions.Add(sesh.SessionId, sesh);
                return true;
            }

            return false;
        }

        public static bool RemoveSession(FollowMeSession sesh)
        {
            if (m_activeSessionIds.Contains(sesh.SessionId))
            {
                m_activeSessionIds.Remove(sesh.SessionId);

                try
                {
                    m_sessions.Remove(sesh.SessionId);
                    return true;
                }
                catch
                {
                    Logger.logger.Error("[SESSION-MANAGER][ERROR] : Could not remove the session, as it was not found");
                    return false;
                }
            }

            return false;
        }


    }
}