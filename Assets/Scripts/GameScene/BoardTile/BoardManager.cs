using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private static BoardManager instance;
    public static BoardManager Instance => instance;

    [SerializeField] private List<TileNode> nodes = new List<TileNode>();

    public GameObject boardObject;

    public PlayerMove playerMove;

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

    public void InitNode()
    {
        GameDatas gameDatas = GameManager.Instance.gameDatas;
        gameDatas.tileDataList ??= new List<TileData> ();

        TileNode[] tiles = boardObject.GetComponentsInChildren<TileNode>();

        nodes.Clear();

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].SetIndex(i);
            tiles[i].SetReward();
            tiles[i].SetTileType();

            nodes.Add(tiles[i]);

            TileData savedTileData = gameDatas.tileDataList.Find(saveData => saveData.tileIndex == tiles[i].tileIndex);

            if (savedTileData != null)
            {
                tiles[i].RestoreBuilding(savedTileData.buildingCount);
            }
        }

        playerMove.gameObject.transform.position = GetTile(GameManager.Instance.gameDatas.currentPlayerPositionTileNum).transform.position + Vector3.up * playerMove.heightOffset;
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

    public void SaveTileData(GameDatas targetData)
    {
        targetData.tileDataList ??= new List<TileData>();
        targetData.tileDataList.Clear();

        foreach(TileNode tile in nodes)
        {
            targetData.tileDataList.Add(new TileData
            {
                tileIndex = tile.tileIndex,
                buildingCount = tile.buildingCount
            });
        }
    }
}
