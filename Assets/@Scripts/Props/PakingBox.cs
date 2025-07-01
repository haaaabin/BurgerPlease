using Unity.VisualScripting;
using UnityEngine;

public class PakingBox : MonoBehaviour
{
    public BurgerPile pile;
    private const int MAX_BURGER_COUNT = 4;
    public bool IsFull => pile.ObjectCount >= MAX_BURGER_COUNT;

}