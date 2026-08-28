using System.Collections.Generic;
using TMPro;
using Unity.Mathematics; // 마우스 호버(Tooltip)를 위해 필수
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechUpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("데이터 연결")]
    public UpgradeBranchSO nodeData;

    [Header("UI 컴포넌트")]
    public Button nodeButton;
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text costText;

    [Header("상태 저장")]
    public int currentLevel = 0;
    public bool isUnlocked = false;

    [Header("스킬 트리 구조 (여기서 직접 연결!)")]
    public bool isRootNode = false; // 이 버튼이 시작점인가?
    public List<TechUpgradeUI> nextNodes; //다음 해금될 UI노드 버튼
    public Outline outLine;
    public List<Image> branchImage;

    private void Awake()
    {
        outLine = GetComponent<Outline>();
        nodeButton = GetComponent<Button>();
        iconImage = GetComponent<Image>();
    }

    private void Start()
    {
        iconImage.sprite = nodeData.nodeIcon;
        nameText.text = nodeData.nodeName;
        levelText.text = currentLevel.ToString();
        costText.text = nodeData.requiredCosts[currentLevel].ToString();

        nodeButton.onClick.AddListener(() => OnClickNode());

        RefreshUI();
    }

    //private void OnEnable()
    //{
    //    iconImage.sprite = nodeData.nodeIcon;
    //    nameText.text = nodeData.nodeName;
    //    levelText.text = currentLevel.ToString();
    //    costText.text = nodeData.requiredCosts[currentLevel].ToString();

    //    nodeButton.onClick.AddListener(() => OnClickNode());

    //    RefreshUI();
    //}

    // 매니저(SkillTreeManager)가 게임 시작 시 한 번씩 호출해줄 초기화 함수
    public void SetupNode(bool startUnlocked)
    {
        isUnlocked = startUnlocked;

        if (nodeData != null)
        {
            iconImage.sprite = nodeData.nodeIcon;
            nameText.text = nodeData.nodeName;
        }

        if (isUnlocked)
        {
            gameObject.SetActive(true);
            RefreshUI();
        }
        else
        {
            gameObject.SetActive(false); // 해금되지 않은 노드는 아예 숨김
        }
    }

    // 앞선 노드를 마스터해서 이 노드가 비로소 해금될 때 호출되는 함수
    public void UnlockNode()
    {
        isUnlocked = true;
        gameObject.SetActive(true); // 숨어있던 노드가 화면에 나타남!

        // 등장할 때 부드럽게 커지는 팝업 애니메이션 등을 여기에 넣으면 좋습니다.
        // transform.localScale = Vector3.zero;
        // transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); // DOTween 예시

        RefreshUI();
    }

    // 레벨업을 하거나 해금될 때 UI 글자들을 갱신해주는 함수
    private void RefreshUI()
    {
        levelText.text = $"{currentLevel} / {nodeData.maxLevel}";

        if (currentLevel >= nodeData.maxLevel)
        {
            costText.text = "MAX";
            nodeButton.interactable = false; // 마스터하면 클릭 불가
        }
        else
        {
            // 여러 종류의 재화를 텍스트 하나로 묶어줄 임시 문자열
            string costString = "";

            // SO에 등록된 모든 요구 비용(List)을 하나씩 꺼내서 계산
            foreach (CostData costData in nodeData.requiredCosts)
            {
                // 공식: 기본비용 * (배율 ^ 현재레벨)
                double currentPrice = costData.baseCost * Mathf.Pow(costData.costMultiplier, currentLevel);

                // 텍스트 누적 (예: "Food 100\nWood 50\n")
                costString += $"{costData.currency} : {currentPrice:N0}\n";
            }

            // 완성된 여러 줄의 텍스트를 UI에 적용
            costText.text = costString;

            bool enoughCurrencies = CalcEnoughCurrency();

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
    private double CalculateNextCost()
    {
        if (nodeData.requiredCosts.Count == 0)
            return 0;

        CostData data = nodeData.requiredCosts[0];
        // 반올림 ( 기본비용 * (배율 ^ 현재레벨))
        return math.round(data.baseCost * Mathf.Pow(data.costMultiplier, currentLevel));
    }

    // 버튼을 클릭했을 때 (인스펙터의 OnClick에 연결할 함수)
    public void OnClickNode()
    {
        if (currentLevel >= nodeData.maxLevel) return;

        double requiredCost = CalculateNextCost();

        // 1. 재화가 충분한지 확인 (PlayerStatusSO 등과 연동)
        if (!CalcEnoughCurrency())
            return;

        // 2. 재화 차감
        // PlayerManager.Instance.UseCurrency(nodeData.requiredCosts[0].currency, requiredCost);

        // 3. 레벨 증가 및 능력치 적용
        currentLevel++;
        RefreshUI();

        // Manager에게 능력치 적용하라고 지시 (예: 주사위 속도 증가 등)
        // SkillTreeManager.Instance.ApplyStat(nodeData.targetStat, nodeData.baseStatValue);

        // 4. 방금 클릭으로 만렙(마스터)을 찍었다면?
        if (currentLevel >= nodeData.maxLevel)
        {
            // Manager에게 다음 노드들의 숨김을 풀고 선을 그어달라고 요청!
            TechUpgradeTreeManager.Instance.OnNodeMastered(this);
            outLine.effectColor = Color.green;
        }
    }

    private bool CalcEnoughCurrency()
    {
        foreach (CostData costData in nodeData.requiredCosts)
        {
            double currentPrice = costData.baseCost * Mathf.Pow(costData.costMultiplier, currentLevel);

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


    // --- [마우스 호버 시 툴팁 기능] ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nodeData == null || !isUnlocked) return;

        //rectTransform의 x, y 값과 height의 값을 같이 보냄
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector3 uiPosition = rect.position;
        float height = rect.rect.height * 0.5f;
        // 글로벌 툴팁 창에 스킬 '이름'과 '설명'을 같이 전달
        UpgradeTooltip.Instance.ShowTooltip(nodeData.nodeName, nodeData.description, uiPosition);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isUnlocked) return;

        // 마우스가 나가면 툴팁 끄기
        UpgradeTooltip.Instance.HideTooltip();
    }
}