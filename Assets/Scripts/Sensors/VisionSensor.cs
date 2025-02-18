using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    private Transform thisTransform;

    public float viewRange;
    [Tooltip("The view range when it's dark")]
    public float viewRangeReduced;
    public float agentsViewRange { private set; get; }
    private float viewRangeSrq;

    [Range(0, 360f)]
    public float viewAngle;
    [Tooltip("The view range when it's dark")]
    [Range(0, 360f)]
    public float viewAngleReduced;
    public float agentsViewAngle { private set; get; }
    private float cosViewAngle;

    List<DetectableObject> detectableObjects;

    void Start()
    {
        thisTransform = transform;

        viewRangeSrq = viewRange * viewRange;
        agentsViewRange = viewRange;
        agentsViewAngle = viewAngle;
        cosViewAngle = Mathf.Cos(viewAngle * Mathf.Deg2Rad);

        detectableObjects = new List<DetectableObject>();
    }

    public List<DetectableObject> GetAllVisibleTargets(LayerMask targetLayers) {
        // Setup lists
        detectableObjects = DetectableObjectManager.instance.AllObjects();
        List<DetectableObject> visibleObjects = new List<DetectableObject>();

        // For every detectable object
        int numberOfObjects = detectableObjects.Count;
        for (int i = 0; i < numberOfObjects; i++) {
            DetectableObject detectableObject = detectableObjects[i];

            // Skip if agent is ourself
            if (detectableObject.gameObject == gameObject) {
                continue;
            }

            Vector3 dirToTarget = detectableObject.transform.position - thisTransform.position;

            // If agent is outside of view range, they cannot be seen
            if (dirToTarget.sqrMagnitude > viewRangeSrq) {
                continue;
            }

            dirToTarget.Normalize();

            // If outside is outside of view angle, they cannot be seen
            if (Vector3.Dot(dirToTarget, thisTransform.forward) <= cosViewAngle) {
                continue;
            }

            // Perform Raycast
            RaycastHit hit;
            if (Physics.Raycast(thisTransform.position, dirToTarget, out hit, agentsViewRange, targetLayers)) {
                if (hit.collider.gameObject == detectableObject.gameObject) {
                    // If the object can be seen, add it to the list of visible objects
                    visibleObjects.Add(detectableObject);
                }
            }
        }

        // Return all found visible objects
        return visibleObjects;
    }

    /// <summary>
    /// Recude the range and angle at which this agent can see
    /// </summary>
    public void ReduceVisibility() {
        agentsViewRange = viewRangeReduced;
        viewRangeSrq = viewRangeReduced * viewRangeReduced;

        agentsViewAngle = viewAngleReduced;
        cosViewAngle = Mathf.Cos(viewAngleReduced * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Reset the range and angle at which this agent can see back to the original values
    /// </summary>
    public void SetNormalVisibility() {
        agentsViewRange = viewRange;
        viewRangeSrq = viewRange * viewRange;

        agentsViewAngle = viewAngle;
        cosViewAngle = Mathf.Cos(viewAngle * Mathf.Deg2Rad);
    }
}
