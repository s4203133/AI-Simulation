using UnityEngine;

namespace GOAP {

    public class Blackboard : MonoBehaviour {

        // List of useful components to be accessed by the GOAP system

        private AIAgent agent;

        public Vector3 targetLocation;
        public GameObject targetObject;
        private Rabbit targetRabbit;

        private void Start() {
            agent = GetComponent<AIAgent>();    
        }

        public bool isHungry => agent.hunger.isHungry;
        public bool isTired => agent.tiredness.isTired;

        public bool isSleeping;
        public bool wantsToReproduce => agent.reproduction.wantsToReproduce;

        [SerializeField] private ESoundCategories rabbitSounds;
        public bool hasHeardRabbit => agent.sensorySystem.hearingSensor.HasHeardSoundOfType(rabbitSounds);

        [SerializeField] private ESoundCategories foxSounds;
        public bool hasHeardFox => agent.sensorySystem.hearingSensor.HasHeardSoundOfType(foxSounds);

        public bool caughtSenseOfSmell => agent.sensorySystem.senseOfSmell.hasScent;

        public bool ranOutOfEnergy => agent.stamina <= 0.01;

        public bool unawareTarget = true;
    }

}