using CognitoSampleApp.AWS;
using System.Web.Mvc;

namespace CognitoSampleApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CognitoInit awsInit = new CognitoInit();
            CognitoUser user = new CognitoUser() { UserName= "testUser", Password="something.Patimio.13" };
            awsInit.CreateAsync(user);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}