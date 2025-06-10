using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector2 JoystickDir { get; set; } = Vector2.zero;

    #region Burger
    public GameObject BurgerPrefab;

    private Transform _burgerRoot;
    public Transform BurgerRoot
    {
        get
        {
            if (_burgerRoot == null)
            {
                GameObject go = new GameObject("@BurgerRoot");
                _burgerRoot = go.transform;
            }
            return _burgerRoot;
        }
    }

    public GameObject SpawnBurger()
    {
        GameObject go = Instantiate(BurgerPrefab);
        go.name = BurgerPrefab.name;
        go.transform.parent = BurgerRoot;
        return go;
    }

    public void DeSpawnBurger(GameObject burger)
    {
        Destroy(burger);
    }
    #endregion

}