
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace ConsoleAppcorewebjob
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }


        private static void Main(string[] args)
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";
            //Determines the working environment as IHostingEnvironment is unavailable in a console app

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (isDevelopment) //only add secrets in development
            {
                builder.AddUserSecrets<MyOptions>();
            }

            Configuration = builder.Build();

            var services = new ServiceCollection()
               .Configure<MyOptions>(Configuration.GetSection(nameof(MyOptions)))
               .AddOptions()
               .BuildServiceProvider();

            var kk = services.GetService<MyOptions>();
            var obj = new SecretConsumer().OnGet();
        }
    }
    public class MyOptions
    {
        public string Secret1 { get; set; }
        public string Secret2 { get; set; }
    }

    public class SecretConsumer {

        private readonly MyOptions _secrets;

        public SecretConsumer(IOptions<MyOptions> secrets)
        {
            _secrets = secrets.Value;
        }
        public string OnGet()
        {
            return _secrets.Secret1;
        }
    }
}
