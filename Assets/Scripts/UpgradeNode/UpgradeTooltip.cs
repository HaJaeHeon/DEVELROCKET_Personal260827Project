using System.Collections.Generic;
using TMPro;           // 텍스트는 무조건 TMP 사용
using UnityEngine;

// 이 스크립트는 캔버스 최하단(가장 앞쪽)에 띄워둘 Tooltip Panel에 붙입니다.
//[RequireComponent(typeof(CanvasGroup))]
public class UpgradeTooltip : MonoBehaviour
{
    public static UpgradeTooltip Instance;

    [Header("UI 컴포넌트 (TMP)")]
    public TMP_Text nameText;        // 노드 이름
    public TMP_Text requiredCosts; //필요 재화

    private RectTransform rectTransform; // 나 자신의 rectTransform
    [SerializeField] private float tooltipYOffset = 10f;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(false); // 시작할 땐 숨김
    }

    // 마우스를 올렸을 때 호출할 함수 (이름과 필요 재화량을 같이 받습니다)
    public void ShowTooltip(string name, List<CostData> costs, int currentLevel, int maxLevel, Vector3 uiPosition, float buttonHeight, Transform tr)
    {
        gameObject.SetActive(true);

        nameText.text = name;

        string tempString = "";

        if (currentLevel != maxLevel)
        {
            foreach (CostData costData in costs)
            {
                // 기본비용 * (배율 ^ 현재레벨)
                int currentPrice = (int)(costData.baseCost * Mathf.Pow(costData.costMultiplier, currentLevel));

                // 텍스트 누적 (예: "Food 100\nWood 50\n")
                tempString += $"\n{costData.currency} : {currentPrice:N0}";
            }
            requiredCosts.text = tempString;
            //requiredCosts.text = tempString.TrimEnd('\n');
        }
        else
        {
            requiredCosts.text = "MAX";
        }

        gameObject.transform.SetParent(tr);

        // 텍스트가 바뀌었으니 실제 UI 높이를 즉시 다시 계산하라고 강제 명령
        // (이 코드가 없으면 이전 스킬의 작은 툴팁 크기로 계산해버려서 덜 내려옵니다)
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(requiredCosts.rectTransform);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        float myHeight = rectTransform.rect.height * rectTransform.lossyScale.y;

        float yOffset = (buttonHeight * 0.5f) + (myHeight) + tooltipYOffset;
        float expectedTopY = uiPosition.y + yOffset;

        // 화면 밖으로 나가는지 검사
        if (expectedTopY > Screen.height)
        {
            // 아래로 띄울 때 이동할 거리
            float downOffset = (buttonHeight * 0.5f) + tooltipYOffset;
            gameObject.transform.position = uiPosition + (Vector3.down * downOffset);
        }
        else
        {
            // 공간이 넉넉하면 위로
            gameObject.transform.position = uiPosition + (Vector3.up * yOffset);
        }
    }

    // 마우스가 빠져나갔을 때 호출할 함수
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}