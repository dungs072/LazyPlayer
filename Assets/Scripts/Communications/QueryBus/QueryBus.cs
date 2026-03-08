using System;
using System.Collections.Generic;

/// <summary>
/// QueryBus is used for querying data from other modules, such as GamePlay elements, UI elements, etc. It allows you to request data without needing to know where it comes from, which can help decouple your code and make it more modular.
/// </summary>
public static class QueryBus
{
    private static Dictionary<Type, Delegate> queries = new();

    public static void Subscribe<TQuery, TResult>(Func<TQuery, TResult> handler)
    {
        queries[typeof(TQuery)] = handler;
    }

    public static TResult Query<TQuery, TResult>(TQuery query)
    {
        if (queries.TryGetValue(typeof(TQuery), out var del))
        {
            if (del is Func<TQuery, TResult> typedHandler)
            {
                return typedHandler(query);
            }

            var result = del.DynamicInvoke(query);
            if (result == null)
            {
                return default;
            }

            if (result is TResult typedResult)
            {
                return typedResult;
            }

            throw new InvalidCastException(
                $"Query result type mismatch for {typeof(TQuery)}. Handler returned {result.GetType()}, requested {typeof(TResult)}"
            );
        }

        throw new Exception($"No query handler for {typeof(TQuery)}");
    }
}
