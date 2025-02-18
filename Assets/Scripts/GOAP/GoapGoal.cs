using System.Collections.Generic;
using UnityEngine;

namespace GOAP {

    public class GoapGoal : MonoBehaviour {
        public string goalName;
        public int priority;

        [Tooltip("What conditions need to be met to satisfy this goal")]
        public List<Condition> conditionsToSatisfy;

        /// <summary>
        /// For each condition to be satisfied, check if a list of effects match
        /// </summary>
        /// <param name="effects"></param>
        /// <returns></returns>
        public bool AllConditionsSatisfied(List<Condition> effects) {
            foreach (Condition condition in conditionsToSatisfy) {
                for (int i = 0; i < effects.Count; i++) {
                    if (effects[i].key == condition.key) {
                        // If a match was found, but the values aren't the same then the conditions aren't satisfied
                        if (effects[i].value != condition.value) {
                            return false;
                        }
                        continue;
                    }
                }
            }
            // If we managed to make it the whole way through the loop, then all the conditions have been satisfied
            return true;
        }
    }

/*    public static class GoalPriority {
        public const int Maximum = 100;

        public const int Relaxed = 1;
        public const int Investigative = 40;
        public const int Aggressive = 80;

        public const int minimum = 0;

        public const int DoNotRun = 0;
    }*/
}