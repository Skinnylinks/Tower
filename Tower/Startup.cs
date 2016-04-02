using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tower.Startup))]
namespace Tower
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
