using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CodingCards.Helpers
{
    public class Secrets
    {
        public static string GetAppSettingsValue(IConfiguration Configuration, string name)
        {
            var value = Configuration.GetSection("AppSettings")[name];
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }
            return Environment.GetEnvironmentVariable(name);
        }

        public static string GetConnectionString(IConfiguration Configuration, string name)
        {
            var value = Configuration.GetConnectionString(name);
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
