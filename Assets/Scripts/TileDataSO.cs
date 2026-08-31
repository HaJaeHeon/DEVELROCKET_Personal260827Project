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

    [field: SerializeField] public int tileReward { get; private set; }
}
