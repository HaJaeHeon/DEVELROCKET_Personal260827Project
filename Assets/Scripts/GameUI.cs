using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

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
                    foodText.text = account[i].Amount.ToString();
                    break;
                case CurrencyType.Wood:
                    woodText.text = account[i].Amount.ToString();
                    break;
                case CurrencyType.Stone:
                    StoneText.text = account[i].Amount.ToString();
                    break;
                case CurrencyType.Industry:
                    IndustryText.text = account[i].Amount.ToString();
                    break;
            }
        }
    }
}
