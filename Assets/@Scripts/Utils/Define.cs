using UnityEngine;

public static class Define
{
    public enum EState
    {
        None,
        Idle,
        Move,
    }

    public enum ETrayObject
    {
        None,
        Trash,
        Burger,
    }

    public const int GRILL_MAX_BURGER_COUNT = 20;
    public const float GRILL_SPAWN_BURGER_INTERVAL = 1f; 

    public static int IDLE = Animator.StringToHash("Idle");
    public static int MOVE = Animator.StringToHash("Move");
    public static int SERVING_IDLE = Animator.StringToHash("ServingIdle");
    public static int SERVING_MOVE = Animator.StringToHash("ServingMove");
}
