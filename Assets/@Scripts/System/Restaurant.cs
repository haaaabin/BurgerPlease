using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Restaurant : MonoBehaviour
{
    public List<SystemBase> RestaurantSystems = new List<SystemBase>();

    public int StageNum = 0;
    public List<UnlockableBase> Props = new List<UnlockableBase>();
    public List<WorkerController> Workers = new List<WorkerController>();

    private RestaurantData _data;

    private void OnEnable()
    {
        GameManager.Instance.AddEventListener(EEventType.HireWorker, OnHireWorker);
        StartCoroutine(CoDistributeWorkerAI());
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveEventListener(EEventType.HireWorker, OnHireWorker);
    }

    public void SetInfo(RestaurantData data)
    {
        _data = data;

        RestaurantSystems = GetComponentsInChildren<SystemBase>().ToList();
        Props = GetComponentsInChildren<UnlockableBase>().ToList();
        if (RestaurantSystems == null || Props == null || _data == null)
            Debug.Log("null");
        for (int i = 0; i < Props.Count; i++)
        {
            UnlockableStateData stateData = data.UnlockableStates[i];
            Props[i].SetInfo(stateData);
        }

        Tutorial tutorial = GetComponent<Tutorial>();
        if (tutorial != null)
            tutorial.SetInfo(data);

        for (int i = 0; i < data.WorkerCount; i++)
            OnHireWorker();
    }

    private void OnHireWorker()
    {
        GameObject go = GameManager.Instance.SpawnWorker();
        WorkerController wc = go.GetComponent<WorkerController>();
        go.transform.position = Vector3.zero;

        // 나중에는 직원 배치를 여러 시스템(MainCounter, Drive-Thru 등) 중 하나를 골라서 한다.
        Workers.Add(wc);

        _data.WorkerCount = Mathf.Max(_data.WorkerCount, Workers.Count);
    }

    // 일감 분배
    private IEnumerator CoDistributeWorkerAI()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            yield return new WaitUntil(() => Workers.Count > 0);

            foreach (WorkerController worker in Workers)
            {
                // 어딘가에 소속되어 있으면 스킵
                if (worker.CurrentSystem != null)
                    continue;

                // 어떤 시스템에 일감이 남아 있으면, 해당 시스템으로 배정
                foreach (SystemBase system in RestaurantSystems)
                {
                    if (system.HasJob)
                        system.AddWorker(worker);
                }
            }
        }
    }

}