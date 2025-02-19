using UnityEngine;

public class SpawnCreature : MonoBehaviour
{
    [SerializeField] private GameObject rabbitPrefab;
    [SerializeField] private GameObject foxPrefab;

    public static SpawnCreature instance;

    private void Start() {
        if(instance != null) {
            Debug.LogWarning("Multiple SpawnCreature components in the scene. PLease ensure there is only one");
            Destroy(this);
        }
        instance = this;
    }

    public static GameObject Spawn(CreatureType type, Vector3 location) {
        GameObject newCreature = null;
        if (type == CreatureType.rabbit) {
            newCreature = Instantiate(instance.rabbitPrefab, location, Quaternion.identity);
        } else {
            newCreature = Instantiate(instance.foxPrefab, location, Quaternion.identity);
        }

        return newCreature;
    }
}

public enum CreatureType {
    rabbit,
    fox
}
