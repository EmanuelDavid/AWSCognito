using CognitoSampleApp.AWS;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CognitoSampleApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            CognitoInit awsInit = new CognitoInit();
            await awsInit.LoginUserAsync();
            return View();
        }

        [Route("/home/login")]
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