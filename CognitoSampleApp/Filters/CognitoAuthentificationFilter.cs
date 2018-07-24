using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CognitoSampleApp.Filters
{
    public class CognitoAuthentificationFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            }

            string authentificationToken = actionContext.Request.Headers.Authorization.Parameter;
            string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authentificationToken));
            base.OnAuthorization(actionContext);
        }
    }
}