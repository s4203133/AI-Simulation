using UnityEngine;

namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]

    public class Blackboard {
        // Holds active nodes to query each frame to help performance 
        public NodeStack nodeStack = new NodeStack();

        public AIAgent agent;

        public Vector3 moveToPosition;
        public GameObject target;

        public float health;

        public bool isHungry => agent.hunger.isHungry;
        public bool isFamished => agent.hunger.isFamished;
        public bool isTired => agent.tiredness.isTired;

        public bool wantsToReproduce => agent.reproduction.wantsToReproduce;
        public GameObject reproductionPartner;

        public bool feelsThreatened => agent.combat.feelsThreatened;
        public bool isHiding => agent.combat.isHidden;
        public HidingSpot hidingZone;
        public bool canAttack => agent.combat.canAttack;


        public bool hasCaughtScentTrail => agent.sensorySystem.senseOfSmell.hasScent;
        public ScentTrail trailToFollow;
    }
}