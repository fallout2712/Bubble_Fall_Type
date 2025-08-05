using UnityEngine;

public abstract class ProjectileBehavior : BallBehavior, ILaunchable, IAttachable, IRicochetable
{
    public LayerMask WallLayer;
    // public Rigidbody2D Rb;
    public Vector2 LastDirection;
    public bool HasBeenUsed = false;
    public bool AlreadyAttached = false;
    private Vector2 _moveDirection = Vector2.zero;
    public TransformMover2D transformMover2D;
    protected override void Awake()
    {
        base.Awake();
        // Rb = GetComponent<Rigidbody2D>();

        if (transformMover2D == null)
            transformMover2D = gameObject.AddComponent<TransformMover2D>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & WallLayer) != 0)
        {
            // Для рикошета от стены нужно вычислить точку контакта
            Vector2 contactPoint = GetContactPoint(collider);
            Ricochet(contactPoint, LastDirection);
            return;
        }

        Ball targetBall = collider.gameObject.GetComponent<Ball>();
        if (targetBall == null) return;

        AttachTo(targetBall);
    }
    public Vector2 GetContactPoint(Collider2D wallCollider)
    {
        // Получаем ближайшую точку на коллайдере стены к центру мяча
        Vector2 ballCenter = transform.position;
        Vector2 contactPoint = wallCollider.ClosestPoint(ballCenter);

        return contactPoint;
    }
    public virtual void Ricochet(Vector2 contactPoint, Vector2 lastDirection)
    {
        // Rb.velocity = Vector2.zero;

        // Вычисляем нормаль к поверхности
        Vector2 ballCenter = transform.position;
        Vector2 normal = (ballCenter - contactPoint).normalized;

        // Отражаем вектор по формуле: reflected = direction - 2 * (direction · normal) * normal
        Vector2 reflectedDirection = lastDirection - 2 * Vector2.Dot(lastDirection, normal) * normal;

        AudioManager.Instance.Play("ProjectilePop");

        Launch(reflectedDirection.normalized, GameManager.Instance.LaunchPower);
    }
    public virtual void AttachTo(Ball targetBall)
    {
        if (AlreadyAttached) return;
        AlreadyAttached = true;

        Cluster cluster = targetBall.transform.parent.GetComponent<Cluster>();
        if (cluster == null) return;

        transform.SetParent(cluster.transform);

        Vector3 relativePos = transform.position - targetBall.transform.position;
        CollisionSide side = GetCollisionSide(relativePos);
        Debug.Log($"AttachTo: relativePos={relativePos}, collisionSide={side}, targetGrid=({targetBall.GirdPosition[0]}, {targetBall.GirdPosition[1]})");

        cluster.AddNewBallAtGrid(targetBall.GirdPosition[0], targetBall.GirdPosition[1], GetCollisionSide(relativePos), SpriteIndex);

        AudioManager.Instance.Play("ProjectilePop");

        ProjectileEvents.RaiseBallProjectileAttached(this);

        Destroy(gameObject);
    }
    CollisionSide GetCollisionSide(Vector3 relativePos)
    {
        float absX = Mathf.Abs(relativePos.x);
        float absY = Mathf.Abs(relativePos.y);

        // Определяем диагональное столкновение
        if (absX > 0.1f && absY > 0.1f) // порог для избежания деления на 0
        {
            if (relativePos.x > 0 && relativePos.y > 0)
                return CollisionSide.TopRight;
            else if (relativePos.x < 0 && relativePos.y > 0)
                return CollisionSide.TopLeft;
            else if (relativePos.x > 0 && relativePos.y < 0)
                return CollisionSide.BottomRight;
            else
                return CollisionSide.BottomLeft;
        }

        // Определяем прямое столкновение
        if (absX > absY)
            return relativePos.x > 0 ? CollisionSide.Right : CollisionSide.Left;
        else
            return relativePos.y > 0 ? CollisionSide.Top : CollisionSide.Bottom;
    }
    public virtual void Launch(Vector2 direction, float speed)
    {
        // Rb.velocity = Vector2.zero;
        LastDirection = direction.normalized;
        // Rb.velocity = LastDirection * speed;
        transformMover2D.Stop();
        transformMover2D.SetDirection(LastDirection, speed);
    }
}
