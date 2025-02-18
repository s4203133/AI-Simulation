using TheKiwiCoder;

public class ChaseTarget : ActionNode
{
    public float speed = 5;
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
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if(blackboard.target == null) {
            return State.Failure;
        }

        context.agent.destination = blackboard.target.transform.position;

        if (context.agent.pathPending) {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || blackboard.feelsThreatened) {
            return State.Failure;
        }

        return State.Running;
    }
}
