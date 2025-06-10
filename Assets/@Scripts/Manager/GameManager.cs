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

    #region
    public GameObject MoneyPrefab;

    private Transform _moneyRoot;
    public Transform MoneyRoot
    {
        get
        {
            if (_moneyRoot == null)
            {
                GameObject go = new GameObject("@MoneyRoot");
                _moneyRoot = go.transform;
            }
            return _moneyRoot;
        }
    }

    public GameObject SpawnMoney()
    {
        GameObject go = Instantiate(MoneyPrefab);
        go.name = MoneyPrefab.name;
        go.transform.parent = MoneyRoot;
        return go;
    }

    public void DeSpawnMoney(GameObject money)
    {
        Destroy(money);
    }
    #endregion
}