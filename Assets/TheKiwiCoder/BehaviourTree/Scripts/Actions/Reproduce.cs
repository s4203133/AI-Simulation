using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

public class Reproduce : ActionNode
{
    [Space(15)]
    public float duration;
    private float timer;

    bool canReproduceWithThisPatner;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
        timer = 0;
        blackboard.reproductionPartner = blackboard.target;
        canReproduceWithThisPatner = GetPermission(blackboard.reproductionPartner.GetComponent<Rabbit>());
    }

    protected override void OnStop() {
        canReproduceWithThisPatner = false;
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        if(blackboard.reproductionPartner == null || !canReproduceWithThisPatner) {
            return State.Failure;
        }

        AIAgent partnerAgent = blackboard.reproductionPartner.GetComponent<AIAgent>();
        if (PartnerStillValid(partnerAgent)) {
            return State.Failure;
        }

        context.agent.isStopped = true;
        context.aiAgent.reproduction.SetReproductionPartner(partnerAgent.gameObject);
        context.transform.LookAt(partnerAgent.transform.position);

        partnerAgent.GetComponent<NavMeshAgent>().isStopped = true;
        partnerAgent.reproduction.SetReproductionPartner(context.gameObject);
        partnerAgent.transform.LookAt(context.transform.position);

        timer += Time.deltaTime;
        if(timer >= duration) {
            float parentSelector = Random.Range(0f, 1f);
            if(parentSelector > 0.5f) {
                partnerAgent.GetComponent<Pregnancy>().Pregnate(CreatureType.rabbit, context.aiAgent);
            } else {
                context.gameObject.GetComponent<Pregnancy>().Pregnate(CreatureType.rabbit, partnerAgent);
            }

            context.agent.isStopped = false;
            context.aiAgent.reproduction.SetReproductionPartner(null);

            partnerAgent.GetComponent<NavMeshAgent>().isStopped = false;
            partnerAgent.reproduction.SetReproductionPartner(null);

            return State.Success;
        }
        return State.Running;
    }

    private bool GetPermission(AIAgent partner) {
        // If the target partner doesn't want to reproduce, is currently sleeping,  is already reproducing with
        // another AI, or have recently reproduced, then they are invalid
        if (context.aiAgent.reproduction.HasBeenRejectedByThisAgent(partner) ||
            !partner.reproduction.EvalatePartner(context.aiAgent)) {
            // Notify that the AI's attempt was unsuccessful
            context.aiAgent.reproduction.Rejection(partner);
            return false;
        }
        return true;
    }

    private bool PartnerStillValid(AIAgent partner) {
        if(partner.hunger.isFamished || partner.tiredness.isResting) {
            return true;
        }
        return false;
    }
}
