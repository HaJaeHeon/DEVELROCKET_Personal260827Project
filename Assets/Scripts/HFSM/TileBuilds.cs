using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuilds : MonoBehaviour
{
    [field: SerializeField] public bool isProcess { get; private set; }

    public void BuildProcess()
    {
        isProcess = true;
        StartCoroutine(ProcessingBuild());
    }

    public IEnumerator ProcessingBuild()
    {
        Debug.Log($"isProcess : {isProcess}");
        IncomeBuilding();
        yield return StartCoroutine(Build(GameManager.Instance.tile));

        isProcess = false;
        Debug.Log($"isProcess : {isProcess}");
    }

    public void IncomeBuilding()
    {
        TileNode tile = GameManager.Instance.tile;

        if (!TryGetCurrencyType(tile.tileType, out CurrencyType type))
        {
            Debug.LogWarning($"IncomeBuild: {tile.tileType}에 대응하는 CurrencyType이 없습니다.");
            return;
        }

        Dictionary<CurrencyType, int> receipt = new Dictionary<CurrencyType, int> { { type, tile.reward } };

        GameManager.Instance.UpdateAccount(receipt);
    }

    private static bool TryGetCurrencyType(TileType tileType, out CurrencyType currency)
    {
        switch (tileType)
        {
            case TileType.FoodLine: currency = CurrencyType.Food; return true;
            case TileType.WoodLine: currency = CurrencyType.Wood; return true;
            case TileType.StoneLine: currency = CurrencyType.Stone; return true;
            case TileType.IndustryLine: currency = CurrencyType.Industry; return true;
            default: currency = default; return false;
        }
    }

    public IEnumerator Build(TileNode tile)
    {
        if (tile.buildingCount < GameManager.Instance.maxBuildingCount)
        {
            yield return StartCoroutine(tile.BuildBuiling());
        }
        else
        {
            Debug.LogWarning($"{tile.buildingCount}(빌딩수) 가 {GameManager.Instance.maxBuildingCount} (맥스 빌딩 수) 보다 크거나 같다");
        }
    }

}
