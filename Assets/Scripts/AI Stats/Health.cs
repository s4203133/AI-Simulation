using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float health;
    [Tooltip("1 = 1 health deducted every second,\n2 = 2 health deducted every second,\n0.5 = 0.5 health deducted every second, and so on...")]
    [SerializeField] private float healthDeductionSpeed;

    [SerializeField] private GameObject deathParticles;

    public delegate void HealthEvent();
    public HealthEvent OnDead;

    private bool dead;

    void Start()
    {
        health = maxHealth;
        OnDead += Death;
    }

    private void Update() {
        // As soon as health reaches zero, kill the agent
        if (health <= 0) {
            if (!dead) {
                dead = true;
                OnDead.Invoke();
            }
        }
    }

    /// <summary>
    /// Decrease health across time by a multiplyer
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateTimer(float deltaTime) {
        health -= deltaTime * healthDeductionSpeed;
    }

    /// <summary>
    /// Resets health to be the max value
    /// </summary>
    /// <param name="amount"></param>
    public void RestoreHealth(float amount) {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    /// <summary>
    /// Reduce health by a given amount. Will kill the agent if health falls below 0
    /// </summary>
    /// <param name="amount"></param>
    public void DamageHealth(float amount) {
        health -= amount;
        if (health < 0) {
            SoundManager.PlaySound(SoundManager.instance.foxKilled, gameObject);
            health = 0;
            if (!dead) {
                dead = true;
                OnDead.Invoke();
            }
        }
    }

    /// <summary>
    /// Performs any logic that happens once the agent dies, before they are destroyed
    /// </summary>
    private void Death() {
        GetComponent<NavMeshAgent>().isStopped = true;
        SoundManager.PlaySound(SoundManager.instance.agentDeath, gameObject);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public float CurrentHealth() {
        return health;
    }
}
