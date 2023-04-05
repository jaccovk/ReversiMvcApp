using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ReversiMvcApp.Areas.Identity.IdentityHostingStartup))]
namespace ReversiMvcApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}