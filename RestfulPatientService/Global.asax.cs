using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Runtime.Serialization;
using RestfulPatientService.Models;

namespace RestfulPatientService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SetSerializer<Patient>( new DataContractSerializer(typeof(Patient), new Type[] { typeof(Phone) }) );
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();

            //Debug.WriteLine(exception);
        }
    }
}
