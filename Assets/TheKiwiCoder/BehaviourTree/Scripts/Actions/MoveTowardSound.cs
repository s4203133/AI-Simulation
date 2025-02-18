using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveTowardSound : ActionNode
{
    [Space(15)]
    public ESoundCategories soundType;
    public float moveSpeed;

    protected override void OnStart() {
        context.animator.SetBool("Moving", true);
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (context.aiAgent.combat.feelsThreatened) {
            return State.Failure;
        }

        // Get all sounds of the given type
        List<HeardSound> sounds = context.aiAgent.sensorySystem.hearingSensor.GetHeardSoundsOfType(soundType);
        // Find the closest sound the agent has heard for them to move towards
        float minDist = float.MaxValue;
        Vector3 location = Vector3.zero;
        int count = sounds.Count;
        for (int i = 0; i < count; i++) {
            float dist = Vector3.Distance(context.transform.position, sounds[i].location);
            if(dist < minDist) {
                minDist = dist;
                location = sounds[i].location;
            }
        }
        // Set location but null the target, as the agent woukdn't know what the object is, only it's location
        blackboard.moveToPosition = location;
        blackboard.target = null;
        return State.Success;
    }
}
