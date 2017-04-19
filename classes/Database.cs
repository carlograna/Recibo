using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ReceiptExport.classes
{
    public class Database
    {
        public static DbProviderFactory GetFactory()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["itemCS"];
            var providerName = connectionString.ProviderName;
            var factory = DbProviderFactories.GetFactory(providerName);

            return factory;
        }

        //public static IDbConnection CreateConnection()
        //{            
        //    var connectionString = ConfigurationManager.ConnectionStrings["itemCS"];
        //    var providerName = connectionString.ProviderName;
        //    var factory = DbProviderFactories.GetFactory(providerName);
        //    var connection = factory.CreateConnection();
        //    return connection;
        //}

        //public static IDbCommand CreateCommand(string sql, IDbConnection con)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.CommandText = sql;
        //    cmd.Connection = (SqlConnection)con;
        //    return cmd;
        //}

        //public static IDbDataAdapter CreateAdapter()
        //{
        //    return null;
        //}
    }
}
