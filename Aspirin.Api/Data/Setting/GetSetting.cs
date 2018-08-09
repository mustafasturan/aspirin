using System.Threading;
using System.Threading.Tasks;
using Aspirin.Api.Model.Core;
using Dapper;
using MediatR;

namespace Aspirin.Api.Data.Setting
{
    public class GetSetting : IRequest<string>
    {
        public GetSetting(string key)
        {
            Key = key;
        }
        public string Key { get; }
    }

    public class GetSettingHandler : IRequestHandler<GetSetting, string>
    {
        private readonly IConnectionHelper _connectionHelper;

        public GetSettingHandler(IConnectionHelper connectionHelper)
        {
            _connectionHelper = connectionHelper;
        }

        public async Task<string> Handle(GetSetting request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT TOP 1 value FROM Settings WHERE key = @Key";
            using (var connection = _connectionHelper.GetAspirinDbConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<string>(sql, request);
            }
        }
    }
}
