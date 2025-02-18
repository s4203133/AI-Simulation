using TheKiwiCoder;
using UnityEngine;

public class FollowScentTrail : ActionNode
{
    [Space(15)]
    private ScentTrail trail;
    private int index;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        trail = context.aiAgent.sensorySystem.senseOfSmell.currentTrailFollowing;
        if (trail.scentTrail.Count > 1) {
            index = GetProgressThroughTrail();
            context.agent.SetDestination(trail.scentTrail[index].transform.position);
        }
        context.animator.SetBool("Moving", true);
    }

    protected override void OnStop() {
        context.aiAgent.sensorySystem.senseOfSmell.FoundEndOfTrail();
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (context.aiAgent.combat.feelsThreatened || TimeOfDaySystem.DayOrNight() == TimeOfDay.NIGHT) {
            return State.Failure;
        }

        trail = context.aiAgent.sensorySystem.senseOfSmell.currentTrailFollowing;
        if (!blackboard.hasCaughtScentTrail || trail.scentTrail == null || trail.scentTrail.Count == 0) {
            return State.Failure;
        }

        // If theres only one scent node in the list, then return true as we've already found it
        if(trail.scentTrail.Count == 1) {
            return State.Success;
        }

        float dist = Vector3.Distance(context.transform.position, context.agent.destination);
        if(dist < 3f) {
            // Move to the next Node
            index++;
            // If we reached the end of the trail, then the agent has succeeded
            if (index >= trail.scentTrail.Count) {
                return State.Success;
            }
            context.agent.SetDestination(trail.scentTrail[index].transform.position);
        }

        return State.Running;
    }

    private int GetProgressThroughTrail() {
        index = 0;
        ScentNode currentNode = context.aiAgent.sensorySystem.senseOfSmell.foundNode;
        for(int i = 0; i < trail.scentTrail.Count; i++) {
            if(currentNode == trail.scentTrail[i]) {
                index = i;
                break;
            }
        }
        return index;
    }
}
