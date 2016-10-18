using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PushNotification
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string conn = ConfigurationManager.ConnectionStrings["sqlConnectionStr"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            SqlDependency.Start(conn);
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            NotificationComponents nc = new NotificationComponents();
            DateTime currentTime = DateTime.Now;
            HttpContext.Current.Session["LastUpdated"] = currentTime;
            nc.RegiterNotification(currentTime);

        }
        protected void Application_End()
        {
            SqlDependency.Stop(conn);
        }
    }
}
