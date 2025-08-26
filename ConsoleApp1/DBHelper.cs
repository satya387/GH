using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace GHAPIServices
{
    public class DBHelper
    {
        private readonly string _connectionString;
        public DBHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;

                    connection.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }
        public void ExecuteNonQuery(string storedProcedure, DBParameterCollection parameters, CommandType commandType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = commandType;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(new SqlParameter(param.Name, param.Value));
                    }

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public class DBParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }

            public DBParameter(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }

        public class DBParameterCollection : List<DBParameter> { }
    }
}
