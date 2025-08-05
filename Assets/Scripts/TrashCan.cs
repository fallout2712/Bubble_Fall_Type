using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Header("Detection Settings")]
    public Vector2 boxSize = new Vector2(10f, 1f);
    public Vector2 boxOffset = Vector2.zero;
    public LayerMask СellLayer;
    private void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll((Vector2)transform.position + boxOffset, boxSize, 0f, СellLayer);

        foreach (var hit in hits)
        {
            Destroy(hit.gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + boxOffset, boxSize);
    }
}
