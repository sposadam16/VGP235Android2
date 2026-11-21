using UnityEngine;

[CreateAssetMenu(menuName = "Cookie Clicker/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public int baseCost = 10;
    public int pointsPerLevel = 1;
    public float costMultiplier = 1.5f;
    public int GetCost(int level) { return Mathf.CeilToInt(baseCost * Mathf.Pow(costMultiplier, level)); }
}
