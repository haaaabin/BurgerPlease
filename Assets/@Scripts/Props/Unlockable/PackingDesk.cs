using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using static Define;

public class PackingDesk : UnlockableBase
{
    private BurgerPile _burgerPile;
    private PackingPile _packingPile;

    private int _currentBurgerRemaining = 0;

    public List<WorkerController> Workers = new List<WorkerController>();

    // 햄버거 포장 상호작용
    private WorkerInteraction _burgerInteraction;
    public WorkerController CurrentBurgerWorker => _burgerInteraction.CurrentWorker;

    private WorkerInteraction _packingBoxInteraction;
    public WorkerController CurrentPackingBoxWorker => _packingBoxInteraction.CurrentWorker;

    private WorkerInteraction _moveBurgerBoxInteraction;
    public WorkerController CurrentTakingBoxWorker => _moveBurgerBoxInteraction.CurrentWorker;

    // 직원 위치
    public Transform BurgerWorkerPos;
    public Transform PackingWorkerPos;
    public Transform MovePackingBoxWorkerPos;

    // 포장 데스크에 있는 햄버거 개수
    public int BurgerCount => _burgerPile.ObjectCount;

    // 포장 데스크에 있는 햄버거 개수가 포장 박스 최대 개수보다 적은지
    public bool NeedMoreBurgers => (BurgerCount < Define.PACKING_BOX_MAX_BURGER_COUNT);

    public bool IsPackingBox => (_currentBox != null && !_currentBox.IsFull);

    // 포장 완료된 박스 개수
    public int PackingCount => _packingPile.ObjectCount;

    [SerializeField]
    private Transform _packingBoxSpawnPos;

    private PackingBox _currentBox;

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _packingPile = Utils.FindChild<PackingPile>(gameObject);

        // 햄버거 인터랙션
        _burgerInteraction = _burgerPile.GetComponent<WorkerInteraction>();
        _burgerInteraction.InteractInterval = 0.1f;
        _burgerInteraction.OnInteraction = OnBurgerInteraction;

        // 포장 인터랙션
        _packingBoxInteraction = _packingBoxSpawnPos.GetComponent<WorkerInteraction>();
        _packingBoxInteraction.InteractInterval = 0.1f;
        _packingBoxInteraction.OnInteraction = OnPackingInteraction;

        // 박스 가져오기 인터랙션
        _moveBurgerBoxInteraction = _packingPile.GetComponent<WorkerInteraction>();
        _moveBurgerBoxInteraction.InteractInterval = 0.1f;
        _moveBurgerBoxInteraction.OnInteraction = OnTakingBoxInteraction;
    }

    #region Interaction
    // 햄버거를 포장 pile로 옮기는 상호작용
    private void OnBurgerInteraction(WorkerController wc)
    {
        _burgerPile.TrayToPile(wc.Tray);
    }

    // 포장 작업 상호작용
    private void OnPackingInteraction(WorkerController wc)
    {
        // 박스가 없으면 생성
        if (_currentBox == null && _burgerPile.ObjectCount >= Define.PACKING_BOX_MAX_BURGER_COUNT)
        {
            GameObject boxGO = GameManager.Instance.SpawnPackingBox();
            _currentBox = boxGO.GetComponent<PackingBox>();
            _currentBox.transform.position = _packingBoxSpawnPos.position;
            _currentBox.transform.rotation = Quaternion.identity;
        }

        // 박스가 꽉 차지 않았으면 버거 이동
        if (_currentBox != null && !_currentBox.IsFull && _burgerPile.ObjectCount > 0)
        {
            _burgerPile.PileToPile(_currentBox.pile);
        }

        // 박스가 다 차면 박스 이동 코루틴 실행
        if (_currentBox != null && _currentBox.IsFull)
        {
            StartCoroutine(DelayedMoveBox(_currentBox));
            _currentBox = null;
        }
    }

    // 박스가 다 찼을 때 잠시 후 포장 pile로 이동
    private IEnumerator DelayedMoveBox(PackingBox box)
    {
        yield return new WaitForSeconds(0.7f);
        MoveBoxToPackingPile(box);
    }

    // 박스를 포장 pile로 점프 이동
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

    // 포장된 박스를 직원이 가져가는 상호작용
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

    // 드라이브스루에서 햄버거가 필요할 때 호출
    public void RequestBurger(int count)
    {
        _currentBurgerRemaining = count;
    }
}
