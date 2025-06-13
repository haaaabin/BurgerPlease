using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Define;

[RequireComponent(typeof(BoxCollider))]
public class PileBase : MonoBehaviour
{
    #region Fields
    [SerializeField]
    protected int _row = 2;

    [SerializeField]
    protected int _column = 2;

    [SerializeField]
    protected Vector3 _size = new Vector3(0.5f, 0.1f, 0.5f);

    [SerializeField]
    protected float _dropInterval = 0.05f;
    #endregion

    #region Content
    protected EObjectType _objectType = EObjectType.None;

    public void SpawnObject()
    {
        switch (_objectType)
        {
            case EObjectType.Burger:
                {
                    GameObject go = GameManager.Instance.SpawnBurger();
                    AddToPile(go, false);
                }
                break;

            case EObjectType.Money:
                {
                    GameObject go = GameManager.Instance.SpawnMoney();
                    AddToPile(go, false);
                }
                break;

            case EObjectType.Trash:
                {
                    GameObject go = GameManager.Instance.SpawnTrash();
                    AddToPile(go, false);
                }
                break;
        }
    }

    public void SpawnObjectWithJump(Vector3 spawnPos)
    {
        switch (_objectType)
        {
            case EObjectType.Burger:
                {
                    GameObject go = GameManager.Instance.SpawnBurger();
                    go.transform.position = spawnPos;
                    AddToPile(go, false);
                }
                break;

            case EObjectType.Money:
                {
                    GameObject go = GameManager.Instance.SpawnMoney();
                    go.transform.position = spawnPos;
                    AddToPile(go, false);
                }
                break;

            case EObjectType.Trash:
                {
                    GameObject go = GameManager.Instance.SpawnTrash();
                    go.transform.position = spawnPos;
                    AddToPile(go, false);
                }
                break;
        }
    }

    public void DeSpawnObject()
    {
        if (ObjectCount == 0) return;

        switch (_objectType)
        {
            case EObjectType.Burger:
                {
                    GameObject go = RemoveFromPile();
                    GameManager.Instance.DeSpawnBurger(go);
                }
                break;

            case EObjectType.Money:
                {
                    GameObject go = RemoveFromPile();
                    GameManager.Instance.DeSpawnMoney(go);
                }
                break;

            case EObjectType.Trash:
                {
                    GameObject go = RemoveFromPile();
                    GameManager.Instance.DeSpawnTrash(go);
                }
                break;
        }
    }

    public void DeSpawnObjectWithJump(Vector3 destPos, Action onDeSpawnCallback = null)
    {
        if (ObjectCount == 0) return;

        switch (_objectType)
        {
            case EObjectType.Burger:
                {
                    GameObject go = RemoveFromPile();
                    go.transform
                        .DOJump(destPos, 3, 1, 0.3f)
                        .OnComplete(() =>
                        {
                            GameManager.Instance.DeSpawnBurger(go);
                            onDeSpawnCallback?.Invoke();
                        });
                }
                break;

            case EObjectType.Money:
                {
                    GameObject go = RemoveFromPile();
                    go.transform
                        .DOJump(destPos, 3, 1, 0.3f)
                        .OnComplete(() =>
                        {
                            GameManager.Instance.DeSpawnMoney(go);
                            onDeSpawnCallback?.Invoke();
                        });
                }
                break;

            case EObjectType.Trash:
                {
                    GameObject go = RemoveFromPile();
                    go.transform
                        .DOJump(destPos, 3, 1, 0.3f)
                        .OnComplete(() =>
                        {
                            GameManager.Instance.DeSpawnTrash(go);
                            onDeSpawnCallback?.Invoke();
                        });
                }
                break;
        }
    }

    //Tray -> Pile
    public void TrayToPile(TrayController tray)
    {
        if (_objectType == EObjectType.None)
            return;
        if (tray.CurrentTrayObjectType != EObjectType.None && _objectType != tray.CurrentTrayObjectType)
            return;

        Transform t = tray.RemoveFromTray();
        if (t == null)
            return;

        t.rotation = Quaternion.identity;

        AddToPile(t.gameObject, jump: true);
    }

    public void PileToTray(TrayController tray)
    {
        if (_objectType == EObjectType.None)
            return;
        if (tray.CurrentTrayObjectType != EObjectType.None && _objectType != tray.CurrentTrayObjectType)
            return;

        GameObject go = RemoveFromPile();
        if (go == null)
            return;

        tray.AddToTray(go.transform);
    }

    #endregion

    #region Pile
    protected Stack<GameObject> _objects = new Stack<GameObject>();

    // 현재 스택에 쌓인 오브젝트의 개수를 외부에서 읽을 수 있게 하는 읽기 전용 프로퍼티
    public int ObjectCount => _objects.Count;

    private void AddToPile(GameObject go, bool jump = false)
    {
        _objects.Push(go);

        Vector3 pos = GetPositionAt(_objects.Count - 1);

        if (jump)
            go.transform.DOJump(pos, 5, 1, 0.3f);
        else
            go.transform.position = pos;
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
    #endregion

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
