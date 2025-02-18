using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField] private GameObject endingScreen;
    [SerializeField] private Animator fadeAnim;

    void Start()
    {
        AIAgentManager.OnEnd += End;
    }

    // Activates the ending scene when there are no agents left in the simulation
    private void End() {
        endingScreen.SetActive(true);
    }

    public void RestartLevel(float delay) {
        StartCoroutine(LoadSceneWithDelay(delay));
    }

    private IEnumerator LoadSceneWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }
}
