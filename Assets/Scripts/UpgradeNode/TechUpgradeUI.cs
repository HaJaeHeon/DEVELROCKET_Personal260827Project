using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechUpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("데이터 연결")]
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
    [SerializeField] private bool isUnlocked = false;

    [Header("스킬 트리 구조 (Inspector 직접 연결)")]
    [SerializeField] private Outline outLine;
    [field: SerializeField] public bool isRootNode { get; private set; } // 이 버튼이 시작점인가?
    [field: SerializeField] public List<TechUpgradeUI> nextNodes { get; private set; } //다음 해금될 UI노드 버튼

    private Vector3 uiPosition;
    private float height;

    // 컴포넌트 널 체크하기
    private void Awake()
    {
        if(outLine == null)    
            outLine = GetComponent<Outline>();

        if(nodeButton == null)   
            nodeButton = GetComponent<Button>();

        if(iconImage == null)
            iconImage = GetComponent<Image>();
    }

    //연결해야할 부분 초기화, ui refresh
    private void Start()
    {
        iconImage.sprite = nodeData.nodeIcon;
        nameText.text = nodeData.nodeName;
        levelText.text = currentLevel.ToString();
        //costText.text = nodeData.requiredCosts[currentLevel].ToString();
        descriptionText.text = nodeData.description;
        nodeButton.onClick.AddListener(() => OnClickNode());
        maxLevel = nodeData.maxLevel;

        RectTransform rect = gameObject.GetComponent<RectTransform>();
        uiPosition = rect.position;
        height = rect.rect.height * rect.lossyScale.y;

        RefreshUI();
    }

    // 매니저가 게임 시작 시 한 번씩 호출해줄 초기화 함수
    public void SetupNode(bool startUnlocked)
    {
        isUnlocked = startUnlocked;

        if (nodeData != null)
        {
            iconImage.sprite = nodeData.nodeIcon;
            nameText.text = nodeData.nodeName;
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
    private int CalculateNextCost()
    {
        if (nodeData.requiredCosts.Count == 0)
            return 0;

        CostData data = nodeData.requiredCosts[0];
        // 반올림 ( 기본비용 * (배율 ^ 현재레벨))
        return Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.costMultiplier, currentLevel));
    }

    // 버튼을 클릭했을 때 (인스펙터의 OnClick에 연결할 함수)
    public void OnClickNode()
    {
        if (currentLevel >= nodeData.maxLevel) return;

        int requiredCost = CalculateNextCost();

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
            TechUpgradeTreeManager.Instance.OnNodeMastered(this);
            outLine.effectColor = Color.green;


        }
        UpgradeTooltip.Instance.ShowTooltip(nodeData.nodeName, nodeData.requiredCosts, currentLevel, maxLevel, uiPosition, height, transform);
    }


    // gameManager에서 값 변경 필요
    public void SetStats(StatType type)
    {
        switch(type)
        {
            case StatType.None:
                break;
            case StatType.IncomeMultiplier:
                //GameManager.Instance.tile.??
                break;
            case StatType.BuildCostDiscount: 
                break;
            case StatType.DiceCooldownReduction:
                break;
        }
    }

    public void SetUnlocks(UnlockType type)
    {
        switch(type)
        {
            case UnlockType.None:
                break;
            case UnlockType.UnlockAnimatedRoll:
                GameManager.Instance.diceUpgrade_1 = true;
                break;
            case UnlockType.UnlockAutoRoll:
                GameManager.Instance.diceUpgrade_2 = true;
                break;
            case UnlockType.UnlockBuildCount1:
                GameManager.Instance.buildUpgrade_1 = true;
                break;
            case UnlockType.UnlockBuildCount2:
                GameManager.Instance.buildUpgrade_2 = true;
                break;


            case UnlockType.UnlockGlobalBuild:
                break;
        }
    }

    private bool BoolEnoughCurrency()
    {
        foreach (CostData costData in nodeData.requiredCosts)
        {
            int currentPrice = (int)(costData.baseCost * Mathf.Pow(costData.costMultiplier, currentLevel));

            foreach (var item in GameManager.Instance.myAccountList)
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
        Dictionary<CurrencyType, int> receipt = new Dictionary<CurrencyType, int>();

        foreach (CostData costData in nodeData.requiredCosts)
        {
            int currentPrice = (int)(costData.baseCost * Mathf.Pow(costData.costMultiplier, currentLevel));

            receipt.Add(costData.currency, -currentPrice);
        }
        GameManager.Instance.UpdateAccount(receipt);
    }


    //===========================================================
    // 마우스 포인터 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nodeData == null || !isUnlocked) return;

        //rectTransform의 x, y 값과 height의 값을 같이 보냄
        
        // 글로벌 툴팁 창에 스킬 '이름'과 '설명'을 같이 전달
        UpgradeTooltip.Instance.ShowTooltip(nodeData.nodeName, nodeData.requiredCosts, currentLevel, maxLevel, uiPosition, height, transform);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isUnlocked) return;

        // 마우스가 나가면 툴팁 끄기
        UpgradeTooltip.Instance.HideTooltip();
    }
}