using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP {

    public class Fox : AIAgent {

        [Space(10)]
        [Header("GOAP SYSTEM")]
        [SerializeField] private GOAPAgent actionPlanner;
        [SerializeField] private Blackboard blackBoard;
        [SerializeField] private GameObject goalHolder;
        private List<GoapGoal> goalList;
        int goalCount;

        public bool isSneaking;

        protected override void Start() {
            base.Start();

            goalList = new List<GoapGoal>();
            goalList = goalHolder.GetComponents<GoapGoal>().ToList();
            goalCount = goalList.Count;
        }

        protected override void Update() {
            base.Update();
            CheckGoals();
            stats.currentAction = actionPlanner.currentActionName;
        }

        private void AddGoal(string goalName) {
            int goals = actionPlanner.allGoals.Count;
            for(int i = 0; i < goals; i++) {
                // If the goal already exists, return
                if (actionPlanner.allGoals[i].goalName == goalName) {
                    return;
                }
            }
            // Loop through all possible goals, and add the desired one
            for (int i = 0; i < goalCount; i++) {
                if (goalList[i].goalName == goalName) {
                    actionPlanner.allGoals.Add(goalList[i]);
                }
            }
        }

        private void SetBelief(eConditions belief, bool newValue) {
            for(int i = 0; i < actionPlanner.goalBeliefs.allBeliefs.Count; i++) {
                Condition currentBelief = actionPlanner.goalBeliefs.allBeliefs[i];
                if (currentBelief.key == belief) {
                    currentBelief.value = newValue;
                }
            }
        }

        public void ChangeBelief(eConditions belief, bool newValue) {
            SetBelief(belief, newValue);
        }

        //public bool hungerCheck;

        private void CheckGoals() {
            if (blackBoard.isHungry) {
                AddGoal(GoalType.EAT_FOOD);
                SetBelief(eConditions.HUNGRY, true);
            }

            if (blackBoard.isTired) {
                AddGoal(GoalType.SLEEP);
                SetBelief(eConditions.TIRED, true);
            }

            if (blackBoard.wantsToReproduce) {
                AddGoal(GoalType.REPRODUCE);
                SetBelief(eConditions.DESIRE_TO_REPRODUCE, true);
            }

            if (blackBoard.hasHeardRabbit){
                SetBelief(eConditions.HAS_HEARD_RABBIT, true);
            } else {
                SetBelief(eConditions.HAS_HEARD_RABBIT, false);
            }

            if (blackBoard.hasHeardFox) {
                SetBelief(eConditions.HAS_HEARD_FOX, true);
            } else {
                SetBelief(eConditions.HAS_HEARD_FOX, false);
            }

            if (memory.GetRabbits.Count() > 0) {
                SetBelief(eConditions.CAN_SEE_RABBIT, true);
            } else {
                SetBelief(eConditions.CAN_SEE_RABBIT, false);
            }

            if (memory.GetFoxes.Count() > 0) {
                SetBelief(eConditions.CAN_SEE_FOX, true);
            } else {
                SetBelief(eConditions.CAN_SEE_FOX, false);
            }

            if (blackBoard.caughtSenseOfSmell) {
                SetBelief(eConditions.HAS_SCENT, true);
            } else {
                SetBelief(eConditions.HAS_SCENT, false);
            }

            if (blackBoard.ranOutOfEnergy) {
                AddGoal(GoalType.REST);
                SetBelief(eConditions.RUN_OUT_OF_STAMINA, true);
            } else {
                SetBelief(eConditions.RUN_OUT_OF_STAMINA, false);
            }

            if (blackBoard.unawareTarget) {
                SetBelief(eConditions.TARGET_IS_UNSUSPECTING, true);
            } else {
                SetBelief(eConditions.TARGET_IS_UNSUSPECTING, false);
            }

            // Add 'IDLE' and 'WANDER' as a goal to fall back on if none are achievable
            AddGoal(GoalType.IDLE);
            AddGoal(GoalType.WANDER);
        }

        protected override void OnDead() {
            base.OnDead();
        }
    }

    public static class GoalType {
        public static string IDLE = "Idle";
        public static string WANDER = "Wander";
        public static string EAT_FOOD = "Not Hungry";
        public static string SLEEP = "Sleep";
        public static string REPRODUCE = "Reproduce";
        public static string REST = "Rest";
    }
}