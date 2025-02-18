

public class Hunger : AIStat
{
    public bool isHungry { private set; get; }
    public bool isFamished { private set; get; }

    public override void UpdateTimer(float deltaTime) {
        base.UpdateTimer(deltaTime);
        // If hunger is over the threshold, mark this agent as being hungry
        isHungry = weight.Evaluate(value) >= threshold ? true : false;        
    }

    /// <summary>
    /// Once hunger has reached the max, the agent becomes famished (This will start to reduce their health)
    /// </summary>
    protected override void ExceededMax() {
        isFamished = true;
        base.ExceededMax(); 
    }

    /// <summary>
    /// Once health has been restored to below maximum, the agent will no longer be famished
    /// </summary>
    protected override void StillIncreasing() {
        isFamished = false;
    }

    /// <summary>
    /// Reduce how much the agent is hungry
    /// </summary>
    /// <param name="amount"></param>
    public void RestoreHunger(float amount) {
        value -= amount;
        if(value < 0 ) {
            value = 0;
        }
    }
}
