using UnityEngine;

public class Combat : MonoBehaviour
{
    public bool feelsThreatened;
    public bool isBeingChased;
    public bool isHidden { private set; get; }
    public bool canAttack;

    public AIAgent currentAttacker;

    [Range(0f, 1f)]
    public float defense;
    public Vector2 randomDefenseStartingValue;

    [Range(0f, 1f)]
    public float attack;
    public Vector2 randomAttackStartingValue;
    
    public int kills;

    public void RandomiseStartingValue() {
        defense = Random.Range(randomDefenseStartingValue.x, randomDefenseStartingValue.y);
        attack = Random.Range(randomAttackStartingValue.x, randomAttackStartingValue.y);
    }

    /// <summary>
    /// Generate a random chance to be able to attack, and return if successful or not
    /// </summary>
    /// <returns></returns>
    public bool AttackChance() {
        float chance = Random.Range(0f, 1f);
        if (chance <= attack) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Generate a random chance for this agent to dodge an attack, and return if successful or not. Caches the incoming attacker.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    public bool WillAgentDodge(AIAgent attacker) {
        currentAttacker = attacker;
        float chance = Random.Range(0f, 1f);
        if(chance <= defense) {
            canAttack = true;
            return true;
        }
        canAttack = false;
        return false;
    }

    /// <summary>
    /// Deal damage to a given agent
    /// </summary>
    /// <param name="agentToAttack"></param>
    /// <param name="damageToDeal"></param>
    public void AttackAgent(AIAgent agentToAttack, float damageToDeal) {
        agentToAttack.health.DamageHealth(damageToDeal);
        canAttack = false;
    }

    /// <summary>
    /// Occupy a hiding spot, and trigger the agent to be disabled from being detected
    /// </summary>
    public void Hide() {
        isHidden = true;
        GetComponent<DetectableObject>().Remove();
    }

    public void UnHide() {
        isHidden = false;
    }

    /// <summary>
    /// Mark this agent as an target for a predator. Remove from other agents memory to prevent too many predators all chasing the same prey
    /// </summary>
    /// <param name="value"></param>
    public void MarkAsPrey(bool value) {
        isBeingChased = value;
        GetComponent<DetectableObject>().Remove();
    }
}
