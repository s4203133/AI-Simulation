using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private int startingAmount;

    [SerializeField] private float spawnNewFoodTime;
    private float timer;

    [SerializeField] private Food[] foodPool;

    private List<GameObject> spawnableTiles;
    private int tileIndex;

    private bool spawnActive;

    void Start()
    {
        // Generate all tiles that food can spawn on
        GetSpawnableTiles();
        tileIndex = 0;
        timer = spawnNewFoodTime;
        spawnActive = false;
    }

    public void StartSpawning() {
        spawnActive = true;
        SpawnMultipleFood(startingAmount);
    }

    void Update()
    {
        if (!spawnActive) {
            return;
        }

        // Spawn food every time the timer reaches 0
        timer -= Time.deltaTime;
        if(timer <= 0) {
            SpawnFood();
            timer = spawnNewFoodTime;
        }
    }

    private void GetSpawnableTiles() {
        // Get all tiles in the scene
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("GreenLand");

        // Shuffle the list so that it's in a random order
        spawnableTiles = new List<GameObject>();
        spawnableTiles = allTiles.OrderBy(x => Random.value).ToList();
    }

    private void SpawnFood() {
        // Get the next available food from the object pool
        GameObject food = GetAvailableFood();

        // If one was found
        if(food != null) {
            // Choose the next tile in the shuffled list
            tileIndex++;
            if (tileIndex >= foodPool.Length) { 
                tileIndex = 0;
            }

            GameObject tile = spawnableTiles[tileIndex];

            // Move the food to be on top of the tile, and enable it
            food.transform.position = tile.transform.position + Vector3.up;
            food.SetActive(true);
        }
    }

    private void SpawnMultipleFood(int amount) {
        // Spawn a given amount of food
        for(int i = 0; i < amount; i++) {
            SpawnFood();
        }
    }

    private GameObject GetAvailableFood() {
        for(int i = 0; i < foodPool.Length; i++) {
            // If the food is disabled, it's ready to be spawned in by the object pool so return
            if (!foodPool[i].isActiveAndEnabled) {
                return foodPool[i].gameObject;
            }
        }
        // Otherwise, every object is enabled in the scene so none can be used
        return null;
    }
}
