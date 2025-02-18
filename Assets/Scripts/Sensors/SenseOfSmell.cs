using UnityEngine;

public class SenseOfSmell : MonoBehaviour
{
    public ScentTrail currentTrailFollowing { private set; get; }
    public ScentNode foundNode { private set; get; }
    public bool hasScent;

    public EDetectableObjectCategories interestedScentTypes;

    [SerializeField] private float rememberScentTime;
    private float timer;

    private AIAgent thisAgent;

    private void Awake() {
        thisAgent = GetComponent<AIAgent>();
    }

    void Update()
    {
        // Once the timer reaches 0, lose the scent trail (timer is reset each time the agent finds a scent trail)
        timer -= Time.deltaTime;    
        if(timer <= 0) {
            hasScent = false;
            currentTrailFollowing = null;
        }
    }

    public void CaughtScent(ScentNode scentNode, ScentTrail newScentTrail) {
        timer = rememberScentTime;
        if (newScentTrail == null || // If the trail the scent node belongs to doesn't exist (agent has probably been destroyed) 
            newScentTrail.GetComponent<AIAgent>() == thisAgent || // If the scent node was produced by this agent
            currentTrailFollowing != null) { // The agent already has a trail to follow
            return;
        }

        // Check that the scent node is the same type as the agent is looking for
        EDetectableObjectCategories scentType = newScentTrail.scentType;
        if((interestedScentTypes & scentType) == scentType) {
            hasScent = true;
            foundNode = scentNode;
            currentTrailFollowing = newScentTrail;
        }
    }

    /// <summary>
    /// Check the trail this agent is following is of a given type
    /// </summary>
    /// <param name="scentType"></param>
    /// <returns></returns>
    public bool ScentTrailIsOfType(EDetectableObjectCategories scentType) {
        if ((currentTrailFollowing.scentType & scentType) == scentType) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Reset variables when the agent has reached the end of the scent trail
    /// </summary>
    public void FoundEndOfTrail() {
        currentTrailFollowing = null;
        foundNode = null;
        hasScent = false;
    }
}
