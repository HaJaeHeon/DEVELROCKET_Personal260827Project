using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LineRendererObject : MonoBehaviour
{
    public LineRenderer lineRender;
    public RectTransform movingImage;

    private float lineTime;
    private int pointCount;

    private void Awake()
    {
        if(lineRender == null)
        {
            lineRender = GetComponent<LineRenderer>();
        }
    }

    private void Start()
    {
        lineRender.positionCount = 0;
    }

    void Update()
    {
        lineTime += Time.deltaTime;

        float x = lineTime;
        float y = Mathf.Pow(x, 2);

        // 2. 이미지 이동 (UI 좌표계 보정 필요 시 배수 곱하기)
        Vector2 newPos = new Vector2(x * 100f, y * 100f);
        movingImage.anchoredPosition = newPos;

        // 3. 이미지가 이동한 실제 위치를 화면 공간(World) 좌표로 변환하여 선 그리기
        // UI의 anchoredPosition이 아닌, 실제 Transform 위치를 줘야 LineRenderer가 정확히 따라갑니다.
        lineRender.positionCount = pointCount + 1;
        lineRender.SetPosition(pointCount, movingImage.position);
        pointCount++;
    }
}
