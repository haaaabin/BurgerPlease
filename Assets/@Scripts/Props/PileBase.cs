using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileBase : MonoBehaviour
{
    [SerializeField]
    private int _row = 2;

    [SerializeField]
    private int _column = 2;

    [SerializeField]
    private Vector3 _size = new Vector3(0.5f, 0.1f, 0.5f);

    [SerializeField]
    private float _dropInterval = 0.05f;

    protected Stack<GameObject> _objects = new Stack<GameObject>();

    // 현재 스택에 쌓인 오브젝트의 개수를 외부에서 읽을 수 있게 하는 읽기 전용 프로퍼티
    public int ObjectCount => _objects.Count;

    public void AddToPile(GameObject go)
    {
        _objects.Push(go);

        go.transform.position = GetPositionAt(_objects.Count - 1);
    }

    public GameObject RemoveFromPile()
    {
        if (_objects.Count == 0)
            return null;

        return _objects.Pop();
    }

    private Vector3 GetPositionAt(int pileIndex)
    {
        // 1.기준 위치 계산
        Vector3 offset = new Vector3((_row - 1) * _size.x / 2, 0, (_column - 1) * _size.z / 2);
        Vector3 startPos = transform.position - offset;

        // 2. pileIndex를 기반으로 행, 열, 높이 계산
        int row = (pileIndex / _row) % _column;
        int column = pileIndex % _row;
        int height = pileIndex / (_row * _column);

        // 3. 위치 좌표 계산
        float x = startPos.x + column * _size.x;
        float y = startPos.y + height * _size.y;
        float z = startPos.z + row * _size.z;

        return new Vector3(x, y, z);
    }

    #region Editor
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 offset = new Vector3((_row - 1) * _size.x / 2, 0, (_column - 1) * _size.z / 2);
        Vector3 startPos = transform.position - offset;

        Gizmos.color = Color.yellow;

        for (int r = 0; r < _row; r++)
        {
            for (int c = 0; c < _column; c++)
            {
                Vector3 center = startPos + new Vector3(r * _size.x, _size.y / 2, c * _size.z);
                Gizmos.DrawWireCube(center, _size);
            }
        }
    }
#endif
    #endregion
}
