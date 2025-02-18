using UnityEngine;
using TheKiwiCoder;
using System.Collections.Generic;

public class MoveAwayFromSound : ActionNode
{
    [Space(15)]
    public ESoundCategories soundType;
    public float moveSpeed;
    [Tooltip("How wide of a search range to look for new tiles to move to")]
    public float searchRadius;
    public LayerMask walkableGround;

    List<HeardSound> sounds = new List<HeardSound>();

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        sounds.Clear();
        context.animator.SetBool("Moving", true);
    }

    protected override void OnStop() {
        context.animator.SetBool("Moving", false);
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        sounds = context.aiAgent.sensorySystem.hearingSensor.GetHeardSoundsOfType(soundType);
        Vector3 averageLocations = Vector3.zero;
        int count = sounds.Count;
        for(int i = 0; i < count; i++) {
            averageLocations += sounds[i].location;
        }
        averageLocations /= count;
        Vector3 direction = (context.transform.position - averageLocations).normalized;

        Collider[] surroundingTiles = Physics.OverlapSphere(context.transform.position + (direction * searchRadius), searchRadius, walkableGround);

        if (surroundingTiles.Length == 0) {
            Debug.Log("No Tiles Found");
            return State.Failure;
        } else if (surroundingTiles.Length == 1) {
            blackboard.target = surroundingTiles[0].gameObject;
            blackboard.moveToPosition = surroundingTiles[0].transform.position;
        } else {
            float maxDistance = 0;
            GameObject selectedTile = surroundingTiles[0].gameObject;
            for(int i = 0; i < surroundingTiles.Length; i++) {
                float dist = Vector3.Distance(averageLocations, surroundingTiles[i].transform.position);
                if(dist > maxDistance) {
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
