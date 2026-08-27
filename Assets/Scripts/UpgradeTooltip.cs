using DG.Tweening;     // 팝업 애니메이션용
using TMPro;           // 텍스트는 무조건 TMP 사용
using UnityEngine;
using UnityEngine.InputSystem;

// 이 스크립트는 캔버스 최하단(가장 앞쪽)에 띄워둘 Tooltip Panel에 붙입니다.
[RequireComponent(typeof(CanvasGroup))]
public class UpgradeTooltip : MonoBehaviour
{
    public static UpgradeTooltip Instance;

    [Header("UI 컴포넌트 (TMP)")]
    public TextMeshProUGUI nameText;        // 스킬 이름
    public TextMeshProUGUI descriptionText; // 스킬 설명

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup; // 투명도 조절용

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        gameObject.SetActive(false); // 시작할 땐 숨김
    }

    void Update()
    {
        // Update문 안의 코드 수정
        if (gameObject.activeSelf)
        {
            // 마우스 마우스 좌표를 가져옴 (Vector2를 Vector3로 변환)
            Vector3 mousePos = Mouse.current.position.ReadValue();
            transform.position = mousePos + new Vector3(30f, -30f, 0f);
        }
    }

    // 마우스를 올렸을 때 호출할 함수 (이름과 설명을 같이 받습니다)
    public void ShowTooltip(string name, string desc)
    {
        nameText.text = name;
        descriptionText.text = desc;

        gameObject.SetActive(true);

        // [애니메이션] 투명도 0 -> 1 페이드 인
        //canvasGroup.alpha = 0f;
        //canvasGroup.DOFade(1f, 0.2f);

        // [애니메이션] 크기 0 -> 1 통통 튀며 커지기
        //rectTransform.localScale = Vector3.zero;
        //rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }

    // 마우스가 빠져나갔을 때 호출할 함수
    public void HideTooltip()
    {
        // DOTween 애니메이션 멈추고 숨기기
        canvasGroup.DOKill();
        rectTransform.DOKill();
        gameObject.SetActive(false);
    }
}