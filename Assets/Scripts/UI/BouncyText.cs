using UnityEngine;
using DG.Tweening;

public class BouncyText : MonoBehaviour
{
    [SerializeField] private float punchScale = 0.2f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private float delayBetween = 1f;

    void Start()
    {
        Animate();
    }

    void Animate()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOPunchScale(Vector3.one * punchScale, duration, 1, 0.5f))
           .AppendInterval(delayBetween)
           .OnComplete(Animate);
    }

    void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
