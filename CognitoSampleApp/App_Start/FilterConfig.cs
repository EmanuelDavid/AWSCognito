﻿using CognitoSampleApp.Filters;
using System.Web;
using System.Web.Mvc;

namespace CognitoSampleApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
          //  filters.Add(new CognitoAuthentificationFilterAttribute());
        }
    }
}
