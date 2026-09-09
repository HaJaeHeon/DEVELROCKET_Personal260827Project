using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechUpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("데이터 연결")]
    [SerializeField] private TechUpgradeTreeManager manager;
    [SerializeField] private UpgradeBranchSO nodeData;

    
    [Header("UI 컴포넌트")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text descriptionText;
    [field: SerializeField] public Button nodeButton {  get; set; }

    [Header("상태 저장")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private int maxLevel;
    public bool isUnlocked = false;

    [Header("스킬 트리 구조 (Inspector 직접 연결)")]
    [SerializeField] private Outline outLine;
    [field: SerializeField] public bool isRootNode { get; private set; } // 이 버튼이 시작점인가?
    [field: SerializeField] public List<TechUpgradeUI> nextNodes { get; private set; } //다음 해금될 UI노드 버튼

    private UnityEngine.Vector3 uiPosition;
    private float height;

    private Dictionary<StatType, Action> statUpgradeAction;

    private GameDatas gameDatas;

    private void OnEnable()
    {
        GameManager.Instance.OnRefreshUI += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnRefreshUI -= RefreshUI;
    }

    // 컴포넌트 널 체크하기
    private void Awake()
    {
        if(outLine == null)    
            outLine = GetComponent<Outline>();

        if(nodeButton == null)   
            nodeButton = GetComponent<Button>();

        if(iconImage == null)
            iconImage = GetComponent<Image>();

        if (manager == null)
            manager = transform.root.GetComponent<TechUpgradeTreeManager>();
    }

    //연결해야할 부분 초기화, ui refresh
    private void Start()
    {
        gameDatas = GameManager.Instance.gameDatas;

        InitSkillNode();


        InitStatUpgrade();

        RefreshUI();
    }

    public void InitSkillNode()
    {       
        //costText.text = nodeData.requiredCosts[currentLevel].ToString();
        // 업그레이드 가능 레벨로 확인 가능한 노드들
        levelText.text = currentLevel.ToString();
        iconImage.sprite = nodeData.nodeIcon;
        nodeButton.onClick.AddListener(() => OnClickNode());
        maxLevel = nodeData.maxLevel;
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        uiPosition = rect.position;
        height = rect.rect.height * rect.lossyScale.y;

        UpgradeInfo info = gameDatas.myUpgrades.FindUpgradeID(nodeData.nodeID);

        if (info == null)
        {
            //Debug.Log("null");
            nameText.text = string.Format(nodeData.nodeName, (nodeData.baseStatValue * Mathf.Pow(nodeData.statMultiplierPerLevel, currentLevel)));
            descriptionText.text = string.Format(nodeData.description, nodeData.baseStatValue, nodeData.statMultiplierPerLevel);
        }
        else
        {
            Debug.Log($"{info.nodeId} : not null");
            currentLevel = info.currentUpgradeCount;
            nameText.text = string.Format(nodeData.nodeName, (info.upgradeValue * Mathf.Pow(info.statMultiplierPerLevel, info.currentUpgradeCount)));
            descriptionText.text = string.Format(nodeData.description, info.upgradeValue, info.statMultiplierPerLevel);
            if (currentLevel >= maxLevel)
            {
                manager.OnNodeMastered(this);
                outLine.effectColor = Color.green;
            }
        }

        //// bool 로 업그레이드 한 노드들
        ///
        switch(nodeData.targetUnlock)
        {
            case UnlockType.UnlockGlobalBuild:
                break;
            case UnlockType.UnlockAnimatedRoll:
                CheckBooleans(GameManager.Instance.gameDatas.diceUpgrade_1);
                break;
            case UnlockType.UnlockAutoRoll:
                CheckBooleans(GameManager.Instance.gameDatas.diceUpgrade_2);
                break;
            case UnlockType.UnlockAutoBuild:
                break;
            case UnlockType.UnlockBuildCount1:
                CheckBooleans(GameManager.Instance.gameDatas.buildingCountUpgrade_1);
                break;
            case UnlockType.UnlockBuildCount2:
                CheckBooleans(GameManager.Instance.gameDatas.buildingCountUpgrade_2);
                break;
            case UnlockType.UnlockBuildCount3:
                CheckBooleans(GameManager.Instance.gameDatas.buildingCountUpgrade_3);
                break;
            case UnlockType.GameClear:
                CheckBooleans(GameManager.Instance.gameDatas.gameClear);
                break;
            case UnlockType.None:
                break;
        }
    }

    public void CheckBooleans(bool data)
    {
        if (data == false)
            return;
        else
        {
            currentLevel = maxLevel;
            manager.OnNodeMastered(this);
            outLine.effectColor = Color.green;
        }
    }

    /// <summary>
    /// 이 부분에 스탯 부분이 올라야 하는 애들 등록 
    /// 아래에 또 Action부분 넣어야함
    /// gameManager에도 넣어야함
    /// </summary>
    public void InitStatUpgrade()
    {
        statUpgradeAction = new Dictionary<StatType, Action>
        {
            { StatType.LineFlat, ApplyLineFlat },
            { StatType.LineMultiplier, ApplyLineMulti},
            { StatType.TileFlat, ApplyTileFlat },
            { StatType.TileMultiplier, ApplyTileMulti },
            { StatType.BuildingFlat, ApplyBuildingFlat  },
            { StatType.BuildingMultiplier,ApplyBuildingMulti },
            { StatType.IncomeFlat, ApplyIncomeFlat  },
            { StatType.IncomeMultiplier, ApplyIncomeMulti },
            { StatType.DiceCooldownReduction, ApplyDiceRollDurationReduction  },
            { StatType.PlayerJumpReduction, ApplyPlayerJumpDurationReduction  },
            { StatType.BuildSpeedReduction, ApplyBuildSpeedDurationReduction  },
            { StatType.None, () => {Debug.Log("Stat 없음"); } }
        };
    }

    // 매니저가 게임 시작 시 한 번씩 호출해줄 초기화 함수
    public void SetupNode(bool startUnlocked)
    {
        isUnlocked = startUnlocked;

        if (nodeData != null)
        {
            iconImage.sprite = nodeData.nodeIcon;
            nameText.text = string.Format(nodeData.nodeName, nodeData.baseStatValue);
        }

        // 업그레이드 해금 여부 판단하여 켜고 끄기
        // 켰으면 UI에 들어갈 내용 갱신
        if (isUnlocked)
        {
            gameObject.SetActive(true);
            RefreshUI();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // 앞선 노드를 마스터해서 이 노드가 비로소 해금될 때 호출되는 함수
    public void UnlockNode()
    {
        isUnlocked = true;
        gameObject.SetActive(true);

        RefreshUI();
    }

    // 레벨업을 하거나 해금될 때 UI 글자들을 갱신해주는 함수
    private void RefreshUI()
    {
        levelText.text = $"{currentLevel} / {nodeData.maxLevel}";

        if (currentLevel >= nodeData.maxLevel)
        {
            levelText.text = "MAX";
            nodeButton.interactable = false; // 마스터하면 클릭 불가
        }
        else
        {
            bool enoughCurrencies = BoolEnoughCurrency();

            // outline 색깔 변경으로 현재 구매 가능한지 불가한지 변경
            // OnClickNode 에서 MaxLevel과 currentLevel과 같으면 마스터 색깔로 변경
            if (!enoughCurrencies)
            {
                outLine.effectColor = Color.red;
            }
            else if (currentLevel < nodeData.maxLevel && enoughCurrencies)
            {
                outLine.effectColor = Color.yellow;
            }
        }
    }

    // 다음 레벨업 비용을 계산하는 로직
    private BigInteger CalculateNextCost()
    {
        if (nodeData.requiredCosts.Count == 0)
            return 0;

        CostData data = nodeData.requiredCosts[0];
        // 반올림 ( 기본비용 * (배율 ^ 현재레벨))
        return (BigInteger)(data.baseCost * Mathf.Pow(data.costMultiplier, currentLevel));
    }

    // 버튼을 클릭했을 때 (인스펙터의 OnClick에 연결할 함수)
    public void OnClickNode()
    {
        if (currentLevel >= nodeData.maxLevel) return;

        BigInteger requiredCost = CalculateNextCost();

        // 재화가 충분한지 확인
        if (!BoolEnoughCurrency())
            return;

        // 재화 차감
        SpendCurrencies();

        // 레벨 증가 및 능력치 적용
        currentLevel++;
        RefreshUI();

        // Manager에게 능력치 적용하라고 지시 (예: 주사위 속도 증가 등)
        // SkillTreeManager.Instance.ApplyStat(nodeData.targetStat, nodeData.baseStatValue);
        SetStats(nodeData.targetStat);
        SetUnlocks(nodeData.targetUnlock);


        // 방금 클릭으로 마스터?
        if (currentLevel >= nodeData.maxLevel)
        {
            // 다음 노드들의 숨김을 풀고 직선 긋기
            manager.OnNodeMastered(this);
            outLine.effectColor = Color.green;


        }
        UpgradeTooltip.Instance.ShowTooltip(nodeData.nodeName, nodeData.requiredCosts, currentLevel, maxLevel, uiPosition, height, transform, nodeData.baseStatValue);
    }


    // gameManager에서 값 변경 필요
    public void SetStats(StatType type)
    {
        if(statUpgradeAction.TryGetValue(type, out Action value))
        {
            if (type == StatType.None)
            {
                Debug.Log("None");
                return;
            }
            else
            {
                value.Invoke();
            }
        }
        else
        {
            Debug.LogError($"{type}에 등록되지 않음");
        }
    }

    public void SetUnlocks(UnlockType type)
    {
        switch(type)
        {
            case UnlockType.None:
                break;
            case UnlockType.UnlockAnimatedRoll:
                gameDatas.diceUpgrade_1 = true;
                break;
            case UnlockType.UnlockAutoRoll:
                gameDatas.diceUpgrade_2 = true;
                break;
            case UnlockType.UnlockBuildCount1:
                gameDatas.buildingCountUpgrade_1 = true;
                break;
            case UnlockType.UnlockBuildCount2:
                gameDatas.buildingCountUpgrade_2 = true;
                break;
            case UnlockType.UnlockBuildCount3:
                gameDatas.buildingCountUpgrade_3 = true;
                break;

            case UnlockType.GameClear:
                gameDatas.gameClear = true;
                GameManager.Instance.GameClear();
                Time.timeScale = 0;
                break;
            default:
                break;
        }
    }

    private bool BoolEnoughCurrency()
    {
        foreach (CostData costData in nodeData.requiredCosts)
        {
            BigInteger currentPrice = (BigInteger)(costData.baseCost * BigInteger.Pow(costData.costMultiplier, currentLevel));

            foreach (var item in GameManager.Instance.gameDatas.myAccountList)
            {
                if (item.currencyType == costData.currency)
                {
                    if (item.Amount < currentPrice)
                        return false;
                }
            }
        }
        return true;
    }

    //GameManager에 저장되어있는 재화만큼 감산
    private void SpendCurrencies()
    {
        Dictionary<CurrencyType, BigInteger> receipt = new Dictionary<CurrencyType, BigInteger>();

        foreach (CostData costData in nodeData.requiredCosts)
        {
            BigInteger currentPrice = (BigInteger)(costData.baseCost * BigInteger.Pow(costData.costMultiplier, currentLevel));

            receipt.Add(costData.currency, -currentPrice);
        }
        GameManager.Instance.UpdateAccount(receipt);
    }


    //=====================업그레이드 로직===================================
    private void ConfirmData(List<UpgradeInfo> info)
    {
        UpgradeInfo newUpgradeInfo = new UpgradeInfo
        {
            nodeId = nodeData.nodeID,
            upgradeValue = nodeData.baseStatValue,
            statMultiplierPerLevel = nodeData.statMultiplierPerLevel,
            currentUpgradeCount = currentLevel
        };
        if (info.Count == 0)
        {
            info.Add(newUpgradeInfo);
            return;
        }
        else
        {
            foreach (var value in info)
            {
                if (value.nodeId == nodeData.nodeID)
                {
                    value.currentUpgradeCount = currentLevel;
                    return;
                }
            }
            info.Add(newUpgradeInfo);
        }
    }
    
    private void ApplyLineFlat()
    {
        ConfirmData(gameDatas.myUpgrades.flat_lineValueUpgrade);
    }
    private void ApplyLineMulti()
    {
        ConfirmData(gameDatas.myUpgrades.mult_lineValueUpgrade);
    }
    private void ApplyTileFlat()
    {
        ConfirmData(gameDatas.myUpgrades.flat_tileValueUpgrade);
    }
    private void ApplyTileMulti()
    {
        ConfirmData(gameDatas.myUpgrades.mult_tileValueUpgrade);
    }
    private void ApplyBuildingFlat()
    {
        ConfirmData(gameDatas.myUpgrades.flat_buildingValueUpgrade);
    }
    private void ApplyBuildingMulti()
    {
        ConfirmData(gameDatas.myUpgrades.mult_buildingValueUpgrade);
    }
    private void ApplyIncomeFlat()
    {
        ConfirmData(gameDatas.myUpgrades.flat_IncomeValueUpgrade);
    }
    private void ApplyIncomeMulti()
    {
        ConfirmData(gameDatas.myUpgrades.mult_IncomeValueUpgrade);
    }
    private void ApplyDiceRollDurationReduction()
    {
        ConfirmData(gameDatas.myUpgrades.diceRollDurationUpgrade);
    }
    private void ApplyPlayerJumpDurationReduction()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("gameManager가 null입니다! 인스펙터 연결을 확인하세요.");
            return;
        }

        if (gameDatas.myUpgrades == null)
        {
            Debug.LogError("gameManager.myUpgrades가 null입니다! GameManager 스크립트에서 myUpgrades를 new로 생성했는지 확인하세요.");
            return;
        }
        ConfirmData(gameDatas.myUpgrades.playerjumpDurationUpgrade);
    }
    private void ApplyBuildSpeedDurationReduction()
    {
        ConfirmData(gameDatas.myUpgrades.buildSpeedUpgrade);
    }

    //===========================================================
    // 마우스 포인터 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nodeData == null || !isUnlocked) return;

        //rectTransform의 x, y 값과 height의 값을 같이 보냄
        
        // 글로벌 툴팁 창에 스킬 '이름'과 '설명'을 같이 전달
        UpgradeTooltip.Instance.ShowTooltip(nodeData.nodeName, nodeData.requiredCosts, currentLevel, maxLevel, uiPosition, height, transform, nodeData.baseStatValue);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isUnlocked) return;

        // 마우스가 나가면 툴팁 끄기
        UpgradeTooltip.Instance.HideTooltip();
    }
}