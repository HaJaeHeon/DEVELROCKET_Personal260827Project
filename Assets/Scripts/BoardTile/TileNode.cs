using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class TileNode : MonoBehaviour
{
    [SerializeField] private TileDataSO data;
    [field:SerializeField] public int tileIndex { get; private set; }
    [field: SerializeField] public int reward { get; private set; }
    [field: SerializeField] public TileType tileType { get; private set; }
    [field: SerializeField] public int buildingCount { get; private set; }
    
    [SerializeField] GameObject buildPrefab;


    private Vector3[] buildTransforms = { new Vector3(0.75f, 1f, 0.75f), new Vector3(0.75f, 1f, 0f), new Vector3(0.75f, 1f, -0.75f) };

    private void Start()
    {
        buildingCount = 0;
    }


    public void SetIndex(int num)
    {
        tileIndex = num;
    }

    // 각 줄의 타일 위치에 따라 타일의 보상 값 다르게 함
    // 한 줄 내 일반 타일의 크기를 일단 9로 잡음 (일반 타일 8칸 + 특수타일 1칸)
    public void SetReward()
    {
        reward = (int)((tileIndex % 9 * 0.1f + 1f) * data.tileReward);
    }

    public void SetTileType()
    {
        tileType = data.tileType;
    }

    public IEnumerator BuildBuiling()
    {
        if(buildingCount >= buildTransforms.Length)
        {
            Debug.LogWarning($"{name}: buildTransforms 슬롯({buildTransforms.Length}개)을 초과하는 건설 요청입니다. maxBuildingCount 설정을 확인하세요.");
            yield return null;
        }

        GameObject obj = GameObject.Instantiate(buildPrefab);
        obj.transform.SetParent(gameObject.transform);

        obj.transform.localPosition = buildTransforms[buildingCount] + Vector3.up;

        yield return obj.transform.DOLocalMove(buildTransforms[buildingCount], GameManager.Instance.buildSpeed).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            buildingCount++;
        }).WaitForCompletion();
    }
}
