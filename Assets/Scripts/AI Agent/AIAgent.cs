using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour {
    private bool run;

    public AIAgentTypes aiType;
    private NavMeshAgent agent;

    [Header("AGENT STATS")]
    [SerializeField] private Vector2 speedRange;
    [HideInInspector] public float speed;
    [SerializeField] private Vector2 famishedSpeedRange;
    [HideInInspector] public float famishedSpeed;

    [SerializeField] private Vector2 staminaRange;
    [HideInInspector] public float maxStamina; 
    public float stamina { private set; get; }
    public float staminaReductionRate;
    private bool isUsingStamina;

    public Health health;
    public Hunger hunger;
    public Tiredness tiredness;
    public Reproduction reproduction;
    public Combat combat;
    public AgentStats stats;
    bool statsAlreadySet;

    [Space(10)]
    [Header("SENSORY SYSTEM")]
    public Memory memory;
    public SensorySystem sensorySystem { private set; get; }

    protected virtual void Start() {
        run = true;

        agent = GetComponent<NavMeshAgent>();

        health.OnDead += OnDead;

        if (!statsAlreadySet) {
            GenerateRandomStats();
            stats = new AgentStats(this);
        }

        stamina = maxStamina;

        sensorySystem = GetComponent<SensorySystem>();

        AIAgentManager.AddAgent(this, aiType);
    }

    private void OnDestroy() {
        AIAgentManager.RemoveAgent(this, aiType);
    }

    protected virtual void Update() {
        if (!run) {
            return;
        }

        UpdateStats();

        HandleStamina();
    }

    public void UpdateStats() {
        hunger.UpdateTimer(Time.deltaTime);
        tiredness.UpdateTimer(Time.deltaTime); ;
        reproduction.UpdateTimer(Time.deltaTime);

        if (hunger.isFamished) {
            health.UpdateTimer(Time.deltaTime);
        }

        stats.timeAlive += Time.deltaTime;
        stats.numOfKills = combat.kills;
        stats.viewDistance = sensorySystem.ViewRange;
        stats.viewAngle = sensorySystem.ViewAngle;
    }

    private void GenerateRandomStats() {
        speed = Random.Range(speedRange.x, speedRange.y);
        famishedSpeed = Random.Range(famishedSpeedRange.x, famishedSpeedRange.y);
        maxStamina = Random.Range(staminaRange.x, staminaRange.y);
        stamina = maxStamina;

        hunger.RandomiseStartingValue();
        tiredness.RandomiseStartingValue();
        reproduction.RandomiseStartingValue();

        combat.RandomiseStartingValue();
    }

    public void InitializeGenes(AgentStats parent1, AgentStats parent2) {
        stats = GenerateStats.GetStatsForNewAgent(parent1, parent2);
        statsAlreadySet = true;
        GenerateStats.SetStats(this, stats);
    }

    public void HandleStamina() {
        if (isUsingStamina) {
            stamina -= Time.deltaTime / (staminaReductionRate * 0.1f);
            if(stamina < 0) { stamina = 0; }
        } else {
            stamina += Time.deltaTime / (staminaReductionRate * 0.1f);
            if (stamina > maxStamina) { stamina = maxStamina; }
        }
    }

    public void RefillStamina() {
        stamina = maxStamina;
    }

    public void IsUsingStamina(bool value) {
        isUsingStamina = value;
    }

    /// <summary>
    /// Run any clean up logic before the agent dies
    /// </summary>
    protected virtual void OnDead() {
        run = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "ScentTrail") {
            ScentNode foundNode = other.GetComponent<ScentNode>();
            sensorySystem.senseOfSmell.CaughtScent(foundNode, foundNode.GetScentTrail());
        }
    }

    public Vector3 PredictedDestination() {
        return transform.position + (agent.velocity.normalized * 3); 
    }

    public void SpawnDustParticles(GameObject particles) {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}

public enum AIAgentTypes {
    RABBIT,
    FOX
}