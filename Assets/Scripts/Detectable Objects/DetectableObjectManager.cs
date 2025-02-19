using System.Collections.Generic;
using UnityEngine;

public class DetectableObjectManager : MonoBehaviour
{
    public static DetectableObjectManager instance { get; private set; }
    List<DetectableObject> detectableObjects;

    void Awake()
    {
        if (instance != null) {
            Debug.LogError("Multiple DetectableObjectManagers found!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        detectableObjects = new List<DetectableObject>();
    }

    public void Add(DetectableObject detectableObject) {
        detectableObjects.Add(detectableObject);
    }

    public void Remove(DetectableObject detectableObject) {
        detectableObjects.Remove(detectableObject);
    }

    public List<DetectableObject> AllObjects() {
        return detectableObjects;
    }
}
