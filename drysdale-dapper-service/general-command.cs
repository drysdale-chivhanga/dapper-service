using Dapper;
using MySqlConnector;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Drysdale.Dapper.Service
{
    /// <summary>
    /// Class.General-Command
    /// </summary>
    public class GeneralCommand
    {
        #region ExecuteOneAsync...
        /// <summary>
        /// ExecuteOneAsync
        /// </summary>
        /// <param name="proc">The Stored Procedure</param>
        /// <param name="insertprms">The Insertion-Model</param>
        /// <param name="con">The Database Connection String</param>
        /// <returns>Create-Count Which-Is-One</returns>
        public async Task<int> ExecuteOneAsync(string proc, DynamicParameters insertprms, IDbConnection con) =>
            await con.ExecuteAsync(sql: proc, param: insertprms, commandType: CommandType.StoredProcedure);
        #endregion

        #region ChoiceBasedExecuteOneAsync...
        /// <summary>
        /// ChoiceBasedExecuteOneAsync
        /// </summary>
        /// <param name="sqlQuery">The Incoming-Sql (Raw-Sql/Stored-Procedure)</param>
        /// <param name="queryParams">The Sql-Parameters</param>
        /// <param name="conBroker">The Connection-Broker</param>
        /// <returns>The Count-Of-Executed-And-Affected-Rows</returns>
        public async Task<int> ChoiceBasedExecuteOneAsync(string sqlQuery, DynamicParameters queryParams, ConnectionBroker conBroker)
        {
            int affectedRows = 0;
            if (conBroker != null)
            {
                switch (conBroker.TargetedDatabase)
                {
                    case TargetDbType.OracleMySQL://OracleMySql Server
                        using (MySqlConnection con = new MySqlConnection(conBroker.TheValue))
                        {
                            affectedRows = await con.ExecuteAsync(sql: sqlQuery, param: queryParams);
                        }
                        break;
                    default://Microsoft SQL Server
                        using (IDbConnection con = new SqlConnection(conBroker.TheValue))
                        {
                            affectedRows = await con.ExecuteAsync(sql: sqlQuery, param: queryParams, commandType: CommandType.StoredProcedure);
                        }
                        break;
                }
            }
            return affectedRows;
        }
        #endregion

        #region ExecuteManyAsync...
        /// <summary>
        /// ExecuteManyAsync
        /// </summary>
        /// <param name="proc">The Stored Procedure</param>
        /// <param name="insertList">The Insertion-Models-List</param>
        /// <param name="con">The Database Connection String</param>
        /// <returns>Execution-Count Result</returns>
        public async Task<int> ExecuteManyAsync(string proc, object insertList, IDbConnection con) =>
            await con.ExecuteAsync(sql: proc, insertList, commandType: CommandType.StoredProcedure);
        #endregion
    }
}
