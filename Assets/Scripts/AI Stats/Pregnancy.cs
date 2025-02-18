using UnityEngine;

public class Pregnancy : MonoBehaviour
{
    private AIAgent agent;

    public bool isPregnant { private set; get; }
    private CreatureType typeToGiveBirthTo;
    
    [SerializeField] private float maxGestationTime;
    private float gestationTime;
    private float gestationTimer;
    
    // Max amount of offspring an agent can have at once
    public int maxAmountOfOffspring;
    // The total amount of offspring this agent has gave birth to
    private int offspringAmount;

    [Space(20)]
    public AgentStats parent1;
    public AgentStats parent2;

    private void Start() {
        agent = GetComponent<AIAgent>();
        isPregnant = false;

        // Log warning if variables aren't set up correctly
        if (maxGestationTime < 1) {
            Debug.LogWarning("Max Gestation Time on " + gameObject.name + " must be greater than 1!");
        }
        if(maxAmountOfOffspring <= 0) {
            Debug.LogWarning("Max Amount Of Offspring on " + gameObject.name + " must be greater than 0!");
        }
    }

    void Update()
    {
        // If the agent is pregnant, reduce a gestation timer to determine when they should give birth
        if (isPregnant) {
            gestationTimer -= Time.deltaTime;
            if(gestationTimer <= 0) {
                GiveBirth();
            }
        }
    }

    public void Pregnate(CreatureType creatureType, AIAgent pregnatedBy) {
        // If the agent is already pregnant, then don't continue
        // If the agents partner is already pregnant, don't continue as we only want one partner to reproduce
        // (this will prevent multiple agents being pregnated from one reproduction action)
        if(isPregnant || pregnatedBy.GetComponent<Pregnancy>().isPregnant) { return; }

        // Start pregnancy
        isPregnant = true;
        typeToGiveBirthTo = creatureType;

        // Set the length for how long the pregnancy will last
        gestationTime = Random.Range(2f, maxGestationTime);
        gestationTimer = gestationTime;

        // Generate amount of offspring
        offspringAmount = Random.Range(1, maxAmountOfOffspring);

        // Assign the parents stats ready to be used for the genetic algorithm
        parent1 = new AgentStats(agent);
        parent2 = new AgentStats(pregnatedBy);
        ResetReproduction(pregnatedBy);
    }

    /// <summary>
    /// Reset the reproduction values of both agents involved in the reproduction action
    /// </summary>
    /// <param name="partner"></param>
    private void ResetReproduction(AIAgent partner) {
        agent.reproduction.ResetValue();
        partner.reproduction.ResetValue();
    }

    /// <summary>
    /// Give birth to the agents offspring
    /// </summary>
    private void GiveBirth() {
        // If the agent is currently sleeping, don't continue
        if (agent.tiredness.isResting) { return; }

        // Spawn the required amount of offspring, set up their genes
        for (int i = 0; i < offspringAmount; i++) {
            GameObject newOffspring = SpawnCreature.Spawn(typeToGiveBirthTo, transform.position);
            newOffspring.GetComponent<AIAgent>().InitializeGenes(parent1, parent2);
            agent.stats.numOfOffspring++;
        }

        //SoundManager.PlaySound(SoundManager.instance.reproduce);

        // End the pregnancy
        isPregnant = false;
        agent.reproduction.ResetValue();
    }
}
