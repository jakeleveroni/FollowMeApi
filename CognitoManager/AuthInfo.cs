namespace CognitoManager
{
    // Super simple class that is used for storing user authentication information 
    //      Also Authenticated will only be true if all the other attributes have been set 
    //      to something other than string.Empty
    public class AuthInfo
    {
        private string m_userName;
        private string m_password;
        private string m_id;
        private bool m_authed;
		private bool m_accountExists;
		private AWSAuthInfo m_awsCreds;

        public string UserName
        {
            get
            {
                return m_userName;
            }
            set
            {
                m_userName = value;
            }
        }
        public string Password
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
        }
        public string Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }
        public bool Authenticated
        {
            get
            {
                if (m_userName != string.Empty && m_password != string.Empty && m_id != string.Empty)
                {
                    return true;
                }
                return false;
            }
            set
            {
				if (value == true && m_userName != string.Empty && m_password != string.Empty && m_id != string.Empty)
				{
					m_authed = value;
				}
				else if (value == false)
				{
					m_userName = string.Empty;
					m_password = string.Empty;
					m_id = string.Empty;
					m_authed = false;
				}
            }
        }

		public bool UserExists
		{
			get
			{
				return m_accountExists;
			}
			set
			{
				m_accountExists = value;
			}
		}

		public AWSAuthInfo AWSCredentials
		{
			get
			{
				return m_awsCreds;
			}
			set
			{
				if (m_awsCreds == null)
				{
					m_awsCreds = new AWSAuthInfo();
					m_awsCreds = value;
				}
			}
		}

        public AuthInfo()
        {
            m_userName = string.Empty;
            m_password = string.Empty;
            m_id = string.Empty;
            m_authed = false;
			m_accountExists = false;
        	
		}

        public AuthInfo(string user, string pass, string id)
        {
            m_userName = user;
            m_password = pass;
            m_id = id;
			m_authed = false;
			m_accountExists = false;
        }
    }

}
