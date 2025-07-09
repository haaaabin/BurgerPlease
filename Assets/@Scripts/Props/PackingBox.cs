using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PackingBox : MonoBehaviour
{
    public BurgerPile pile;
    public bool IsFull => pile.ObjectCount >= PACKING_BOX_MAX_BURGER_COUNT;
}