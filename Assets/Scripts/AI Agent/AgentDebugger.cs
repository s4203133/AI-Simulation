using UnityEngine;

public class AgentDebugger : MonoBehaviour
{
    AIAgent agent;
    Transform thisTransform;
    public bool drawGizmos;

    VisionSensor vision;
    CloseProximitySensor closeness;
    HearingSensor hearing;

    [HideInInspector] public Vector3 fleeDirection;
    [HideInInspector] public Vector3 forwardDirection;

    private void Start() {
        thisTransform = transform;
        vision = GetComponent<VisionSensor>();
        closeness = GetComponent<CloseProximitySensor>();
        hearing = GetComponent<HearingSensor>();
    }

    private void OnDrawGizmos() {
        if (!drawGizmos) {
            return;
        }

        vision = GetComponent<VisionSensor>();
        closeness = GetComponent<CloseProximitySensor>();
        hearing = GetComponent<HearingSensor>();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeness.radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearing.hearingRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, vision.viewRange);
        Vector3 angleA = DirectionFromAngle(vision.viewAngle / 2);
        Vector3 angleB = DirectionFromAngle(-vision.viewAngle / 2);
        Gizmos.DrawLine(transform.position, transform.position + (angleA * vision.viewRange));
        Gizmos.DrawLine(transform.position, transform.position + (angleB * vision.viewRange));

        //Gizmos.color = Color.magenta;
        //Gizmos.DrawLine(thisTransform.position + thisTransform.up, thisTransform.position + thisTransform.up + (forwardDirection * 3));
    }

    private Vector3 DirectionFromAngle(float angle) {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
