using UnityEngine;
using TheKiwiCoder;

public class MoveToSetPosition : ActionNode {
    public Vector3 targetPosition;

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

        // Reduce the speed the agent moves at if they are hungry
        context.agent.speed = speed;
        if (context.aiAgent.hunger.isFamished) {
            context.agent.speed = famishedSpeed;
        }

        context.agent.destination = targetPosition;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if (context.agent.pathPending) {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return State.Failure;
        }

        return State.Running;
    }
}
