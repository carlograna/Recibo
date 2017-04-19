using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ReceiptExport.classes;

namespace ReceiptExport
{
    class SDUConnection
    {
        public SDUConnection() { }

        public DataTable GetDataTable(string sqlString, string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sqlString, conn);
                    DataTable table = new DataTable();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd.CommandText, conn);
                    adapter.Fill(table);

                    return table;

                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public DataTable GetDataTableWithStoredProcedure(string spName, string connectionString)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    DataTable table = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter();

                    adapter.SelectCommand = new SqlCommand();
                    adapter.SelectCommand.Connection = conn;
                    adapter.SelectCommand.CommandText = spName;
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                    adapter.Fill(table);

                    return table;
                }

            }
            catch (SqlException ex)
            {
                throw (ex);
            }
        }

        public int UpdateQuery(string sqlString, string connectionString)
        {
            try
            {
                int rowsAffected;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    
                    SqlCommand cmd = new SqlCommand(sqlString, conn);
                    conn.Open();
                    return rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }
    }
}
