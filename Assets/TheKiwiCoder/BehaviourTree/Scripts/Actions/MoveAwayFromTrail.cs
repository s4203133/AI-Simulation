using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveAwayFromTrail : ActionNode
{
    private List<ScentNode> trail;

    [Space(15)]
    [SerializeField] private float searchRange;
    [SerializeField] private LayerMask walkableGroundLayers;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        trail = blackboard.trailToFollow.scentTrail;
        context.animator.SetBool("Moving", true);
    }

    protected override void OnStop() {
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (context.aiAgent.combat.feelsThreatened || TimeOfDaySystem.DayOrNight() == TimeOfDay.NIGHT) {
            return State.Failure;
        }

        if (trail.Count == 0) {
            return State.Failure;
        }
        Vector3 trailDirection = (context.transform.position) - (trail[0].transform.position);

        Collider[] surroundingTiles = Physics.OverlapSphere(context.transform.position + (trailDirection.normalized * searchRange), searchRange, walkableGroundLayers);

        if (surroundingTiles.Length == 0) {
            Debug.Log("No Tiles Found");
            return State.Failure;
        } else if (surroundingTiles.Length == 1) {
            blackboard.target = surroundingTiles[0].gameObject;
            blackboard.moveToPosition = surroundingTiles[0].transform.position;
        } else {
            float maxDistance = 0;
            GameObject selectedTile = surroundingTiles[0].gameObject;
            for (int i = 0; i < surroundingTiles.Length; i++) {
                float dist = Vector3.Distance(trail[0].transform.position, surroundingTiles[i].transform.position);
                if (dist > maxDistance) {
                    maxDistance = dist;
                    selectedTile = surroundingTiles[i].gameObject;
                }
            }
            blackboard.target = selectedTile;
            blackboard.moveToPosition = selectedTile.transform.position;
        }

        return State.Success;
    }
}
