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
            private DB m_tmpDB;
            private string m_userPool;
            private string m_federationId;
            public string UserName { get; }
            public string Password { get; }

            public CustomLogInManager(string userName, string password, DB db)
            {
                UserName = userName;
                Password = password;
                m_tmpDB = db;
                m_stsConfig = new AmazonSecurityTokenServiceConfig();
            }

            // returns the auth info for the user, the auth info will have 
            // a false Authenticated value if query fails
            public AuthInfo AuthenticateUserInApp()
            {
                UserModel um = null;
                List<UserModel> users = new List<UserModel>();
                AuthInfo authInfo = new AuthInfo();

                try
                {
                    users = m_tmpDB.QueryUsersByUserNameAndPassword(UserName, Password);
                    m_tmpDB = null;
                }
                catch (Exception ex)
                {
                    Tools.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : Could not create the temporary Database accessor, " + ex.Message);
                    return new AuthInfo(FolloMeErrorCodes.Uninitialized);
                }

                if (users.Count > 1)
                {
                    Tools.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : More than one user with that username and password exist, THIS SHOULD NEVER HAPPEN");
                    return new AuthInfo(FolloMeErrorCodes.MultipleUsersFound);
                }
                else if (users.Count == 0)
                {
                    Tools.logger.Error("[CUSTOM-LOGIN-MANAGER][INFO] : No user information returned user does not exist, create an account or verify you entered the correct credentials");
                    return new AuthInfo(FolloMeErrorCodes.NoAccountExists);
                }
                else
                {
                    um = users[0];
                }

                if (um != null)
                {
                    authInfo = new AuthInfo(UserName, Password, um.UserId.ToString(), FolloMeErrorCodes.APIVerified);
                    authInfo.UserExists = true;
                    authInfo.Authenticated = true;
                }
                else
                {
                    Tools.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : User does not exist, must create a new account first");
                    return new AuthInfo(FolloMeErrorCodes.NotAPIVerified);
                }

                if (RequestCognitoCredentials(authInfo))
                {
                    authInfo.StatusCode = FolloMeErrorCodes.AWSAndAPIVerified;
                    return authInfo;
                }
                else
                {
                    Tools.logger.Error("[CUSTOM-LOGIN-MANAGER][ERROR] : Could not get credentials from AWS");
                    return authInfo;
                }
            }

            private bool RequestCognitoCredentials(AuthInfo authInfo)
            {
                using (AmazonSecurityTokenServiceClient stsClient = new AmazonSecurityTokenServiceClient(m_stsConfig))
                {
                    try
                    {
                        // auth user through FolloMe federation idfentities
                        GetSessionTokenRequest sessionTokenReq = new GetSessionTokenRequest()
                        {
                            DurationSeconds = 3600,
                        };

                        Credentials creds = stsClient.GetSessionToken(sessionTokenReq).Credentials;
                        SessionAWSCredentials sessionCreds = new SessionAWSCredentials(creds.AccessKeyId, creds.SecretAccessKey, creds.SessionToken);

                        authInfo.AWSCredentials = new AWSAuthInfo(creds.AccessKeyId, creds.SecretAccessKey, creds.SessionToken);
                        authInfo.StatusCode = FolloMeErrorCodes.AWSVerified;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Tools.logger.Error("[REQ-COG-CREDS][ERROR] : Could not get session credentials, " + ex.Message);
                        authInfo.StatusCode = FolloMeErrorCodes.NotAWSVerified;
                        return false;
                    }
                }
            }

            public void Dispose()
            {
                if (m_cognitoClient != null)
                {
                    m_cognitoClient.Dispose();
                }

                if (m_tmpDB != null)
                {
                    m_tmpDB.Dispose();
                }
            }
        }
    }
}
