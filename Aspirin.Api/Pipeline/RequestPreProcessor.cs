using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR.Pipeline;

namespace Aspirin.Api.Pipeline
{
    public class RequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IValidator<TRequest> _validator;

        public RequestPreProcessor(IValidator<TRequest> validator = null)
        {
            _validator = validator;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator == null)
            {
                return Task.CompletedTask;
            }

            var context = new ValidationContext(request);

            var failures = _validator.Validate(context).Errors.Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return Task.CompletedTask;
        }
    }
}
