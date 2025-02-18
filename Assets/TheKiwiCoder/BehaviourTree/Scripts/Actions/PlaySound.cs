using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class PlaySound : ActionNode
{
    [Space(15)]
    public bool useObjectPositionLocation;
    public HeardSound sound;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        if (useObjectPositionLocation) {
            sound.location = context.transform.position;
        }
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        List<AIAgent> agents = AIAgentManager.AllAgents();
        int count = agents.Count;
        for (int i = 0; i < count; i++) {
            HearingManager.instance.EmitSound(sound.location, sound.soundCategory, sound.volume, context.gameObject);
        }
        return State.Success;
    }
}
