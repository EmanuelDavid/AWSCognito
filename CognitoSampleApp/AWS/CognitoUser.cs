using Amazon.CognitoIdentityProvider;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CognitoSampleApp.AWS
{
    public class CognitoUser : IdentityUser
    {
        public string Password { get; set; }
        public UserStatusType Status { get; set; }
    }
}