using System.Collections.Generic;
using UnityEngine;

public class Projectile : Cell
{
    [SerializeField] private float _radius = 1f;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _powerForce = 50f;
    private Rigidbody2D rb;
    private Vector2 _lastDirection;
    public bool HasBeenUsed = false;
    private bool _alreadyAttached = false;


    private new void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        ApplyColor();
    }

    private void OnTriggerEnter2D(Collider2D collision) // Только кокоджамбики
    {
        if (_alreadyAttached) return;

        Cell targetCell = collision.GetComponent<Cell>();
        if (targetCell == null || targetCell == this) return;

        _alreadyAttached = true;

        Cluster cluster = targetCell.transform.parent.GetComponent<Cluster>();
        if (cluster == null) return;

        transform.SetParent(cluster.transform);

        Vector3 relativePos = transform.position - targetCell.transform.position;
        Vector3 offset = Vector3.zero;

        if (relativePos.x > 0 && relativePos.y > 0)
            offset = new Vector3(0.55f, 1.05f, 0f);
        else if (relativePos.x < 0 && relativePos.y > 0)
            offset = new Vector3(-0.55f, 1.05f, 0f);
        else if (relativePos.x > 0 && relativePos.y < 0)
            offset = new Vector3(0.55f, -1.05f, 0f);
        else if (relativePos.x < 0 && relativePos.y < 0)
            offset = new Vector3(-0.55f, -1.05f, 0f);

        transform.position = targetCell.transform.position + offset;

        GetComponent<Collider2D>().isTrigger = true;

        if (rb != null)
            Destroy(rb);

        AudioManager.Instance.Play("ProjectilePop");
        FindAndDestroyConnectedObjects();
        HasBeenUsed = true;
        HasBall = true;

        ProjectileEvents.RaiseProjectileAttached(this);
    }

    public void OnCollisionEnter2D(Collision2D collision) // Стены
    {
        if (((1 << collision.gameObject.layer) & _wallLayer) != 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            rb.velocity = Vector2.zero;

            Vector2 velocity = _lastDirection;

            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
                velocity.x *= -1;
            }
            else
            {
                velocity.y *= -1;
            }

            AudioManager.Instance.Play("ProjectilePop");

            Shoot(velocity.normalized);
        }
    }

    private void FindAndDestroyConnectedObjects()
    {
        List<Cell> connected = new List<Cell>();
        Queue<Cell> queue = new Queue<Cell>();

        queue.Enqueue(this);
        connected.Add(this);

        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();
            Collider2D[] hits = Physics2D.OverlapCircleAll(current.transform.position, _radius, _targetLayer);

            foreach (var hit in hits)
            {
                Cell neighbor = hit.GetComponent<Cell>();
                if (neighbor != null && neighbor.cellColor == cellColor && !connected.Contains(neighbor))
                {
                    connected.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        Debug.Log($"Найдено связанных объектов: {connected.Count}");

        if (connected.Count >= 3)
        {
            foreach (var obj in connected)
            {
                obj.AddBallPopEffect();
                UIController.Instance.AddScore(10);
            }
            int repitSoinds = connected.Count;
            if (repitSoinds >= 15)
                repitSoinds = 15;
            AudioManager.Instance.PlaySequence("Pop", repitSoinds, 0.04f);
        }
        else
        {
            Debug.Log("Мало объектов для удаления");
        }
    }
    public void Shoot(Vector2 dir)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        _lastDirection = dir.normalized;
        rb.velocity = _lastDirection * _powerForce;
    }

}
