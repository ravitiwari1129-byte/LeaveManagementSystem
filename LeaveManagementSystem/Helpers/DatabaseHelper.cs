using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace LeaveManagementSystem.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Private method to convert Hashtable to SqlParameter[]
        private SqlParameter[] ConvertHashtableToSqlParameters(Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return null;

            var sqlParams = new List<SqlParameter>();
            foreach (DictionaryEntry entry in parameters)
            {
                sqlParams.Add(new SqlParameter(entry.Key.ToString(), entry.Value ?? DBNull.Value));
            }
            return sqlParams.ToArray();
        }

        // ExecuteStoredProcedure - Accepts Hashtable only
        public DataTable ExecuteStoredProcedure(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Convert Hashtable to SqlParameter[] internally
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                    cmd.Parameters.AddRange(sqlParams);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // ExecuteNonQuery - Accepts Hashtable only
        public int ExecuteNonQuery(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Convert Hashtable to SqlParameter[] internally
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                    cmd.Parameters.AddRange(sqlParams);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // ExecuteScalar - Accepts Hashtable only
        public object ExecuteScalar(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Convert Hashtable to SqlParameter[] internally
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                    cmd.Parameters.AddRange(sqlParams);

                conn.Open();
                return cmd.ExecuteScalar() ?? 0;
            }
        }

        // ExecuteDataSet - For multiple result sets (optional)
        public DataSet ExecuteDataSet(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Convert Hashtable to SqlParameter[] internally
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                    cmd.Parameters.AddRange(sqlParams);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        // ExecuteReader - For forward-only data reading (optional)
        public SqlDataReader ExecuteReader(string procedureName, Hashtable parameters = null)
        {
            SqlConnection conn = GetConnection();
            SqlCommand cmd = new SqlCommand(procedureName, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Convert Hashtable to SqlParameter[] internally
            SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
            if (sqlParams != null)
                cmd.Parameters.AddRange(sqlParams);

            conn.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public bool EmailExists(string email)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand("USP_CheckEmailExists", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Email", email);

                SqlParameter existsParam = new SqlParameter("@Exists", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(existsParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return Convert.ToBoolean(existsParam.Value);
            }
        }

        public int ExecuteWithOutputParameter(string procedureName,Hashtable parameters,string outputParameterName)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter[] sqlParams =
                    ConvertHashtableToSqlParameters(parameters);

                if (sqlParams != null)
                    cmd.Parameters.AddRange(sqlParams);

                SqlParameter outputParam =
                    new SqlParameter(outputParameterName, SqlDbType.Int);

                outputParam.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(outputParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return outputParam.Value != DBNull.Value? Convert.ToInt32(outputParam.Value): 0;
            }
        }
    }
}