using System;
using System.Collections;
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
    public  List<Account> myAccountList;
    private static GameManager instance;
    public static GameManager Instance => instance;

    public PlayerMove move;
    public int diceNum;

    public event Action OnRefreshUI;


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
                if(item.currencyType == type)
                    item.Amount -= amount;
                //Debug.Log($"{item.currencyType} / {item.Amount}");
            }
        }
        OnRefreshUI?.Invoke();
    }

    //public void PlayerDice(int diceNum)
    //{
    //    move.MoveInInspector(diceNum);
    //}
}
