using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class MultiplierCurveGraphic : MaskableGraphic
{
    private const float AxisSeconds = 10f;

    [Header("UI")]
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private RectTransform head;

    [Header("Line")]
    // 선을 그릴 때 필요한 정점의 갯수
    [SerializeField, Range(8, 256)] private int segments = 80;
    // 선 두께
    [SerializeField, Min(0.1f)] private float thickness = 5f;
    // 그래프 영역의 테두리와 실제 곡선 사이에 두는 여백입니다. 상하좌우에 동일하게 적용합니다.
    [SerializeField, Min(0f)] private float padding = 24f;

    [Header("Vertical Axis")]
    // 낮은 결과도 항상 화면 맨 위까지 올라가는 것을 방지.
    [SerializeField, Min(1.01f)] private float minimumAxisMax = 5f;

    // 최종점 위쪽에 남겨둘 여유.
    [SerializeField, Range(0f, 0.5f)] private float topMargin = 0.15f;

    [Header("Head")]
    // 자식 이미지 회전 여부
    [SerializeField] private bool rotateHead = true;
    // 이미지가 바라보는 방향과 곡선의 방향 차이 보정
    [SerializeField] private float angleOffset;

    // 배율 결과값
    private float finalMultiplier = 1f;
    // 실제 세로축 상한
    private float axisMax = 5f;
    // 재생이 끝나는 시간( 0 ~ 10 초)
    private float duration = AxisSeconds;
    // 이번 재생에서 지금까지 진행된 시간 (duration에 도달하면 멈춤)
    private float elapsed;
    // 현재 재생중인지 / true 일때 시간 증가
    private bool playing;

    public bool IsPlaying => playing;
    public float CurrentMultiplier => EvaluateMultiplier(elapsed);

    public float multiNum;
    public float durationTime;

    [ContextMenu("그래프 시작(multiNum, durationTime")]
    public void PlayInInspector()
    {
        Play(multiNum, durationTime);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        raycastTarget = false;
        Refresh();
    }

    // 재생 전에 확정된 결과를 전달합니다.
    // playSeconds는 0초 초과, 최대 10초입니다.
    public void Play(float resultMultiplier, float playSeconds = 10f)
    {
        if (float.IsNaN(resultMultiplier)
            || float.IsInfinity(resultMultiplier)
            || float.IsNaN(playSeconds)
            || float.IsInfinity(playSeconds))
        {
            Debug.LogError("배율과 재생 시간은 유효한 숫자여야 합니다.", this);
            return;
        }

        if (resultMultiplier < 1f
            || playSeconds <= 0f
            || playSeconds > AxisSeconds)
        {
            Debug.LogError("배율은 1 이상, 시간은 0초 초과 10초 이하여야 합니다.", this);
            return;
        }

        finalMultiplier = resultMultiplier;
        duration = playSeconds;

        // 재생 시작 시 한 번만 결정합니다.
        // 재생 중에는 세로축을 변경하지 않습니다.
        axisMax = Mathf.Max(
            Mathf.Max(1.01f, minimumAxisMax),
            1f + (finalMultiplier - 1f) * (1f + topMargin)
        );

        elapsed = 0f;
        playing = true;

        Refresh();
    }

    [ContextMenu("그래프 리셋")]
    public void ResetGraph()
    {
        playing = false;
        elapsed = 0f;
        finalMultiplier = 1f;
        axisMax = Mathf.Max(1.01f, minimumAxisMax);

        Refresh();
    }

    private void Update()
    {
        if (!playing)
            return;

        // 게임 일시정지(Time.timeScale = 0) 시 함께 정지합니다.
        elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);

        if (elapsed >= duration)
            playing = false;

        Refresh();
    }

    private float EvaluateMultiplier(float seconds)
    {
        float progress = Mathf.Clamp01(seconds / duration);

        // 시작은 완만하고 뒤로 갈수록 가파르게 상승.
        return Mathf.Lerp(
            1f,
            finalMultiplier,
            progress * progress
        );
    }

    private Rect GetPlotRect()
    {
        Rect r = rectTransform.rect;

        float inset = Mathf.Clamp(
            padding,
            0f,
            Mathf.Max(0f, Mathf.Min(r.width, r.height) * 0.49f)
        );

        return new Rect(
            r.xMin + inset,
            r.yMin + inset,
            Mathf.Max(0f, r.width - inset * 2f),
            Mathf.Max(0f, r.height - inset * 2f)
        );
    }

    private Vector2 GetPoint(float seconds)
    {
        Rect r = GetPlotRect();

        float xRatio = Mathf.Clamp01(seconds / AxisSeconds);

        float yRatio =
            (EvaluateMultiplier(seconds) - 1f) / (axisMax - 1f);

        return new Vector2(
            r.xMin + r.width * xRatio,
            r.yMin + r.height * yRatio
        );
    }

    private Vector2 GetTangent(float seconds)
    {
        Rect r = GetPlotRect();
        float progress = Mathf.Clamp01(seconds / duration);

        float dx = r.width / AxisSeconds;

        float dy =
            r.height
            * (finalMultiplier - 1f)
            * 2f * progress
            / (duration * (axisMax - 1f));

        Vector2 tangent = new Vector2(dx, dy);

        return tangent.sqrMagnitude > 0f
            ? tangent.normalized
            : Vector2.right;
    }

    private void Refresh()
    {
        if (multiplierText != null)
            multiplierText.text = $"{CurrentMultiplier:F2}배";

        UpdateHead();
        SetVerticesDirty();
    }

    private void UpdateHead()
    {
        if (head == null)
            return;

        // Head는 GraphArea의 직접 자식이어야 합니다.
        head.anchorMin = rectTransform.pivot;
        head.anchorMax = rectTransform.pivot;
        head.anchoredPosition = GetPoint(elapsed);

        if (rotateHead)
        {
            Vector2 tangent = GetTangent(elapsed);

            float angle =
                Mathf.Atan2(tangent.y, tangent.x)
                * Mathf.Rad2Deg;

            head.localRotation = Quaternion.Euler(
                0f, 0f, angle + angleOffset
            );
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        UpdateHead();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect plot = GetPlotRect();

        if (elapsed <= 0f || plot.width <= 0f || plot.height <= 0f)
            return;

        int count = Mathf.Clamp(segments, 8, 256);
        float halfWidth = Mathf.Max(0.1f, thickness) * 0.5f;

        for (int i = 0; i <= count; i++)
        {
            float seconds = elapsed * i / count;

            Vector2 point = GetPoint(seconds);
            Vector2 tangent = GetTangent(seconds);

            Vector2 normal =
                new Vector2(-tangent.y, tangent.x) * halfWidth;

            vh.AddVert(point + normal, color, Vector2.zero);
            vh.AddVert(point - normal, color, Vector2.zero);

            if (i == 0)
                continue;

            int previous = (i - 1) * 2;

            vh.AddTriangle(previous, previous + 2, previous + 1);
            vh.AddTriangle(previous + 1, previous + 2, previous + 3);
        }
    }
}