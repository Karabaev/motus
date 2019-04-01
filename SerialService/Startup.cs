using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SerialService.Startup))]
namespace SerialService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
