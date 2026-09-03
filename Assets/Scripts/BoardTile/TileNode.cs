using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Linq;

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

    
    public void SetReward()
    {
        reward = TileReward() + BuildingReward();
    }

    // 각 줄의 타일 위치에 따라 타일의 보상 값 다르게 함
    // 한 줄 내 일반 타일의 크기를 일단 9로 잡음 (일반 타일 8칸 + 특수타일 1칸)
    // 이걸 어떻게 나눠야할지 감 안잡힘

    // 각 줄에서의
    // [[n번째 타일의 가치(n번째이면 1.n) * 현재 줄의 기초 재화량]] = 현재 타일의 기초 재화량
    //최종 획득량 = (기본값 + 고정수치 합) × (1 + 일반 업그레이드 퍼센트 합)
    //              × (특수 업그레이드 1 배율) × (특수 업그레이드 2 배율) ...
    public int TileReward()
    {
        int baseTileReward = (int)((tileIndex % 9 * 0.1f + 1f) * data.tileReward);
        int flatValues = GameManager.Instance.myUpgrades.flat_tileValueUpgrade.Sum
                            (value => value.upgradeValue * value.currentUpgradeCount);
        float multiValues = 1f;
        foreach(var upgrade in GameManager.Instance.myUpgrades.mult_tileValueUpgrade)
        {
            //multValues *= upgrade^;// 업그레이드 수치에  업그레이드 갯수만큼 ^ 하기
            multiValues *= upgrade.upgradeValue ^ upgrade.currentUpgradeCount;
        }

        return (int)((baseTileReward + flatValues) * multiValues);
    }

    public int BuildingReward()
    {
        int baseBuildingReward = 0;
        int flatValues = GameManager.Instance.myUpgrades.flat_buildingValueUpgrade.Sum
                            (value => value.upgradeValue * value.currentUpgradeCount);
        float multiValues = 1f;
        foreach (var upgrade in GameManager.Instance.myUpgrades.mult_buildingValueUpgrade)
        {
            multiValues *= upgrade.upgradeValue ^ upgrade.currentUpgradeCount;
        }

        return (int)((baseBuildingReward + flatValues) * multiValues);
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
