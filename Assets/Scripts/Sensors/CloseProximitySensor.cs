using GOAP;
using System.Collections.Generic;
using UnityEngine;

public class CloseProximitySensor : MonoBehaviour
{
    private Transform thisTransform;

    public float radius;
    private float radiusSqrd;

    public float heightenedAwarenessRadius;
    private float heightenedAwarenessRadiusSqrd;

    [SerializeField] private LayerMask targetLayers;

    void Start()
    {
        thisTransform = transform;
        radiusSqrd = radius * radius;
        heightenedAwarenessRadiusSqrd = heightenedAwarenessRadius * heightenedAwarenessRadius;
    }

    /// <summary>
    /// Return a list of all game object within a close radius of this agent
    /// </summary>
    /// <param name="highAlert"></param>
    /// <returns></returns>
    public List<DetectableObject> AllCloseObjects(bool highAlert) {
        // Gets all objects that can be detected
        List<DetectableObject> detectableObjects = DetectableObjectManager.instance.AllObjects();
        List<DetectableObject> closeObjects = new List<DetectableObject>();
        int numberOfObjects = detectableObjects.Count;
        // Determine the range at which to detect objects based on if the agent is alert or not
        float range = highAlert ? heightenedAwarenessRadiusSqrd : radiusSqrd;

        for (int i = 0; i < numberOfObjects; i++) {
            DetectableObject detectableObject = detectableObjects[i];

            // Skip if agent is ourself
            if (detectableObject.gameObject == gameObject) {
                continue;
            }

            // If agent is outside of view range, they cannot be seen
            Vector3 dirToTarget = detectableObject.transform.position - thisTransform.position;
            if (dirToTarget.sqrMagnitude > range) {
                continue;
            }

            dirToTarget.Normalize();

            // Perform Raycast to see if the object is behind any obstalce
            RaycastHit hit;
            if (Physics.Raycast(thisTransform.position, dirToTarget, out hit, radius, targetLayers)) {
                if (hit.collider.gameObject == detectableObject.gameObject) {
                    bool add = true;
                    if(hit.collider.gameObject.TryGetComponent(out Fox agent)) {
                        // If the detected agent is a fox, and they are sneaking, don't include them in detection
                        if (agent.isSneaking) {
                            add = false;
                        }
                    }
                    // If the object is within view, add them to the list of close objects
                    if (add) {
                        closeObjects.Add(detectableObject);
                    }
                }
            }
        }
        return closeObjects;
    }
}
