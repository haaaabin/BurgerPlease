using System.Collections;
using UnityEngine;

// 1. 패티 애니메이션
// 2. 햄버거 주기적으로 생성 -> 코루틴
// 3. [Collider] 길찾기 막기
// 4. Burger Pile 
// 5. [Trigger] 햄버거 영역 안으로 들어오면 플레이어가 갖고 감
public class Grill : MonoBehaviour
{
    private BurgerPile _burgers;

    void Start()
    {
        _burgers = Utils.FindChild<BurgerPile>(gameObject);

        PlayerInteraction interaction = _burgers.GetComponent<PlayerInteraction>();
        interaction.InteractInterval = 0.2f;
        interaction.OnPlayerInteraction = OnPlayerBurgerInteraction;

        StartCoroutine(CoSpawnBurgers());
    }

    IEnumerator CoSpawnBurgers()
    {
        while (true)
        {
            yield return new WaitUntil(() => _burgers.ObjectCount < Define.GRILL_MAX_BURGER_COUNT);

            GameObject go = GameManager.Instance.SpawnBurger();
            _burgers.AddToPile(go);

            yield return new WaitForSeconds(Define.GRILL_SPAWN_BURGER_INTERVAL);
        }
    }

    private void OnPlayerBurgerInteraction(PlayerController pc)
    {
        GameObject go = _burgers.RemoveFromPile();
        if (go == null)
            return;

        pc.Tray.AddToTray(go.transform);
    }
}
