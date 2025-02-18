using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP {

    /// <summary>
    /// Class for GOAP actions to derive from, contains related variables and components that can be accessed
    /// </summary>
    public abstract class GoapAction : MonoBehaviour {
        public string actionName;

        public float duration;
        public int cost;
        public ESoundCategories soundToPlay;
        public Blackboard blackboard;

        public List<Condition> preConditions;
        protected Dictionary<eConditions, bool> PreConditions;

        public List<Condition> postConditions;
        protected Dictionary<eConditions, bool> Effects;

        public bool isRunning;

        protected GameObject agent;
        protected Fox aiAgent;
        protected NavMeshAgent navMeshAgent;
        protected Animator animator;
        protected SensorySystem sensorySystem;
        protected Memory memory;

        private bool initialised = false;

        protected void Initialise(GameObject Agent) {
            if (!initialised) {
                agent = Agent;
                aiAgent = agent.GetComponent<Fox>();
                navMeshAgent = agent.GetComponent<NavMeshAgent>();
                animator = agent.GetComponent<Animator>();
                sensorySystem = agent.GetComponent<SensorySystem>();
                memory = agent.GetComponent<Memory>();
            }
        }
    }

    /// <summary>
    /// Interface for any GOAP action to implement.
    /// </summary>
    public interface IAction {

        string ActionName();

        float Duration();

        int CalculateCost();

        bool IsAchievable(GameObject Agent);

        List<Condition> GetPreConditions();
        List<Condition> GetEffects();

        bool StartAction(GameObject Agent);

        bool UpdateAction(float deltaTime);

        void ComleteAction();

        void StopAction();

        bool Running();

        bool WithinRange();
    }

}


