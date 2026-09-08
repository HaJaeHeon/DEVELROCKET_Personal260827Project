using System;
using System.Collections.Generic;
using System.Numerics;
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
    public UBigInt Amount;
}

[Serializable]
public class UpgradeInfo
{
    public int nodeId;
    public long upgradeValue;
    public float statMultiplierPerLevel;
    public int currentUpgradeCount;
}

/// <summary>
/// TechUpgradeUI 에서 넣고
/// 이 부분에 스탯 부분이 올라야 하는 애들 등록 
/// 아래에 또 Action부분 넣어야함
/// gameManager에도 넣어야함
/// </summary>
[Serializable]
public class Upgrades
{
    public List<UpgradeInfo> flat_lineValueUpgrade = new();
    public List<UpgradeInfo> mult_lineValueUpgrade = new();
    public List<UpgradeInfo> flat_tileValueUpgrade = new();
    public List<UpgradeInfo> mult_tileValueUpgrade = new();
    public List<UpgradeInfo> flat_buildingValueUpgrade = new();
    public List<UpgradeInfo> mult_buildingValueUpgrade = new();
    public List<UpgradeInfo> flat_IncomeValueUpgrade = new();
    public List<UpgradeInfo> mult_IncomeValueUpgrade = new();
    public List<UpgradeInfo> diceRollDurationUpgrade = new();
    public List<UpgradeInfo> playerjumpDurationUpgrade = new();
    public List<UpgradeInfo> buildSpeedUpgrade = new();
}

public class GameManager : MonoBehaviour
{
    public List<Account> myAccountList = new();
    public Upgrades myUpgrades = new();

    private static GameManager instance;
    public static GameManager Instance => instance;

    public GameObject gameClearPanel;
    public EarnEffectUI earnEffect;
    public bool isGameStart = false;
    public HFSMManager hfsmManager;

    public int diceNum;
    public TileNode tile;

    public event Action OnRefreshUI;

    public bool diceUpgrade_1 = false;
    public bool diceUpgrade_2 = false;
    public bool gameClear = false;

    //점프 시간을 줄여서 animation속도 짧게하기
    public float jumpDuration;
    //말 이동 대기 시간 짧게
    [field:SerializeField] public float waitForNextNode { get; private set; }
    // 주사위 돌아가는 시간
    [field:SerializeField] public float rollDuration { get; private set; }
    // 건물 내려오는 속도
    [field: SerializeField] public float buildSpeed { get; private set; }

    public DiceMode currentDiceMode
    {
        get
        {
            return !diceUpgrade_1 ? DiceMode.Physics : !diceUpgrade_2 ? DiceMode.Animated : DiceMode.Auto;
        }
    }
    public List<int> listMaxBuildCount = new List<int>{ { 3 }, { 6 },{ 9 }, { 10 } };
    public bool buildingCountUpgrade_1 = false;
    public bool buildingCountUpgrade_2 = false;
    public bool buildingCountUpgrade_3 = false;
    public int maxBuildingCount {  get
        {
            return !buildingCountUpgrade_1 ? listMaxBuildCount[0] : 
                !buildingCountUpgrade_2 ? listMaxBuildCount[1] : 
                !buildingCountUpgrade_3 ? listMaxBuildCount[2] : listMaxBuildCount[3];
        }
    }

    public void Init()
    {
        buildingCountUpgrade_1 = false;
        buildingCountUpgrade_2 = false;
        buildingCountUpgrade_3 = false;
        diceUpgrade_1 = false;
        diceUpgrade_2 = false;
        gameClear = false;
        isGameStart = false;
        diceNum = -1;
        jumpDuration = 0.6f;
        rollDuration = 0.6f;
        buildSpeed = 0.6f;

        waitForNextNode = 0.1f;

        foreach (var account in myAccountList)
        {
            account.Amount = 0;
        }
        myUpgrades.flat_lineValueUpgrade.Clear();
        myUpgrades.mult_lineValueUpgrade.Clear();
        myUpgrades.flat_tileValueUpgrade.Clear();
        myUpgrades.mult_tileValueUpgrade.Clear();
        myUpgrades.flat_buildingValueUpgrade.Clear();
        myUpgrades.mult_buildingValueUpgrade.Clear();
        myUpgrades.flat_IncomeValueUpgrade.Clear();
        myUpgrades.mult_IncomeValueUpgrade.Clear();
        myUpgrades.diceRollDurationUpgrade.Clear();
        myUpgrades.playerjumpDurationUpgrade.Clear();
        myUpgrades.buildSpeedUpgrade.Clear();
    }



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

    private void Start()
    {
        gameClearPanel.SetActive(false);
        diceUpgrade_1 = false;
        diceUpgrade_2 = false;
        gameClear = false;
        isGameStart = false;
    }

    public void UpdateAccount(Dictionary<CurrencyType, BigInteger> receipt)
    {
        foreach (var bill in receipt)
        {
            CurrencyType type = bill.Key;
            BigInteger amount = bill.Value;
            //Debug.Log($"{bill.Key} / {bill.Value}");

            foreach (var item in myAccountList)
            {
                if (item.currencyType == type)
                {
                    item.Amount += amount;
                    earnEffect.SetCurrenciesTransform(type, amount);
                    Debug.Log($"{item.currencyType} / {item.Amount}");
                }
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
    
    public void GameClear()
    {
        hfsmManager.OnTriggerStopHfsm();
        isGameStart = false;
        gameClearPanel.SetActive(true);
    }

    public void CalculateJumpDuration()
    {
        myUpgrades.playerjumpDurationUpgrade.ForEach((value) =>
        jumpDuration = value.upgradeValue * Mathf.Pow(value.statMultiplierPerLevel, value.currentUpgradeCount));
        Debug.Log($"{jumpDuration} >> jumpDuration to calc");
    }
}
