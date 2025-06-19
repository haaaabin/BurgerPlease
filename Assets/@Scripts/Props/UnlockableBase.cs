using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public enum EUnlockedState
{
    Hidden,
    ProcessingConstruction,
    Unlocked
}

public class UnlockableBase : MonoBehaviour
{
    public UI_ConstructionArea ConstructionArea;
    private UnlockableStateData _data;

    public void SetInfo(UnlockableStateData data)
    {
        _data = data;
        SetUnlockedState(data.State);
        ConstructionArea.RefreshUI();
    }

    private EUnlockedState State
    {
        get { return _data.State; }
        set { _data.State = value; }
    }

    public bool IsUnlocked => State == EUnlockedState.Unlocked;
    public long SpentMoney
    {
        get { return _data != null ? _data.SpentMoney : 0; }
        set { if (_data != null) { _data.SpentMoney = value; } }
    }

    public void SetUnlockedState(EUnlockedState state)
    {
        State = state;

        if (state == EUnlockedState.Hidden)
        {
            gameObject.SetActive(false);
            ConstructionArea.gameObject.SetActive(false);
        }
        else if (state == EUnlockedState.ProcessingConstruction)
        {
            gameObject.SetActive(false);
            ConstructionArea.gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
            ConstructionArea.gameObject.SetActive(false);
        }
    }
}