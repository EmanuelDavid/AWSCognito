using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CognitoSampleApp.Filters
{
    public class CognitoAuthentificationFilter : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Grab the current request headers
            var headers = httpContext.Request.Headers;

           if (httpContext.Request.Headers["Authorization"] == null)
            {
                httpContext.Response.StatusCode = 401;
                return false;
            }
            return true;
            //string authentificationToken = actionContext.Request.Headers.Authorization.Parameter;
            //string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authentificationToken));
            //base.OnAuthorization(actionContext);
        }
    }
}