using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindingWayMaze_Player : MonoBehaviour
{
    public Vector2Int CurrentVector = new Vector2Int(0, 1);

    [SerializeField] private RectTransform SelfRectTransform;
    [SerializeField] private float Speed = 300;  
    [SerializeField] private FindingWayMaze_GameController GameController;

    private List<Transform> WayCellList = new List<Transform>();
    private int IndexWayCellList = 0;
    private Transform SafeCellTransform;
    private List<Vector2Int> WayCellVec2List = new List<Vector2Int>();
    private Vector2 MoveDirectionVector;
    private bool CanMove;

    private void Update()
    {
        if (CanMove)
        {
            MoveToSafeCell();
        }
    }
    public void SetCanMove()
    {
        CanMove = true;
    }
    /// <summary>
    /// Set thông tin cần thiết để di chuyển 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="vector2Ints"></param>
    public void SetWayCellList(List<Transform> list, List<Vector2Int> vector2Ints)
    {
        WayCellList = list;
        WayCellVec2List = vector2Ints;
        IndexWayCellList = 0;
        DebugLogPos();
    }
    /// <summary>
    /// hàm di chuyển đến từng cell
    /// </summary>
    private void MoveToSafeCell()
    {
        SafeCellTransform = WayCellList[IndexWayCellList];
        MoveDirectionVector = (SafeCellTransform.localPosition - transform.localPosition).normalized;
        if ((SafeCellTransform.localPosition - transform.localPosition).magnitude <= 10)
        {
            IndexWayCellList++;
            if (IndexWayCellList >= WayCellList.Count)
            {
                StartCoroutine(DelaySetNewTarget());
                CanMove = false;
                return;
            }
            else
            {
                CurrentVector = WayCellVec2List[IndexWayCellList];
                SafeCellTransform = WayCellList[IndexWayCellList];
            }
        }
        SelfRectTransform.anchoredPosition += Speed * Time.deltaTime * MoveDirectionVector;
    }
    /// <summary>
    /// Delay 3 giây mới set target mới
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelaySetNewTarget()
    {
        yield return new WaitForSeconds(3);
        GameController.SetNewTurn();
    }
    private void DebugLogPos()
    {
        Debug.LogError("Way Pos");
        for (int i = 0; i < WayCellVec2List.Count; i++)
        {
            Debug.Log(WayCellVec2List[i]);
        }
    }
}
