using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UpgradeData clickUpgrade;
    public UpgradeData autoUpgrade;
    public Text scoreText, rateText, clickLabel, autoLabel;
    public Button clickBuyButton, autoBuyButton;
    private int points;
    private int clickLevel;
    private int autoLevel;
    private float timer;

    void Start() { RefreshUI(); }
    void Update()
    {
        timer += Time.deltaTime;
        while (timer >= 1f)
        {
            timer -= 1f;
            AddPoints(autoLevel * autoUpgrade.pointsPerLevel);
        }
    }
    public void ClickCookie() { AddPoints(1 + clickLevel * clickUpgrade.pointsPerLevel); }
    public void AddPoints(int amount) { points += amount; RefreshUI(); }
    public void BuyClickUpgrade()
    {
        int cost = clickUpgrade.GetCost(clickLevel);
        if (points < cost) return;
        points -= cost;
        clickLevel++;
        RefreshUI();
    }
    public void BuyAutoUpgrade()
    {
        int cost = autoUpgrade.GetCost(autoLevel);
        if (points < cost) return;
        points -= cost;
        autoLevel++;
        RefreshUI();
    }
    void RefreshUI()
    {
        scoreText.text = "Cookies: " + points.ToString("N0");
        rateText.text = (1 + clickLevel * clickUpgrade.pointsPerLevel) + " per click  |  " + (autoLevel * autoUpgrade.pointsPerLevel) + " per second";
        clickLabel.text = clickUpgrade.upgradeName + " - Level " + clickLevel + "\n+" + clickUpgrade.pointsPerLevel + " per click\nCost: " + clickUpgrade.GetCost(clickLevel);
        autoLabel.text = autoUpgrade.upgradeName + " - Level " + autoLevel + "\n+" + autoUpgrade.pointsPerLevel + " per second\nCost: " + autoUpgrade.GetCost(autoLevel);
        clickBuyButton.interactable = points >= clickUpgrade.GetCost(clickLevel);
        autoBuyButton.interactable = points >= autoUpgrade.GetCost(autoLevel);
    }
}
