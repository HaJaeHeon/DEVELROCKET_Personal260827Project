using NUnit.Framework;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text StoneText;
    [SerializeField] private TMP_Text IndustryText;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button menuButton;

    private GameDatas gameDatas;

  
    private void Start()
    {
        gameDatas = GameManager.Instance.gameDatas;
        UpdateUI();
    }
    private void OnEnable()
    {
        GameManager.Instance.OnRefreshUI += UpdateUI;
        menuButton.onClick.AddListener(TOStartScene);
        saveButton.onClick.AddListener(GameManager.Instance.ClickSaveButton);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRefreshUI -= UpdateUI;
        menuButton.onClick.RemoveListener(TOStartScene);
        saveButton.onClick.RemoveListener(GameManager.Instance.ClickSaveButton);
    }

    public void UpdateUI()
    {
        List<Account> account = gameDatas.myAccountList;

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

    public string TransInt(BigInteger num)
    {
        if (num < 1000)
        {
            return num.ToString();
        }

        string[] units = { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

        int exp = (int)(BigInteger.Log10(num) / 3);

        if (exp >= units.Length)
            exp = units.Length - 1;

        BigInteger value = BigInteger.Pow(1000, exp);
        BigInteger newValue = (num * 100) / value;

        double finalValue = (double)newValue / 100d;

        return finalValue.ToString("0.00") + units[exp];
    }

    private void TOStartScene()
    {
        LoadingManager.Instance.LoadSceneWithLoading("StartScene");
    }
}
