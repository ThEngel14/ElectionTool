using ElectionTool;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace ElectionTool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
