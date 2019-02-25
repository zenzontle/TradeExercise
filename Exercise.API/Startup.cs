using Exercise.API;
using Exercise.API.App_Start;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Startup))]
namespace Exercise.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var jsonSerializer = config.Formatters.JsonFormatter;
            config.Formatters.Add(jsonSerializer);

            WebApiConfig.Register(config);

            app.UseNinjectMiddleware(() => NinjectConfig.Instance).UseNinjectWebApi(config);

            config.Formatters.JsonFormatter.Indent = true;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }
    }
}