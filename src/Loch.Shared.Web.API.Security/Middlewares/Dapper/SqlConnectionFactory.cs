// -----------------------------------------------------------------------
// <copyright file="SqlConnectionFactory.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Loch.Shared.Web.API.Security.Middlewares.Dapper
{
    public class SqlConnectionFactory : IAPISecurityDBConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory(string connectionString) => _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            SqlConnection sqlConnection = new SqlConnection(this._connectionString);
            await sqlConnection.OpenAsync();
            IDbConnection connectionAsync = (IDbConnection)sqlConnection;
            sqlConnection = (SqlConnection)null;
            return connectionAsync;
        }
    }
}