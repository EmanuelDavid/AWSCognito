using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace CognitoSampleApp.AWS
{
    public class CognitoInit
    {

        public readonly AmazonCognitoIdentityProviderClient Client =
            new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), FallbackRegionFactory.GetRegionEndpoint());
        public readonly string ClientId = ConfigurationManager.AppSettings["CLIENT_ID"];
        public readonly string PoolId = ConfigurationManager.AppSettings["USERPOOL_ID"];
    }
}