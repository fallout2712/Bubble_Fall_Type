using UnityEngine;
using DG.Tweening;

public class BallPopEffect : MonoBehaviour
{
    [Header("Ball Pop Effect")]
    private float _popScale = 1.2f;
    private float _popDuration = 0.1f;
    private float _punchStrength = 0.15f;
    private float _punchDuration = 0.1f;
    private bool _popped = false;

    private void Start()
    {
        UseBallPopEffect();
    }

    public void UseBallPopEffect()
    {
        if (_popped) return;
        _popped = true;

        float randomDelay = Random.Range(0f, 0.2f);

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(randomDelay);
        seq.Append(transform.DOScale(_popScale, _popDuration).SetEase(Ease.OutBack));
        seq.Append(transform.DOPunchScale(Vector3.one * _punchStrength, _punchDuration, 10, 1));
        seq.AppendCallback(() =>
        {
            GameObject fx = Resources.Load<GameObject>("BabbleEffect");
            if (fx != null)
            {
                Instantiate(fx, transform.position, Quaternion.identity);
            }

            seq.Kill();
            Destroy(gameObject);
        });
    }
}
