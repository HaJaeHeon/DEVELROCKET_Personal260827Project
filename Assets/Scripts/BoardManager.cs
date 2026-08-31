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
    }


    private void Start()
    {
        InitNode();
    }

    private void InitNode()
    {
        TileNode[] tiles = boardObject.GetComponentsInChildren<TileNode>();

        nodes.Clear();

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].SetIndex(i);
            nodes.Add(tiles[i]);
        }
    }

    // 노드 갯수로 나눈 나머지가 현재 노드
    public TileNode GetTile(int num)
    {
        return nodes[num % nodes.Count];
    }
}
