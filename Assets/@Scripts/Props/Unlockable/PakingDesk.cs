using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class PakingDesk : UnlockableBase
{
    private BurgerPile _burgerPile;
    private PakingPile _pakingPile;

    public DriveThruSystem Owner;

    int _spawnBurgerRemaining = 0;

    public List<WorkerController> Workers = new List<WorkerController>();

    private WorkerInteraction _burgerInteraction;
    private WorkerInteraction _pakingBoxInteraction;
    private WorkerInteraction _takingBoxInteraction;
    public WorkerController CurrentBurgerWorker => _burgerInteraction.CurrentWorker;
    public WorkerController CurrentPakingBoxWorker => _pakingBoxInteraction.CurrentWorker;
    public WorkerController CurrentTakingBoxWorker => _takingBoxInteraction.CurrentWorker;

    public Transform BurgerWorkerPos;
    public Transform PakingWorkerPos;
    public Transform TakingWorkerPos;

    public int BurgerCount => _burgerPile.ObjectCount;
    public bool NeedMoreBurgers => (_spawnBurgerRemaining > 0 && BurgerCount < _spawnBurgerRemaining);

    [SerializeField]
    private Transform _pakingBoxSpawnPos;

    private PakingBox _currentBox;

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _pakingPile = Utils.FindChild<PakingPile>(gameObject);

        // 햄버거 인터랙션.
        _burgerInteraction = _burgerPile.GetComponent<WorkerInteraction>();
        _burgerInteraction.InteractInterval = 0.1f;
        _burgerInteraction.OnInteraction = OnBurgerInteraction;

        // 포장 인터랙션.
        _pakingBoxInteraction = _pakingBoxSpawnPos.GetComponent<WorkerInteraction>();
        _pakingBoxInteraction.InteractInterval = 0.1f;
        _pakingBoxInteraction.OnInteraction = OnPakingInteraction;

        // 박스 가져오기 인터랙션.
        _takingBoxInteraction = _pakingPile.GetComponent<WorkerInteraction>();
        _takingBoxInteraction.InteractInterval = 0.1f;
        _takingBoxInteraction.OnInteraction = OnTakingBoxInteraction;
    }

    #region Interaction
    private void OnBurgerInteraction(WorkerController wc)
    {
        _burgerPile.TrayToPile(wc.Tray);
    }
    private void OnPakingInteraction(WorkerController wc)
    {
        // 박스가 없으면 생성
        if (_currentBox == null && _burgerPile.ObjectCount >= 4)
        {
            GameObject boxGO = GameManager.Instance.SpawnPakingBox();
            _currentBox = boxGO.GetComponent<PakingBox>();
            _currentBox.transform.position = _pakingBoxSpawnPos.position;
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

    private IEnumerator DelayedMoveBox(PakingBox box)
    {
        yield return new WaitForSeconds(0.4f); // 원하는 시간만큼 대기

        MoveBoxToPakingPile(box);
    }

    private void MoveBoxToPakingPile(PakingBox box)
    {
        Transform boxTransform = box.transform;
        box.transform.SetParent(_pakingPile.transform);

        boxTransform
            .DOJump(_pakingPile.transform.position, 1.5f, 1, 0.5f)
            .OnComplete(() =>
            {
                _pakingPile.AddToPile(box.gameObject, jump: false);
            });
    }

    private void OnTakingBoxInteraction(WorkerController wc)
    {
        if (_pakingPile.ObjectCount <= 0)
            return;

        GameObject go = _pakingPile.RemoveFromPile();
        if (go == null)
            return;

        wc.Tray.AddToTray(go.transform);
    }
    #endregion
}
