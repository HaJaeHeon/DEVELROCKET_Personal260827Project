using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIZoomIn : MonoBehaviour
{
    [Header("줌인 관련")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2.0f;


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
        // 매 프레임 마우스 휠 입력값을 직접 읽기
        float scrollInput = Mouse.current.scroll.ReadValue().y;

        // 휠 입력이 발생했을 때만 즉시 크기를 변경
        if (scrollInput != 0)
        {
            float currentScale = contentRect.localScale.x;

            // 새로운 크기 계산 및 제한
            float newScale = currentScale + (scrollInput * zoomSpeed);
            newScale = Mathf.Clamp(newScale, minZoom, maxZoom);

            contentRect.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
