using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
namespace Safehouse.Repository.MySql
{
    public abstract class MySQLRepository<T>
    {
        protected string connectionString;

        public MySQLRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var Connection = new MySqlConnection(connectionString))
                using (var cmd = new MySqlCommand(query, Connection))
                {
                    await Connection.OpenAsync();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        }
                    }

                    var reader = await cmd.ExecuteNonQueryAsync();

                    await Connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ex.Data["parameters"] = parameters;
                throw ex;
            }

            return true;
        }

        public async Task<string> ExecuteNonQueryGetId(string query, Dictionary<string, object> parameters = null)
        {
            string insertId = null;
            try
            {
                using (var Connection = new MySqlConnection(connectionString))
                using (var cmd = new MySqlCommand(query, Connection))
                {
                    await Connection.OpenAsync();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        }
                    }

                    var reader = await cmd.ExecuteNonQueryAsync();

                    insertId = cmd.LastInsertedId.ToString();

                    await Connection.CloseAsync();


                }
            }
            catch (Exception ex)
            {
                ex.Data["parameters"] = parameters;
                throw ex;
            }

            return insertId;
        }

        public async Task<DataSet> ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            var dataset = new DataSet();
            var datatable = new DataTable();

            try
            {
                using (var Connection = new MySqlConnection(connectionString))
                using (var cmd = new MySqlCommand(query, Connection))
                {
                    await Connection.OpenAsync();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        }
                    }

                    var reader = await cmd.ExecuteReaderAsync();

                    await datatable.LoadAsync(reader);

                    dataset.Tables.Add(datatable);

                    await Connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ex.Data["parameters"] = parameters;
                throw ex;
            }

            return dataset;
        }

        public async Task<DataSet> ExecuteCommand(string commandName, Dictionary<string, object> parameters = null)
        {
            var dataset = new DataSet();
            var datatables = new List<DataTable>();

            try
            {
                using (var Connection = new MySqlConnection(connectionString))
                using (var cmd = new MySqlCommand(commandName, Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await Connection.OpenAsync();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                        }
                    }

                    var reader = await cmd.ExecuteReaderAsync();

                    do
                    {
                        var table = new DataTable();

                        await table.LoadAsync(reader);

                        datatables.Add(table);
                    }
                    while (await reader.NextResultAsync());

                    dataset.Tables.AddRange(datatables.ToArray());

                    await Connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ex.Data["parameters"] = parameters;
                throw ex;
            }

            return dataset;
        }

        internal ParameterizedOrQuery BuildOrQuery(string columnName, string parameterPrefix, List<string> values)
        {
            var query = new ParameterizedOrQuery()
            {
                WhereQuery = "",
                Parameters = new Dictionary<string, object>()
            };

            for (var i = 0; i < values.Count; i++)
            {
                query.WhereQuery += String.Format("{0} = {1}{2}", columnName, parameterPrefix, i);

                query.Parameters.Add(String.Format("{0}{1}", parameterPrefix, i), values[i]);

                if (i < values.Count - 1)
                    query.WhereQuery += " OR ";
            }

            return query;
        }
    }
}
