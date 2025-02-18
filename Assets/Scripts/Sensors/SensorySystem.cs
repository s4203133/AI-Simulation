using System.Collections.Generic;
using UnityEngine;

public class SensorySystem : MonoBehaviour
{
    [HideInInspector] public AIAgent agent;

    [Header("Vision")]
    [SerializeField] private VisionSensor visionSensor;
    [SerializeField] private LayerMask targetObjectLayers;
    private bool intermittent;

    [Space(5)]
    [Header("Hearing")]
    public HearingSensor hearingSensor;

    [Space(5)]
    [Header("Close Proximity")]
    [SerializeField] private CloseProximitySensor closeProximitySensor;

    [Space(5)]
    [Header("Scent")]
    public SenseOfSmell senseOfSmell;

    [Header("Threats")]
    [SerializeField] private EDetectableObjectCategories threatTypes;

    private void Start() {
        agent = GetComponent<AIAgent>();    
    }

    private void Update() {
        if (intermittent) {
            // Get all objects that can be seen and that are within a close range
            List<DetectableObject> seenObjects = visionSensor.GetAllVisibleTargets(targetObjectLayers);
            seenObjects.AddRange(closeProximitySensor.AllCloseObjects(agent.combat.feelsThreatened));
            // Add the found objects into memory
            if (seenObjects != null) {
                agent.memory.AddDetectableObjects(seenObjects);
            }
            intermittent = false;
        } else {
            intermittent = true;
        }
    }

    public bool HasHeardSound() {
        return hearingSensor.allHeardSounds.Count > 0;
    }

    public bool HasHeardSoundOfType(ESoundCategories soundType) {
        return hearingSensor.HasHeardSoundOfType(soundType);
    }

    public Vector3 GetGeneralSoundArea(ESoundCategories soundType) {
        return hearingSensor.GetGeneralSoundArea(soundType);
    }

    public void ReduceVisibility() {
        visionSensor.ReduceVisibility();
    }

    public void ResetVisibility() {
        visionSensor.SetNormalVisibility();
    }

    public float ViewRange => visionSensor.agentsViewRange;
    public float ViewAngle => visionSensor.agentsViewAngle;
}
