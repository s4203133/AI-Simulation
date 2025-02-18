using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentInfoUI : MonoBehaviour
{
    [SerializeField] private Animator tabAnimator;
    [SerializeField] private Animator openingButtonAnimator;

    public AIAgent currentAgent {  get; private set; }

    [SerializeField] private GameObject panelParent;

    [Header("Title Text")]
    [SerializeField] private TextMeshProUGUI agentType;

    [Header("Agent Logo")]
    [SerializeField] private Image agentLogo;
    [SerializeField] private Sprite rabbitLogo;
    [SerializeField] private Sprite foxLogo;

    [Header("Agent Behaviour")]
    [SerializeField] private TextMeshProUGUI currentAction;

    [Header("Main Stats")]
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI stamina;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI hunger;
    [SerializeField] private TextMeshProUGUI tiredness;
    [SerializeField] private TextMeshProUGUI reproduce;

    [Header("Sensory System Stats")]
    [SerializeField] private TextMeshProUGUI viewDistance;
    [SerializeField] private TextMeshProUGUI viewAngle;
    [SerializeField] private TextMeshProUGUI hearingRange;

    [Header("Combat Stats")]
    [SerializeField] private TextMeshProUGUI Attack;
    [SerializeField] private TextMeshProUGUI Defense;
    [SerializeField] private TextMeshProUGUI Kills;

    [Header("Life Stats")]
    [SerializeField] private TextMeshProUGUI maxOffspring;
    [SerializeField] private TextMeshProUGUI currentOffspring;
    [SerializeField] private TextMeshProUGUI TimeAlive;
    private string lastTimeAliveText;

    private bool panelOpen;
    bool panelAnimating;

    private void Start() {
        panelOpen = true;
        MouseCursor.selectedAgent += HandlePanel;
    }

    private void Update() {
        ConstantUpdate();
    }

    // When an agent is selected determine how to handle the ui panel based on it's state
    public void HandlePanel(AIAgent agent) {
        // Don't continue if:
        if (agent == null || // If the agent doesnt exist
            (panelOpen && agent == currentAgent) || // If the agent is already the currently selected one
            panelAnimating) { // If the panel is currently animating
            return;
        }
        currentAgent = agent;
        // If the panel is already open, refresh it by closing it, updating the info, and re opening it
        if (panelOpen) {
            StartCoroutine(RefreshPanel(agent));
            return;
        }

        // If the panel is closed, update the info on it, and open it
        UpdatePanelAgentInfo(agent);
        OpenPanel();
    }

    public void OpenPanel() {
        tabAnimator.SetTrigger("Open");
        openingButtonAnimator.SetBool("Clicked", true);
        SoundManager.PlaySound(SoundManager.instance.uiOpenWindow);
        panelOpen = true;
    }

    public void ClosePanel() {
        SoundManager.PlaySound(SoundManager.instance.uiCloseWindow);
        tabAnimator.SetTrigger("Close");
        panelOpen = false;
    }

    /// <summary>
    /// Updates all information on the agents UI panel to match the stats of the selected agent
    /// </summary>
    /// <param name="agentInfo"></param>
    private void UpdatePanelAgentInfo(AIAgent agentInfo) {
        agentType.text = agentInfo.aiType.ToString();

        if(agentInfo.aiType == AIAgentTypes.RABBIT) {
            agentLogo.sprite = rabbitLogo;
        } else {
            agentLogo.sprite = foxLogo;
        }

        currentAction.text = agentInfo.stats.currentAction;
        speed.text = agentInfo.speed.ToString("F2");
        stamina.text = agentInfo.stamina.ToString("F2");
        health.text = agentInfo.health.CurrentHealth().ToString("F0");
        hunger.text = agentInfo.hunger.GetValue().ToString("F2");
        tiredness.text = agentInfo.tiredness.GetValue().ToString("F2"); ;
        reproduce.text = agentInfo.reproduction.GetValue().ToString("F2");

        viewDistance.text = agentInfo.stats.viewDistance.ToString("F2");
        viewAngle.text = agentInfo.stats.viewAngle.ToString("F0");
        hearingRange.text = agentInfo.stats.hearingRange.ToString("F0");

        Attack.text = agentInfo.combat.attack.ToString("F2");
        Defense.text = agentInfo.combat.defense.ToString("F2");
        Kills.text = agentInfo.stats.numOfKills.ToString();

        maxOffspring.text = agentInfo.stats.maxOffspring.ToString();
        currentOffspring.text = agentInfo.stats.numOfOffspring.ToString();
        TimeAlive.text = agentInfo.stats.timeAlive.ToString("F0");
    }

    /// <summary>
    /// Close the panel first, wait for a delay, then update the info and open it again
    /// </summary> 
    /// <param name="newAgent"></param>
    /// <returns></returns>
    private IEnumerator RefreshPanel(AIAgent newAgent) {
        ClosePanel();
        panelAnimating = true;
        yield return new WaitForSeconds(0.6f);
        panelAnimating = false;
        if (newAgent == null) {
            yield break;
        }
        UpdatePanelAgentInfo(newAgent);
        OpenPanel();
    }

    // Functions and coroutines to be used by UI buttons in Unity for opening and closing the UI panel
    public void OpenPanelFromUI() {
        StartCoroutine(_OpenPanelFromUI());
    }

    public void ClosePanelFromUI() {
        StartCoroutine(_ClosePanelFromUI());
    }

    private IEnumerator _OpenPanelFromUI() {
        panelAnimating = true;  
        openingButtonAnimator.SetBool("Clicked", true);
        yield return new WaitForSeconds(0.4f);
        OpenPanel();
        panelAnimating = false;
    }

    private IEnumerator _ClosePanelFromUI() {
        panelAnimating = true;
        ClosePanel();
        yield return new WaitForSeconds(0.4f);
        openingButtonAnimator.SetBool("Clicked", false);
        panelAnimating = false;
    }

    /// <summary>
    /// Update any values that change across time
    /// </summary>
    private void ConstantUpdate() {
        if(currentAgent == null) {
            if (panelOpen) {
                ClosePanel();
            }
            return;
        }
        // Only update the UI text if the values have changed
        // As the whole UI has to be re drawn if one thing changes, this prevents unnecessary re-drawing of the UI when there's been no changes
        if (currentAction.text != currentAgent.stats.currentAction){
            currentAction.text = currentAgent.stats.currentAction;
        }
        if (stamina.text != currentAgent.stamina.ToString("F2")) {
            stamina.text = currentAgent.stamina.ToString("F2");
        }
        if (health.text != currentAgent.health.CurrentHealth().ToString("F0")) {
            health.text = currentAgent.health.CurrentHealth().ToString("F0");
        }
        if (hunger.text != currentAgent.hunger.GetValue().ToString("F2")) {
            hunger.text = currentAgent.hunger.GetValue().ToString("F2");
        }
        if (tiredness.text != currentAgent.tiredness.GetValue().ToString("F2")) {
            tiredness.text = currentAgent.tiredness.GetValue().ToString("F2");
        }
        if (reproduce.text != currentAgent.reproduction.GetValue().ToString("F2")) {
            reproduce.text = currentAgent.reproduction.GetValue().ToString("F2");
        }
        if (viewDistance.text != currentAgent.stats.viewDistance.ToString("F2")) {
            viewDistance.text = currentAgent.stats.viewDistance.ToString("F2");
        }
        if (viewAngle.text != currentAgent.stats.viewAngle.ToString("F0")) {
            viewAngle.text = currentAgent.stats.viewAngle.ToString("F0");
        }
        if (currentOffspring.text != currentAgent.stats.numOfOffspring.ToString()) {
            currentOffspring.text = currentAgent.stats.numOfOffspring.ToString();
        }
        if (Kills.text != currentAgent.stats.numOfKills.ToString()) {
            Kills.text = currentAgent.stats.numOfKills.ToString();
        }

        // Format the time the agent has been alive into minutes and seconds
        int minutes = (int)currentAgent.stats.timeAlive / 60;
        int seconds = (int)currentAgent.stats.timeAlive - (minutes * 60);
        string timeAliveText = minutes + "m " + seconds.ToString().PadLeft(2, '0') + "s"; 

        if (timeAliveText != lastTimeAliveText) {
            TimeAlive.text = timeAliveText;
            lastTimeAliveText = timeAliveText;  
        }
    }
}
