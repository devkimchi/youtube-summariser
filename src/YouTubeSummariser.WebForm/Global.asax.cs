using System;
using System.Net.Http;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Web;

using YouTubeSummariser.WebForm.Facade;

namespace YouTubeSummariser.WebForm
{
    public class Global : HttpApplication, IContainerProviderAccessor
    {
        // Provider that holds the application container.
        private static IContainerProvider _containerProvider;

        // Instance property that will be used by Autofac HttpModules
        // to resolve and inject dependencies.
        public IContainerProvider ContainerProvider
        {
            get { return _containerProvider; }
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();
            builder.RegisterType<HttpClient>().InstancePerRequest();
            builder.Register<YouTubeSummariserClient>(p =>
            {
                var http = p.Resolve<HttpClient>();
                var facade = new YouTubeSummariserClient(http) { ReadResponseAsString = true };

                return facade;
            })
                   .As<YouTubeSummariserClient>()
                   .InstancePerRequest();

            var container = builder.Build();
            _containerProvider = new ContainerProvider(container);
        }
    }
}
