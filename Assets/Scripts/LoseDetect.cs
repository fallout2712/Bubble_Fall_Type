using UnityEngine;

public class LoseDetect : MonoBehaviour
{
    [Header("Detection Settings")]
    public Vector2 boxSize = new Vector2(10f, 1f);
    public Vector2 boxOffset = Vector2.zero;
    public LayerMask СellLayer;

    private void FixedUpdate()
    {
        if (GameManager.Instance.GetGameState() != GameState.Playing)
            return;

        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + boxOffset, boxSize, 0f, СellLayer);

        foreach (var hit in hits)
        {
            Ball targetCell = hit.GetComponent<Ball>();
            if (targetCell != null && targetCell.HasBall)
            {
                Debug.Log("Loss — найден Cell с HasBall = true");
                GameManager.Instance.LoseGame();
                return;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + boxOffset, boxSize);
    }
}
