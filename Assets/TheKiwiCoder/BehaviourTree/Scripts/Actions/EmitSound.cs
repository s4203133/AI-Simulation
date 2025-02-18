using UnityEngine;
using TheKiwiCoder;

public class EmitSound : ActionNode
{
    [Space(20)]
    [Tooltip("Will override the 'location' variable with the position of the agents transform")]
    public bool useTransformLocation;
    public Vector3 location;
    public ESoundCategories soundType;
    public float volume;
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        if (useTransformLocation) {
            location = context.transform.position;
        }
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        HearingManager.instance.EmitSound(location, soundType, volume, context.gameObject);
        return State.Success;
    }
}
