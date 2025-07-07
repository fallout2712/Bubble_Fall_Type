using UnityEngine;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    public CellColors cellColor;
    public bool HasBall = true;
    private SpriteRenderer sr;
    [SerializeField] private Sprite[] _bubbleSprites;

    protected void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void ApplyColor()
    {
        switch (cellColor)
        {
            case CellColors.Red:
                // sr.color = Color.red;
                sr.sprite = _bubbleSprites[0];
                break;
            case CellColors.Green:
                // sr.color = Color.green;
                sr.sprite = _bubbleSprites[1];
                break;
            case CellColors.Yellow:
                // sr.color = Color.yellow;
                sr.sprite = _bubbleSprites[2];
                break;
            case CellColors.Blue:
                // sr.color = Color.blue;
                sr.sprite = _bubbleSprites[3];
                break;
            default:
                // sr.color = Color.white;
                sr.sprite = _bubbleSprites[0];
                break;
        }
    }
    public void SetRandomColor()
    {
        CellColors[] values = (CellColors[])System.Enum.GetValues(typeof(CellColors));

        cellColor = values[Random.Range(0, values.Length)];

        ApplyColor();
    }
    public void SetFallingCell() // Падающая клетОчка
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
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody2D>();

            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Extrapolate;
            rb.velocity = new Vector2(0f, -5f);

            Destroy(GetComponent<Cell>());
        });
    }

    public void AddBallPopEffect() // Эффект лопания
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
        Destroy(GetComponent<Cell>());
        gameObject.AddComponent<BallPopEffect>();
    }
}
