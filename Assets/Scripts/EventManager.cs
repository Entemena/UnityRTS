/* Adapted from:
 * https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa */

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor.PackageManager;

[System.Serializable]
public class TypedEvent : UnityEvent<object> { }

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> _events;
    private Dictionary<string, TypedEvent> _typedEvents;
    private static EventManager _eventManager;

    public static EventManager Instance
    {
        get
        {
            if (!_eventManager)
            {
                _eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!_eventManager)
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                else
                    _eventManager.Init();
            }

            return _eventManager;
        }
    }

    void Init()
    {
        if (_events == null) 
        {
            _events = new Dictionary<string, UnityEvent>();
            _typedEvents = new Dictionary<string, TypedEvent>();
        }
    }

    public static void AddListener(string eventName, UnityAction listener)
    {
        UnityEvent _event = null;
        if (Instance._events.TryGetValue(eventName, out _event))
        {
            _event.AddListener(listener);
        }
        else
        {
            _event = new UnityEvent();
            _event.AddListener(listener);
            Instance._events.Add(eventName, _event);
        }
        
    }
    public static void RemoveListener(string eventName, UnityAction listener) 
    { 
        if (_eventManager == null)
        {
            return;
        }
        UnityEvent _event = null;
        if (Instance._events.TryGetValue (eventName, out _event))
        {
            _event.RemoveListener(listener);
        }
    }
    public static void TriggerEvent(string eventName) 
    {
        UnityEvent _event = null;
        if (Instance._events.TryGetValue (eventName, out _event))
        {
            _event.Invoke();
        }
    }
    public static void AddTypedListener(string eventName, UnityAction<object> listener)
    {
        TypedEvent evt = null;
        if (Instance._typedEvents.TryGetValue(eventName, out evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new TypedEvent();
            evt.AddListener(listener);
            Instance._typedEvents.Add(eventName, evt);
        }
    }

    public static void RemoveTypedListener(string eventName, UnityAction<object> listener)
    {
       if (_eventManager == null) return;
          TypedEvent evt = null;
         if (Instance._typedEvents.TryGetValue(eventName, out evt))
           evt.RemoveListener(listener);
}

    public static void TriggerTypedEvent(string eventName, object data)
    {
        TypedEvent evt = null;
        if (Instance._typedEvents.TryGetValue(eventName, out evt))
            evt.Invoke(data);
    }
}
