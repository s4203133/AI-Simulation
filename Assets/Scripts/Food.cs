using UnityEngine;

public class Food : MonoBehaviour
{
    public float healthToRestore { get; private set; }
    public float hungerToRestore { get; private set; }

    public delegate void FoodEvent(DetectableObject thisObject);
    public static FoodEvent OnTagged;
    public bool isTaken;

    private void Start() {
        healthToRestore = 10;
        hungerToRestore = 0.4f;
    }

    public void Eat() {
        gameObject.SetActive(false);
    }

    public void TagAsTarget() {
        OnTagged.Invoke(GetComponent<DetectableObject>());
        isTaken = true;
    }

    public void NoLongerTarget() {
        isTaken = false;
    }
}
