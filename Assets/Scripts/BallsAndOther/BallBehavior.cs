using UnityEngine;
using DG.Tweening;

public class BallBehavior : MonoBehaviour
{
    public Sprite[] BubbleSprites;
    public int SpriteIndex;
    public bool HasBall = true;
    protected Ball _ball;
    public SpriteRenderer spriteRenderer;
    public int[] GirdPosition = new int[2]; // [X, Y]

    protected virtual void Awake()
    {
        _ball = GetComponent<Ball>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ApplySprite(int index)
    {
        if (spriteRenderer == null)
            return;
        SpriteIndex = index;
        spriteRenderer.sprite = BubbleSprites[SpriteIndex];
    }

    public void SetRandomSprite()
    {
        ApplySprite(Random.Range(0, BubbleSprites.Length));
    }

    public void SetFallingBall()
    {
        gameObject.layer = LayerMask.NameToLayer("FallingCell");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(Random.Range(0.2f, 0.4f));
        seq.Append(transform.DOPunchScale(Vector3.one * 0.1f, 0.15f, 5, 1));
        seq.AppendCallback(() =>
        {
            // Rigidbody2D rb = GetComponent<Rigidbody2D>();
            // if (rb == null)
            //     rb = gameObject.AddComponent<Rigidbody2D>();

            // rb.gravityScale = 0f;
            // rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            // rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            // rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
            // float downForce = -9f;
            // rb.velocity = new Vector2(0f, downForce);

            TransformMover2D transformMover2D = gameObject.AddComponent<TransformMover2D>();
            transformMover2D.SetDirection(Vector3.down, 9f);

            Destroy(this); // Удаляем BallBehavior
        });
    }

    public void AddBallPopEffect()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        Destroy(this); // Удаляем BallBehavior
        UIController.Instance.AddScore(10);
        gameObject.AddComponent<BallPopEffect>();
    }
}
