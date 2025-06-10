using UnityEngine;
using UnityEngine.AI;

public class NavigationTest : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
}
