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
}


[CreateAssetMenu(fileName = "New TileData", menuName = "BoardGame/TileData")]

public class TileDataSO : ScriptableObject
{
    [field: SerializeField] public TileType tileType {  get; private set; }

    public int tileIndex;
    public int tileReward;
}
