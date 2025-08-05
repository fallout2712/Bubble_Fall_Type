using UnityEngine;

public class TransformMover2D : MonoBehaviour
{
    private float _moveSpeed = 5f;

    private Vector2 _moveDirection = Vector2.zero;

    public void SetDirection(Vector2 direction, float moveSpeeed)
    {
        _moveSpeed = moveSpeeed;
        _moveDirection = direction.normalized;
    }
    public void Stop()
    {
        _moveDirection = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero)
        {
            transform.position += (Vector3)(_moveDirection.normalized * _moveSpeed * Time.deltaTime);
        }
    }
}
