using System;
using Amazon;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using System.Collections.Generic;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace CognitoManager
{
    public class CognitoLoginManager
    {
        public AnonymousAWSCredentials AnonyCreds;
        public AmazonCognitoIdentityClient CognitoClient;

        public bool Initialize()
        {
            AnonyCreds = new AnonymousAWSCredentials();
            CognitoClient = new AmazonCognitoIdentityClient(AnonyCreds, RegionEndpoint.USWest2);
            GetIdRequest idRequest = new GetIdRequest();
            idRequest.AccountId = "563945701376";
            idRequest.IdentityPoolId = "us-west-2:5669ec9a-b6df-44f2-8746-5cfc2760f56d";

            idRequest.Logins = new Dictionary<string, string>()
            {
                { "graph.facebook.com", "FacebookSessionToken" }
            };

            GetIdResponse idResponse = CognitoClient.GetId(idRequest);

            
            return true;
        }
    }
}
