using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Define;

// 1. navmeshagent로 이동하게 만들어야 해
// 2. 목적지를 정해야해

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class GuestController : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    private float _moveSpeed = 3;

    [SerializeField]
    private float _rotateSpeed = 360;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private UI_OrderBubble _orderBubble;
    public TrayController Tray { get; private set; }

    private EState _state = EState.None;
    public EState State
    {
        get { return _state; }
        private set
        {
            if (_state == value)
                return;
            _state = value;
            UpdateAnimation();
        }
    }
    public bool IsServing => Tray.Visible;

    public int CurrentDestQueueIndex;  //내가 현재 위치해 있는 인덱스 

    // 목적지 설정 속성
    public Vector3 Destination
    {
        get { return _navMeshAgent.destination; }
        set
        {
            _navMeshAgent.SetDestination(value);
            _navMeshAgent.isStopped = false;
            LookAtDestination();
        }
    }

    private EGuestState _guestState = EGuestState.None;
    public EGuestState GuestState
    {
        get { return _guestState; }
        set
        {
            _guestState = value;
            UpdateAnimation();
        }
    }

    // 목적지에 도착했는지 여부를 확인하는 속성
    public bool HasArrivedAtDestination
    {
        get
        {
            Vector3 dir = Destination - transform.position;
            return dir.magnitude < 0.2f;
        }
    }

    public int OrderCount
    {
        set
        {
            _orderBubble.Count = value;

            if (value > 0)
                _orderBubble.gameObject.SetActive(true);
            else
                _orderBubble.gameObject.SetActive(false);
        }
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _orderBubble = Utils.FindChild<UI_OrderBubble>(gameObject);
        Tray = Utils.FindChild<TrayController>(gameObject);

        _navMeshAgent.speed = _moveSpeed;
        _navMeshAgent.stoppingDistance = 0.05f;
        _navMeshAgent.radius = 0.1f;
        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        Destination = transform.position;

        OrderCount = 0;
    }

    void Update()
    {
        if (HasArrivedAtDestination)
        {
            _navMeshAgent.isStopped = true;
            State = EState.Idle;
        }
        else
        {
            State = EState.Move;
            LookAtDestination();
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void LookAtDestination()
    {
        Vector3 moveDir = (Destination - transform.position).normalized;
        if (moveDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);
        }
    }

    int _lastAnim = -1;

    public void UpdateAnimation()
    {
        int nextAnim = -1;

        switch (State)
        {
            case EState.Idle:
                nextAnim = IsServing ? Define.SERVING_IDLE : Define.IDLE;
                // _animator.CrossFade(IsServing ? Define.SERVING_IDLE : Define.IDLE, 0.1f);
                break;
            case EState.Move:
                nextAnim = IsServing ? Define.SERVING_MOVE : Define.MOVE;
                // _animator.CrossFade(IsServing ? Define.SERVING_MOVE : Define.MOVE, 0.1f);
                break;
        }

        switch (GuestState)
        {
            case EGuestState.Eating:
                nextAnim = Define.EATING;
                break;
        }

        if (_lastAnim == nextAnim)
            return;

        _animator.CrossFade(nextAnim, 0.01f);
        _lastAnim = nextAnim;
    }
}
