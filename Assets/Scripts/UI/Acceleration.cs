using UnityEngine;
using UnityEngine.EventSystems;

public class Acceleration : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Кнопка ускорения
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.Instance.BoostFallSpeed();
        AudioManager.Instance.PlayLoop("SaturdayMorning");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.BoostFallSpeed();
        AudioManager.Instance.StopLoop();
    }
}
