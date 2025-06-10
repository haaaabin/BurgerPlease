using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + dir);
    }
}
