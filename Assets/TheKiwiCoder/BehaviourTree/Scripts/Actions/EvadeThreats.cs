using UnityEngine;
using TheKiwiCoder;
using System.Collections.Generic;

public class EvadeThreats : ActionNode {
    [Space(15)]
    public float obstacleDetectionRadius;
    public LayerMask obstacleLayers;
    public LayerMask walkableLayers;

    public LayerMask hidingZoneLayers;
    private GameObject hidingZone;

    Vector3 currentVelocity;
    private Vector3 lastPosition;
    private float speed;

    private AgentDebugger agentDebugger;
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);

        agentDebugger = context.gameObject.GetComponent<AgentDebugger>();
        // Reduce the speed the agent moves at if they are hungry
        speed = context.aiAgent.speed;
        if (context.aiAgent.hunger.isFamished) {
            speed = context.aiAgent.famishedSpeed;
        }

        context.aiAgent.IsUsingStamina(true);
        context.agent.ResetPath();
        hidingZone = null;
        currentVelocity = context.agent.velocity;
        context.animator.SetBool("Moving", true);
    }

    protected override void OnStop() {
        if(context.aiAgent.stamina > 0) { 
        context.aiAgent.IsUsingStamina(false);
        }
        context.agent.ResetPath();
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (!blackboard.feelsThreatened) {
            return State.Failure;
        }


        if(context.aiAgent.stamina <= 0) {
            return State.Failure;
        }

        if (blackboard.canAttack) {
            blackboard.moveToPosition = context.agent.destination;
            return State.Success;
        }

        // While fleeing, check if the rabbit can see a hiding spot to move to
        if(hidingZone == null) {
            if (context.memory.ItemExistsInMemory(EDetectableObjectCategories.TALL_GRASS)) {
                hidingZone = context.memory.GetClosestItem(EDetectableObjectCategories.TALL_GRASS, context.transform).gameObject;
                blackboard.hidingZone = hidingZone.GetComponent<HidingSpot>();
            }
        } else {
            // If the hiding spot the agent is moving to has been taken, unassign it 
            if (blackboard.hidingZone.isTaken) {
                blackboard.hidingZone = null;
                hidingZone = null;
                return State.Running;
            }
            // Move towards the hiding spot. If the rabbit has reached it, then they have successfully evaded the fox
            context.agent.SetDestination(hidingZone.transform.position);
            float distToTarget = Vector3.Distance(context.transform.position, hidingZone.transform.position);
            if (distToTarget <= 1f) {
                context.aiAgent.combat.Hide();
                blackboard.hidingZone.Occupy();
                return State.Success;
            }
            return State.Running;
        }

        Vector3 newDirection = Vector3.zero;
        List<DetectableObject> threats = context.aiAgent.memory.GetFoxes;
        int numberOfThreats = threats.Count;
        if(numberOfThreats == 0) {
            context.agent.ResetPath();
            return State.Failure;
        }

        for (int i = 0; i < numberOfThreats; i++) {
            Vector3 targetDirection = (context.transform.position - threats[i].transform.position).normalized;
            Vector3 newVelocity = targetDirection * speed;
            Vector3 force = newVelocity - currentVelocity;
            newDirection += force;
        }

        newDirection.y = 0;
        Vector3 targetPosition = context.transform.position + newDirection.normalized * speed;

        context.agent.SetDestination(targetPosition);

        agentDebugger.forwardDirection = (context.transform.forward);
        agentDebugger.fleeDirection = (targetPosition);

        currentVelocity = (context.transform.position - lastPosition);
        lastPosition = context.transform.position;

        return State.Running;
    }

    bool CheckForObstalces(Vector3 fromPosition) {
        if (Physics.SphereCast(context.transform.position, obstacleDetectionRadius, context.transform.forward, out RaycastHit hit, 3, obstacleLayers)) {

            Collider[] surroundingTiles = Physics.OverlapSphere(hit.transform.position, 10, walkableLayers);
            float closestDist = float.MaxValue;
            GameObject targetTile = null;

            for (int i = 0; i < surroundingTiles.Length; i++) {
                float dist = Vector3.Distance(hit.transform.position, fromPosition);
                if (dist < closestDist) {
                    closestDist = dist;
                    targetTile = surroundingTiles[i].gameObject;
                }
            }
            context.agent.SetDestination(targetTile.transform.position);
            return true;
        }
        return false;
    }

    bool CheckForHidingZone() {
        return context.memory.ItemExistsInMemory(EDetectableObjectCategories.TALL_GRASS);
    }
}

