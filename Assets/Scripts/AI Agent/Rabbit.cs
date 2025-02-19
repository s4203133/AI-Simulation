using TheKiwiCoder;
using UnityEngine;

public class Rabbit : AIAgent {
    [Space(10)]
    [Header("BEHAVIOUR TREE SYSTEM")]
    [SerializeField] private BehaviourTreeRunner behaviourTreeRunner;
    private Blackboard blackboard => behaviourTreeRunner.tree.blackboard;

    protected override void Start() {
        base.Start();
        blackboard.agent = this;
    }

    protected override void Update() {
        base.Update();
        combat.feelsThreatened = (memory.GetFoxes.Count > 0) ? true : false;
    }

    protected override void OnDead() {

    }
}
