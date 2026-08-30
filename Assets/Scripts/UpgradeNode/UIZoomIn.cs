using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIZoomIn : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 2.0f;

    [Header("Smooth Settings")]
    public float smoothSpeed = 10f; // 부드럽게 따라가는 속도 (숫자가 작을수록 꿀렁거림)

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRect;

    private void Awake()
    {
        contentRect = scrollRect.content;

        // 휠 굴릴 때 화면이 위아래로 움직이는 유니티 기본 기능 끄기
        scrollRect.scrollSensitivity = 0f;
    }

    void Update()
    {
        // 1. 매 프레임 마우스 휠 입력값을 직접 읽어옵니다.
        float scrollInput = Mouse.current.scroll.ReadValue().y;

        // 2. 휠 입력이 발생했을 때만 즉시 크기를 변경합니다.
        if (scrollInput != 0)
        {
            float currentScale = contentRect.localScale.x;

            // 새로운 크기 계산 및 제한
            float newScale = currentScale + (scrollInput * zoomSpeed);
            newScale = Mathf.Clamp(newScale, minZoom, maxZoom);

            // 즉시 적용!
            contentRect.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
