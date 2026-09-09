using UnityEngine;
using DamageNumbersPro;
using System.Numerics;
using TMPro;

public class EarnEffectUI : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private DamageNumber numberPrefab;
    [Header("텍스트")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text IndustryText;

    private GameUI gameUI;

    private void Awake()
    {
        gameUI = GetComponent<GameUI>();
    }


    public void SetCurrenciesTransform(CurrencyType currency, BigInteger amount)
    {
        RectTransform currentRect = null;

        switch(currency)
        {
            case CurrencyType.Food:
                currentRect = foodText.rectTransform;
                break;
            case CurrencyType.Wood:
                currentRect = woodText.rectTransform;
                break;
            case CurrencyType.Stone:
                currentRect = stoneText.rectTransform;
                break;
            case CurrencyType.Industry:
                currentRect = IndustryText.rectTransform;
                break;
            default:
                Debug.Log($"currencyType ??? {currency}");
                break;
        }
        SpawnEffect(currentRect, amount);
    }


    private void SpawnEffect(UnityEngine.RectTransform rectParent, BigInteger amount)
    {
        DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, UnityEngine.Vector2.zero, gameUI.TransInt(amount));
    }
}
