using System;
using System.Collections.Generic;

/// <summary>
/// QueryBus is used for querying data from other modules, such as GamePlay elements, UI elements, etc. It allows you to request data without needing to know where it comes from, which can help decouple your code and make it more modular.
/// </summary>
public static class QueryBus
{
    private static Dictionary<Type, Delegate> queries = new();

    public static void Subscribe<TQuery, TResult>(Func<TQuery, TResult> handler)
        where TQuery : IQueryResult<TResult>
    {
        queries[typeof(TQuery)] = handler;
    }

    public static TResult Query<TResult>(IQueryResult<TResult> query)
    {
        var queryType = query.GetType();
        if (queries.TryGetValue(queryType, out var del))
        {
            return (TResult)del.DynamicInvoke(query);
        }

        throw new Exception($"No query handler for {queryType}");
    }
}
