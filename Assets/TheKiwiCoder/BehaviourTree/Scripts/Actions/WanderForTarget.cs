using UnityEngine;
using TheKiwiCoder;

public class WanderForTarget : ActionNode
{
    [Space(15)]
    public EDetectableObjectCategories objectType;

    public float speed = 1;
    public float famishedSpeed = 0.5f;

    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.isStopped = false;

        // Reduce the speed the agent moves at if they are hungry
        context.agent.speed = context.aiAgent.speed;
        if (context.aiAgent.hunger.isFamished) {
            context.agent.speed = context.aiAgent.famishedSpeed;
        }

        context.agent.destination = blackboard.moveToPosition;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
        context.animator.SetBool("Moving", true);
    }

    protected override void OnStop() {
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if (blackboard.feelsThreatened || TimeOfDaySystem.DayOrNight() ==TimeOfDay.NIGHT) {
            return State.Failure;
        }
        if (context.agent.pathPending) {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance || context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || blackboard.feelsThreatened) {
            return State.Failure;
        }

        DetectableObject target = context.memory.GetClosestItem(objectType, context.transform);

        if (target != null) {
            blackboard.target = target.gameObject;
            blackboard.moveToPosition = target.transform.position;
            return State.Success;
        } 

        return State.Running;
    }

}
