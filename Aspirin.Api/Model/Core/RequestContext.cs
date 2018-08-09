using System.Collections.Generic;

namespace Aspirin.Api.Model.Core
{
    public class RequestContext
    {
        public Dictionary<string, object> CurrentObjects { get; set; }
    }
}
