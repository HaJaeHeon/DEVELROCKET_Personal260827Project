using DG.Tweening;     // 팝업 애니메이션용
using TMPro;           // 텍스트는 무조건 TMP 사용
using UnityEngine;
using UnityEngine.InputSystem;

// 이 스크립트는 캔버스 최하단(가장 앞쪽)에 띄워둘 Tooltip Panel에 붙입니다.
//[RequireComponent(typeof(CanvasGroup))]
public class UpgradeTooltip : MonoBehaviour
{
    public static UpgradeTooltip Instance;

    [Header("UI 컴포넌트 (TMP)")]
    public TextMeshProUGUI nameText;        // 스킬 이름
    public TextMeshProUGUI descriptionText; // 스킬 설명

    private RectTransform rectTransform;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(false); // 시작할 땐 숨김
    }

    // 마우스를 올렸을 때 호출할 함수 (이름과 설명을 같이 받습니다)
    public void ShowTooltip(string name, string desc, Vector3 uiPosition)
    {
        nameText.text = name;
        descriptionText.text = desc;

        gameObject.transform.position = uiPosition + Vector3.up * (rectTransform.rect.height * 0.5f);

        gameObject.SetActive(true);
    }

    // 마우스가 빠져나갔을 때 호출할 함수
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}