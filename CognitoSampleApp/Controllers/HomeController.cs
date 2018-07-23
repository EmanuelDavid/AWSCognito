using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using CognitoSampleApp.AWS;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CognitoSampleApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignUp(FormCollection collection)
        {
            //TODO : server and client validation on this fields
            string username = Convert.ToString(collection["username"]);
            string passWord = Convert.ToString(collection["pass"]);
            string email = Convert.ToString(collection["email"]);

            CognitoInit awsInit = new CognitoInit();

            try
            {
                SignUpRequest signUpRequest = new SignUpRequest()
                {
                    ClientId = awsInit.ClientId,
                    Password = passWord,
                    Username = username
                };
                AttributeType emailAttribute = new AttributeType()
                {
                    Name = "email",
                    Value = email
                };
                signUpRequest.UserAttributes.Add(emailAttribute);

                var signUpResult = await awsInit.Client.SignUpAsync(signUpRequest);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                ViewBag.SignInStatus = $"Sign up error: {ex.Message}";
                return View("Index");
            }

            ViewBag.LoginStatus = "Your accouant has been created!";
            return RedirectToAction("Home");
        }

        [HttpPost]
        public async Task<ActionResult> Login(FormCollection collection)
        {
            string username = Convert.ToString(collection["username"]);
            string passWord = Convert.ToString(collection["pass"]);

            string loginResult = await LoginUserAsync(username, passWord);
            if (loginResult.Equals(ChallengeNameType.NEW_PASSWORD_REQUIRED, StringComparison.Ordinal))
            {
                return RedirectToAction("ResetPassword", new { username });
            }
            if (!String.IsNullOrWhiteSpace(loginResult))
            {
                ViewBag.LoginStatus = loginResult;
                return View("Index");
            }

            ViewBag.LoginStatus = "You are now logged in!";
            return RedirectToAction("Home");
        }

        [HttpGet]
        public ActionResult ResetPassword(string username)
        {
            //ResetPasswordUser user = new ResetPasswordUser { UserName = username };
            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(FormCollection collection)
        {
            string username = Convert.ToString(collection["username"]);
            string passWord = Convert.ToString(collection["pass"]);
            string newPassWord = Convert.ToString(collection["newPass"]);

            string loginResult = await LoginUserAsync(username, passWord, newPassWord);

            return RedirectToAction("Index");
        }

        public ActionResult Home()
        {
            return View();
        }

        /// <summary>
        /// Try to login to aws using cognito
        /// If user  has not changed the password then he will be redirected to password change
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        private async Task<string> LoginUserAsync(string username, string password, string newPassword = null)
        {
            CognitoInit awsInit = new CognitoInit();

            CognitoUserPool userPool = new CognitoUserPool(awsInit.PoolId, awsInit.ClientId, awsInit.Client);
            CognitoUser user = new CognitoUser(username, awsInit.ClientId, userPool, awsInit.Client);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            try
            {
                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                if (authResponse.ChallengeName != null && authResponse.ChallengeName.ToString().Equals(ChallengeNameType.NEW_PASSWORD_REQUIRED, StringComparison.OrdinalIgnoreCase))
                {
                    if (String.IsNullOrWhiteSpace(newPassword))
                    {
                        authResponse = await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest { SessionID = authResponse.SessionID, NewPassword = "some.PATIuite.13" });
                    }
                    return ChallengeNameType.NEW_PASSWORD_REQUIRED;
                }
                if (String.IsNullOrWhiteSpace(authResponse.ChallengeName))
                {
                    return authResponse.AuthenticationResult.AccessToken;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Login didn't succeded!";
        }
    }
}