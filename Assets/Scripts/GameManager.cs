using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Account
{
    public CurrencyType currencyType;
    public int Amount;
}

public class GameManager : MonoBehaviour
{
    //public Dictionary<CurrencyType, int> myAccount = new Dictionary<CurrencyType, int>();
    public List<Account> myAccountList;
    private static GameManager instance;
    public static GameManager Instance => instance;

    public PlayerMove move;

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

    public void PlayerDice(int diceNum)
    {
        move.MoveInInspector(diceNum);
    }
}
