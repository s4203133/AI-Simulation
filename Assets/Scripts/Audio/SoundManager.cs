using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("AI AGENTS SOUNDS")]
    public AudioClip agentEating;
    public AudioClip agentAttacking;
    public AudioClip agentDeath;
    public AudioClip rabbitAttack;
    public AudioClip FoxAttack;
    public AudioClip rabbitKilled;
    public AudioClip foxKilled;
    public AudioClip reproduce;
    public AudioClip eating;

    [Header("UI SOUNDS")]
    public AudioClip uiHover;
    public AudioClip uiSelect;
    public AudioClip uiOpenWindow;
    public AudioClip uiCloseWindow;

    [Header("OTHER SOUNDS")]
    public AudioClip gameStart;
    public AudioClip gameOver;
    public AudioClip selectingAgent;


    [Header("AUDIO SOURCES  ")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AgentInfoUI agentInfoUI;

    void Start()
    {
        if(instance != null) {
            Debug.LogWarning("Too many 'Audio Managers' in the scene, please unsure there is only one");
            Destroy(this);
        }
        instance = this;
    }

    // Plays a given sound
    public static void PlaySound(AudioClip sound) {
        instance.audioSource.PlayOneShot(sound);
    }

    // Only plays a given sound if it's within range of the manager
    // The manager follows the camera so the result is sound effects that only play when they are in view 
    public static void PlaySound(AudioClip sound, GameObject caller) {
        float dist = Vector3.Distance(caller.transform.position, instance.transform.position);
        if (dist <= 26f) {
            instance.audioSource.PlayOneShot(sound);
        }
    }
}