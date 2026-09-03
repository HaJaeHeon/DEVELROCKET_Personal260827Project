using UnityEngine;

public enum TileType
{
    StartZone,
    Island,
    Tax,
    FoodLine,
    WoodLine,
    StoneLine,
    IndustryLine,
    Festival,
    Travel
}


[CreateAssetMenu(fileName = "New TileData", menuName = "BoardGame/TileData")]

public class TileDataSO : ScriptableObject
{
    [field: SerializeField] public TileType tileType {  get; private set; }

    // 각 줄의 가장 처음 타일에서 생산 될 타일의 재화량
    [field: SerializeField] public int tileReward { get; private set; }
    // 각 줄의 가장 처음 타일에서 생산 될 건물의 재화량
    [field: SerializeField] public int buildingReward { get; private set; }
}
