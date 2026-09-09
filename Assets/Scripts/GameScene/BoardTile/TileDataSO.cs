using System;
using System.Numerics;
using UnityEngine;

[Serializable]
public struct UBigInt
{
    [SerializeField]
    private string stringValue;

    public BigInteger Value
    {
        get
        {
            if (string.IsNullOrEmpty(stringValue)) return 0;
            BigInteger.TryParse(stringValue, out BigInteger result);
            return result;
        }
        set
        {
            stringValue = value.ToString();
        }
    }

    public static implicit operator BigInteger(UBigInt u) => u.Value;
    public static implicit operator UBigInt(BigInteger b) => new UBigInt { Value = b };
    public static implicit operator UBigInt(int i) => new UBigInt { Value = i };
}

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
    [field: SerializeField] public UBigInt tileReward { get; private set; }
    // 각 줄의 가장 처음 타일에서 생산 될 건물의 재화량
    [field: SerializeField] public BigInteger buildingReward { get; private set; }
}
