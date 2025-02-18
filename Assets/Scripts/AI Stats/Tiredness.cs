using UnityEngine;

public class Tiredness : AIStat
{
    [SerializeField] private float restTime;
    public bool isTired { private set; get; }

    [HideInInspector] public bool isResting;

    private void Start() {
        // As the simulation starts during the day, initialise the tiredness value part way through
        value = 0.3f;
    }

    public override void UpdateTimer(float deltaTime) {
        // If the agent is resting, decrease their tiredness value, other wise, increase it across time
        if (!isResting) {
            base.UpdateTimer(deltaTime);
        } else {
            Rest(deltaTime / restTime);
        }
        // When the tiredness value exceeds the threshold, set this agent to be tired
        isTired = weight.Evaluate(value) >= threshold ? true : false;
    }

    // While resting, decrease the tiredness amount
    public void Rest(float amount) {
        value -= amount;
        if(value <= 0) {
            isResting = false;
        }
    }
}
