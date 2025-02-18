using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Add to UI elements. Prevents the mouse cursor from raycasting to objects in the scene while it's hovering over the UI
/// </summary>
public class UIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private MouseCursor mouseCursor;

    void Start() {
        mouseCursor = FindObjectOfType<MouseCursor>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mouseCursor.EnableSelecting(false);
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseCursor.EnableSelecting(true);
    }
}
