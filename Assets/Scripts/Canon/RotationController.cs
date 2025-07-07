using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float MaxRotationAngle = 60f;

    [Header("Trajectory Prediction")]
    [SerializeField] private LineRenderer _trajectoryLine;
    [SerializeField] private float _projectileRadius = 0.5f;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _blockLayer;

    private bool _isAiming = false;

    public void StartAiming(Vector2 screenPos)
    {
        _isAiming = true;
        AimTo(screenPos);
    }

    public void ContinueAiming(Vector2 screenPos)
    {
        if (!_isAiming) return;
        AimTo(screenPos);
    }

    public void ReleaseAiming(Vector2 screenPos)
    {
        if (!_isAiming) return;
        _isAiming = false;

        GameManager.Instance.ShootProjectile();
        AudioManager.Instance.Play("WhooshMotion");
    }

    private void AimTo(Vector2 screenPos)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 direction = (worldPoint - transform.position).normalized;
        float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, -MaxRotationAngle, MaxRotationAngle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GetGameState() == GameState.Playing)
            ShowTrajectory();
    }

    private void ShowTrajectory()
    {
        if (!GameManager.Instance.CanShoot)
        {
            _trajectoryLine.positionCount = 0;
            return;
        }

        Vector2 position = transform.position;
        Vector2 direction = GetAimDirection();
        float remainingLength = 100f;
        List<Vector3> points = new List<Vector3> { position };

        int maxBounces = 10;
        int bounces = 0;

        while (remainingLength > 0f && bounces <= maxBounces) //Отскоки, немного пофиксить радиус на шариках
        {
            RaycastHit2D hit = Physics2D.Raycast(position, direction, remainingLength, _wallLayer | _blockLayer);
            if (hit.collider != null)
            {
                float distance = Vector2.Distance(position, hit.point);
                remainingLength -= distance;

                bool isWall = ((1 << hit.collider.gameObject.layer) & _wallLayer) != 0;
                Vector2 adjustedHitPoint = isWall ? hit.point + hit.normal * _projectileRadius : hit.point;
                points.Add(adjustedHitPoint);

                if (((1 << hit.collider.gameObject.layer) & _blockLayer) != 0)
                    break;

                direction = Vector2.Reflect(direction, hit.normal);
                position = adjustedHitPoint + direction * 0.01f;
                bounces++;
            }
            else
            {
                Vector2 nextPoint = position + direction * remainingLength;
                points.Add(nextPoint);
                break;
            }
        }

        _trajectoryLine.positionCount = points.Count;
        _trajectoryLine.SetPositions(points.ToArray());
    }

    public Vector2 GetAimDirection()
    {
        float z = transform.eulerAngles.z;
        return new Vector2(-Mathf.Sin(z * Mathf.Deg2Rad), Mathf.Cos(z * Mathf.Deg2Rad)).normalized;
    }
    public void ClearTrajectory()
    {
        _trajectoryLine.positionCount = 0;
    }
}
