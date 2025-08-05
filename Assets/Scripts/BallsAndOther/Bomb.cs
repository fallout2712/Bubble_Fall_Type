using UnityEngine;

public class Bomb : ProjectileBehavior
{
    public float ExplosionRadius = 4f;
    protected override void Awake()
    {
        if (transformMover2D == null)
            transformMover2D = gameObject.AddComponent<TransformMover2D>();
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
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

        Explosion();
    }

    private void Explosion()
    {
        if (AlreadyAttached) return;
        AlreadyAttached = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

        foreach (var hit in hits)
        {
            Ball ball = hit.GetComponent<Ball>();
            if (ball != null && ball.HasBall)
            {
                ball.AddBallPopEffect();
            }
        }

        // Можно добавить эффект взрыва, звук и затем уничтожить сам объект бомбы
        AudioManager.Instance.Play("Explode"); // если есть звук

        GameObject fx = Resources.Load<GameObject>("ExplodeEffect");
        if (fx != null)
        {
            Instantiate(fx, transform.position, Quaternion.identity);
        }

        ProjectileEvents.RaiseBallProjectileAttached(this);

        Destroy(gameObject);
    }

}
