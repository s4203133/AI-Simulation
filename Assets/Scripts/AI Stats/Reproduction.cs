using System.Collections.Generic;
using UnityEngine;

public class Reproduction : AIStat
{
    AIAgent agent;

    public bool wantsToReproduce { private set; get; }

    [Space(15)]
    [SerializeField] private float timeBetweenReproductions;
    private float recentAttemptTimer;

    private GameObject reproductionPartner;

    private List<AIAgent> agentsRejectedBy;
    public int rejCount => agentsRejectedBy.Count;

    public bool highStandards;

    void Start()
    {
        agent = GetComponent<AIAgent>();

        recentAttemptTimer = 0;
        reproductionPartner = null;
        agentsRejectedBy = new List<AIAgent>();
        highStandards = Random.Range(0f, 1f) <= 0.2f ? true : false;

        DetectableObject.onDestroyed += RemoveAgentRejectedBy;
    }

    public override void UpdateTimer(float deltaTime) {
        // If the reproduction value exceeds the threshold, then the agent will start looking for a partner
       base.UpdateTimer(deltaTime);
       wantsToReproduce = weight.Evaluate(value) > threshold ? true : false;
        
        recentAttemptTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Reset the desire to reproduce, and start a timer to prevent the agent from reproducing for a short while
    /// </summary>
    public override void ResetValue() {
        base.ResetValue();
        wantsToReproduce = false;
        recentAttemptTimer = timeBetweenReproductions;
    }

    /// <summary>
    /// Evaluate the agents conditions to determine if they will reproduce or not
    /// </summary>
    /// <param name="interestedAgent"></param>
    /// <returns></returns>
    public bool EvalatePartner(AIAgent interestedAgent) {
        if (agent.tiredness.isResting ||
            agent.hunger.isFamished ||
          !WeArePartner(interestedAgent.gameObject) || RecentlyReproduced()) {
            return false;
        }

        // If the agent has high standards, check the interested agent meets the requirements
        if (highStandards) {
            bool agentMeetsStandards = CheckQualityOfPartner(interestedAgent);
            return agentMeetsStandards;
        }

        return true;
    }

    /// <summary>
    /// Only accept partners that have stats that are equal to or better than outs
    /// </summary>
    /// <param name="interestedAgent"></param>
    /// <returns></returns>
    private bool CheckQualityOfPartner(AIAgent interestedAgent) {
        // Counter to keep track of how many stats are undesirable in the partner being checked
        int count = 0;
        // Check for situations where this agents stats should be better
        if (agent.stats.moveSpeed > interestedAgent.stats.moveSpeed) { count++; }
        if (agent.stats.famishedSpeed > interestedAgent.stats.famishedSpeed) { count++; }
        if (agent.stats.reproductiveUrgeSpeed > interestedAgent.stats.reproductiveUrgeSpeed) { count++; }
        if (agent.stats.maxOffspring > interestedAgent.stats.maxOffspring) { count++; }
        if (agent.stats.viewDistance > interestedAgent.stats.viewDistance) { count++; }
        if (agent.stats.viewAngle > interestedAgent.stats.viewAngle) { count++; }
        if (agent.stats.hearingRange > interestedAgent.stats.hearingRange) { count++; }
        if (agent.stats.closeProximityRadius > interestedAgent.stats.closeProximityRadius) { count++; }
        if (agent.stats.attack > interestedAgent.stats.attack) { count++; }
        if (agent.stats.defense > interestedAgent.stats.defense) { count++; }

        // Check for situation where it's better for this agent to have lower stats
        if (agent.stats.staminaReduction < interestedAgent.stats.staminaReduction) { count++; }
        if (agent.stats.hungerIncreaseSpeed < interestedAgent.stats.hungerIncreaseSpeed) { count++; }
        if (agent.stats.tirednessIncreaseSpeed < interestedAgent.stats.tirednessIncreaseSpeed) { count++; }

        // If there was more than 5 undesireable traits, then reject this agent (This allows some level of acceptance)
        if(count > 5) {
            return false;
        }

        // If all stats passed, then return true as this is a valid agent
        return true;
    }

    /// <summary>
    /// Add a given agent to the list of agents they've been rejected by
    /// </summary>
    /// <param name="agentRejectedBy"></param>
    public void Rejection(AIAgent agentRejectedBy) {
        if(!agentsRejectedBy.Contains(agentRejectedBy)) {
            agentsRejectedBy.Add(agentRejectedBy);
        }
    }

    /// <summary>
    /// Given an agent, return if they have been rejected by them in the past
    /// </summary>
    /// <param name="agent"></param>
    /// <returns></returns>
    public bool HasBeenRejectedByThisAgent(AIAgent agent) {
        int rejectedCount = agentsRejectedBy.Count;
        for(int i = 0; i < rejectedCount; i++) {
            if (agentsRejectedBy[i] == agent) { 
                return true; 
            }
        }
        return false;
    }

    /// <summary>
    /// Removes an agent from the rejected list
    /// </summary>
    /// <param name="agent"></param>
    public void RemoveAgentRejectedBy(DetectableObject agent) {
        int rejectedCount = agentsRejectedBy.Count;
        if(rejectedCount == 0) { 
            return; 
        }
        for (int i = 0; i < rejectedCount; i++) {
            if (agentsRejectedBy[i] == null) {
                agentsRejectedBy.RemoveAt(i);
                rejectedCount = agentsRejectedBy.Count;
                continue;
            }
            if (agentsRejectedBy[i] != null && agentsRejectedBy[i].gameObject == agent.gameObject) {
                agentsRejectedBy.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// Given a list of agents, return all of them this agent hasn't been rejected by
    /// </summary>
    /// <param name="agents"></param>
    /// <returns></returns>
    public List<DetectableObject> AgentAvailable(List<DetectableObject> agents) {
        // For each agent in the rejected by agents list
        int rejectedCount = agentsRejectedBy.Count;
        for (int i = 0; i < rejectedCount; i++) {
            // If this agent is null, remove them from the list
            if (agentsRejectedBy[i] == null) {
                agentsRejectedBy.RemoveAt(i);
                rejectedCount = agentsRejectedBy.Count;
                continue;
            }
            // If the given agents list contains this 'agent rejected by' index, then remove them 
            DetectableObject agentRejectedby = agentsRejectedBy[i].GetComponent<DetectableObject>();
            if (agents.Contains(agentRejectedby)) {
                agents.Remove(agentRejectedby);
            }
        }

        // Return the resulting list, with all agents that this agent has been rejected by having been removed
        return agents;
    }

    // Assign this agents reproduction partner
    public void SetReproductionPartner(GameObject partner) {
        reproductionPartner = partner;
    }

    // Check if this agent either doesn't have a partner, or the one in question is our current one
    public bool WeArePartner(GameObject partner) {
        if(reproductionPartner == null || reproductionPartner == partner) {
            return true;
        }
        return false;
    }

    public bool RecentlyReproduced() {
        return (recentAttemptTimer > 0);
    }
}
