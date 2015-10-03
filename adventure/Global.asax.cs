using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Adventure.Services;

namespace Adventure
{
    public class WebApiApplication : HttpApplication
    {
        private TwitterHashtagMonitor _twitterHashtagMonitor;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure( WebApiConfig.Register );
            FilterConfig.RegisterGlobalFilters( GlobalFilters.Filters );
            RouteConfig.RegisterRoutes( RouteTable.Routes );
            BundleConfig.RegisterBundles( BundleTable.Bundles );

            Task.Run( () => new TwitterHashtagMonitor().Monitor() );
            Task.Run(async () =>
                     {
                         _twitterHashtagMonitor = new TwitterHashtagMonitor();
                         await _twitterHashtagMonitor.Monitor();
                     });
        }

        protected void Application_End()
        {
            _twitterHashtagMonitor.Close();
        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior( System.Web.SessionState.SessionStateBehavior.Required );
        }
    }
}
