using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [HideInInspector] public bool isTaken;

    public delegate void hideEvent(DetectableObject thisObject);
    public static hideEvent OnHide;

    private DetectableObject detectable;

    private void Start() {
        isTaken = false;
        detectable = GetComponent<DetectableObject>();
    }

    public void Occupy() {
        isTaken = true;
        OnHide(detectable);
    }

    public void UnOccupy() {
        isTaken = false;
    }
}
