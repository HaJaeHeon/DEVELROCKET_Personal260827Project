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

    private Transform targetTransform;
    private float targetButtonHeight;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(false); // 시작할 땐 숨김
    }

    private void LateUpdate()
    {
        // 툴팁이 켜져 있고, 쫓아갈 타겟이 존재할 때만 실행
        if (gameObject.activeSelf && targetTransform != null)
        {
            UpdateTooltipPosition();
        }
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

        // 텍스트가 바뀌었으니 실제 UI 높이를 즉시 다시 계산하라고 강제 명령
        // (이 코드가 없으면 이전 스킬의 작은 툴팁 크기로 계산해버려서 덜 내려옵니다)
        //UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(requiredCosts.rectTransform);
        //UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(requiredCosts.rectTransform);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        targetTransform = tr;
        targetButtonHeight = buttonHeight;

        UpdateTooltipPosition();
    }
    private void UpdateTooltipPosition()
    {
        // 1. 레이아웃 강제 갱신
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(requiredCosts.rectTransform);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        Vector3 currentUIPos = targetTransform.position;

        // ★ [핵심] 줌(스케일)이 변할 때 버튼의 '실시간 실제 높이'를 다시 계산합니다.
        RectTransform targetRect = targetTransform.GetComponent<RectTransform>();
        float currentButtonHeight = targetRect != null ? (targetRect.rect.height * targetTransform.lossyScale.y) : targetButtonHeight;

        // 툴팁 자체의 높이와 줌 배율
        float myHeight = (rectTransform.rect.height * rectTransform.lossyScale.y);
        float halfButtonH = currentButtonHeight * 0.5f;
        float zoomScale = targetTransform.lossyScale.y;
        float adjustedOffset = tooltipYOffset * zoomScale;

        // 2. 타겟 노드의 월드 좌표를 '화면 픽셀 좌표(Screen Point)'로 변환
        Camera canvasCam = rectTransform.GetComponentInParent<Canvas>().worldCamera;
        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(canvasCam, currentUIPos);

        // 3. 위쪽에 툴팁을 띄웠을 때 툴팁 꼭대기가 닿을 것으로 예상되는 스크린 Y 좌표 계산
        float expectedTopScreenY = targetScreenPos.y + halfButtonH + myHeight + adjustedOffset;

        // 4. 화면 천장을 뚫었는지 검사하여 위/아래 배치 결정
        if (expectedTopScreenY > Screen.height)
        {
            // 아래쪽 배치
            float downOffset = halfButtonH + (myHeight * 0.5f) + adjustedOffset;
            Vector3 downPos = currentUIPos + (Vector3.down * downOffset);
            downPos.z = 0f;
            gameObject.transform.position = downPos;
        }
        else
        {
            // 위쪽 배치
            float upOffset = halfButtonH + (myHeight * 0.5f) + adjustedOffset;
            Vector3 targetPos = currentUIPos + (Vector3.up * upOffset);
            targetPos.z = 0f;
            gameObject.transform.position = targetPos;
        }
    }

    // 마우스가 빠져나갔을 때 호출할 함수
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}