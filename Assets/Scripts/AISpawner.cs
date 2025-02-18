using System.Linq;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameObject rabbitPrefab;
    [Range(10, 50)]
    [SerializeField] public int rabbitsToSpawn;

    [SerializeField] private GameObject foxPrefab;
    [Range(10, 50)]
    [SerializeField] public int foxesToSpawn;

    [SerializeField] private GameObject rabbitSpawnVolume;
    [SerializeField] private GameObject foxSpawnVolume;

    [SerializeField] private LayerMask tilesToSpawnOn;

    public void SpawnAllAgents() {
        SpawnRabbits();
        SpawnFoxes();
    }

    public void SpawnRabbits() {
        SpawnAgentsInArea(rabbitPrefab, rabbitsToSpawn, rabbitSpawnVolume);
    }

    public void SpawnFoxes() {
        SpawnAgentsInArea(foxPrefab, foxesToSpawn, foxSpawnVolume);
    }

    private void SpawnAgentsInArea(GameObject agentPrefab, int amountToSpawn, GameObject spawnVolume) {
        // Get all tiles in the rabbit spawn volume that can be used to place each new rabbit on
        Collider[] spawnTiles = Physics.OverlapBox(spawnVolume.transform.position, spawnVolume.transform.localScale * 0.5f, spawnVolume.transform.rotation, tilesToSpawnOn);

        // Shuffle the list so that it's in a random order
        Collider[] spawnableTiles = new Collider[spawnTiles.Length];
        spawnableTiles = spawnTiles.OrderBy(x => Random.value).ToArray();

        for (int i = 0; i < amountToSpawn; i++) {
            Instantiate(agentPrefab, spawnableTiles[i].transform.position + Vector3.up, Quaternion.identity);
        }
    }
}
