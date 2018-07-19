using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System.Configuration;
using System.Threading.Tasks;

namespace CognitoSampleApp.AWS
{
    public class CognitoInit
    {
        private readonly AmazonCognitoIdentityProviderClient _client =
            new AmazonCognitoIdentityProviderClient();
        private readonly string _clientId = ConfigurationManager.AppSettings["CLIENT_ID"];
        private readonly string _poolId = ConfigurationManager.AppSettings["USERPOOL_ID"];


        public void CreateAsync(CognitoUser user)
        {
            //// Register the user using Cognito
            //var signUpRequest = new SignUpRequest
            //{
            //    ClientId = ConfigurationManager.AppSettings["CLIENT_ID"],
            //    Password = user.Password,
            //    Username = user.Email,

            //};

            //var emailAttribute = new AttributeType
            //{
            //    Name = "email",
            //    Value = user.Email
            //};
            //signUpRequest.UserAttributes.Add(emailAttribute);

            //SignUpResponse result = _client.SignUpAsync(signUpRequest).Result;

        }

        public async Task GetCredsAsync()
        {
            AmazonCognitoIdentityProviderClient provider =
                new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());
            CognitoUserPool userPool = new CognitoUserPool(ConfigurationManager.AppSettings["USERPOOL_ID"], ConfigurationManager.AppSettings["CLIENT_ID"], provider);
            CognitoUser user = new CognitoUser("username", ConfigurationManager.AppSettings["CLIENT_ID"], userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = "userPassword"
            };

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var accessToken = authResponse.AuthenticationResult.AccessToken;

        }
    }
}