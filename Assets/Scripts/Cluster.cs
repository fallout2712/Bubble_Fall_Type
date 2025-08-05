using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    private int[] Gird = new int[2]; // [X, Y]
    public GameObject prefCircle;
    private Vector2 _buildGridPos = Vector2.zero;
    public float _spaceWithGrid = 1;
    public float OffsetSide = 0.25f;
    private float _fallSpeed;
    private bool _isFall = true;
    [SerializeField] private LayerMask _ballLayer;
    [SerializeField] private float _radiusFindingBall = 1f;
    private float _startYPos;
    public int XXX;
    public int YYY;
    public CollisionSide collisionSide1;
    public void SetUpCluster(int[] grid, float spaceGird, float offsetSide)
    {
        Gird = grid;
        _spaceWithGrid = spaceGird;
        OffsetSide = offsetSide;

        _startYPos = transform.position.y;
    }
    public void BuildGrid(Vector2 poss)
    {
        _buildGridPos = poss;

        int spriteCount = prefCircle.GetComponent<Ball>().BubbleSprites.Length;

        PatternLevelGenerator generator = new PatternLevelGenerator(Gird[0], Gird[1], spriteCount);
        int[,] patternGrid = generator.GenerateGrid();

        for (int y = 0; y < Gird[1]; y++)
        {
            float offset = (y % 2 == 0) ? -OffsetSide : OffsetSide;

            for (int x = 0; x < Gird[0]; x++)
            {
                Vector2 spawnPos = _buildGridPos + new Vector2(x * _spaceWithGrid + offset, y * _spaceWithGrid);
                GameObject circle = Instantiate(prefCircle, spawnPos, Quaternion.identity, transform);

                Ball ball = circle.GetComponent<Ball>();
                ball.ApplySprite(patternGrid[x, y]);
                ball.GirdPosition = new int[] { x, y };
            }
        }
    }
    private void CheckSettings()
    {
        _isFall = GameManager.Instance.IsFall;
        _fallSpeed = GameManager.Instance.FallSpeed;
    }
    private void Update()
    {
        if (transform.position.y <= -60f)
            Destroy(gameObject);

        CheckSettings();
    }
    private void FixedUpdate()
    {
        if (_isFall)
            transform.position += Vector3.down * _fallSpeed * Time.deltaTime;

    }
    public void AddNewBallAtGrid(int gridX, int gridY, CollisionSide collisionSide, int spriteIndex)
    {
        Vector2Int newGridPos = GetNewGridPosition(gridX, gridY, collisionSide);
        Debug.Log($"Запрос на добавление шара по направлению {collisionSide}, новая позиция {newGridPos}");

        // Вместо проверки границ - просто проверяем разумные пределы
        if (newGridPos.x < -Gird[0] || newGridPos.x >= Gird[0] + Gird[0] ||
            newGridPos.y < -Gird[1] || newGridPos.y >= Gird[1] + Gird[1])
        {
            Debug.LogWarning($"Позиция {newGridPos} слишком далеко от сетки!");
            return;
        }

        if (IsBallAtPosition(newGridPos.x, newGridPos.y))
        {
            Debug.LogWarning($"Позиция {newGridPos} уже занята!");
            return;
        }

        // Вычисляем мировые координаты (позволяем отрицательные Y)
        float offset = (newGridPos.y % 2 == 0) ? -OffsetSide : OffsetSide;
        Vector2 spawnPos = _buildGridPos + new Vector2(newGridPos.x * _spaceWithGrid + offset, newGridPos.y * _spaceWithGrid);
        float currentMoveOffset = transform.position.y - _startYPos;
        spawnPos += new Vector2(0f, currentMoveOffset);

        GameObject circle = Instantiate(prefCircle, spawnPos, Quaternion.identity, transform);

        Ball ball = circle.GetComponent<Ball>();
        ball.ApplySprite(spriteIndex);
        ball.GirdPosition = new int[] { newGridPos.x, newGridPos.y };

        Debug.Log($"Шар добавлен в позицию: {newGridPos.x}, {newGridPos.y}");

        FindAndDestroyConnectedObjects(ball, spriteIndex);
    }
    private Vector2Int GetNewGridPosition(int currentX, int currentY, CollisionSide side)
    {
        Vector2Int newPos = new Vector2Int(currentX, currentY);
        bool isEvenRow = currentY % 2 == 0;

        switch (side)
        {
            case CollisionSide.Right:
                newPos.x += 1;
                break;

            case CollisionSide.Left:
                newPos.x -= 1;
                break;

            case CollisionSide.Top:
                newPos.y += 1;
                break;

            case CollisionSide.Bottom:
                newPos.y -= 1;
                break;

            case CollisionSide.TopRight:
                newPos.y += 1;
                // В шахматном порядке диагонали зависят от четности строки
                if (isEvenRow)
                    newPos.x += 0; // Для четной строки TopRight остается в том же столбце
                else
                    newPos.x += 1; // Для нечетной строки TopRight сдвигается вправо
                break;

            case CollisionSide.TopLeft:
                newPos.y += 1;
                if (isEvenRow)
                    newPos.x -= 1; // Для четной строки TopLeft сдвигается влево
                else
                    newPos.x += 0; // Для нечетной строки TopLeft остается в том же столбце
                break;

            case CollisionSide.BottomRight:
                newPos.y -= 1;
                if (isEvenRow)
                    newPos.x += 0; // Для четной строки BottomRight остается в том же столбце
                else
                    newPos.x += 1; // Для нечетной строки BottomRight сдвигается вправо
                break;

            case CollisionSide.BottomLeft:
                newPos.y -= 1;
                if (isEvenRow)
                    newPos.x -= 1; // Для четной строки BottomLeft сдвигается влево
                else
                    newPos.x += 0; // Для нечетной строки BottomLeft остается в том же столбце
                break;
        }

        return newPos;
    }

    private bool IsBallAtPosition(int gridX, int gridY)
    {
        // Проверяем, есть ли уже шар в этой позиции
        Ball[] allBalls = GetComponentsInChildren<Ball>();
        foreach (Ball ball in allBalls)
        {
            if (ball.GirdPosition != null &&
                ball.GirdPosition[0] == gridX &&
                ball.GirdPosition[1] == gridY)
            {
                return true;
            }
        }
        return false;
    }

    private void FindAndDestroyConnectedObjects(Ball currentBall, int currentBallIndex)
    {
        List<Ball> connected = new List<Ball>();
        Queue<Ball> queue = new Queue<Ball>();

        queue.Enqueue(currentBall);
        connected.Add(currentBall);

        while (queue.Count > 0)
        {
            Ball current = queue.Dequeue();
            Collider2D[] hits = Physics2D.OverlapCircleAll(current.transform.position, _radiusFindingBall, _ballLayer);

            foreach (var hit in hits)
            {
                Ball neighbor = hit.GetComponent<Ball>();
                if (neighbor != null && neighbor.SpriteIndex == currentBallIndex && !connected.Contains(neighbor))
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
}
