using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DiceMode
{
    Physics,
    Animated,
    Auto
};
public enum TileMode
{
    Event,
    Build
}

[Serializable]
public class Account
{
    public CurrencyType currencyType;
    public int Amount;
}

public class GameManager : MonoBehaviour
{
    public List<Account> myAccountList;
    private static GameManager instance;
    public static GameManager Instance => instance;

    public PlayerMove move;
    public int diceNum;
    public TileNode tile;

    [field: SerializeField] public float buildSpeed { get; private set; }
    public event Action OnRefreshUI;
    public int maxBuildingCount = 3;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void UpdateAccount(Dictionary<CurrencyType, int> receipt)
    {
        foreach (var bill in receipt)
        {
            CurrencyType type = bill.Key;
            int amount = bill.Value;
            //Debug.Log($"{bill.Key} / {bill.Value}");

            foreach (var item in myAccountList)
            {
                if (item.currencyType == type)
                    item.Amount += amount;
                //Debug.Log($"{item.currencyType} / {item.Amount}");
            }
        }
        OnRefreshUI?.Invoke();
    }

    public TileMode CalcTileType()
    {
        switch (tile.tileType)
        {
            case TileType.FoodLine:
            case TileType.WoodLine:
            case TileType.StoneLine:
            case TileType.IndustryLine:
                return TileMode.Build;

            case TileType.StartZone:
            case TileType.Island:
            case TileType.Festival:
            case TileType.Travel:
            case TileType.Tax:
            default:
                Debug.Log("아직 안만듦");
                return TileMode.Event;
        }
    }
}
