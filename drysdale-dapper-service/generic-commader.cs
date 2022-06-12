using Dapper;
using Drysdale.Query.Response;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Drysdale.Dapper.Service
{
    public interface IGenericQueryCommander
    {
        /// <summary>
        /// Get-Data-Asynchronously
        /// </summary>
        /// <typeparam name="T">The Tatgeted-Requested-Model(s)</typeparam>
        /// <typeparam name="U">The Sql-Parameter(s)</typeparam>
        /// <param name="sqlQuery">The Incoming-Sql-Query</param>
        /// <param name="queryParams">Incoming-Parameter(s)</param>
        /// <param name="incomingCon">Incoming-Connection-String</param>
        /// <param name="dbType">Targeted-Database-Type(Mysql,MSSQL)</param>
        /// <param name="queryType">Targeted-Query-Type(Stored-Procedure,RawSql)</param>
        /// <returns>IEnumerable Of Tatgeted-Requested-Model(s)</returns>
        Task<IEnumerable<T>> GetDataAsync<T, U>(string sqlQuery, U queryParams, string incomingCon, TargetDbType dbType, TargetQueryType queryType);

        /// <summary>
        /// Save-Data-Asynchronously
        /// </summary>
        /// <typeparam name="U">The Sql-Parameter(s)</typeparam>
        /// <param name="sqlQuery">The Incoming-Sql-Query</param>
        /// <param name="queryParams">Incoming-Parameter(s)</param>
        /// <param name="incomingCon">Incoming-Connection-String</param>
        /// <param name="dbType">Targeted-Database-Type(Mysql,MSSQL)</param>
        /// <param name="queryType">Targeted-Query-Type(Stored-Procedure,RawSql)</param>
        /// <returns>QueryResponse-Object</returns>
        Task<QueryResponse> SaveDataAsync<U>(string sqlQuery, U queryParams, string incomingCon, TargetDbType dbType,TargetQueryType queryType);
    }

    /// <summary>
    /// Class.Sql-Query-Commander
    /// </summary>
    public class GenericQueryCommander : IGenericQueryCommander
    {
        #region GetDataAsync...
        /// <summary>
        /// Get-Data-Asynchronously
        /// </summary>
        /// <typeparam name="T">The Tatgeted-Requested-Model(s)</typeparam>
        /// <typeparam name="U">The Sql-Parameter(s)</typeparam>
        /// <param name="sqlQuery">The Incoming-Sql-Query</param>
        /// <param name="queryParams">Incoming-Parameter(s)</param>
        /// <param name="incomingCon">Incoming-Connection-String</param>
        /// <param name="dbType">Targeted-Database-Type(Mysql,MSSQL)</param>
        /// <param name="queryType">Targeted-Query-Type(Stored-Procedure,RawSql)</param>
        /// <returns>IEnumerable Of Tatgeted-Requested-Model(s)</returns>
        public async Task<IEnumerable<T>> GetDataAsync<T, U>(string sqlQuery, U queryParams, string incomingCon, TargetDbType dbType, TargetQueryType queryType)
        {

            switch (dbType)
            {
                case TargetDbType.OracleMySQL://OracleMySql Server
                    using (MySqlConnection con = new MySqlConnection(incomingCon))
                    {
                        return queryType switch
                        {
                            TargetQueryType.RawSql => await con.QueryAsync<T>(sql: sqlQuery, param: queryParams),
                            //Stored-Procedure
                            _ => await con.QueryAsync<T>(sql: sqlQuery, param: queryParams),
                        };
                    }
                default://Microsoft SQL Server
                    using (IDbConnection con = new SqlConnection(incomingCon))
                    {
                        return queryType switch
                        {
                            TargetQueryType.RawSql => await con.QueryAsync<T>(sql: sqlQuery, param: queryParams),
                            //Stored-Procedure
                            _ => await con.QueryAsync<T>(sql: sqlQuery, param: queryParams),
                        };
                    }
            }
        }
        #endregion

        #region SaveDataAsync...
        /// Save-Data-Asynchronously
        /// </summary>
        /// <typeparam name="U">The Sql-Parameter(s)</typeparam>
        /// <param name="sqlQuery">The Incoming-Sql-Query</param>
        /// <param name="queryParams">Incoming-Parameter(s)</param>
        /// <param name="incomingCon">Incoming-Connection-String</param>
        /// <param name="dbType">Targeted-Database-Type(Mysql,MSSQL)</param>
        /// <param name="queryType">Targeted-Query-Type(Stored-Procedure,RawSql)</param>
        /// <returns>QueryResponse-Object</returns>
        public async Task<QueryResponse> SaveDataAsync<U>(string sqlQuery, U queryParams, string incomingCon, TargetDbType dbType, TargetQueryType queryType)
        {
            QueryResponse response = new()
            {
                Code = false,
                Message = "Error Detected"
            };

            try
            {
                switch (dbType)
                {
                    case TargetDbType.OracleMySQL://OracleMySql Server
                        
                        using (MySqlConnection con = new(incomingCon))
                        {
                            switch (queryType)
                            {
                                case TargetQueryType.RawSql:
                                    await con.ExecuteAsync(sql: sqlQuery, param: queryParams, commandType: CommandType.Text);
                                    break;
                                //Stored-Procedure
                                default:
                                    await con.ExecuteAsync(sql: sqlQuery, param: queryParams, commandType: CommandType.StoredProcedure);
                                    break;
                            }

                        }
                        break;
                    default://Microsoft SQL Server
                        using (IDbConnection con = new SqlConnection(incomingCon))
                        {
                            switch (queryType)
                            {
                                case TargetQueryType.RawSql:
                                    await con.ExecuteAsync(sql: sqlQuery, param: queryParams, commandType: CommandType.Text);
                                    break;
                                //Stored-Procedure
                                default:
                                    await con.ExecuteAsync(sql: sqlQuery, param: queryParams, commandType: CommandType.StoredProcedure);
                                    break;
                            }
                        }
                        break;
                }
                response.Code = true;
                response.Message = "Success!";
            }
            catch (System.Exception ex)
            {
                response.Message = $"Error-Detected {ex.Message}";
            }
            return response;
        }
        #endregion

    }
}
