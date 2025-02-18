using UnityEngine;

public class ScentNode : MonoBehaviour
{
    private float intensity;
    private ScentTrail parentScentTrail;
    private EDetectableObjectCategories scentType;

    void Update()
    {
        // Decrease the intensity of the node across time. If it reaches 0, then destroy it
        intensity -= Time.deltaTime;
        if(intensity <= 0) {
            Destroy(gameObject);
            parentScentTrail.scentTrail.Remove(this);
        }
    }

    public void SetIntensity(float intensity) {
        this.intensity = intensity;
    }

    public void SetScentTrail(ScentTrail scentTrail) {
        parentScentTrail = scentTrail;
    }

    public ScentTrail GetScentTrail() {
        return parentScentTrail;
    }

    public void SetScentType(EDetectableObjectCategories scentType) {
        this.scentType = scentType;
    }

    public EDetectableObjectCategories GetScentType() {
        return scentType;
    }
}
