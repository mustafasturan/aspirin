using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Aspirin.Api.Model.Core
{
    public interface IConnectionHelper
    {
        IDbConnection GetAspirinDbConnection();
    }

    public class ConnectionHelper : IConnectionHelper
    {
        private readonly IConfiguration _configuration;

        public ConnectionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetAspirinDbConnection()
        {
            var connStr = _configuration["ConnectionStrings:AspirinDB"];
            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new ArgumentException("AspirinDB connection string is missing in configuration sources.");
            }

            var connection = new NpgsqlConnection(connStr);
            connection.Open();
            return connection;
        }
    }

    public static class ConnectionHelperExtensions
    {
        public static void AddConnectionHelper(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionHelper, ConnectionHelper>();
        }
    }
}
