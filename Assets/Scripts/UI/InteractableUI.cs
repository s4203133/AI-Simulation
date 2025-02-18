using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Add to any UI elements that can be interacted with. Will change the mouse cursor and play sounds to make it feel more respondive
/// </summary>
public class InteractableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private MouseCursor mouseCursor;

    void Start() {
        mouseCursor = FindObjectOfType<MouseCursor>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        SoundManager.PlaySound(SoundManager.instance.uiHover);
        mouseCursor.HoveringOverUI(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        mouseCursor.HoveringOverUI(false);
    }
}
