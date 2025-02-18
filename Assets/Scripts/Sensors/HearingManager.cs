using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour
{
    public static HearingManager instance { get; private set; }
    List<HearingSensor> listeners;

    void Awake() {
        if (instance != null) {
            Debug.LogError("Multiple HearingManager found!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        listeners = new List<HearingSensor>();
    }

    /// <summary>
    /// Add a hearing agent to the list of all hearing sensors in the scene
    /// </summary>
    /// <param name="listener"></param>
    public void Add(HearingSensor listener) {
        listeners.Add(listener);
    }

    /// <summary>
    /// Remove a hearing agent from the list of all hearing sensors in the scene
    /// </summary>
    /// <param name="listener"></param>
    public void Remove(HearingSensor listener) {
        listeners.Remove(listener);
    }

    public List<HearingSensor> AllObjects() {
        return listeners;
    }

    /// <summary>
    /// Emits a given sound at a location with a volume, and the GameObject that produced the sound
    /// </summary>
    /// <param name="location"></param>
    /// <param name="category"></param>
    /// <param name="volume"></param>
    /// <param name="caller"></param>
    public void EmitSound(Vector3 location, ESoundCategories category, float volume, GameObject caller) {
        // Notify all listeners that a sound was played
        int numOfListeners = listeners.Count;
        for(int i =0; i < numOfListeners; i++) {
            if (listeners[i].gameObject != caller) {
                listeners[i].OnHeardSound(location, category, volume, caller);
            }
        }
    }
}
