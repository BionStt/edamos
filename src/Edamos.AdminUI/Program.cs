using Edamos.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Edamos.AdminUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .AddEdamosDefault(args)
                .UseStartup<Startup>()
                .Build().Run();
        }
    }
}
