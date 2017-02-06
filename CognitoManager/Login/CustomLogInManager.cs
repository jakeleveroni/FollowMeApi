using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using FollowMeDataBase.Models;
using FollowMeDataBase.DBCallWrappers;
using Utility;

// TODO This is untested code for signing in and requesting credentials through cognito.
// This is getting put in the backlog until sessioning is implemented so it can 
// be integrated into a session

namespace LogInManager
{
    namespace Login
    {
        public class CustomLogInManager : IDisposable
        {
            AmazonSecurityTokenServiceConfig m_stsConfig;
            private AmazonCognitoIdentityClient m_cognitoClient;
            private DB m_db;
            private string m_userPool;
            private string m_federationId;

            public CustomLogInManager()
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
                    Logger.logger.Info("[CUSTOM-LOGIN-MANAGER][NOTE] : Creating Security token service client now...");
                    m_stsConfig = new AmazonSecurityTokenServiceConfig();
                }
                catch (Exception ex)
                {
                    Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : Could not create security token client, " + ex.Message);
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
            public AuthInfo AuthenticateUserInApp(string username, string password)
            {
                UserModel um = null;
                List<UserModel> users = new List<UserModel>();
                AuthInfo authInfo = new AuthInfo();

                users = m_db.QueryUsersByUserNameAndPassword(username, password);

                if (users.Count > 1)
                {
                    Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : More than one user with that username and password exist");
                    return null;
                }
                else if (users.Count == 0)
                {
                    Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][INFO] : No user information returned user does not exist");
                    return null;
                }
                else
                {
                    um = users[0];
                }

                if (um != null)
                {
                    authInfo = new AuthInfo(username, password, um.UserId.ToString());
                    authInfo.UserExists = true;
                    authInfo.Authenticated = true;
                }
                else
                {
                    Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : User does not exist, must create a new account first");
                    return null;
                }

                if (RequestCognitoCredentials(authInfo))
                {
                    return authInfo;
                }
                else
                {
                    Logger.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : Could not get credentials from AWS");
                    return null;
                }
            }

            private bool RequestCognitoCredentials(AuthInfo authInfo)
            {
                using (AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient(m_stsConfig))
                {
                    try
                    {
                        GetSessionTokenRequest sessionTokenReq = new GetSessionTokenRequest();

                        Credentials creds = stsClient.GetSessionToken(sessionTokenReq).Credentials;

                        SessionAWSCredentials sessionCreds = new SessionAWSCredentials(creds.AccessKeyId, creds.SecretAccessKey, creds.SessionToken);

                        authInfo.AWSCredentials = new AWSAuthInfo(creds.AccessKeyId, creds.SessionToken, creds.SessionToken);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.logger.Error("[REQ-COG-CREDS][ERROR] : Could not get session credentials, " + ex.Message);
                        return false;
                    }
                }
            }

            public void Dispose()
            {
                m_cognitoClient.Dispose();
                m_db.Dispose();
            }
        }
    }
}
