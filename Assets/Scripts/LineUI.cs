using UnityEngine;
using UnityEngine.UI;

public class LineUI : MonoBehaviour
{
    [SerializeField] private RawImage lineImage;
    [SerializeField] private float lineSpeed;
    //[SerializeField] private RectTransform nodeA;
    //[SerializeField] private RectTransform nodeB;

    [Header("연결할 두 노드")]
    public RectTransform nodeA;
    public RectTransform nodeB;

    [Header("점선 이미지 (Raw Image)")]
    public RectTransform lineRect;

    [Header("점선 셋팅")]
    [Tooltip("점선 하나당 대략적인 픽셀 길이 (숫자가 작을수록 촘촘해짐)")]
    public float dashSize = 50f;

    [Tooltip("노드 아이콘을 가리지 않게 띄워주는 여백 (노드의 반지름 정도 픽셀값)")]
    public float nodePadding = 30f; // = 160f;
    public Vector2 pivotPadding; //= new Vector2(127f, 80f);

    private void OnEnable()
    {
        lineImage = GetComponent<RawImage>();
        lineRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        MoveLine();
    }

    public void MoveLine()
    {
        Rect uvRect = lineImage.uvRect;
        uvRect.x -= Time.deltaTime * lineSpeed;
        lineImage.uvRect = uvRect;
    }

    public void DrawLine()
    {
        if (nodeA == null || nodeB == null || lineRect == null || lineRect.parent == null) return;
        if (lineImage == null) lineImage = lineRect.GetComponent<RawImage>();

        // 1. 월드 좌표를 부모(lineRect.parent) 기준 로컬 좌표로 변환
        Vector3 worldPosA = nodeA.position;
        Vector3 worldPosB = nodeB.position;

        Vector3 localPosA = lineRect.parent.InverseTransformPoint(worldPosA);
        Vector3 localPosB = lineRect.parent.InverseTransformPoint(worldPosB);

        // 2. 방향 벡터와 거리 계산
        Vector2 direction = (Vector2)(localPosB - localPosA);
        float distance = direction.magnitude;

        // 3. 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 4. 시작 위치 적용 (여백 포함)
        Vector2 directionNormalized = direction.normalized;
        lineRect.localPosition = (Vector2)localPosA + (directionNormalized * nodePadding);

        // 5. 실제 선 길이 계산 및 적용 (양쪽 여백 빼기)
        float actualLineLength = distance - (nodePadding * 2f);
        if (actualLineLength < 0) actualLineLength = 0;

        lineRect.sizeDelta = new Vector2(actualLineLength, lineRect.sizeDelta.y);
        lineRect.localEulerAngles = new Vector3(0, 0, angle);

        // ==============================================================
        // 6. 점선 절대 안 찌그러지게 (원본 이미지 비율 자동 계산)
        // ==============================================================
        if (lineImage.texture != null)
        {
            // 텍스처 원본 비율 계산
            float textureRatio = (float)lineImage.texture.width / lineImage.texture.height;
            float perfectDashWidth = lineRect.sizeDelta.y * textureRatio;

            // 타일링 횟수 계산 (Width)
            float tilingW = perfectDashWidth > 0 ? actualLineLength / perfectDashWidth : 1f;

            // ★ 핵심: 애니메이션이 건드리고 있는 X값(스크롤)과 Y, Height는 그대로 유지하고 
            // Width(타일링 반복 횟수)만 덮어씌웁니다.
            lineImage.uvRect = new Rect(lineImage.uvRect.x, lineImage.uvRect.y, tilingW, 1f);
        }
    }
}
