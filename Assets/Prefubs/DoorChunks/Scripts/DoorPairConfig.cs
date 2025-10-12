using UnityEngine;

public enum BonusTypes { Addition, Difference, Multiplication, Division }

[CreateAssetMenu(menuName = "Game/Doors/Door Pair Config", fileName = "Door_")]
public class DoorPairConfig : ScriptableObject
{
    [System.Serializable]
    public struct Side { 
        public BonusTypes type; 
        public int value; 
        public string labelOverride; 
    }

    [Header("Sides")]
    public Side left;
    public Side right;

    [Header("Colors")]
    public Color bonusColor = Color.green;
    public Color penaltyColor = Color.red;

    public static bool IsBonus(BonusTypes t) =>
        t == BonusTypes.Addition || t == BonusTypes.Multiplication;

    public static string FormatLabel(BonusTypes type, int valueAbs) => type switch
    {
        BonusTypes.Addition => $"+{valueAbs}",
        BonusTypes.Difference => $"-{valueAbs}",
        BonusTypes.Multiplication => $"×{valueAbs}",
        BonusTypes.Division => $"÷{Mathf.Max(1, valueAbs)}",
        _ => valueAbs.ToString()
    };
}