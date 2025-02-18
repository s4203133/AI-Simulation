using UnityEngine;
using TheKiwiCoder;

public class MoveToPosition : ActionNode {

    public float speed = 1;
    public float famishedSpeed = 0.5f;

    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    private bool hadTargetOnStart;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        context.agent.isStopped = false;

        // Reduce the speed the agent moves at if they are hungry
        context.agent.speed = context.aiAgent.speed;
        if (context.aiAgent.hunger.isFamished) {
            context.agent.speed = context.aiAgent.famishedSpeed;
        }

        context.agent.destination = blackboard.moveToPosition;
        hadTargetOnStart = blackboard.target != null;
        context.animator.SetBool("Moving", true);
    }

    protected override void OnStop() {
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if (context.agent.pathPending) {
            return State.Running;
        }

        float distToTarget = Vector3.Distance(context.transform.position, context.agent.destination);
        if (distToTarget <= tolerance) {
            return State.Success;
        }

        if (TestFailConditions()) {
            return State.Failure;
        }

        return State.Running;
    }

    private bool TestFailConditions() {
        // General Node Failure
        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return true;
        }
        // If the AI had a target object when the node was activated, but doesn't have one this frame, return failure
        if(hadTargetOnStart && (blackboard.target == null || !blackboard.target.activeInHierarchy)) {
            context.agent.isStopped = true;
            return true;
        }
        // If the AI feels threatened, stop moving towards its goal
        if (blackboard.feelsThreatened || context.aiAgent.combat.canAttack) {
            return true;
        }
        return false;
    }
}
