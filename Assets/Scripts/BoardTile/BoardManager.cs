using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private static BoardManager instance;
    public static BoardManager Instance => instance;

    [SerializeField] private List<TileNode> nodes = new List<TileNode>();

    [SerializeField] private GameObject boardObject;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitNode(); 
    }

    //현재 Start 부분에서 맵 데이터를 초기화 하는데 추후에 바뀔 수 있음 CSV로 저장하여 불러오는 방식을 채택할 수도 있음
    private void Start()
    {
    }

    private void InitNode()
    {
        TileNode[] tiles = boardObject.GetComponentsInChildren<TileNode>();

        nodes.Clear();

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].SetIndex(i);
            tiles[i].SetReward();
            tiles[i].SetTileType();

            nodes.Add(tiles[i]);
        }
    }

    // 노드 갯수로 나눈 나머지가 현재 노드
    public TileNode GetTile(int num)
    {
        if (nodes.Count == 0)
        {
            Debug.LogError("BoardManager: nodes가 초기화되지 않았습니다.");
            return null;
        }
        return nodes[num % nodes.Count];
    }
}
