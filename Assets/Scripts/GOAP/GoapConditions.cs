using System.Collections.Generic;

namespace GOAP {

    [System.Serializable]
    public class Condition {
        public eConditions key;
        public bool value;

        public Condition(eConditions _key, bool _value) {
            key = _key;
            value = _value;
        }

        /// <summary>
        /// Compare that all the conditions in 'conditionsA' exist in 'conditionsB'
        /// </summary>
        /// <param name="conditionsA"></param>
        /// <param name="conditionsB"></param>
        /// <returns></returns>
        public static bool ConditionsMatch(List<Condition> test, List<Condition> conditions) {
            bool allMatch = true;
            int tests = test.Count;
            int amountOfConditions = conditions.Count;
            // For every condition, check that there is a matching condition in the opposite list that exists
            for (int i = 0; i < tests; i++) {
                Condition condition = test[i];
                bool match = false;
                for (int j = 0; j < amountOfConditions; j++) {
                    // If the condition exists in the other list of conditions, then continue to the next iteration
                    if (condition.key == conditions[j].key) {
                        if (condition.value == conditions[j].value) {
                            match = true;
                            continue;
                        }
                    }
                }
                if (!match) { allMatch = false; }
            }
            // If every condition found a corresponding condition in the other list, then this is a successful match
            return allMatch;
        }
    }

    /// <summary>
    /// All possible conditions the agent can believe
    /// </summary>
    public enum eConditions {
        DO_NOTHING,
        AGENT_MOVING,
        HUNGRY,
        TIRED,
        DESIRE_TO_REPRODUCE,
        HAS_TARGET_DESTINATION,
        REACHED_TARGET_DESTINATION,
        HAS_REPRODUCTION_MATE,
        REACHED_REPRODUCTION_MATE,
        HAS_HEARD_RABBIT,
        HAS_HEARD_FOX,
        HAS_LAST_KNOWN_RABBIT_LOCATION,
        HAS_LAST_KNOWN_FOX_LOCATION,
        CAN_SEE_RABBIT,
        CAN_SEE_FOX,
        HAS_SCENT,
        RUN_OUT_OF_STAMINA,
        TARGET_IS_UNSUSPECTING
    }

}