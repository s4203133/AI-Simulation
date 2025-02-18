
using UnityEngine;

public class TimeOfDaySystem : MonoBehaviour
{
    public static TimeOfDaySystem instance;

    [SerializeField] private Light directionLight;
    [SerializeField] private DayVisuals dayVisuals;

    private float currentTime;
    [SerializeField] private float daySpeed;
    [SerializeField] private float nightSpeed;

    private bool countDown;
    private bool dayStarted;
    private bool nightStarted;

    private void Start() {
        if (instance != null) {
            Debug.LogWarning("There are multiple 'Time Of Day Systems' in the scene! Please ensure there is only one");
            Destroy(this);
        }
        instance = this;
    }

    void TickTime() {
        RenderSettings.ambientLight = dayVisuals.ambientColour.Evaluate(currentTime);
        RenderSettings.fogColor = dayVisuals.fogColour.Evaluate(currentTime);
        directionLight.color = dayVisuals.directionalColour.Evaluate(currentTime);
        directionLight.transform.rotation = Quaternion.Euler(new Vector3((currentTime * 360f) - 90, -30, 0));

        float speed = (DayOrNight() == TimeOfDay.DAY ? daySpeed : nightSpeed);
        currentTime += (Time.deltaTime * speed);
        currentTime %= 24;
    }

    void Update()
    {
        if (!countDown) {
            return;
        }
        TickTime();
    }

    public static TimeOfDay DayOrNight() {
        if(instance.directionLight.transform.rotation.eulerAngles.x > 0 &&
            instance.directionLight.transform.rotation.eulerAngles.x <= 160) {
            instance.Day();
            return TimeOfDay.DAY;
        }
        instance.Night();
        return TimeOfDay.NIGHT;
    }

    public void StartDay() {
        countDown = true;
        currentTime = 0.38f;
    }

    private void Day() {
        if (!dayStarted) {
            AIAgentManager.ResetAgentVisibility();
            dayStarted = true;
            nightStarted = false;
        }
    }

    private void Night() {
        if(!nightStarted) {
            AIAgentManager.ReduceAgentVisibility();
            nightStarted = true;
            dayStarted = false;
        }
    }
}

[System.Serializable]
public class DayVisuals {
    public Gradient ambientColour;
    public Gradient directionalColour;
    public Gradient fogColour;
}

public enum TimeOfDay {
    DAY,
    NIGHT
}