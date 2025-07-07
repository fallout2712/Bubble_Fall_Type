using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDetector : MonoBehaviour
{
    public float radius = 1f; // Чуть больше, чем шаг сетки
    public LayerMask CellLayer;
    public void DetectFloatingCells()
    {
        Cell[] allCells = FindObjectsOfType<Cell>(); // СРОЧНО!!!!! ПЕРЕДАЛАТЬ

        // Список клеток, которые связаны с верхом
        List<Cell> connectedToTop = new List<Cell>();

        // Очередь для BFS (обхода в ширину)
        Queue<Cell> queue = new Queue<Cell>();

        //Найти самую верхнюю координату Y среди всех клеток с HasBall = true
        float topY = float.MinValue;
        foreach (var c in allCells)
        {
            if (!c.HasBall) continue; //
            if (c.transform.position.y > topY)
                topY = c.transform.position.y; // максимум по Y среди приклеенных
        }

        // очередь
        foreach (var c in allCells)
        {
            if (!c.HasBall) continue;
            if (Mathf.Abs(c.transform.position.y - topY) < 0.1f)
            {
                connectedToTop.Add(c); // Эта клетка считается связанной с верхом
                queue.Enqueue(c);      // в очередь для обхода
            }
        }

        // BFS все клетки с HasBall = true, которые соединены с верхними клетками
        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();

            Collider2D[] neighbors = Physics2D.OverlapCircleAll(current.transform.position, radius, CellLayer);

            foreach (var hit in neighbors)
            {
                Cell neighbor = hit.GetComponent<Cell>();
                if (neighbor != null && neighbor.HasBall && !connectedToTop.Contains(neighbor))
                {
                    connectedToTop.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Удаление
        int floatingCount = 0;
        foreach (var c in allCells)
        {
            if (!c.HasBall) continue;

            if (!connectedToTop.Contains(c))
            {
                c.SetFallingCell();
                UIController.Instance.AddScore(10);
                floatingCount++;
            }
        }
        if (floatingCount > 0)
        {
            AudioManager.Instance.Play("DropWhoosh");
        }
        Debug.Log("Удалено висячих объектов: " + (allCells.Length - connectedToTop.Count));
    }
    public void ClearAllBalls()
    {
        Cell[] allCells = FindObjectsOfType<Cell>();

        int removedCount = 0;

        foreach (var cell in allCells)
        {

            cell.SetFallingCell();
            removedCount++;
        }

        Debug.Log("Удалено всех шаров: " + removedCount);

        AudioManager.Instance.Play("Fail");
    }

}
