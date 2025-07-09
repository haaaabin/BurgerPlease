using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum ETutorialState
{
	None,
	CreateDoor,
	CreateFirstTable,
	CreateBurgerMachine,
	CreateCounter,
	PickupBurger,
	PutBurgerOnCounter,
	SellBurger,
	CleanTable,
	CreateSecondTable,
	CreateHROffice,
	CreateBurgerPackingDesk,
	CreateDriveThruCounter,
	PackBurgerBox,
	SellDriveThruBurger,
	CreateUpgradeOffice,

	Done,
}

public class Tutorial : MonoBehaviour
{
	[SerializeField]
	private MainCounterSystem _mainCounterSystem;
	[SerializeField]
	private DriveThruSystem _driveThruSystem;

	private RestaurantData _data;

	private ETutorialState _state
	{
		get { return _data.TutorialState; }
		set { _data.TutorialState = value; }
	}

	private float _expAmount = 1f;

	public void SetInfo(RestaurantData data)
	{
		_data = data;

		if (_state == ETutorialState.None)
			_state = ETutorialState.CreateDoor;

		StartCoroutine(CoStartTutorial());
	}

	IEnumerator CoStartTutorial()
	{
		yield return new WaitForEndOfFrame();

		RestaurantDoor door = _mainCounterSystem.Door;
		Counter counter = _mainCounterSystem.Counter;
		Grill grill = _mainCounterSystem.Grill;
		Table firstTable = _mainCounterSystem.Tables[0];
		Table secondTable = _mainCounterSystem.Tables[1];
		Office hrOffice = _mainCounterSystem.Offices[0];
		Office upgradeOffice = _mainCounterSystem.Offices[1];
		TrashCan trashCan = _mainCounterSystem.TrashCan;

		door.SetUnlockedState(EUnlockedState.Hidden);
		counter.SetUnlockedState(EUnlockedState.Hidden);
		grill.SetUnlockedState(EUnlockedState.Hidden);
		firstTable.SetUnlockedState(EUnlockedState.Hidden);
		secondTable.SetUnlockedState(EUnlockedState.Hidden);
		hrOffice.SetUnlockedState(EUnlockedState.Hidden);
		upgradeOffice.SetUnlockedState(EUnlockedState.Hidden);

		// 드라이브 스루 시스템 초기화.
		PackingDesk packingDesk = _driveThruSystem.PackingDesk;
		DriveThruCounter driveThruCounter = _driveThruSystem.DriveThruCounter;

		packingDesk.SetUnlockedState(EUnlockedState.Hidden);
		driveThruCounter.SetUnlockedState(EUnlockedState.Hidden);

		grill.StopSpawnBurger = true;


		if (_state == ETutorialState.CreateDoor)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Door");

			door.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => _mainCounterSystem.Door.IsUnlocked);

			Utils.PlayBounceEffect(door.transform);
			door.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(door.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			// starEffectFinished가 true가 될 때까지 대기
			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.CreateFirstTable;
		}

		if (_state == ETutorialState.CreateFirstTable)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create First Table");

			firstTable.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => firstTable.IsUnlocked);
			Utils.PlayBounceEffect(firstTable.transform);
			firstTable.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(firstTable.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			// starEffectFinished가 true가 될 때까지 대기
			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.CreateBurgerMachine;
		}

		firstTable.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateBurgerMachine)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create BurgerMachine");

			grill.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => grill.IsUnlocked);
			Utils.PlayBounceEffect(grill.transform);
			grill.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(grill.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.CreateCounter;
		}

		grill.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateCounter)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Counter");

			counter.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => counter.IsUnlocked);
			Utils.PlayBounceEffect(counter.transform);
			counter.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(counter.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.PickupBurger;
		}

		counter.SetUnlockedState(EUnlockedState.Unlocked);
		grill.StopSpawnBurger = false;

		if (_state == ETutorialState.PickupBurger)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Pickup Burger");

			yield return new WaitUntil(() => grill.CurrentWorker != null);
			_state = ETutorialState.PutBurgerOnCounter;
		}

		if (_state == ETutorialState.PutBurgerOnCounter)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Put Burger On Counter");

			yield return new WaitUntil(() => counter.CurrentBurgerWorker != null);
			_state = ETutorialState.SellBurger;
		}

		if (_state == ETutorialState.SellBurger)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Sell Burger");

			yield return new WaitUntil(() => firstTable.TableState == Define.ETableState.Reserved);
			_state = ETutorialState.CleanTable;
		}

		if (_state == ETutorialState.CleanTable)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("");

			// 테이블 위 쓰레기 생성 대기.
			yield return new WaitUntil(() => firstTable.TableState == Define.ETableState.Dirty);

			GameManager.Instance.GameSceneUI.SetToastMessage("Clean Table");

			// 테이블 위 쓰레기를 줍고.
			yield return new WaitUntil(() => firstTable.TableState != Define.ETableState.Dirty);

			// 쓰레기통에 버린다.
			yield return new WaitUntil(() => trashCan.CurrentWorker != null);
			_state = ETutorialState.CreateSecondTable;
		}

		if (_state == ETutorialState.CreateSecondTable)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Second Table");

			secondTable.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => secondTable.IsUnlocked);
			Utils.PlayBounceEffect(secondTable.transform);
			secondTable.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(secondTable.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.CreateHROffice;
		}
		secondTable.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateHROffice)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Office");

			hrOffice.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => hrOffice.IsUnlocked);
			Utils.PlayBounceEffect(hrOffice.transform);
			hrOffice.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(hrOffice.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.CreateBurgerPackingDesk;
		}
		hrOffice.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateBurgerPackingDesk)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Burger Packing Desk");

			packingDesk.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => packingDesk.IsUnlocked);
			Utils.PlayBounceEffect(packingDesk.transform);
			packingDesk.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(packingDesk.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.CreateDriveThruCounter;
		}
		packingDesk.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.CreateDriveThruCounter)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create DriveThru Counter");

			driveThruCounter.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => driveThruCounter.IsUnlocked);
			Utils.PlayBounceEffect(driveThruCounter.transform);
			driveThruCounter.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(driveThruCounter.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.PackBurgerBox;
		}
		driveThruCounter.SetUnlockedState(EUnlockedState.Unlocked);

		if (_state == ETutorialState.PackBurgerBox)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Pack Burger Box");

			yield return new WaitUntil(() => packingDesk.IsPackingBox);
			_state = ETutorialState.SellDriveThruBurger;
		}

		if (_state == ETutorialState.SellDriveThruBurger)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Sell DriveThru Burger");

			yield return new WaitUntil(() => driveThruCounter.IsSellBurgerBox);
			_state = ETutorialState.CreateUpgradeOffice;
		}

		if (_state == ETutorialState.CreateUpgradeOffice)
		{
			GameManager.Instance.GameSceneUI.SetToastMessage("Create Upgrade Office");

			upgradeOffice.SetUnlockedState(EUnlockedState.ProcessingConstruction);

			yield return new WaitUntil(() => upgradeOffice.IsUnlocked);
			Utils.PlayBounceEffect(upgradeOffice.transform);
			upgradeOffice.UnlockEffect.OnPlayParticleSystem();

			bool starEffectFinished = false;
			GameManager.Instance.GameSceneUI.PlayStarEffectFromWorld(upgradeOffice.transform.position, () =>
			{
				GameManager.Instance.AddExp(_expAmount);
				starEffectFinished = true;
			});

			yield return new WaitUntil(() => starEffectFinished);

			_state = ETutorialState.Done;
		}

		upgradeOffice.SetUnlockedState(EUnlockedState.Unlocked);

		GameManager.Instance.GameSceneUI.SetToastMessage("");

		yield return null;
	}

}
