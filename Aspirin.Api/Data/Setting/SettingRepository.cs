using System.Threading.Tasks;
using Aspirin.Api.Model.Core;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace Aspirin.Api.Data.Setting
{
    public interface ISettingRepository
    {
        Task<string> GetSetting(string key);
    }

    public class SettingRepository: ISettingRepository
    {
        private readonly IConnectionHelper _connectionHelper;

        public SettingRepository(IConnectionHelper connectionHelper)
        {
            _connectionHelper = connectionHelper;
        }

        public async Task<string> GetSetting(string key)
        {
            const string sql = @"SELECT value FROM settings WHERE key = @key LIMIT 1";
            using (var connection = _connectionHelper.GetAspirinDbConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<string>(sql, new { key });
            }
        }
    }

    public static class SettingRepositoryExtensions
    {
        public static void AddSettingRepository(this IServiceCollection services)
        {
            services.AddSingleton<ISettingRepository, SettingRepository>();
        }
    }
}
