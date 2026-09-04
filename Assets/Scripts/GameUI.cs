using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text StoneText;
    [SerializeField] private TMP_Text IndustryText;

    private void OnEnable()
    {
    }

    private void Start()
    {
        GameManager.Instance.OnRefreshUI += UpdateUI;
        UpdateUI();
    }

    public void UpdateUI()
    {
        List<Account> account = GameManager.Instance.myAccountList;

        for(int i = 0; i < account.Count; i++)
        {
            switch (account[i].currencyType)
            {
                case CurrencyType.Food:
                    foodText.text = TransInt(account[i].Amount);
                    break;
                case CurrencyType.Wood:
                    woodText.text = TransInt(account[i].Amount);
                    break;
                case CurrencyType.Stone:
                    StoneText.text = TransInt(account[i].Amount);
                    break;
                case CurrencyType.Industry:
                    IndustryText.text = TransInt(account[i].Amount);
                    break;
            }
        }
    }

    private string TransInt(BigInteger num)
    {
        if (num < 1000)
        {
            return num.ToString();
        }

        string[] units = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

        int exp = (int)(BigInteger.Log10(num) / 3);

        if (exp >= units.Length)
            exp = units.Length - 1;

        BigInteger value = num / BigInteger.Pow(1000, exp);

        return value.ToString("0.00") + units[exp];
    }
}
