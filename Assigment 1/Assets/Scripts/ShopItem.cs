using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public GameManager game;
    public int points = 1000;
    public void Buy() { game.AddPoints(points); }
}
