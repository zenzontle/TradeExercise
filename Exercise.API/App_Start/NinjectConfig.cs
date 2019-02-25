using Exercise.Core.Repositories;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Exercise.API.App_Start
{
    public class NinjectConfig
    {
        private static Lazy<IKernel> Kernel = new Lazy<IKernel>(() =>
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            RegisterServices(kernel);
            RegisterDomainModules(kernel);
            DependencyResolver.SetResolver(t => kernel.TryGet(t), t => kernel.GetAll(t));
            return kernel;
        });

        public static IKernel Instance => Kernel.Value;

        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(Assembly.GetExecutingAssembly());
        }

        private static void RegisterDomainModules(IKernel kernel)
        {
            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<IAvatarRepository>().To<AvatarRepository>();
        }
    }
}