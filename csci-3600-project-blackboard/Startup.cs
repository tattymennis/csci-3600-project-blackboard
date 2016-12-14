using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(csci_3600_project_the_struggle.Startup))]
namespace csci_3600_project_the_struggle
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
