using UnityEngine;
using TheKiwiCoder;

public class CompareFloat : ActionNode
{
    [Space(15)]
    [SerializeField] private string varaibleName;
    private float variableToCompare;
    public float value;
    public ComparisonTypes comparisonType;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        blackboard.nodeStack.PopNode();
    }

    protected override State OnUpdate() {
        float variableToCompare = (float)blackboard.GetType().GetField(varaibleName).GetValue(blackboard);
        return Compare(variableToCompare);
    }

    public State Compare(float comparingVariable) {
        switch (comparisonType) {
            case ComparisonTypes.Equal:
                if (comparingVariable == value)
                    return State.Success;
                break;
            case ComparisonTypes.NotEqual:
                if (comparingVariable != value)
                    return State.Success;
                break;
            case ComparisonTypes.GreaterThan:
                if (comparingVariable > value)
                    return State.Success;
                break;
            case ComparisonTypes.GreaterThanOrEqualTo:
                if (comparingVariable >= value)
                    return State.Success;
                break;
            case ComparisonTypes.LessThan:
                if (comparingVariable < value)
                    return State.Success;
                break;
            case ComparisonTypes.LessThanOrEqualTo:
                if (comparingVariable <= value)
                    return State.Success;
                break;
        }
        return State.Failure;
    }
}

public enum ComparisonTypes {
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqualTo,
    LessThan,
    LessThanOrEqualTo,
}
