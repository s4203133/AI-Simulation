using UnityEngine;
using TheKiwiCoder;

public class RegisterObjectsAsThreats : ActionNode
{
    [Space(15)]
    public LayerMask objectType;
    public Material taggedMaterial;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        /*List<DetectableObject> visibleObjects = context.sensorySystem.GetAllVisibleObjects();
        int numberOfVisibleObjects = visibleObjects.Count;
        for(int i = 0; i < numberOfVisibleObjects; i++) {
            GameObject visiblebject = visibleObjects[i].gameObject;
            if (visiblebject.layer == objectType) {
                //context.sensorySystem.AddThreat(visiblebject);
                visiblebject.GetComponent<MeshRenderer>().material = taggedMaterial;
            }
        }*/
        return State.Success;
    }
}
