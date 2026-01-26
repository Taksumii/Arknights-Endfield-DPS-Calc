using System;
using System.Collections.Generic;
using AKEndfieldDmgCalc.Models;

namespace AKEndfieldDmgCalc.Helpers
{
    public sealed class BaselineCache
    {
        private readonly Dictionary<HitContext, (double Raw100, double Final100)> _cache = new();

        public bool TryGet(HitContext key, out double raw100, out double final100)
        {
            if (_cache.TryGetValue(key, out var v))
            {
                raw100 = v.Raw100;
                final100 = v.Final100;
                return true;
            }
            raw100 = 0;
            final100 = 0;
            return false;
        }

        public void Set(HitContext key, double raw100, double final100)
            => _cache[key] = (raw100, final100);

        public void Clear() => _cache.Clear();
    }
}
