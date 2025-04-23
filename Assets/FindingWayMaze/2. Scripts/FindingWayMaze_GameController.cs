using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindingWayMaze_GameController : MonoBehaviour
{
    [SerializeField] private Color WayColor;
    [SerializeField] private Color WayedColor;
    [SerializeField] private Color WallColor;
    [SerializeField] private Color TargetColor;

    [SerializeField] private FindingWayMaze_Player Player;

    [SerializeField] private List<Image> CellList;

    [SerializeField] private int[,] LayoutTemp;

    Dictionary<int, int[,]> RandomMapDict = new Dictionary<int, int[,]>()
    {
        {0, new int[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 }
            }
        },
        {1, new int[,]
            {
                { 2, 0, 0, 0, 2 },
                { 0, 0, 0, 0, 0 },
                { 2, 0, 0, 0, 2 },
                { 0, 0, 2, 0, 0 },
                { 2, 0, 0, 0, 2 },
                { 0, 0, 2, 0, 0 },
                { 2, 0, 0, 0, 2 }
            }
        },
        {2, new int[,]
            {
                { 2, 0, 0, 0, 2 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 2, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 2, 0, 0, 0, 2 },
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 }
            }
        },
        {3, new int[,]
            {
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 2, 0, 0 },
                { 2, 0, 2, 0, 2 },
                { 0, 0, 2, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 }
            }
        },
        {4, new int[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 2, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 2, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 }
            }
        },
        {5, new int[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 },
                { 2, 0, 2, 0, 2 },
                { 0, 0, 0, 0, 0 },
                { 0, 2, 0, 2, 0 },
                { 0, 0, 0, 0, 0 }
            }
        },
    };

    private int RandomLayoutIndex = 0;

    private List<Vector2Int> WayListTemp = new List<Vector2Int>();

    private int Row = 7;
    private int Col = 5;

    private void Start()
    {
        SetLayout();
        Player.SetCanMove();
    }

    /// <summary>
    /// Set layout lần đầu 
    /// </summary>
    private void SetLayout()
    {
        RandomLayoutIndex = Random.Range(0, RandomMapDict.Count);

        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                CellList[i * Col + j].color = WayColor;
                if (RandomMapDict[RandomLayoutIndex][i, j] == 2)
                {
                    CellList[i * Col + j].color = WallColor;
                }
                else
                {
                    WayListTemp.Add(new Vector2Int(i, j));
                }
            }
        }
        SetRandomTarget();
    }

    /// <summary>
    /// Hàm lưu lại layout trước khi sửa
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void SetLayoutTemp()
    {
        LayoutTemp = new int[Row, Col];
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                LayoutTemp[i, j] = RandomMapDict[RandomLayoutIndex][i, j];
            }
        }
    }
    /// <summary>
    /// set random tartget từ list các cell k phải tường
    /// </summary>
    private void SetRandomTarget()
    {
        int ran = Random.Range(0, WayListTemp.Count);
        SetLayoutTemp();
        Vector2Int targetPos = new Vector2Int(WayListTemp[ran].x, WayListTemp[ran].y);
        LayoutTemp[targetPos.x, targetPos.y] = 3;
        CellList[targetPos.x * Col + targetPos.y].color = TargetColor;
        FindPathBot(LayoutTemp);
    }
    /// <summary>
    /// Set lại đường đi khi player đến đích
    /// </summary>
    public void SetNewTurn()
    {
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                CellList[i * Col + j].color = WayColor;
                if (RandomMapDict[RandomLayoutIndex][i, j] == 2)
                {
                    CellList[i * Col + j].color = WallColor;
                }
            }
        }
        SetRandomTarget();
        Player.SetCanMove();
    }

    private void FindPathBot(int[,] layout)
    {
        // Tìm đường đi
        List<Vector2Int> path = new List<Vector2Int>();
        path = FindClosestZero(layout, Player.CurrentVector);

        List<Transform> cellList = new List<Transform>();
        if (path != null)
        {
            foreach (Vector2Int step in path)
            {
                cellList.Add(CellList[step.x * Col + step.y].transform);
            }
        }
        Player.SetWayCellList(cellList, path);
    }
    static List<Vector2Int> FindClosestZero(int[,] matrix, Vector2Int start)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        // Kiểm tra tính hợp lệ của bước di chuyển
        bool IsValid(int x, int y, bool[,] visited)
        {
            return x >= 0 && x < rows && y >= 0 && y < cols &&
                   matrix[x, y] != 2 && !visited[x, y];
        }

        // Sử dụng BFS để tìm giá trị 0 gần nhất
        Queue<(int, int, List<Vector2Int>)> queue = new Queue<(int, int, List<Vector2Int>)>();
        bool[,] visited = new bool[rows, cols];

        // Thêm vị trí xuất phát vào hàng đợi
        queue.Enqueue(((int)start.x, (int)start.y, new List<Vector2Int> { start }));
        visited[(int)start.x, (int)start.y] = true;

        // Các hướng di chuyển (lên, xuống, trái, phải)
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            var (x, y, path) = queue.Dequeue();

            // Nếu gặp giá trị 0, trả về đường đi
            if (matrix[x, y] == 3)
            {
                return path;
            }

            // Duyệt các hướng di chuyển
            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsValid(newX, newY, visited))
                {
                    visited[newX, newY] = true;
                    var newPath = new List<Vector2Int>(path) { new Vector2Int(newX, newY) };
                    queue.Enqueue((newX, newY, newPath));
                }
            }
        }

        // Không tìm được vị trí có giá trị 0
        return null;
    }
}
