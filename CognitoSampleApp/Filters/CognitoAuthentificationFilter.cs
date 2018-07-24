using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CognitoSampleApp.Filters
{
    public class CognitoAuthentificationFilterAttribute : AuthorizeAttribute
    {
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //     Grab the current request headers
        //    var headers = httpContext.Request.Headers;

        //   if (httpContext.Request.Headers["Authorization"] == null)
        //    {
        //        httpContext.Response.StatusCode = 401;
        //        return false;
        //    }
        //    return true;
        //    string authentificationToken = actionContext.Request.Headers.Authorization.Parameter;
        //    string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authentificationToken));
        //    base.OnAuthorization(actionContext);
        //}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //filterContext. HttpContext.Response.Headers.Keys
            var requestKeys = filterContext.HttpContext.Request.Headers.AllKeys;

            base.OnAuthorization(filterContext);
            //you ve got headers add the headeer that you whant (token) and verify it

        }
    }
}