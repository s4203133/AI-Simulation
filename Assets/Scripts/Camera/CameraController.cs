using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Transform thisTransform;

    private Vector3 dragStartPos;
    private Vector3 targetPos;
    private GameObject target;
    bool followTargetObject;
    [SerializeField] private Vector3 followTargetOffset;

    [SerializeField] private float moveSensitivity;
    [SerializeField] private float smoothness;

    public bool canInitiateMovement;
    public bool gameStarted;

    void Start()
    {
        cam = GetComponent<Camera>();
        thisTransform = transform;
        targetPos = thisTransform.position;
        
        canInitiateMovement = false;
        gameStarted = false;
        AIAgentManager.OnEnd += DeactivateCamera;
    }

    void Update()
    {
        PanCamera();
    }

    private void FixedUpdate() {
        thisTransform.position = Vector3.Lerp(thisTransform.position, targetPos, smoothness);
    }

    void PanCamera() {
        if(followTargetObject && target != null) {
            FollowTargetGameObject();
            return;
        }
        if (!canInitiateMovement || !gameStarted) {
            return;
        }
        Vector3 pos = thisTransform.position;

        // When the left mouse button is clicked, get the position of the mouse on the screen
        if(Input.GetMouseButtonDown(0)) {
            dragStartPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // While the left mouse button is still down, get the distance between the current mouse position and the starting position
        if (Input.GetMouseButton(0)) {
            Vector3 direction = (dragStartPos - cam.ScreenToWorldPoint(Input.mousePosition));
            targetPos = pos + (direction * moveSensitivity);
        }

        targetPos = new Vector3(Mathf.Clamp(targetPos.x, -125, 100), 20, Mathf.Clamp(targetPos.z, -135, 100));
    }

    public void ActivateCamera() {
        gameStarted = true;
    }

    public void DeactivateCamera() {
        gameStarted = false;
    }

    public void TargetGameObject(GameObject newTarget) {
        followTargetObject = true;
        target = newTarget;
    }

    private void FollowTargetGameObject() {
        targetPos = target.transform.position + followTargetOffset;
        targetPos.y = 20;
    }

    public void SetFollowingTarget(bool value) {
        followTargetObject = value;
    }
}
