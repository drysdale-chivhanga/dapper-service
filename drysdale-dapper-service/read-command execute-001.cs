using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Drysdale.Dapper.Service
{
    #region Class.ReadCommand001...
    /// <summary>
    /// Class.ReadCommand-001
    /// </summary>
    public class ReadCommand<TEntity>
    {
        #region ExecuteAsync "With-Dynamic-Parameter(s)"...
        /// <summary>
        /// ExecuteAsync "With-Dynamic-Parameter(s)"
        /// </summary>
        /// <param name="proc">The Stored Procedure</param>
        /// <param name="prms">The Dapper Dynamic Parameters</param>
        /// <param name="con">The Database Connection String</param>
        /// <returns>Single-List</returns>
        public async Task<IEnumerable<TEntity>> ExecuteAsync(string proc, DynamicParameters prms, IDbConnection con)
            => await con.QueryAsync<TEntity>(sql: proc, param: prms, commandType: CommandType.StoredProcedure);
        #endregion

        #region ChoiceBasedExecuteAsync...
        /// <summary>
        /// ChoiceBasedExecuteAsync
        /// </summary>
        /// <param name="sqlQuery">The Incoming-Sql (Raw-Sql/Stored-Procedure)</param>
        /// <param name="queryParams">The Sql-Parameters</param>
        /// <param name="connectionBroker">The Connection-Broker</param>
        /// <returns>The Choice-Based-Executed 01-List</returns>
        public async Task<List<TEntity>> ChoiceBasedExecuteAsync(string sqlQuery, DynamicParameters queryParams, ConnectionBroker connectionBroker)
        {
            List<TEntity> dummyList = new List<TEntity>();
            if (connectionBroker != null)
            {
                switch (connectionBroker.TargetedDatabase)
                {
                    case TargetDbType.OracleMySQL:
                        using (MySqlConnection db = new MySqlConnection(connectionBroker.TheValue))
                        {
                            dummyList = (List<TEntity>)await db.QueryAsync<TEntity>(sql: sqlQuery, param: queryParams);
                        }
                        break;
                    default:
                        bool storeProcedureExist = GeneralCommand.StoredProcedureExist(connectionBroker.NakedID, sqlQuery);
                        if (storeProcedureExist)
                        {
                            using IDbConnection db = new SqlConnection(connectionBroker.TheValue);
                            dummyList = (List<TEntity>)await db.QueryAsync<TEntity>(sql: sqlQuery, param: queryParams, commandType: CommandType.StoredProcedure);
                        }
                        break;
                }
            }
            return dummyList;
        }
        #endregion
    }
    #endregion
}
