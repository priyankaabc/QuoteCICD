using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using QuoteCalculator.Service.Helper;

namespace QuoteCalculator.Service.Common
{
    public class BaseRepository
    {

        protected virtual TEntity Get<TEntity>(string sp_name, object param)
        {
            var connection = GetDbConnection();
            return connection.Query<TEntity>($"{sp_name}", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        protected virtual List<TEntity> GetAll<TEntity>(string sp_name, object param = null)
        {
            var connection = GetDbConnection();
            return connection.Query<TEntity>($"{sp_name}", param, commandType: CommandType.StoredProcedure).ToList();
        }

        protected virtual int ExecuteStoredProcedure(string sp_name, object param = null)
        {
            var connection = GetDbConnection();
            return connection.Execute($"{sp_name}", param: param, commandType: CommandType.StoredProcedure);
        }
        protected virtual int ExecuteQuery(string query)
        {
            var connection = GetDbConnection();
            return connection.Execute($"{query}");
        }
        protected virtual T ExecuteQuery<T>(string query, object param = null)
        {
            var connection = GetDbConnection();
            return connection.QueryFirstOrDefault<T>(query, param);
        }
        protected IDbConnection GetDbConnection()
        {
            var connection = AESEncryptionDecryptionHelper.Decrypt(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            
            return new SqlConnection(connection);
        }
    }
}
