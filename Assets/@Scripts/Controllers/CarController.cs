using System;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class CarController : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    private float _moveSpeed = 3;

    [SerializeField]
    protected float _rotateSpeed = 360;

    private NavMeshAgent _navMeshAgent;
    private UI_OrderBubble _orderBubble;
    private ParticleSystem _particle;

    public int CurrentDestQueueIndex;

    private ECarState _carState = ECarState.None;
    public ECarState CarState
    {
        get { return _carState; }
        set
        {
            _carState = value;
        }
    }

    public TrayController Tray { get; protected set; }

    #region OrderBubble
    public int OrderCount
    {
        set
        {
            _orderBubble.Count = value;

            if (value > 0)
            {
                _orderBubble.gameObject.SetActive(true);
            }
            else
            {
                _orderBubble.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _orderBubble = GetComponentInChildren<UI_OrderBubble>();
        Tray = Utils.FindChild<TrayController>(gameObject);
        _particle = GetComponentInChildren<ParticleSystem>();

        _navMeshAgent.speed = _moveSpeed;
        _navMeshAgent.stoppingDistance = 0.1f;
        _navMeshAgent.radius = 0.01f;
        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        Destination = transform.position;
        OrderCount = 0;

        _particle.Play();
    }

    void Update()
    {
        // 중력 작용.
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (OnArrivedAtDestCallback != null)
        {
            if (HasArrivedAtDestination)
            {
                OnArrivedAtDestCallback?.Invoke();
                OnArrivedAtDestCallback = null;
            }
        }
    }

    #region NavMeshAgent
    public Vector3 Destination
    {
        get { return _navMeshAgent.destination; }
        protected set
        {
            _navMeshAgent.SetDestination(value);
            _navMeshAgent.isStopped = false;
            LookAtDestination();
        }
    }

    public bool HasArrivedAtDestination
    {
        get
        {
            Vector3 dir = Destination - transform.position;
            return dir.sqrMagnitude < 0.2f;
        }
    }

    protected void LookAtDestination()
    {
        Vector3 moveDir = (Destination - transform.position).normalized;
        if (moveDir != Vector3.zero && moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);
        }
    }

    private Action OnArrivedAtDestCallback;

    public void SetDestination(Vector3 dest, Action onArrivedAtDest = null)
    {
        Destination = dest;
        OnArrivedAtDestCallback = onArrivedAtDest;
    }
    #endregion
}
