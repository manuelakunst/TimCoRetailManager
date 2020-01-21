using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library.Internal.DataAccess
{
    public class SqlDataAccess : IDisposable, ISqlDataAccess
    {
        public SqlDataAccess(IConfiguration config, ILogger<ISqlDataAccess> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GetConnectionString(string name)
        {
            //return @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = TRMData; Integrated Security = True; Connect Timeout = 60; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            return _config.GetConnectionString(name);
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string cnn = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(cnn))
            {
                List<T> rows = connection.Query<T>(storedProcedure,
                                                    parameters,
                                                    commandType: CommandType.StoredProcedure).ToList();
                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string cnn = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(cnn))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }



        // ACHTUNG: SQL Transaction in C# sollte nur selten verwendet werden. 
        // Das Offen-Halten der DB-Connection ist immer ein Risiko (Performanz!)

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _isConnectionClosed = false;

            _transaction = _connection.BeginTransaction();
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            List<T> rows = _connection.Query<T>(storedProcedure, parameters,
                                                commandType: CommandType.StoredProcedure,
                                                transaction: _transaction).ToList();
            return rows;
        }

        public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters,
                                commandType: CommandType.StoredProcedure,
                                transaction: _transaction);
        }

        private bool _isConnectionClosed = false;
        private readonly IConfiguration _config;
        private readonly ILogger<ISqlDataAccess> _logger;

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _connection?.Close();
            _isConnectionClosed = true;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _connection?.Close();
            _isConnectionClosed = true;
        }

        public void Dispose()
        {
            if (_isConnectionClosed == false)
            {
                try
                {
                    CommitTransaction();

                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Commit transaction failed in the dispose method.");
                }
            }
            _transaction = null;
            _connection = null;
        }
    }

}
