using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    [Tooltip("The time in seconds that it takes for an object to be removed from memory when it is no longer in range of the AI")]
    [SerializeField] private float durationObjectsStayInMemory;

    // Food
    private List<DetectableObject> rabbitFood = new List<DetectableObject>();
    public List<DetectableObject> GetRabbitFood => rabbitFood;

    // Rabbits
    private List<DetectableObject> rabbits = new List<DetectableObject>();
    public List<DetectableObject> GetRabbits => rabbits;

    // Foxes
    private List<DetectableObject> foxes = new List<DetectableObject>();
    public List<DetectableObject> GetFoxes => foxes;

    // Hiding Zones
    private List<DetectableObject> hidingZones = new List<DetectableObject>();
    public List<DetectableObject> GetHidingZones => hidingZones;

    // All Objects In Memory
    [SerializeField] private List<MemoryObject> allObjects = new List<MemoryObject>();

    void Awake()
    {
        rabbitFood = new List<DetectableObject>();
        rabbits = new List<DetectableObject>();
        foxes = new List<DetectableObject>();
        hidingZones = new List<DetectableObject>();

        DetectableObject.onDestroyed += RemoveItem;
        HidingSpot.OnHide += RemoveItem;
        Food.OnTagged += RemoveItem;
    }

    private void Update() {
        StartCoroutine(TickMemoryObjects());
    }

    // Adding and Removing objects from memory

    /// <summary>
    /// Add an object into memory
    /// </summary>
    /// <param name="newObject"></param>
    public void AddDetectableObject(DetectableObject newObject) {
        List<DetectableObject> listToAddTo = GetListOfType(newObject.objectCategory);
        if (!listToAddTo.Contains(newObject)) {
            listToAddTo.Add(newObject);
        }
    }

    /// <summary>
    /// Remove an object from memory
    /// </summary>
    /// <param name="newObject"></param>
    public void RemoveDetectableObject(DetectableObject newObject) {
        List<DetectableObject> listToAddTo = GetListOfType(newObject.objectCategory);

        // If this is the last element to remove from the list, clear it instead
        if (listToAddTo.Count == 1) {
            listToAddTo.Clear();
            return;
        }

        // If the object exists in memory, remove it
        if (listToAddTo.Contains(newObject)) {
            listToAddTo.Remove(newObject);
        }
    }

    /// <summary>
    /// Returns a list of every type of object in memory
    /// </summary>
    /// <returns></returns>
    public List<DetectableObject> AllObjects() {
        List<DetectableObject> list = new List<DetectableObject>();
        list.AddRange(rabbitFood);
        list.AddRange(rabbits);
        list.AddRange(foxes);
        list.AddRange(hidingZones);
        return list;
    }
    
    /// <summary>
    /// Add a range of objects into memory
    /// </summary>
    /// <param name="newItems"></param>
    public void AddDetectableObjects(List<DetectableObject> newItems) {
        int numberOfItems = newItems.Count;
        for (int i = 0; i < numberOfItems; i++) {
            // Check for any conditions that might mean the item is invalid to detect
            bool validItem = FilterItem(newItems[i]);
            if (validItem) {
                // Add 'Item' to corresponding list of items in memory
                DetectableObject item = newItems[i];
                AddDetectableObject(item);

                // Add 'item' to 'allObjects' list
                AddObjectToMemory(item);
            }
        }
    }

    /// <summary>
    /// Adds a detectable object as a memory object
    /// </summary>
    /// <param name="item"></param>
    public void AddObjectToMemory(DetectableObject item) {
        int numOfObjects = allObjects.Count;
        // Check through all objects that exist in memory
        for (int i = 0; i < numOfObjects; i++) {
            // If the object already exists, reset it's timer
            MemoryObject obj = allObjects[i];
            if (obj.thisObject == item) {
                obj.ResetTimer(durationObjectsStayInMemory);
                obj.lastKnownLocation = item.transform.position;
                return;
            }
        }
        // If the object didn't exist in memory, initialise and add it
        MemoryObject newItem = new MemoryObject();
        newItem.thisObject = item;
        newItem.timer = durationObjectsStayInMemory;
        newItem.lastKnownLocation = item.transform.position;
        allObjects.Add(newItem);
        
    }

    /// <summary>
    /// Remove an object both from it's indivudal type list, and as a memory object
    /// </summary>
    /// <param name="itemToRemove"></param>
    public void RemoveItem(DetectableObject itemToRemove) {
        RemoveDetectableObject(itemToRemove);
        RemoveUnseenItem(itemToRemove);
    }

    /// <summary>
    /// Given an object, determine whether it is valid to be detected or not
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool FilterItem(DetectableObject obj) {
        bool returnVal = true; ;
        switch (obj.objectCategory) {
            case (EDetectableObjectCategories.RABBIT):
                returnVal = FilterRabbit(obj);
                break;
            case (EDetectableObjectCategories.RABBIT_FOOD):
                returnVal = FilterRabbitFood(obj);
                break;
        }
        return returnVal;
    }

    /// <summary>
    /// All checks to see if a rabbit can be detected
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool FilterRabbit(DetectableObject obj) {
        Rabbit rabbit = obj.GetComponent<Rabbit>();
        if (rabbit.combat.isHidden || rabbit.combat.isBeingChased) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// All checks to see if a given rabbit food can be detected
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool FilterRabbitFood(DetectableObject obj) {
        if (obj.GetComponent<Food>().isTaken) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Reduce the timer of all objects in memory, and remove it if the timer has reached 0
    /// </summary>
    /// <returns></returns>
    private IEnumerator TickMemoryObjects() {
        // If there are no objects to tick, do not continue
        int numOfObjects = allObjects.Count;
        if(numOfObjects == 0) {
            yield return null; 
        }

        for (int i = 0; i < numOfObjects; i++) {
            MemoryObject objectInMemory = allObjects[i];
            // If the object is ticked and returned false (its timer is less than 0), then remove it from memory
            bool objectStillActive = objectInMemory.TickObject(Time.deltaTime);
            if (!objectStillActive) {
                RemoveItem(objectInMemory.thisObject);
                allObjects.Remove(objectInMemory);
                numOfObjects = allObjects.Count;
            }
        }

        yield return null;
    }

    /// <summary>
    /// Removes an object that hasn't been seen in a while from memory
    /// </summary>
    /// <param name="item"></param>
    public void RemoveUnseenItem(DetectableObject item) {
        // For each object in memory, if it matches the one passed into the function, then remove it
        for (int i = 0; i < allObjects.Count; i++) {
            MemoryObject objectInMemory = allObjects[i];
            if (objectInMemory.thisObject.gameObject == item.gameObject) {
                allObjects.RemoveAt(i);
                break;
            }
        }
    }

    // Functions For Accessing Objects From Memory

    /// <summary>
    /// Returns the closest item of a given type
    /// </summary>
    /// <param name="objectCategory"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public DetectableObject GetClosestItem(EDetectableObjectCategories objectCategory, Transform location) {
        List<DetectableObject> objects = GetListOfType(objectCategory);

        if (objects.Count == 0) return null;

        DetectableObject returnObject = ClosestObject(objects, location.position);

        return returnObject;
    }

    /// <summary>
    /// Returns every object in memory of a given type
    /// </summary>
    /// <param name="objectCategory"></param>
    /// <returns></returns>
    public List<DetectableObject> GetAllVisibleObjectsOfType(EDetectableObjectCategories objectCategory) {
        return GetListOfType(objectCategory);
    }

    /// <summary>
    /// Returns the corresponding list based on the object category passed in.
    /// </summary>
    /// <param name="objectCategory"></param>
    /// <returns></returns>
    private List<DetectableObject> GetListOfType(EDetectableObjectCategories objectCategory) {
        switch (objectCategory) {
            case (EDetectableObjectCategories.RABBIT_FOOD):
                return rabbitFood;
            case (EDetectableObjectCategories.FOX):
                return foxes;
            case (EDetectableObjectCategories.RABBIT):
                return rabbits;
            case (EDetectableObjectCategories.TALL_GRASS):
                return hidingZones;
        }
        return null;
    }

    /// <summary>
    /// Out of the list of objects provided, returns the closest one to the specified position.
    /// </summary>
    /// <param name="objects"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public DetectableObject ClosestObject(List<DetectableObject> objects, Vector3 position) {
        DetectableObject returnObject = null;
        float numberObjects = objects.Count;
        float minDist = float.MaxValue;

        // Get the distance to every target and return the closest one
        for (int i = 0; i < numberObjects; i++) {
            DetectableObject obj = objects[i];
            float dist = (obj.transform.position - position).sqrMagnitude;

            // If distance to target is closer than the current closest target
            if (dist < minDist) {
                returnObject = obj;
                minDist = dist;
            }
        }
        return returnObject;
    }

    /// <summary>
    /// Returns if at least one item of a given type exists in memory
    /// </summary>
    /// <param name="objectCategory"></param>
    /// <returns></returns>
    public bool ItemExistsInMemory(EDetectableObjectCategories objectCategory) {
        switch (objectCategory) {
            case (EDetectableObjectCategories.RABBIT_FOOD):
                return rabbitFood.Count > 0;
            case (EDetectableObjectCategories.FOX):
                return foxes.Count > 0;
            case (EDetectableObjectCategories.RABBIT):
                return rabbits.Count > 0;
            case (EDetectableObjectCategories.TALL_GRASS):
                return hidingZones.Count > 0;
        }
        return false;
    }

    /// <summary>
    /// Returns a list of memory objects of a given type
    /// </summary>
    /// <param name="objectCategory"></param>
    /// <returns></returns>
    private List<MemoryObject> GetObjectsInMemoryofType(EDetectableObjectCategories objectCategory) {
        List<MemoryObject> returnObjects = new List<MemoryObject>();    
        int count = allObjects.Count;
        // For each object in memory, if it's category matches the one passed into the function, add it to a list to be returned
        for(int i = 0; i < count; i++) {
            MemoryObject obj = allObjects[i];
            if (obj.thisObject.objectCategory == objectCategory) {
                returnObjects.Add(obj);
            }
        }
        return returnObjects;
    }

    /// <summary>
    /// Check if an object of a given type can currently be seen by the agent
    /// </summary>
    /// <param name="objectCategory"></param>
    /// <returns></returns>
    public bool ObjectsOfTypeIsInView(EDetectableObjectCategories objectCategory) {
        List<MemoryObject> obj = GetObjectsInMemoryofType(objectCategory);
        int count = obj.Count;
        for(int i = 0; i < count; i++) {
            // If at least one objects timer is close to the max, then its either in view or has just been in view, so return true
            if (obj[i].timer >= (durationObjectsStayInMemory - 0.05f)) {
                return true;
            }
        }
        // If all timers are counting down then all objects are out of range and being forgotten about
        return false;
    }

    /// <summary>
    /// Test if a given object is in view
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool ObjectIsInView(DetectableObject obj) {
        List<MemoryObject> objects = GetObjectsInMemoryofType(obj.objectCategory);
        int count = objects.Count;
        // For each memory object, check if it matches the object passed into the function
        for (int i = 0; i < count; i++) {
            if (objects[i].thisObject.gameObject == obj.gameObject) {
                // If the timer is close to the max, then its either in view or has just been in view, so return true
                if (objects[i].timer >= durationObjectsStayInMemory - 0.05f) {
                    return true;
                }
            }
        }
        // If all timers are counting down then all objects are out of range and being forgotten about
        return false;
    }

    /// <summary>
    /// Returns the last known location of a given object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Vector3 GetObjectsLastKnownLocation(DetectableObject obj) {
        // Loop through all objects and memory, and if it matches the object passed into the function, return it's last known location
        int count = allObjects.Count;
        for(int i = 0; i < count; i++) {
            MemoryObject memoryObj = allObjects[i];    
            if(memoryObj.thisObject.gameObject == obj.gameObject) {
                return memoryObj.lastKnownLocation;
            }
        }
        return Vector3.zero;
    }
}

/// <summary>
/// System for handling the objects in the AI's memory.
/// When an object is no longer detected by an AI agent, it's timer get ticked down to 0, when it is then removed from
/// the AI's memory. Every frame an object is detected, the timer is reset to the original value, ensuring it doesn't get
/// removed from memory while the AI is aware of it.
/// </summary>

enum MemoryObjectTypes {
    food,
    rabbit,
    fox,
}

[System.Serializable]
public class MemoryObject {
    public DetectableObject thisObject;
    public Vector3 lastKnownLocation;
    public float timer;

    public bool TickObject(float deltaTime) {
        // Reduce the timer by delta time. If it reaches 0, return false so that this object can now be removed from memory
        timer -= deltaTime;
        if (timer <= 0) {
            return false;
        }
        return true;
    }

    public void ResetTimer(float amount) {
        timer = amount;
    }
}
