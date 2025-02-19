using System.Collections.Generic;
using UnityEngine;

public class AIAgentManager : MonoBehaviour {
    static AIAgentManager instance;
    List<AIAgent> agents = new List<AIAgent>();

    private int numOfRabbits;
    private int numOfFoxes;

    private bool noAgentsRemaining;

    public delegate void AgentManagerEvent();
    public static AgentManagerEvent OnEnd;

    private bool simulationStarted;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogWarning("Ensure there is only 1 AIAgentManager in the scene!");
            Destroy(this);
        }

        agents = new List<AIAgent>();
        simulationStarted = false;
    }

    void Update() {
        if (simulationStarted && numOfFoxes == 0 && numOfRabbits == 0) {
            if (!noAgentsRemaining) {
                OnEnd.Invoke();
            }
            noAgentsRemaining = true;
        }
    }

    public static void AddAgent(AIAgent agent, AIAgentTypes aiType) {
        instance.agents.Add(agent);
        CalculateNumbers(1, aiType);
        instance.simulationStarted = true;
    }

    public static void RemoveAgent(AIAgent agent, AIAgentTypes aiType) {
        instance.agents.Remove(agent);
        CalculateNumbers(-1, aiType);
        instance.simulationStarted = true;
    }

    public static List<AIAgent> AllAgents() {
        return instance.agents;
    }

    private static void CalculateNumbers(int value, AIAgentTypes aiType) {
        if (aiType == AIAgentTypes.RABBIT) {
            instance.numOfRabbits += value;
        } else if (aiType == AIAgentTypes.FOX) {
            instance.numOfFoxes += value;
        }
    }

    public static void ReduceAgentVisibility() {
        for(int i = 0; i < instance.agents.Count; i++) {
            instance.agents[i].sensorySystem.ReduceVisibility();
        }
    }

    public static void ResetAgentVisibility() {
        for (int i = 0; i < instance.agents.Count; i++) {
            instance.agents[i].sensorySystem.ResetVisibility();
        }
    }
}