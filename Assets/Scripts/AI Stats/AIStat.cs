using UnityEngine;

public abstract class AIStat : MonoBehaviour {

    [SerializeField] protected AnimationCurve weight;
    protected float value;

    [Tooltip("Random value bewteen this range will represent the time in seconds it takes for the value to go from min to max")]
    public Vector2 timeToIncreaseRange;
    public float timeToIncrease;

    [Tooltip("The value on the Animation Curve that determines when effects should start applying to the agent")]
    [SerializeField] protected float threshold;

    private void Start() {
        value = 0;
    }

    public virtual void UpdateTimer(float deltaTime) {
        // Increase the value of this stat across time
        value += (deltaTime / timeToIncrease);

        if (value >= 1) {
            ExceededMax();
        } else {
            StillIncreasing();
        }
    }

    protected virtual void ExceededMax() {
        // Run any logic for if the value reaches it's maximum value
        value = 1;
    }

    protected virtual void StillIncreasing() {
        // Run any logic for while the value is still increasing
    }

    public virtual void ResetValue() {
        value = 0;
    }

    public virtual void RandomiseStartingValue() {
        timeToIncrease =  Random.Range(timeToIncreaseRange.x, timeToIncreaseRange.y);
    }

    public float GetValue() {
        return weight.Evaluate(value);
    }
}
