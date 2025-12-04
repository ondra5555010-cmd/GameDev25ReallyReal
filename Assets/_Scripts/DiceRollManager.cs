using UnityEngine;

public class DiceRollManager : MonoBehaviour
{
    public static DiceRollManager Instance;

    public enum RollMode
    {
        Normal,
        Advantage,
        Disadvantage
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int Roll(int sides, int count = 1, RollMode mode = RollMode.Normal)
    {
        if (sides < 2 || count < 1) return 0;

        int RollSingle()
        {
            int total = 0;
            for (int i = 0; i < count; i++)
                total += Random.Range(1, sides + 1);
            return total;
        }

        int a = RollSingle();
        if (mode == RollMode.Normal) return a;

        int b = RollSingle();

        if (mode == RollMode.Advantage) return Mathf.Max(a, b);
        if (mode == RollMode.Disadvantage) return Mathf.Min(a, b);

        return a;
    }
}