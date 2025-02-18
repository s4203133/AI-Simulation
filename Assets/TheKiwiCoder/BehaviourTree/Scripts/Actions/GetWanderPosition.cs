using UnityEngine;
using TheKiwiCoder;

public class GetWanderPosition : ActionNode
{
    public float wanderRadius;
    public LayerMask walkableGround;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        Collider[] surroundingTiles = Physics.OverlapSphere(context.transform.position, wanderRadius, walkableGround);

        if(surroundingTiles.Length == 0 ) {
            //Debug.Log("Found No Tiles");
            return State.Failure;
        } else if(surroundingTiles.Length == 1) {
            //Debug.Log("Found 1 Tile");
            blackboard.target = surroundingTiles[0].gameObject;
            blackboard.moveToPosition = surroundingTiles[0].transform.position;
        } else {
           // Debug.Log("Found Random Tile");
            GameObject tile = surroundingTiles[Random.Range(0, surroundingTiles.Length)].gameObject;
            blackboard.target = tile;
            blackboard.moveToPosition = tile.transform.position;
        }

        return State.Success;
    }
}
