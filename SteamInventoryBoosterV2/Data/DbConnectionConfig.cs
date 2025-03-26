using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInventoryBoosterV2.Data
{
    public static class DbConnectionConfig
    {
        private readonly static string _dbname = "SteamInventoryBoosterDbV2";
        private readonly static string _dbHost = ".\\SQLEXPRESS";
        public static string GetConnectionString()
        {
            return $"Server={_dbHost};Database={_dbname};Trusted_Connection=True;TrustServerCertificate=True;";
        }
    }
}
