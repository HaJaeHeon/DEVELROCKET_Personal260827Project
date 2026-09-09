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

    public UpgradeInfo FindUpgradeID(int targetId)
    {
        UpgradeInfo result = null;

        result = flat_lineValueUpgrade.Find(x => x.nodeId == targetId) ??
                    mult_lineValueUpgrade.Find(x => x.nodeId == targetId) ??
                    flat_tileValueUpgrade.Find(x => x.nodeId == targetId) ??
                    mult_tileValueUpgrade.Find(x => x.nodeId == targetId) ??
                    flat_buildingValueUpgrade.Find(x => x.nodeId == targetId) ??
                    mult_buildingValueUpgrade.Find(x => x.nodeId == targetId) ??
                    flat_IncomeValueUpgrade.Find(x => x.nodeId == targetId) ??
                    mult_IncomeValueUpgrade.Find(x => x.nodeId == targetId) ??
                    diceRollDurationUpgrade.Find(x => x.nodeId == targetId) ??
                    playerjumpDurationUpgrade.Find(x => x.nodeId == targetId) ??
                    buildSpeedUpgrade.Find(x => x.nodeId == targetId);

        return result;
    }
}
[Serializable]
public class GameDatas
{
    public List<Account> myAccountList = new();
    public Upgrades myUpgrades = new();
    public bool isGameStart = false;
    public bool diceUpgrade_1 = false;
    public bool diceUpgrade_2 = false;
    public bool gameClear = false;
    public List<int> listMaxBuildCount = new List<int> { { 3 }, { 6 }, { 9 }, { 10 } };
    public bool buildingCountUpgrade_1 = false;
    public bool buildingCountUpgrade_2 = false;
    public bool buildingCountUpgrade_3 = false;

    public int diceNum = -1;
    public float jumpDuration = 1f;
    public float waitForNextNode = 0.1f;
    public float rollDuration = 1f;
    public float buildSpeed = 1f;
    public int currentPlayerPositionTileNum = 0;
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    public GameObject gameClearPanel;
    public EarnEffectUI earnEffect;

    public HFSMManager hfsmManager;


    public TileNode tile;

    public event Action OnRefreshUI;

    public GameDatas gameDatas = new();
    public GameDatas saveData = new();


    //점프 시간을 줄여서 animation속도 짧게하기
    //public float jumpDuration { get => gameDatas.jumpDurationValue; private set => gameDatas.jumpDurationValue = value; }
    //말 이동 대기 시간 짧게
    //public float waitForNextNode { get => gameDatas.waitForNextNodeValue; private set => gameDatas.waitForNextNodeValue = value; }
    // 주사위 돌아가는 시간
    //public float rollDuration { get => gameDatas.rollDurationValue; private set => gameDatas.rollDurationValue = value; }
    // 건물 내려오는 속도
    //public float buildSpeed { get => gameDatas.buildSpeedValue; private set => gameDatas.buildSpeedValue = value; }

    public DiceMode currentDiceMode
    {
        get
        {
            return !gameDatas.diceUpgrade_1 ? DiceMode.Physics : !gameDatas.diceUpgrade_2 ? DiceMode.Animated : DiceMode.Auto;
        }
    }

    public int maxBuildingCount { get
        {
            return !gameDatas.buildingCountUpgrade_1 ? gameDatas.listMaxBuildCount[0] :
                !gameDatas.buildingCountUpgrade_2 ? gameDatas.listMaxBuildCount[1] :
                !gameDatas.buildingCountUpgrade_3 ? gameDatas.listMaxBuildCount[2] : gameDatas.listMaxBuildCount[3];
        }
    }

    //public void Init()
    //{
    //    buildingCountUpgrade_1 = false;
    //    buildingCountUpgrade_2 = false;
    //    buildingCountUpgrade_3 = false;
    //    diceUpgrade_1 = false;
    //    diceUpgrade_2 = false;
    //    gameClear = false;
    //    isGameStart = false;
    //    diceNum = -1;
    //    jumpDuration = 0.6f;
    //    rollDuration = 0.6f;
    //    buildSpeed = 0.6f;

    //    waitForNextNode = 0.1f;

    //    foreach (var account in myAccountList)
    //    {
    //        account.Amount = 0;
    //    }
    //    myUpgrades.flat_lineValueUpgrade.Clear();
    //    myUpgrades.mult_lineValueUpgrade.Clear();
    //    myUpgrades.flat_tileValueUpgrade.Clear();
    //    myUpgrades.mult_tileValueUpgrade.Clear();
    //    myUpgrades.flat_buildingValueUpgrade.Clear();
    //    myUpgrades.mult_buildingValueUpgrade.Clear();
    //    myUpgrades.flat_IncomeValueUpgrade.Clear();
    //    myUpgrades.mult_IncomeValueUpgrade.Clear();
    //    myUpgrades.diceRollDurationUpgrade.Clear();
    //    myUpgrades.playerjumpDurationUpgrade.Clear();
    //    myUpgrades.buildSpeedUpgrade.Clear();
    //}



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
        gameDatas = DataManager.Instance.currentDatas;
    }


    public void UpdateAccount(Dictionary<CurrencyType, BigInteger> receipt)
    {
        foreach (var bill in receipt)
        {
            CurrencyType type = bill.Key;
            BigInteger amount = bill.Value;
            //Debug.Log($"{bill.Key} / {bill.Value}");

            foreach (var item in gameDatas.myAccountList)
            {
                if (item.currencyType == type)
                {
                    item.Amount += amount;
                    earnEffect.SetCurrenciesTransform(type, amount);
                    //Debug.Log($"{item.currencyType} / {item.Amount}");
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
        gameDatas.isGameStart = false;
        gameClearPanel.SetActive(true);
    }

    public void CalculateJumpDuration()
    {
        gameDatas.myUpgrades.playerjumpDurationUpgrade.ForEach((value) =>
        gameDatas.jumpDuration = value.upgradeValue * Mathf.Pow(value.statMultiplierPerLevel, value.currentUpgradeCount));
        //Debug.Log($"{gameDatas.jumpDuration} >> jumpDuration to calc");
    }
    public void CalculateBuildSpeed()
    {
        gameDatas.myUpgrades.buildSpeedUpgrade.ForEach((value) =>
        gameDatas.buildSpeed = value.upgradeValue * Mathf.Pow(value.statMultiplierPerLevel, value.currentUpgradeCount));
        //Debug.Log($"{gameDatas.buildSpeed} >> jumpDuration to calc");
    }
    public void CalculateDiceRollSpeed()
    {
        gameDatas.myUpgrades.diceRollDurationUpgrade.ForEach((value) =>
        gameDatas.rollDuration = value.upgradeValue * Mathf.Pow(value.statMultiplierPerLevel, value.currentUpgradeCount));
        //Debug.Log($"{gameDatas.rollDuration} >> rollDuration to calc");
    }

    public void ClickSaveButton()
    {
        DataManager.Instance.CreateNewSaveSafe(this.gameDatas);
    }
    public void ClickInitDataButton()
    {
        DataManager.Instance.LoadInitData();
    }

    public void ClickAccountZero()
    {
        Account newAccount = new Account();
        newAccount.currencyType = CurrencyType.Food;
        newAccount.Amount = 0;
        gameDatas.myAccountList.Add(newAccount);
        newAccount.currencyType = CurrencyType.Wood;
        newAccount.Amount = 0;
        gameDatas.myAccountList.Add(newAccount);
        newAccount.currencyType = CurrencyType.Stone;
        newAccount.Amount = 0;
        gameDatas.myAccountList.Add(newAccount);
        newAccount.currencyType = CurrencyType.Industry;
        newAccount.Amount = 0;
        gameDatas.myAccountList.Add(newAccount);

        foreach (var account in gameDatas.myAccountList)
        {
            account.Amount = 0;
            Debug.Log($"{account.currencyType} type / {account.Amount} amount");
        }
    }
}
