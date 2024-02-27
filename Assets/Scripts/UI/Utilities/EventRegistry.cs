using System;
using UnityEngine.UIElements;

// Copied over from the official UQuiz Unity tutorial project
// https://assetstore.unity.com/packages/essentials/tutorial-projects/quizu-a-ui-toolkit-sample-268492
// downloaded from the Unity Asset Store on 2024-02-19
public class EventRegistry : IDisposable
{
    // Single delegate to hold all unregister actions
    Action m_UnregisterActions;

    // Registers a callback for a specific VisualElement and event type (e.g. ClickEvent, MouseEnterEvent, etc.). 
    // The callback to unregister is added to the common delegate for later cleanup.
    public void RegisterCallback<TEvent>(VisualElement visualElement, Action<TEvent> callback) where TEvent : EventBase<TEvent>, new()
    {
        EventCallback<TEvent> eventCallback = new EventCallback<TEvent>(callback);
        visualElement.RegisterCallback(eventCallback);

        m_UnregisterActions += () => visualElement.UnregisterCallback(eventCallback);
    }

    // Registers a simplified callback for a specific VisualElement and event type without requiring
    // event data. The unregister action is added to the common delegate for later cleanup.
    public void RegisterCallback<TEvent>(VisualElement visualElement, Action callback) where TEvent : EventBase<TEvent>, new()
    {
        EventCallback<TEvent> eventCallback = new EventCallback<TEvent>((evt) => callback());
        visualElement.RegisterCallback(eventCallback);

        m_UnregisterActions += () => visualElement.UnregisterCallback(eventCallback);
    }

    // Registers a value changed callback for a specific BindableElement e.g. (a Slider or TextField, which use a
    // ChangeEvents). When the value of the bindableElement changes, the callback is invoked and receives the
    // the new value of the BindableElement. 
    // 
    // Like the other methods, the callback to unregister is added to the common delegate for later cleanup.
    public void RegisterValueChangedCallback<T>(BindableElement bindableElement, Action<T> callback) where T : struct
    {
        EventCallback<ChangeEvent<T>> eventCallback = new EventCallback<ChangeEvent<T>>(evt => callback(evt.newValue));
        bindableElement.RegisterCallback(eventCallback);

        m_UnregisterActions += () => bindableElement.UnregisterCallback(eventCallback);
    }

    public void RegisterDropdownValueChangedCallback(BindableElement bindableElement, Action<string> callback) 
    {
        EventCallback<ChangeEvent<string>> eventCallback = new EventCallback<ChangeEvent<string>>(evt => callback(evt.newValue));
        bindableElement.RegisterCallback(eventCallback);

        m_UnregisterActions += () => bindableElement.UnregisterCallback(eventCallback);
    }


    // Unregisters all callbacks by invoking each stored unregister action, then sets the common delegate to null.
    // Call this method from the client when all the registered callbacks are no longer needed (e.g.,
    // in the client's OnDisable).
    public void Dispose()
    {
        m_UnregisterActions?.Invoke();
        m_UnregisterActions = null;
    }
}

