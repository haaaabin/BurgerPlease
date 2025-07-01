using UnityEngine;

public class PlayerData
{
    public int Level = 1;
    public float CurrentExp = 0f;

    public float GetMaxExp()
    {
        return 50 + (Level - 1) * 50;
    }

    public bool AddExp(float amount)
    {
        CurrentExp += amount;
        bool leveledUp = false;

        while (CurrentExp >= GetMaxExp())
        {
            CurrentExp -= GetMaxExp();
            Level++;
            leveledUp = true;
        }

        return leveledUp;
    }
}