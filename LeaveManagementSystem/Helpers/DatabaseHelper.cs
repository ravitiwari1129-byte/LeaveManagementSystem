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

        
        private SqlParameter[] ConvertHashtableToSqlParameters(Hashtable parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }
            var sqlParams = new List<SqlParameter>();
            foreach (DictionaryEntry entry in parameters)
            {
                sqlParams.Add(new SqlParameter(entry.Key.ToString(), entry.Value ?? DBNull.Value));
            }
            return sqlParams.ToArray();
        }

        
        public DataTable ExecuteStoredProcedure(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                {
                    cmd.Parameters.AddRange(sqlParams);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public int ExecuteNonQuery(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                {
                    cmd.Parameters.AddRange(sqlParams);
                }
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }


        public object ExecuteScalar(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                {
                    cmd.Parameters.AddRange(sqlParams);
                }
                conn.Open();
                return cmd.ExecuteScalar() ?? 0;
            }
        }

       
        public DataSet ExecuteDataSet(string procedureName, Hashtable parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(procedureName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                {
                    cmd.Parameters.AddRange(sqlParams);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        
        public SqlDataReader ExecuteReader(string procedureName, Hashtable parameters = null)
        {
            SqlConnection conn = GetConnection();
            SqlCommand cmd = new SqlCommand(procedureName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
            if (sqlParams != null)
            {
                cmd.Parameters.AddRange(sqlParams);
            }
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
                SqlParameter[] sqlParams = ConvertHashtableToSqlParameters(parameters);
                if (sqlParams != null)
                {
                    cmd.Parameters.AddRange(sqlParams);
                }
                SqlParameter outputParam = new SqlParameter(outputParameterName, SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);
                conn.Open();
                cmd.ExecuteNonQuery();
                return outputParam.Value != DBNull.Value? Convert.ToInt32(outputParam.Value): 0;
            }
        }


        public DataSet ExecuteStoredProcedureDataSet(string spName, Hashtable ht)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(spName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DictionaryEntry item in ht)
                {
                    cmd.Parameters.AddWithValue(item.Key.ToString(), item.Value);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            return ds;
        }

    }
}