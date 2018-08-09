using System;
using System.Collections.Generic;

namespace Aspirin.Api.Model.Core
{
    public interface ICacheable
    {
        CacheOption CacheOption { get; }
        KeyValuePair<string, TimeSpan> CacheSettings { get; }
    }

    public enum CacheOption
    {
        None = 0,
        Memory = 1,
        Distrubuted = 2,
        Multi = 3
    }
}
