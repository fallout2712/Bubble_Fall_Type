using UnityEngine;

public class Cluster : MonoBehaviour
{
    private int[] Gird = new int[2]; // [X, Y]
    public GameObject prefCircle;
    private Vector2 _buildGridPos = Vector2.zero;
    private float _spaceWithGrid = 1;
    private float _offsetSide = 0.25f;
    private float _fallSpeed;
    private bool _isFall = true;
    public void SetUpCluster(int[] grid, float spaceGird, float offsetSide)
    {
        Gird = grid;
        _spaceWithGrid = spaceGird;
        _offsetSide = offsetSide;
    }
    public void BuildGrid(Vector2 poss)
    {
        _buildGridPos = poss;

        PatternLevelGenerator generator = new PatternLevelGenerator(Gird[0], Gird[1]);
        CellColors[,] patternGrid = generator.GenerateGrid();

        for (int y = 0; y < Gird[1]; y++)
        {
            float offset = (y % 2 == 0) ? -_offsetSide : _offsetSide;

            for (int x = 0; x < Gird[0]; x++)
            {
                Vector2 spawnPos = _buildGridPos + new Vector2(x * _spaceWithGrid + offset, y * _spaceWithGrid);
                GameObject circle = Instantiate(prefCircle, spawnPos, Quaternion.identity, transform);

                Cell cell = circle.GetComponent<Cell>();
                cell.cellColor = patternGrid[x, y];
                cell.ApplyColor();
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
}
