using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation =  Camera.main.transform.rotation;
    }
}
