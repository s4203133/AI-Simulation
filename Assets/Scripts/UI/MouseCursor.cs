using UnityEngine;


public class MouseCursor : MonoBehaviour
{
    public Texture2D cursorStandard;
    public Texture2D cursorGrab;
    public Texture2D cursorHover;

    private bool useCustomCursor;
    private Camera cam;
    private CameraController camController;
    [SerializeField] private LayerMask interactableLayers;
    private GameObject currentTargetObject;

    public bool isHovering;
    public bool isUIHovering;
    public bool selectingAllowed;

    private bool startedHover;

    public delegate void SelectAgentEvent(AIAgent agent);
    public static SelectAgentEvent selectedAgent;

    void Start()
    {
        useCustomCursor = true;
        Cursor.SetCursor(cursorStandard, Vector2.zero, CursorMode.Auto);
        cam = GetComponent<Camera>();
        camController = cam.GetComponent<CameraController>();
        selectingAllowed = true;
    }

    private void Update() {
        // Allow the custom mouse to be toggled on or off
        if (Input.GetKeyDown(KeyCode.G)) {
            ToggleCustomMouse();
        }

        // Set the cursor based on the preferences currently set
        if (useCustomCursor) {
            SetCursor();
        } else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        HandleSelection();
    }

    void SetCursor() {
        // If the cursor is hovering over an interactable object but not grabbing it, set the sprite to a hover one
        if (HoveringOverInteractableObject() && !Grabbing()) {
            Cursor.SetCursor(cursorHover, Vector2.zero, CursorMode.Auto);
            return;
        }
        // If the cursor is hovering over an interactable UI element but not grabbing anything, set the sprite to a hover one
        if (isUIHovering && !Grabbing()) {
            Cursor.SetCursor(cursorHover, Vector2.zero, CursorMode.Auto);
            return;
        }
        // If the user is grabbing something, set the sprite to a gravving one
        if (Grabbing()) {
            Cursor.SetCursor(cursorGrab, Vector2.zero, CursorMode.Auto);
            return;
        }
        // If no conditions are satisfied, set the cursor sprite to a standard one
        Cursor.SetCursor(cursorStandard, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Is the user currently holding down the left mouse button
    /// </summary>
    /// <returns></returns>
    bool Grabbing() {
        return(Input.GetMouseButton(0));
    }

    void ToggleCustomMouse() {
        useCustomCursor = !useCustomCursor;
    }

    /// <summary>
    /// Returns if the user is currently hovering the mouse of an interactable object in the scene
    /// </summary>
    /// <returns></returns>
    bool HoveringOverInteractableObject() {
        // If the cursor is hovering over Ui or selecting isn't allowed, don't continue
        if (isUIHovering) {
            ActivateCamControls(false);
            return false;
        }
        if (!selectingAllowed) {
            return false;
        }
        // Shoot a raycast into the world from the camera to the mouses position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayers)) {
            // Check if the ray hit an interactable object
            if ((interactableLayers & (1 << hit.collider.gameObject.layer)) != 0) {
                // Don't select objects if the user is already grabbing
                if(!Grabbing()) {
                    isHovering = true;
                    // Play sound
                    if (!startedHover) {
                        startedHover = true;
                        SoundManager.PlaySound(SoundManager.instance.selectingAgent);
                    } else {
                        // Cache the currently selected target
                        currentTargetObject = hit.transform.gameObject;
                    }
                }
                return true;
            }
        }

        // If no interactable objects were found, reset variables
        currentTargetObject = null;
        isHovering = false;
        startedHover = false;
        return false;
    }

    void HandleSelection() {
        // If the left mouse button is clicked while hovering over an object
        if (Input.GetMouseButtonDown(0) && isHovering) {
            // Disable the camera from being moved around
            ActivateCamControls(false);
        }
        if (Input.GetMouseButtonUp(0)) {
            // If the left mouse button is released and the user is hovering over an object and not over the UI
            if (isHovering && !isUIHovering) {
                // Shoot a ray to get the target object
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                AIAgent agent = null;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayers)) {
                    // See if the hit object is different to the target the user was selecting when pressing the left mouse button down
                    if (currentTargetObject == null || hit.transform.gameObject != currentTargetObject) {
                        return;
                    }
                    // If it's the correct object, set the camera to follow them, and update the UI
                    agent = hit.transform.GetComponent<AIAgent>();
                    camController.TargetGameObject(currentTargetObject);
                    selectedAgent.Invoke(agent);
                }
            }
            ActivateCamControls(true);
        }
    }

    /// <summary>
    /// Allows camera movement to be enabled or disabled
    /// </summary>
    /// <param name="value"></param>
    void ActivateCamControls(bool value) {
        camController.canInitiateMovement = value;
    }

    /// <summary>
    /// If the mouse is hovering over UI, disable camera contorls, otherwise enable them
    /// </summary>
    /// <param name="value"></param>
    public void HoveringOverUI(bool value) {
        if (value) {
            isUIHovering = true;
            ActivateCamControls(false);
        } else {
            isUIHovering = false;
            if (!Grabbing()) {
                ActivateCamControls(true);
            }
        }
    }

    /// <summary>
    /// Allow interactable objects to be selected
    /// </summary>
    /// <param name="value"></param>
    public void EnableSelecting(bool value) {
        selectingAllowed = value;
    }
}
