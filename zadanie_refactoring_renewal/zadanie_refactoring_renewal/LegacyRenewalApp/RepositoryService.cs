using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacyRenewalApp;

public static class RepositoryService
{
    public static TU DatabaseSearchByKey<T, TU>(Dictionary<T, TU> database, T key)
    {
        var result = database.Where(k => k.Key.Equals(key)).Select(x => x.Value).FirstOrDefault();
        if (result == null) throw new ArgumentException($"Couldn't find {key} in repository");
        return result;
    }
}