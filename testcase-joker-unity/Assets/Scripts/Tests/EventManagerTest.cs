using UnityEngine;
using System.Collections;

public class EventManagerTest : MonoBehaviour
{
    private bool eventReceived = false;

    void Start()
    {
        StartCoroutine(RunTest());
    }

    IEnumerator RunTest()
    {
        Debug.Log("Starting EventManager OnDestroy test");
        
        // Make sure we have an EventManager instance
        var eventManager = EventManager.Instance;
        
        // Register for a test event
        eventManager.RegisterEvent<TestEvent>(OnTestEvent);
        
        // Verify we can receive the event
        eventReceived = false;
        eventManager.Raise(new TestEvent());
        
        // Should show "Event received" in console
        if (eventReceived)
            Debug.Log("✅ Event received successfully before destruction");
        else
            Debug.LogError("❌ Event was not received before destruction");
        
        // Now destroy the EventManager
        Debug.Log("Destroying EventManager...");
        Destroy(eventManager.gameObject);
        
        // Wait a frame to ensure OnDestroy is called
        yield return null;
        
        // Try raising the event again
        eventReceived = false;
        EventBus<TestEvent>.Raise(new TestEvent());
        
        // Should NOT log "Event received" since the binding should be unregistered
        if (!eventReceived)
            Debug.Log("✅ Event was not received after destruction (correct behavior)");
        else
            Debug.LogError("❌ Event was still received after destruction (OnDestroy unregistration failed)");
        
        Debug.Log("Test completed");
    }
    
    private void OnTestEvent(TestEvent evt)
    {
        Debug.Log("Event received");
        eventReceived = true;
    }
}

// Simple test event
public struct TestEvent : IEvent { } 