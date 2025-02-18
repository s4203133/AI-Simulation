using System.Collections.Generic;
using UnityEngine;

public class ScentTrail : MonoBehaviour
{
    [Tooltip("The amount of time in seconds between spawning each scent node")]
    [SerializeField] private float spawnIntervals;
    private float timer;

    [Tooltip("The time in seconds it takes for each scent node to fade out")]
    [SerializeField] private float scentFadeTime;

    [SerializeField] private ScentNode scentNodePrefab;

    public EDetectableObjectCategories scentType;

    private Transform thisTransform;
    private Vector3 lastSpawnPosition = Vector3.zero;

    public List<ScentNode> scentTrail;

    void Start()
    {
        timer = spawnIntervals;
        thisTransform = transform;
    }

    void Update()
    {
        // Spawn a scent node at intervals using a timer
        timer -= Time.deltaTime;
        if(timer <= 0) {
            SpawnScentNode();
            timer = spawnIntervals;
        }
    }

    private void SpawnScentNode() {
        // If the agent hasn't moved since last spawning a node, don't spawn a new one
        if(lastSpawnPosition == thisTransform.position) {
            return;
        }
        // Initialise a scent node, and add it to the trail
        ScentNode newScentNode = Instantiate(scentNodePrefab, thisTransform.position, Quaternion.identity);
        newScentNode.SetIntensity(scentFadeTime);
        newScentNode.SetScentTrail(this);
        newScentNode.SetScentType(scentType);
        scentTrail.Add(newScentNode);
        lastSpawnPosition = thisTransform.position;
    }
}
