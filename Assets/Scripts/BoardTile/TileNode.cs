using UnityEngine;

public class TileNode : MonoBehaviour
{
    [SerializeField] private TileDataSO data;
    [field:SerializeField] public int tileIndex { get; private set;  }

    public void SetIndex(int num)
    {
        tileIndex = num;
    }
}
