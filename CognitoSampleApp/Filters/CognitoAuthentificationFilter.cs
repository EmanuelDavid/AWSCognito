using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CognitoSampleApp.Filters
{
    public class JwtAuthorizationAttrribute : AuthorizeAttribute
    {

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext", nameof(filterContext));
            }

            string headerKeyName = "Authorization";
            var  token = filterContext.RequestContext.HttpContext.Request.Headers[headerKeyName];
            if (string.IsNullOrEmpty(token))
            {
                filterContext.Result = new HttpStatusCodeResult(401);
            }
            else
            {
                string[] tokenSections = token.Split('.');

                if (tokenSections.Length == 3)
                {

                    CorrectBase64Length(tokenSections);

                    var tokenId = Encoding.UTF8.GetString(Convert.FromBase64String(tokenSections[0]));
                    string tokenPayload = Encoding.UTF8.GetString(Convert.FromBase64String(tokenSections[1]));
                    //string tokensignatre = Encoding.UTF8.GetString(Convert.FromBase64String(tokenSections[2]));

                   // var jsonObj = JObject.Parse(tokenId);
                }

                if (true)
                {
                    filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity("David"), null);
                }
                else
                {
                    filterContext.Result = new HttpStatusCodeResult(401);
                }

                base.OnAuthorization(filterContext);
            }
        }
        private void CorrectBase64Length(string[] tokenSections)
        {
            for (int s = 0; s < 3; s++)
            {
                int sectionLenght = tokenSections[s].Length;
                int modulo = sectionLenght % 4;

                if (modulo != 0)
                {
                    int nrOfWhiteSpaces = 4 - modulo;
                    string whiteSpaces = "";
                    for (int i = 0; i < nrOfWhiteSpaces; i++)
                    {
                        whiteSpaces += "=";
                    }
                    tokenSections[s] = tokenSections[s] + whiteSpaces;
                }
            }
        }
    }
}