using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class PackingDesk : UnlockableBase
{
    private BurgerPile _burgerPile;
    private PackingPile _packingPile;

    public DriveThruSystem Owner;

    int _spawnBurgerRemaining = 0;

    public List<WorkerController> Workers = new List<WorkerController>();

    private WorkerInteraction _burgerInteraction;
    private WorkerInteraction _packingBoxInteraction;
    private WorkerInteraction _takingBoxInteraction;
    public WorkerController CurrentBurgerWorker => _burgerInteraction.CurrentWorker;
    public WorkerController CurrentPackingBoxWorker => _packingBoxInteraction.CurrentWorker;
    public WorkerController CurrentTakingBoxWorker => _takingBoxInteraction.CurrentWorker;

    public Transform BurgerWorkerPos;
    public Transform PackingWorkerPos;
    public Transform TakingWorkerPos;

    public int BurgerCount => _burgerPile.ObjectCount;
    public bool NeedMoreBurgers => (_spawnBurgerRemaining > 0 && BurgerCount < _spawnBurgerRemaining);

    public bool IsPackingBox => (_currentBox != null && !_currentBox.IsFull);

    [SerializeField]
    private Transform _packingBoxSpawnPos;

    private PackingBox _currentBox;

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _packingPile = Utils.FindChild<PackingPile>(gameObject);

        // 햄버거 인터랙션.
        _burgerInteraction = _burgerPile.GetComponent<WorkerInteraction>();
        _burgerInteraction.InteractInterval = 0.1f;
        _burgerInteraction.OnInteraction = OnBurgerInteraction;

        // 포장 인터랙션.
        _packingBoxInteraction = _packingBoxSpawnPos.GetComponent<WorkerInteraction>();
        _packingBoxInteraction.InteractInterval = 0.1f;
        _packingBoxInteraction.OnInteraction = OnPackingInteraction;

        // 박스 가져오기 인터랙션.
        _takingBoxInteraction = _packingPile.GetComponent<WorkerInteraction>();
        _takingBoxInteraction.InteractInterval = 0.1f;
        _takingBoxInteraction.OnInteraction = OnTakingBoxInteraction;
    }

    #region Interaction
    private void OnBurgerInteraction(WorkerController wc)
    {
        _burgerPile.TrayToPile(wc.Tray);
    }

    private void OnPackingInteraction(WorkerController wc)
    {
        // 박스가 없으면 생성
        if (_currentBox == null && _burgerPile.ObjectCount >= 4)
        {
            GameObject boxGO = GameManager.Instance.SpawnPackingBox();
            _currentBox = boxGO.GetComponent<PackingBox>();
            _currentBox.transform.position = _packingBoxSpawnPos.position;
            _currentBox.transform.rotation = Quaternion.identity;
        }

        // 박스가 꽉 차지 않았으면 버거 하나 이동
        if (_currentBox != null && !_currentBox.IsFull && _burgerPile.ObjectCount > 0)
        {
            _burgerPile.PileToPile(_currentBox.pile);
        }

        // 코루틴 실행 (즉시 조건만 만족하면 바로 시작됨)
        if (_currentBox != null && _currentBox.IsFull)
        {
            StartCoroutine(DelayedMoveBox(_currentBox));
            _currentBox = null;
        }
    }

    private IEnumerator DelayedMoveBox(PackingBox box)
    {
        yield return new WaitForSeconds(0.4f); // 원하는 시간만큼 대기

        MoveBoxToPackingPile(box);
    }

    private void MoveBoxToPackingPile(PackingBox box)
    {
        Transform boxTransform = box.transform;
        box.transform.SetParent(_packingPile.transform);

        boxTransform
            .DOJump(_packingPile.transform.position, 1.5f, 1, 0.5f)
            .OnComplete(() =>
            {
                _packingPile.AddToPile(box.gameObject, jump: false);
            });
    }

    private void OnTakingBoxInteraction(WorkerController wc)
    {
        if (_packingPile.ObjectCount <= 0)
            return;

        GameObject go = _packingPile.RemoveFromPile();
        if (go == null)
            return;

        wc.Tray.AddToTray(go.transform);
    }
    #endregion
}
