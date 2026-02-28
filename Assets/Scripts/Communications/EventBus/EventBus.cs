using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
/// <summary>
/// communication between GamePlay and UI, screen and screen, GamePlay element and GamePlay element, 
/// </summary>
public static class EventBus
{
    /// <summary>
    /// The Type should be struct for better performance, but class is also supported, just be careful about memory allocation and GC.
    /// </summary>
    private static Dictionary<Type, Delegate> events = new();

    public static void Subscribe<T>(Action<T> listener)
    {
        if (events.TryGetValue(typeof(T), out var existing))
            events[typeof(T)] = Delegate.Combine(existing, listener);
        else
            events[typeof(T)] = listener;
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        if (events.TryGetValue(typeof(T), out var existing))
        {
            var result = Delegate.Remove(existing, listener);
            if (result == null)
                events.Remove(typeof(T));
            else
                events[typeof(T)] = result;
        }
    }

    public static void Publish<T>(T eventData)
    {
        if (events.TryGetValue(typeof(T), out var existing))
            (existing as Action<T>)?.Invoke(eventData);
    }
    //! Must check
    public static async UniTask PublishAsync<T>(T eventData)
    {
        if (!events.TryGetValue(typeof(T), out var existing))
            return;

        var handlers = existing.GetInvocationList();

        var tasks = new List<UniTask>();

        foreach (var handler in handlers)
        {
            if (handler is Func<T, UniTask> asyncHandler)
            {
                tasks.Add(asyncHandler(eventData));
            }
            else if (handler is Action<T> syncHandler)
            {
                syncHandler(eventData);
            }
        }

        await UniTask.WhenAll(tasks);
    }
}