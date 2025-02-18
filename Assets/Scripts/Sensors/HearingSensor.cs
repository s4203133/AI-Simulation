using System.Collections.Generic;
using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    public float hearingRange;
    private float hearingRangeSqr;

    private List<HeardSound> heardSounds;
    public List<HeardSound> allHeardSounds => heardSounds;

    void Start()
    {
        hearingRangeSqr = hearingRange * hearingRange;
        HearingManager.instance.Add(this);
        heardSounds = new List<HeardSound>();
    }

    private void Update() {
        CheckSoundTimersRanOut();
    }

    void OnDestroy()
    {
        HearingManager.instance.Remove(this);
    }

    /// <summary>
    /// When a sound has been produced, chech if it's within range of this agent, and mark it as heard if so
    /// </summary>
    /// <param name="location"></param>
    /// <param name="category"></param>
    /// <param name="volume"></param>
    /// <param name="caller"></param>
    public void OnHeardSound(Vector3 location, ESoundCategories category, float volume, GameObject caller) {
        // Check the sound is within range
        if((location - transform.position).sqrMagnitude < hearingRangeSqr) {
            int allSoundsCount = allHeardSounds.Count;
            // Loop through all heard sounds and see if this sound is once thats already been heard before
            for (int i = 0; i < allSoundsCount; i++) {
                HeardSound thisSound = allHeardSounds[i];
                if (thisSound.associatedObj == caller && thisSound.soundCategory == category) {
                    // If the agent has already heard this sound, update the location at which it was heard at and reset it's timer 
                    thisSound.location = location;
                    thisSound.timer = 10;
                    allHeardSounds[i] = thisSound;
                    return;
                }
            }
            // If this is a new sound, add it into the heard sounds list
            HeardSound sound = new HeardSound();
            sound.location = location;
            sound.soundCategory = category;
            sound.volume = volume;
            sound.associatedObj = caller;
            sound.timer = 10;
            heardSounds.Add(sound);
        }
    }

    /// <summary>
    /// Forget any sounds that haven't been heard in a while
    /// </summary>
    private void CheckSoundTimersRanOut() {
        int numOfSounds = heardSounds.Count;
        if (numOfSounds == 0) {
            return; 
        }

        // For each sound in memory, decrease it's timer and remove it from the heard sounds list if it's below 0
        for(int i = 0; i < numOfSounds; i++) {
            HeardSound sound = heardSounds[i];
            sound.timer -= Time.deltaTime;
            heardSounds[i] = sound;
            if (sound.timer <= 0) {
                heardSounds.Remove(sound);
                numOfSounds--;
            }
        }
    }

    /// <summary>
    /// Check through all heard sounds, and return if one of them is of a given type
    /// </summary>
    /// <param name="soundType"></param>
    /// <returns></returns>
    public bool HasHeardSoundOfType(ESoundCategories soundType) {
        // If there are no sounds at all, then retrun false
        int numOfSounds = heardSounds.Count;
        if (numOfSounds == 0) {
            return false;
        }

        // If at least one sound is of the queried category, then return true
        for (int i = 0; i < numOfSounds; i++) {
            ESoundCategories soundCategory = heardSounds[i].soundCategory;
            if ((soundType & soundCategory) == soundCategory) {
                return true;
            }
        }

        // If the loop finished, then none of the heard sounds have the queried category, so return false
        return false;
    }

    /// <summary>
    /// Return all heard sounds of a given type
    /// </summary>
    /// <param name="soundType"></param>
    /// <returns></returns>
    public List<HeardSound> GetHeardSoundsOfType(ESoundCategories soundType) {
        List<HeardSound> returnList = new List<HeardSound>();
        int count = allHeardSounds.Count;
        // For each sound, add the ones that match the sound type to a list
        for (int i = 0; i < count; i++) {
            ESoundCategories soundCategory = heardSounds[i].soundCategory;
            if ((soundType & soundCategory) == soundCategory) {
                returnList.Add(allHeardSounds[i]);
            }
        }
        // Return all sounds that match
        return returnList;
    }

    /// <summary>
    /// Returns the general area for a set of sounds
    /// </summary>
    /// <param name="soundType"></param>
    /// <returns></returns>
    public Vector3 GetGeneralSoundArea(ESoundCategories soundType) {
        Vector3 location = Vector3.zero;
        int numOfSounds = heardSounds.Count;
        int count = 0;
        // Add up all the locations of the correct sounds
        for (int i = 0; i < numOfSounds; i++) {
            HeardSound sound = heardSounds[i];
            if (sound.soundCategory == soundType) {
                location += sound.location;
                count++;
            }
        }
        // Get the average vector as the return location
        location /= count;
        return location;
    }
}

[System.Serializable]
public struct HeardSound {
    public Vector3 location;
    public ESoundCategories soundCategory;
    public float volume;
    public GameObject associatedObj;
    public float timer;
}