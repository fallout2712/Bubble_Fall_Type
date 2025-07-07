using UnityEngine;
using UnityEngine.EventSystems;

public class AimZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RotationController RotationController;

    public void OnPointerDown(PointerEventData eventData)
    {
        RotationController.StartAiming(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RotationController.ContinueAiming(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RotationController.ReleaseAiming(eventData.position);
    }
}
