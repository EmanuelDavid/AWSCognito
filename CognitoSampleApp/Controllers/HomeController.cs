using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using CognitoSampleApp.AWS;
using CognitoSampleApp.Models;
using System;
using System.Configuration;
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
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            string loginResult = await LoginUserAsync(model.Username, model.Password);
            if (loginResult.Equals(ChallengeNameType.NEW_PASSWORD_REQUIRED, StringComparison.Ordinal))
            {
                return RedirectToAction("NewPasswordRequired", new { model.Username });
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
        public ActionResult NewPasswordRequired(string username)
        {
            //ResetPasswordUser user = new ResetPasswordUser { UserName = username };
            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> NewPasswordRequired(FormCollection collection)
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

        public async Task<ActionResult> RequestPasswordReset( string Username)
        {
            CognitoInit awsInit = new CognitoInit();
            try
            {
              await  awsInit.Client.ForgotPasswordAsync(new ForgotPasswordRequest() { ClientId = awsInit.ClientId, Username = "da1vid" });
            }
            catch(Exception e)
            {
                ViewBag.LoginStatus = e.Message;
                return RedirectToAction("Home");
            }
            ViewBag.Status = "A confirmation code was sent to you re mail";
            return View();
        }

        /// <summary>
        /// Enter verification code received on mail or sms to proceed changing password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ConfirmForgotPassword(ConfirmResetPassword model)
        {
            CognitoInit awsInit = new CognitoInit();
            try
            {
                var result = await awsInit.Client.ConfirmForgotPasswordAsync(new ConfirmForgotPasswordRequest { ClientId = awsInit.ClientId, Username = model.Username, Password = model.Password, ConfirmationCode = model.VerificationCode });
                if(result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.Status = "You re password has been changed";
                }
            }
            catch (Exception e)
            {
                ViewBag.LoginStatus = e.Message;
                return RedirectToAction("Home");
            }

            return RedirectToAction("Home");
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
                    //This case is when a user is created by admin form portal
                    if (String.IsNullOrWhiteSpace(newPassword))
                    {
                        authResponse = await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest { SessionID = authResponse.SessionID, NewPassword = "some.PATIuite.13" });
                    }
                    return ChallengeNameType.NEW_PASSWORD_REQUIRED;
                }
                if (String.IsNullOrWhiteSpace(authResponse.ChallengeName))
                {
                    GetCredentials(authResponse.AuthenticationResult);
                    return authResponse.AuthenticationResult.AccessToken;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Login didn't succeded!";
        }


        private AWSCredentials GetCredentials(AuthenticationResultType authenticationResult)
        {
            Amazon.CognitoIdentity.CognitoAWSCredentials credentials = new Amazon.CognitoIdentity.CognitoAWSCredentials(ConfigurationManager.AppSettings["IDENITYPOOL_ID"], FallbackRegionFactory.GetRegionEndpoint());

            credentials.AddLogin(ConfigurationManager.AppSettings["IDENITY_PROVIDER"], authenticationResult.IdToken);

            return credentials;
        }
    }
}