using System.Collections.Generic;
using UnityEngine;

public class DetectableObject : MonoBehaviour
{
    public EDetectableObjectCategories objectCategory;

    public delegate void DetectableObjEvent(DetectableObject obj);
    public static DetectableObjEvent onDestroyed;


    private Dictionary<Memory, Coroutine> memoryBeingRemovedFrom = new Dictionary<Memory, Coroutine>();

    private void Start()
    {
        DetectableObjectManager.instance.Add(this);
    }

    private void OnDestroy() {
        onDestroyed?.Invoke(this);
        DetectableObjectManager.instance.Remove(this);
    }

    public void Remove() {
        onDestroyed.Invoke(this);
    }
}