using MediatR;

namespace Aspirin.Api.Service
{
    public class Ping : IRequest<string> { }

    public class PingHandler : RequestHandler<Ping, string>
    {
        protected override string Handle(Ping request)
        {
            return "Pong";
        }
    }
}
