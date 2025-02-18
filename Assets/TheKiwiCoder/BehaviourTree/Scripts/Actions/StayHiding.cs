using TheKiwiCoder;

public class StayHiding : ActionNode
{
    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if (blackboard.isFamished || blackboard.isTired || TimeOfDaySystem.DayOrNight() == TimeOfDay.NIGHT) {
            blackboard.hidingZone.UnOccupy();
            context.aiAgent.combat.UnHide();
            return State.Failure;
        }
        if (blackboard.feelsThreatened) {
            return State.Running;
        }
        blackboard.hidingZone.UnOccupy();
        context.aiAgent.combat.UnHide();
        return State.Success;
    }
}
