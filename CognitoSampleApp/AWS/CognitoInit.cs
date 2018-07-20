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
        private readonly AmazonCognitoIdentityProviderClient _client =
            new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), FallbackRegionFactory.GetRegionEndpoint());
        private readonly string _clientId = ConfigurationManager.AppSettings["CLIENT_ID"];
        private readonly string _poolId = ConfigurationManager.AppSettings["USERPOOL_ID"];


        public async Task LoginUserAsync()
        {
            CognitoUserPool userPool = new CognitoUserPool(_poolId, _clientId, _client);
            CognitoUser user = new CognitoUser("nUser", _clientId, userPool, _client);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = "5$EItWQc"// pass that user had received on mail, temp pass
            };

            try
            {
                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                if (authResponse.ChallengeName.ToString().Equals(ChallengeNameType.NEW_PASSWORD_REQUIRED, StringComparison.OrdinalIgnoreCase))
                {
                    authResponse = await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest { SessionID = authResponse.SessionID, NewPassword = "some.PATIuite.13" });
                }
                if (String.IsNullOrEmpty( authResponse.ChallengeName))
                {
                    var accessToken = authResponse.AuthenticationResult.AccessToken;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.ToString());
            }

        }
    }
}