using System.Collections.Generic;
using UnityEngine;

namespace GOAP {
    /// <summary>
    /// Plans the sequence of actions required to satisfy a goal state
    /// </summary>
    public class GoapPlanner {
        public Queue<IAction> Plan(GameObject agent, List<IAction> allActions, GoapGoal desiredGoal, List<Condition> agentBeliefs, bool debugPlan) {
            // Out of all possible actions, filter out the ones that can't be ran by only getting the ones that are achievable
            List<IAction> availableActions = new List<IAction>();
            int numOfActs = allActions.Count;
            for(int i = 0; i < numOfActs; i++) {
                // Check if this action is achievable
                IAction action = allActions[i];
                if (action.IsAchievable(agent)) {
                    availableActions.Add(action);
                }
            }

            // Set up first leaf to go into the graph
            List<Node> leaves = new List<Node>();
            Node start = new Node(null, 0, agentBeliefs, null);

            // Build the tree and record the leaf nodes that provide a solution to the goal
            bool pathFound = BuildTree(start, leaves, availableActions, desiredGoal);

            // Return Nothing If No Plan Was Found
            if (!pathFound) {
                return null;
            }

            // Get the cheapest node from the tree that was built
            Node cheapest = null;
            int leafCount = leaves.Count;
            for(int i = 0; i < leafCount; i++) {
                Node leaf = leaves[i];
                if(cheapest == null) {
                    cheapest = leaf;
                } else {
                    if(leaf.cost < cheapest.cost) {
                        cheapest = leaf;
                    }
                }
            }

            // Follow the cheapest nodes parents up the tree to generate the cheapest plan
            List<IAction> result = new List<IAction>();
            Node n = cheapest;
            while(n != null) {
                if(n.action != null) {
                    result.Insert(0, n.action);
                }
                n = n.parent;
            }

            // Move the actions from the list just made to a queue that will be returned from this function
            Queue<IAction> plan = new Queue<IAction>();
            int resultsLength = result.Count;
            for(int i = 0; i < resultsLength; i++) { 
                IAction action = result[i];
                plan.Enqueue(action);
            }

            // Print out the plan to the console
            if (debugPlan) {
                Debug.Log(agent.gameObject.name + " The Plan Found Is:");
                foreach (IAction action in plan) {
                    Debug.Log(agent.gameObject.name + " " + action.ActionName());
                }
            }

            return plan;
        }

        private bool BuildTree(Node parent, List<Node> leaves, List<IAction> availableActions, GoapGoal desiredGoal) {
            bool foundPath = false;
            // Go through each action available to this node and see if it can be used
            int numOfActs = availableActions.Count;
            for(int i = 0; i < numOfActs; i++) { 
                // Keep track of every condition being satisfied by the sequence of actions
                IAction action = availableActions[i];
                if(Condition.ConditionsMatch(action.GetPreConditions(), parent.requiredEffects)) {

                    // Apply the action's effects to the parent state
                    List<Condition> currentStates = PopulateStates(parent.requiredEffects, action.GetEffects());

                    Node node = new Node(parent, parent.cost + action.CalculateCost(), currentStates, action);

                    // If the goaal has been satisfied, add the most recent node to the leaves
                    if (Condition.ConditionsMatch(desiredGoal.conditionsToSatisfy, currentStates)) {
                        leaves.Add(node);
                        foundPath = true;
                    } else {
                        // Otherwise, remove the current action from available actions, and continue building
                        // the tree using the next node just made
                        List<IAction> subset = ActionSubset(availableActions, action);
                        bool found = BuildTree(node, leaves, subset, desiredGoal);
                        // Check if a path was found from this attemot
                        if (found) {
                            foundPath = true;
                        }
                    }
                }
            }
            return foundPath;
        }

        // Returns a list of actions, removing a given one
        List<IAction> ActionSubset(List<IAction> actions, IAction actionToRemove) {
            List<IAction> subset = new List<IAction>();
            int numOfActions = actions.Count;   
            for(int i = 0; i < numOfActions; i++) {
                IAction action = actions[i];
                if (action != actionToRemove) {
                    subset.Add(action);
                }
            }
            return subset;
        }

        List<Condition> PopulateStates(List<Condition> currentState, List<Condition> stateChange) { 
            List<Condition> states = new List<Condition>();
            // Copy the conditions over
            states.AddRange(currentState);

            int statesCount = states.Count;
            int changesCount = stateChange.Count;
            for(int i = 0; i < changesCount; i++) {
                // if the condition type exists in the states, update the value
                Condition change = stateChange[i];
                bool exists = false;
                for(int j = 0; j < statesCount; j++) {
                    if (states[j].key == change.key) {
                        states.RemoveAt(j);
                        exists = true;
                        break;
                    }
                }

                if(exists) {
                    Condition updated = new Condition(change.key, change.value);
                    states.Add(updated);
                } else {
                    // If it does not exist in the current states, add it int
                    states.Add(new Condition(change.key, change.value));
                }
            }
            return states;
        }
    }

    // Nodes used to build up the graph and hold the cost of the actions
    public class Node {
        public Node parent;
        public float cost;
        // All current beliefs at this nodes position in the graph
        public List<Condition> requiredEffects;
        public IAction action;

        public Node(Node Parent, float Cost, List<Condition> RequiredEffects, IAction Action) {
            parent = Parent;
            cost = Cost;
            requiredEffects = new List<Condition>(RequiredEffects);
            action = Action;
        }   
    }

}