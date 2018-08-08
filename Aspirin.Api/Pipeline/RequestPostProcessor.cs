using System.Threading.Tasks;
using MediatR.Pipeline;

namespace Aspirin.Api.Pipeline
{
    public class RequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        public Task Process(TRequest request, TResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
