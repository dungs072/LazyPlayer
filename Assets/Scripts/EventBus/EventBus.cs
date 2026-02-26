using System;
using System.Collections.Generic;
/// <summary>
/// communication between GamePlay and UI, screen and screen, GamePlay element and GamePlay element, 
/// </summary>
public static class EventBus
{
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
            events[typeof(T)] = Delegate.Remove(existing, listener);
    }

    public static void Publish<T>(T eventData)
    {
        if (events.TryGetValue(typeof(T), out var existing))
            (existing as Action<T>)?.Invoke(eventData);
    }
}