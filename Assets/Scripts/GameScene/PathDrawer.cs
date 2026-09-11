using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[RequireComponent(typeof(TrailRenderer))]
public class PathDrawer : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    [Tooltip("이동할 경로의 목표 지점들")]
    public List<Vector3> waypoints;

    [Tooltip("선을 그리는 데 걸리는 총 시간")]
    public float drawDuration = 3f;

    public float divideXvalue = 1f;

    private void Awake()
    {
        for (int i = 0; i < 100; i++)
        {
            waypoints.Add(transform.localPosition + new Vector3((i/divideXvalue)  ,(Mathf.Pow(i, 2) / 100f), 0));
        }
    }

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        // 1. 시작 전 트레일 초기화 (엉뚱한 곳에서부터 선이 튀는 현상 방지)
        trailRenderer.Clear();

        // 2. Transform 배열을 Vector3 배열로 변환
        Vector3[] pathPositions = new Vector3[waypoints.Count];
        for (int i = 0; i < waypoints.Count; i++)
        {
            pathPositions[i] = waypoints[i];
        }

        // 3. DOPath를 이용해 곡선으로 이동 시작
        transform.DOPath(
            pathPositions,
            drawDuration,
            PathType.CatmullRom, // CatmullRom을 사용하면 점과 점 사이를 부드러운 곡선으로 이어줍니다.
            PathMode.Full3D,
            resolution: 10,      // 곡선의 해상도 (높을수록 부드럽지만 연산량 증가)
            gizmoColor: Color.red
        )
        .SetEase(Ease.InOutSine).SetLink(gameObject); // 시작과 끝을 부드럽게 가감속
    }
}