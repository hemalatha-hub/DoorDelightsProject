using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoorDelightsProject.Startup))]
namespace DoorDelightsProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
