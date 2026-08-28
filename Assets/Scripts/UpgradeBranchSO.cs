using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// 1. 재화 종류 (4라인 테마에 맞춤)
public enum CurrencyType
{
    Food,       // 1라인 (행동력/주사위 관련 노드용)
    Wood,       // 2라인 (건물 효율 관련 노드용)
    Stone,      // 3라인 (패널티 극복/보드판 룰 조작용)
    Industry    // 4라인 (자동화 및 최종 엔딩용)
}

// 2. 노드가 제공하는 능력치 종류
public enum StatType
{
    // 수동 스탯 강화
    IncomeMultiplier,        // 월급/수익 증가
    BuildCostDiscount,       // 건물 건설 비용 감소
    DiceCooldownReduction,   // 주사위 쿨타임 감소

    // 시스템 해금 (특수 기능)
    UnlockGlobalBuild,       // 글로벌 원격 건설 창 해금 (운빨 극복)
    UnlockAutoRoll,          // 자동 주사위 해금
    UnlockAutoBuild,         // 자동 건설 해금

    // 최종 엔딩
    GameClear                // 최종 노드 전용 (스토리 엔딩 트리거)
}

// 3. 비용 데이터 구조체
[System.Serializable]
public struct CostData
{
    public CurrencyType currency;
    public int baseCost;         // 1단계 요구량
    public float costMultiplier;    // 단계별 비용 증가 배율
}

// 4. 스킬 가지(Branch) 핵심 설계도
[CreateAssetMenu(fileName = "New Upgrade Node", menuName = "BoardGame/Upgrade Node")]
public class UpgradeBranchSO : ScriptableObject
{
    [Header("노드 기본 정보")]
    public string nodeID;
    public string nodeName;
    [TextArea] public string description; // "주사위가 자동으로 굴러갑니다!"
    public Sprite nodeIcon;

    [Header("트리 연결 구조")]
    public int maxLevel = 5; // 이 노드의 최대 레벨
    //public List<UpgradeBranchSO> nextNodesToUnlock; // 마스터 시 열릴 다음 노드들
    //public bool isRootNode;

    [Header("비용 및 효과")]
    public List<CostData> requiredCosts; // 여러 재화 동시 요구 가능
    public StatType targetStat;
    public float baseStatValue;
    public float statMultiplierPerLevel;
}