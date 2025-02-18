using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveTowardSoundArea : ActionNode
{
    [Space(15)]
    public ESoundCategories soundType;
    public float moveSpeed;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (context.aiAgent.combat.feelsThreatened) {
            return State.Failure;
        }

        // Get all sounds of the given type
        List<HeardSound> sounds = context.aiAgent.sensorySystem.hearingSensor.GetHeardSoundsOfType(soundType);
        Vector3 averageLocations = Vector3.zero;
        int count = sounds.Count;
        for (int i = 0; i < count; i++) {
            averageLocations += sounds[i].location;
        }
        averageLocations /= count;
        // Set location but null the target, as the agent woukdn't know what the object is, only it's location
        blackboard.moveToPosition = averageLocations + (Random.insideUnitSphere * 3);
        blackboard.target = null;
        return State.Success;
    }
}
