namespace CognitoSampleApp.Models
{
    public class ConfirmResetPassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
    }
}