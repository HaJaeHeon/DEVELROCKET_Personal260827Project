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
    //public Dictionary<CurrencyType, int> myAccount = new Dictionary<CurrencyType, int>();
    public List<Account> myAccountList;
    private static GameManager instance;
    public static GameManager Instance => instance;

    public PlayerMove move;

    public int diceNum;
    public bool isTodo;

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

    public void TodoList()
    {
        StartCoroutine(TodoCoroutine());
    }
    public IEnumerator TodoCoroutine()
    {
        isTodo = true;
        Debug.Log($"istodo : {isTodo}");
        yield return new WaitForSeconds(1f);
        isTodo = false;
        Debug.Log($"istodo : {isTodo}");
    }
}
