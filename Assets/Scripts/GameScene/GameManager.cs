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
public class TileData
{
    public int tileIndex;
    public int buildingCount;
}
[Serializable]
public class Account
{
    public CurrencyType currencyType;
    public UBigInt Amount;

    public Account(CurrencyType type)
    {
        currencyType = type;
        Amount = 0;
    }
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
public class    GameDatas
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

    public GameDatas()
    {
        myAccountList = new List<Account>
        {
            new Account((CurrencyType)0),
            new Account((CurrencyType)1),
            new Account((CurrencyType)2),
            new Account((CurrencyType)3),
        };
    }

    public List<TileData> tileDataList = new();
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

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager is null");
            gameDatas = new GameDatas();
            return;
        }

        gameDatas = DataManager.Instance.currentDatas ?? new GameDatas();
    }

    private void Start()
    {
        gameClearPanel.SetActive(false);
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
        BoardManager.Instance.SaveTileData(this.gameDatas);
        DataManager.Instance.CreateNewSave(this.gameDatas);
    }
    public void ClickInitDataButton()
    {
        DataManager.Instance.LoadInitData();
    }
}
