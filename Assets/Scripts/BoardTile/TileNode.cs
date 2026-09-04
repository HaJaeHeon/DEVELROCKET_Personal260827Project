using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Linq;
using System.Numerics;

public class TileNode : MonoBehaviour
{
    [SerializeField] private TileDataSO data;
    [field:SerializeField] public int tileIndex { get; private set; }
    [field: SerializeField] public UBigInt reward { get; private set; }
    [field: SerializeField] public TileType tileType { get; private set; }
    [field: SerializeField] public int buildingCount { get; private set; }

    [SerializeField] GameObject buildPrefab_1;
    [SerializeField] GameObject buildPrefab_2;
    [SerializeField] GameObject buildPrefab_3;
    [SerializeField] GameObject buildPrefab_4;


    private UnityEngine.Vector3[] buildTransforms = { new UnityEngine.Vector3(0.75f, 1f, 0.75f), new UnityEngine.Vector3(0.75f, 1f, 0f), new UnityEngine.Vector3(0.75f, 1f, -0.75f) };
    private List<GameObject> buildingList = new();

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
        //Debug.Log($"reward : {reward} / tileReward : {TileReward()} / buildingReward : {BuildingReward()}");
    }

    // 각 줄의 타일 위치에 따라 타일의 보상 값 다르게 함
    // 한 줄 내 일반 타일의 크기를 일단 9로 잡음 (일반 타일 8칸 + 특수타일 1칸)
    // 이걸 어떻게 나눠야할지 감 안잡힘

    // 각 줄에서의
    // [[n번째 타일의 가치(n번째이면 1.n) * 현재 줄의 기초 재화량]] = 현재 타일의 기초 재화량
    //최종 획득량 = (기본값 + 고정수치 합) × (1 + 일반 업그레이드 퍼센트 합)
    //              × (특수 업그레이드 1 배율) × (특수 업그레이드 2 배율) ...
    public BigInteger TileReward()
    {
        int multiplier = 10 + (tileIndex % 9);

        BigInteger baseTileReward = ((BigInteger)data.tileReward * multiplier) / 10;

        BigInteger flatValues = 0;

        foreach (var value in GameManager.Instance.myUpgrades.flat_tileValueUpgrade)
        {
            flatValues += (BigInteger)value.upgradeValue * value.currentUpgradeCount;
        }
        BigInteger multiValues = 1;
        foreach(var upgrade in GameManager.Instance.myUpgrades.mult_tileValueUpgrade)
        {
            //multValues *= upgrade pow;// 업그레이드 수치에  업그레이드 갯수만큼 pow 하기
            multiValues *= BigInteger.Pow(upgrade.upgradeValue, upgrade.currentUpgradeCount);
        }
        //Debug.Log($"[보상 추적] 기본값: {baseTileReward} / 고정업글: {flatValues} / 배율업글: {multiValues}");

        return (baseTileReward + flatValues) * multiValues;
    }

    public BigInteger BuildingReward()
    {
        BigInteger baseBuildingReward = data.buildingReward * buildingCount;
        long flatValues = GameManager.Instance.myUpgrades.flat_buildingValueUpgrade.Sum
                            (value => value.upgradeValue * value.currentUpgradeCount);
        BigInteger multiValues = 1;
        foreach (var upgrade in GameManager.Instance.myUpgrades.mult_buildingValueUpgrade)
        {
            multiValues *= BigInteger.Pow(upgrade.upgradeValue, upgrade.currentUpgradeCount);
        }

        return (BigInteger)((baseBuildingReward + flatValues) * multiValues);
    }


    public void SetTileType()
    {
        tileType = data.tileType;
    }

    public IEnumerator BuildBuiling()
    {
        if(buildingCount >= GameManager.Instance.maxBuildingCount)
        {
            Debug.LogWarning($"{buildingCount} 빌딩수가 {GameManager.Instance.maxBuildingCount}맥스빌딩수보다 크다.");
            yield break;
        }
        if (buildingCount != 0 && buildingCount % buildTransforms.Length == 0)
        {
            buildingList.ForEach((value) => Destroy(value));
            buildingList.Clear();
        }

        if (buildingList.Count >= buildTransforms.Length)
        {
            Debug.LogWarning($"{name}: buildTransforms 슬롯({buildTransforms.Length}개)을 초과하는 건설 요청입니다. maxBuildingCount 설정을 확인하세요.");
            yield break;
        }
        

        GameObject obj = GameObject.Instantiate(SelectBuilding());
        obj.transform.SetParent(gameObject.transform);

        obj.transform.localPosition = buildTransforms[buildingCount % buildTransforms.Length] + UnityEngine.Vector3.up;
        buildingList.Add(obj);

        yield return obj.transform.DOLocalMove(buildTransforms[buildingCount % buildTransforms.Length], GameManager.Instance.buildSpeed).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            buildingCount++;
        }).WaitForCompletion();
    }

    public GameObject SelectBuilding()
    {
        return !GameManager.Instance.buildingCountUpgrade_1 ? 
            buildPrefab_1 : !GameManager.Instance.buildingCountUpgrade_2 ? 
            buildPrefab_2 : !GameManager.Instance.buildingCountUpgrade_3 ? buildPrefab_3 : buildPrefab_4;
    }
}
