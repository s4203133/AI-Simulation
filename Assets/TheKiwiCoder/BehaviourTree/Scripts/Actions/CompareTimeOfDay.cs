using TheKiwiCoder;
using UnityEngine;

public class CompareTimeOfDay : ActionNode
{
    [Space(15)]
    [SerializeField] private TimeOfDay timeOfDay;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(TimeOfDaySystem.DayOrNight() == timeOfDay) {
            return State.Success;
        }
        return State.Failure;
    }
}
