using System;
using System.Collections.Generic;

namespace Aspirin.Api.Model.Core
{
    public interface ICacheable
    {
        KeyValuePair<string, TimeSpan> CacheSettings { get; }
    }
}