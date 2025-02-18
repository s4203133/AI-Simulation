using UnityEngine;

public class GenerateStats : MonoBehaviour
{
    /// <summary>
    /// Generates the stats for a new agent. A random type of crossover is selected, so genes are taken from 2 parents
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    public static AgentStats GetStatsForNewAgent(AgentStats parent1, AgentStats parent2) {
        AgentStats returnStats = new AgentStats();
        CrossOverTypes crossOver = RandomCrossOver();
        switch (crossOver) {
            case (CrossOverTypes.RANDOM):
                returnStats = RandomStats(parent1, parent2);
                break;
            case (CrossOverTypes.ONE_POINT):
                returnStats = OnePointCrossoverStats(parent1, parent2);
                break;
            case (CrossOverTypes.TWO_POINT):
                returnStats = TwoPointCrossoverStats(parent1, parent2);
                break;
        }
        return returnStats;
    }

    /// <summary>
    /// Generate each stat by randomly selecting which parent to inherit it from.
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    static AgentStats RandomStats(AgentStats parent1, AgentStats parent2) {
        AgentStats stats = new AgentStats();

        // Select a random parent to get the stats from, and add the chance for mutation
        stats.moveSpeed = RandomParent(parent1, parent2).moveSpeed + MutationChance(15, 0.5f);
        stats.maxStamina = RandomParent(parent1, parent2).maxStamina + MutationChance(15, 0.5f);
        stats.famishedSpeed = RandomParent(parent1, parent2).famishedSpeed + MutationChance(15, 0.2f);
        stats.staminaReduction = (RandomParent(parent1, parent2).staminaReduction + MutationChance(15, 0.25f)) + SpeedTradeOff(stats.moveSpeed);
        stats.hungerIncreaseSpeed = (RandomParent(parent1, parent2).hungerIncreaseSpeed + MutationChance(15, 0.5f)) - SpeedTradeOff(stats.moveSpeed);
        stats.tirednessIncreaseSpeed = RandomParent(parent1, parent2).tirednessIncreaseSpeed + MutationChance(15, 0.5f) - SpeedTradeOff(stats.moveSpeed);
        stats.reproductiveUrgeSpeed = RandomParent(parent1, parent2).reproductiveUrgeSpeed + MutationChance(15, 0.5f);
        stats.maxOffspring = RandomParent(parent1, parent2).maxOffspring;
        stats.viewDistance = RandomParent(parent1, parent2).viewDistance + MutationChance(15, 2f);
        stats.viewAngle = RandomParent(parent1, parent2).viewAngle + MutationChance(15, 2f);
        stats.hearingRange = RandomParent(parent1, parent2).hearingRange + MutationChance(15, 1f);
        stats.closeProximityRadius = RandomParent(parent1, parent2).closeProximityRadius + MutationChance(15, 0.25f);
        stats.highStandards = RandomParent(parent1, parent2).highStandards;  
        stats.attack = RandomParent(parent1, parent2).attack + MutationChance(15, 0.1f);
        stats.defense = RandomParent(parent1, parent2).defense + MutationChance(15, 0.1f);

        return stats;
    }

    /// <summary>
    /// A random consecutive amount of stats will be inherited by parent 1, and the rest will be inherited by parent 2 
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    static AgentStats OnePointCrossoverStats(AgentStats parent1, AgentStats parent2) {
        AgentStats stats = new AgentStats();
        int crossOverPoint = Random.Range(0, 14);
        int index = 0;

        // Select a random parent to get the stats from, and add the chance for mutation
        stats.moveSpeed = OnePointParent(++index, crossOverPoint, parent1, parent2).moveSpeed + MutationChance(15, 0.5f);
        stats.maxStamina = OnePointParent(++index, crossOverPoint, parent1, parent2).maxStamina + MutationChance(15, 0.5f);
        stats.famishedSpeed = OnePointParent(++index, crossOverPoint, parent1, parent2).famishedSpeed + MutationChance(15, 0.2f);
        stats.staminaReduction = OnePointParent(++index, crossOverPoint, parent1, parent2).staminaReduction + MutationChance(15, 0.25f) + SpeedTradeOff(stats.moveSpeed);
        stats.hungerIncreaseSpeed = OnePointParent(++index, crossOverPoint, parent1, parent2).hungerIncreaseSpeed + MutationChance(15, 0.5f) - SpeedTradeOff(stats.moveSpeed);
        stats.tirednessIncreaseSpeed = OnePointParent(++index, crossOverPoint, parent1, parent2).tirednessIncreaseSpeed + MutationChance(15, 0.5f) - SpeedTradeOff(stats.moveSpeed);
        stats.reproductiveUrgeSpeed = OnePointParent(++index, crossOverPoint, parent1, parent2).reproductiveUrgeSpeed + MutationChance(15, 0.5f);
        stats.maxOffspring = OnePointParent(++index, crossOverPoint, parent1, parent2).maxOffspring;
        stats.viewDistance = OnePointParent(++index, crossOverPoint, parent1, parent2).viewDistance + MutationChance(15, 2f);
        stats.viewAngle = OnePointParent(++index, crossOverPoint, parent1, parent2).viewAngle + MutationChance(15, 2f);
        stats.hearingRange = OnePointParent(++index, crossOverPoint, parent1, parent2).hearingRange + MutationChance(15, 1f);
        stats.closeProximityRadius = OnePointParent(++index, crossOverPoint, parent1, parent2).closeProximityRadius + MutationChance(15, 0.25f);
        stats.highStandards = OnePointParent(++index, crossOverPoint, parent1, parent2).highStandards;
        stats.attack = OnePointParent(++index, crossOverPoint, parent1, parent2).attack + MutationChance(15, 0.1f);
        stats.defense = OnePointParent(++index, crossOverPoint, parent1, parent2).defense + MutationChance(15, 0.1f);

        return stats;
    }

    /// <summary>
    /// A random consecutive amount of stats are taken from parent 1, then another random consecutive amount of stats are taken from parent 2, and the rest after that are taken from parent 1 again.
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    static AgentStats TwoPointCrossoverStats(AgentStats parent1, AgentStats parent2) {
        AgentStats stats = new AgentStats();
        int crossOverPoint1 = Random.Range(0, 7);
        int crossOverPoint2 = Random.Range(7, 14);
        int index = 0;

        // Select a random parent to get the stats from, and add the chance for mutation
        stats.moveSpeed = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).moveSpeed + MutationChance(15, 0.5f);
        stats.maxStamina = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).maxStamina + MutationChance(15, 0.5f);
        stats.famishedSpeed = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).famishedSpeed + MutationChance(15, 0.2f);
        stats.staminaReduction = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).staminaReduction + MutationChance(15, 0.25f) + SpeedTradeOff(stats.moveSpeed);
        stats.hungerIncreaseSpeed = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).hungerIncreaseSpeed + MutationChance(15, 0.5f) - SpeedTradeOff(stats.moveSpeed);
        stats.tirednessIncreaseSpeed = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).tirednessIncreaseSpeed + MutationChance(15, 0.5f) - SpeedTradeOff(stats.moveSpeed);
        stats.reproductiveUrgeSpeed = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).reproductiveUrgeSpeed + MutationChance(15, 0.5f);
        stats.maxOffspring = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).maxOffspring;
        stats.viewDistance = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).viewDistance + MutationChance(15, 2f);
        stats.viewAngle = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).viewAngle + MutationChance(15, 2f);
        stats.hearingRange = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).hearingRange + MutationChance(15, 1f);
        stats.closeProximityRadius = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).closeProximityRadius + MutationChance(15, 0.25f);
        stats.highStandards = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).highStandards;
        stats.attack = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).attack + MutationChance(15, 0.1f);
        stats.defense = TwoPointParent(++index, crossOverPoint1, crossOverPoint2, parent1, parent2).defense + MutationChance(15, 0.1f);

        return stats;
    }

    /// <summary>
    ///  Returns one of the random types cross over
    /// </summary>
    /// <returns></returns>
    private static CrossOverTypes RandomCrossOver() {
        return (CrossOverTypes)Random.Range(0, 3);
    }

    /// <summary>
    /// Returns a random parent
    /// </summary>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    private static AgentStats RandomParent(AgentStats parent1, AgentStats parent2) {
        float chance = Random.Range(0f, 1f);
        if(chance > 0.5f) {
            return parent1;
        }
        return parent2;
    }

    /// <summary>
    /// Returns the correct parent given one  cross over point.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="crossOverPoint"></param>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    private static AgentStats OnePointParent(int value, int crossOverPoint, AgentStats parent1, AgentStats parent2) {
        if(value < crossOverPoint) {
            return parent1;
        }
        return parent2;
    }

    /// <summary>
    /// Returns the correct parent given two cross over points.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="crossOverPointOne"></param>
    /// <param name="crossOverPointTwo"></param>
    /// <param name="parent1"></param>
    /// <param name="parent2"></param>
    /// <returns></returns>
    private static AgentStats TwoPointParent(int value, int crossOverPointOne, int crossOverPointTwo, AgentStats parent1, AgentStats parent2) {
        if (value < crossOverPointOne) {
            return parent1;
        } else if(value >= crossOverPointOne && value < crossOverPointTwo) {
            return parent2;
        }
        return parent1;
    }

    /// <summary>
    /// Generate a random chance to mutate a gene, and either increase or decrease it by the given amount
    /// </summary>
    /// <param name="percentageChance"></param>
    /// <param name="maxAmount"></param>
    /// <returns></returns>
    private static float MutationChance(float percentageChance, float maxAmount) {
        int chance = Random.Range(0, 100);
        if(chance <= percentageChance) {
            return Random.Range(-maxAmount, maxAmount);
        }
        return 0;
    }

    /// <summary>
    /// For every multiple of 5 that speed is over (when it reaches 10 or over), return 2. This will be used to reduce the quality of other stats, simulating an element of balance where other genes decrease when another increases 
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    private static float SpeedTradeOff(float speed) {
        float value = (int)(speed - 5) / 5;
        value *= 2f;
        return value;
    }

    /// <summary>
    /// Assign the agents variables given a set of stats
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="stats"></param>
    public static void SetStats(AIAgent agent, AgentStats stats) {
        agent.speed = stats.moveSpeed;
        agent.maxStamina = stats.maxStamina;
        agent.famishedSpeed = stats.famishedSpeed;
        agent.staminaReductionRate = stats.staminaReduction;
        agent.hunger.timeToIncrease = stats.hungerIncreaseSpeed;
        agent.tiredness.timeToIncrease = stats.tirednessIncreaseSpeed;
        agent.reproduction.timeToIncrease = stats.reproductiveUrgeSpeed;
        agent.GetComponent<Pregnancy>().maxAmountOfOffspring = stats.maxOffspring;
        agent.GetComponent<VisionSensor>().viewRange = stats.viewDistance;
        agent.GetComponent<VisionSensor>().viewAngle = stats.viewAngle;
        agent.GetComponent<HearingSensor>().hearingRange = stats.hearingRange;
        agent.GetComponent<CloseProximitySensor>().radius = stats.closeProximityRadius;
        agent.reproduction.highStandards = stats.highStandards;
        agent.combat.attack = stats.attack;
        agent.combat.defense = stats.defense;
    }
}

public enum CrossOverTypes {
    RANDOM,
    ONE_POINT,
    TWO_POINT
}

[System.Serializable]
public struct AgentStats {
    public string currentAction;
    public float timeAlive;
    public float moveSpeed;
    public float maxStamina;
    public float famishedSpeed;
    public float staminaReduction;
    public float hungerIncreaseSpeed;
    public float tirednessIncreaseSpeed;
    public float reproductiveUrgeSpeed;
    public int maxOffspring;
    public int numOfOffspring;
    public float viewDistance;
    public float viewAngle;
    public float hearingRange;
    public float closeProximityRadius;
    public bool highStandards;
    public float attack;
    public float defense;
    public float numOfKills;

    public AgentStats(AIAgent agent) {
        currentAction = "";
        timeAlive = 0;
        moveSpeed = agent.speed;
        maxStamina = agent.maxStamina;
        famishedSpeed = agent.famishedSpeed;
        staminaReduction = agent.staminaReductionRate;
        hungerIncreaseSpeed = agent.hunger.timeToIncrease;
        tirednessIncreaseSpeed = agent.tiredness.timeToIncrease;
        reproductiveUrgeSpeed = agent.reproduction.timeToIncrease;
        maxOffspring = agent.GetComponent<Pregnancy>().maxAmountOfOffspring;
        numOfOffspring = 0;
        viewDistance = agent.GetComponent<VisionSensor>().agentsViewRange;
        viewAngle = agent.GetComponent<VisionSensor>().agentsViewAngle;
        hearingRange = agent.GetComponent<HearingSensor>().hearingRange;
        closeProximityRadius = agent.GetComponent<CloseProximitySensor>().radius;
        highStandards = agent.reproduction.highStandards; 
        attack = agent.combat.attack;
        defense = agent.combat.defense;
        numOfKills = agent.combat.kills;
    }
}