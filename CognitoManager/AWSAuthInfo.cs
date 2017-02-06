using System;
namespace CognitoManager
{
	public class AWSAuthInfo
	{
		private string m_awsAccessKeyId; 		// only set once per instance
		private string m_awsSecretAccessKeyId; 	// only set once per instance
		private string m_sessionToken;          // only set once per instance
		private bool m_initialized = false;

		public string AccessKey
		{
			get
			{
				return m_awsAccessKeyId;
			}
			set
			{
				if (m_awsAccessKeyId == null)
				{
					m_awsAccessKeyId = value;
					m_initialized |= (m_awsAccessKeyId != null && m_awsSecretAccessKeyId != null && m_sessionToken != null);
				}
			}
		}

		public string SecretAccessKey
		{
			get
			{
				return m_awsSecretAccessKeyId;
			}
			set
			{
				if (m_awsSecretAccessKeyId == null)
				{
					m_awsSecretAccessKeyId = value;
					m_initialized |= (m_awsAccessKeyId != null && m_awsSecretAccessKeyId != null && m_sessionToken != null);
				}
			}
		}

		public string SessionToken
		{
			get
			{
				return m_sessionToken;
			}
			set
			{
				if (m_sessionToken == null)
				{
					m_sessionToken = value;
					m_initialized |= (m_awsAccessKeyId != null && m_awsSecretAccessKeyId != null && m_sessionToken != null);
				}
			}
		}

		public AWSAuthInfo()
		{
			m_sessionToken = null;
			m_awsAccessKeyId = null;
			m_awsSecretAccessKeyId = null;
		}

		public AWSAuthInfo(string k, string sk, string t)
		{
			m_sessionToken = t;
			m_awsAccessKeyId = k;
			m_awsSecretAccessKeyId = sk;
			m_initialized = false;
			m_initialized |= (k != null && sk != null && t != null);
		}
	}
}
