using UnityEngine;

public class TileNode : MonoBehaviour
{
    [SerializeField] private TileDataSO data;
    [field:SerializeField] public int tileIndex { get; private set; }
    [field: SerializeField] public int reward { get; private set; }
    [field: SerializeField] public TileType tileType { get; private set; }


    public void SetIndex(int num)
    {
        tileIndex = num;
    }

    // 각 줄의 타일 위치에 따라 타일의 보상 값 다르게 함
    // 한 줄 내 일반 타일의 크기를 일단 9로 잡음 (일반 타일 8칸 + 특수타일 1칸)
    public void SetReward()
    {
        //Debug.Log($"{gameObject.name} :인덱스 {tileIndex}");
        reward = (int)((tileIndex % 9 * 0.1f + 1f) * data.tileReward);
        //Debug.Log($"{gameObject.name} : 타일 리워드 {data.tileReward}");
        //Debug.Log($"{gameObject.name} : 리워드 {reward}");
    }

    public void SetTileType()
    {
        tileType = data.tileType;
    }
}
